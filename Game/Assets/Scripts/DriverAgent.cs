using Controllers;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class DriverAgent : Agent {

    private CarController _car;
    private Vector3 _originalPosition;
    private float _previousDistanceToTarget;

    [SerializeField]
    private Transform target;

    [SerializeField, Range(0f, 10f)]
    private float sensorRange = 1.5f;

    private void Start() {
        this._car = this.GetComponent<CarController>();
        this._originalPosition = this.transform.localPosition;
    }

    public override void OnEpisodeBegin() {
        if (this._car.IsGrounded) return;

        this._previousDistanceToTarget = Vector3.Distance(this.transform.localPosition, this.target.localPosition);

        this._car.Rigidbody.angularVelocity = Vector3.zero;
        this._car.Rigidbody.linearVelocity = Vector3.zero;
        this.transform.localPosition = this._originalPosition;
    }

    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(this._car.Rigidbody.linearVelocity.x);
        sensor.AddObservation(this._car.Rigidbody.linearVelocity.z);
        sensor.AddObservation(this._car.speed);
        sensor.AddObservation(this._car.CheckSensors(this.sensorRange));
    }

    public override void OnActionReceived(ActionBuffers actions) {
        var isSensor = this._car.Drive(actions.ContinuousActions[0], actions.ContinuousActions[1], actions.ContinuousActions[2] > 0, this.sensorRange);

        var currentDistanceToTarget = Vector3.Distance(this.transform.localPosition, this.target.localPosition);
        var distanceChange = _previousDistanceToTarget - currentDistanceToTarget;

        if (isSensor)
            SetReward(-1f);
        else if (distanceChange > 0)
            SetReward(.1f * distanceChange);
        else if (distanceChange < 0)
            SetReward(-.1f * -distanceChange);

        if (!isSensor && (this._previousDistanceToTarget = currentDistanceToTarget) < 1f) {
            this.SetReward(1f);
            this.EndEpisode();
        } else if (isSensor || !this._car.IsGrounded)
            this.EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        continuousActionsOut[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

}
