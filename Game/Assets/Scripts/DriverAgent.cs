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
        sensor.AddObservation(this._car.speed);
    }

    public override void OnActionReceived(ActionBuffers actions) {
        this._car.Drive(actions.ContinuousActions[0], 0, actions.ContinuousActions[1] > 0);

        var currentDistanceToTarget = Vector3.Distance(this.transform.localPosition, this.target.localPosition);
        var distanceChange = _previousDistanceToTarget - currentDistanceToTarget;

        if (distanceChange > 0)
            SetReward(0.1f * distanceChange);
        else if (distanceChange < 0)
            SetReward(-0.1f * -distanceChange);

        if ((this._previousDistanceToTarget = currentDistanceToTarget) < 1f) {
            this.SetReward(1.0f);
            this.EndEpisode();
        } else if (!this._car.IsGrounded)
            this.EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        continuousActionsOut[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

}
