using System.Collections.Generic;
using Code;
using UnityEngine;

// Credits to: https://assetstore.unity.com/packages/tools/game-toolkits/car-maker-166164

namespace Controllers {

    [RequireComponent(typeof(Rigidbody))]
    public class CarController : MonoBehaviour {

        public List<WheelAxle> wheelAxleList;
        public CarSettings carSettings;
        public float speed = 0;

        public Rigidbody Rigidbody { get; private set; }

        public bool IsGrounded => this.wheelAxleList.TrueForAll(w =>
            w.wheelColliderLeft.isGrounded && w.wheelColliderRight.isGrounded ||
            w.wheelMeshLeft.transform.position.y > 0 && w.wheelMeshRight.transform.position.y > 0
        );

        private void Start() {
            this.Rigidbody = this.GetComponent<Rigidbody>();
            this.Rigidbody.mass = this.carSettings.mass;
            this.Rigidbody.linearDamping = this.carSettings.drag;
            this.Rigidbody.centerOfMass = this.carSettings.centerOfMass;
        }

        //public void FixedUpdate() => this.Drive(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), Input.GetKey(KeyCode.Space));

        public void Drive(float vertical, float horizontal, bool isBraking) {
            this.speed = this.Rigidbody.linearVelocity.magnitude;

            var motor = this.carSettings.motorTorque * vertical;
            var steering = this.carSettings.steeringAngle * horizontal;
            var handBrake = isBraking ? this.carSettings.motorTorque * 1 : 0;

            foreach (var wheel in this.wheelAxleList) {
                if (wheel.steering) {
                    wheel.wheelColliderLeft.steerAngle = steering;
                    wheel.wheelColliderRight.steerAngle = steering;
                }

                if (wheel.motor) {
                    wheel.wheelColliderLeft.motorTorque = motor;
                    wheel.wheelColliderRight.motorTorque = motor;
                }

                wheel.wheelColliderLeft.brakeTorque = handBrake;
                wheel.wheelColliderRight.brakeTorque = handBrake;

                this.ApplyWheelVisuals(wheel.wheelColliderLeft, wheel.wheelMeshLeft);
                this.ApplyWheelVisuals(wheel.wheelColliderRight, wheel.wheelMeshRight);
            }
        }

        #region Private Methods

        private void ApplyWheelVisuals(WheelCollider wheelCollider, GameObject wheelMesh) {
            wheelCollider.GetWorldPose(out var position, out var rotation);

            wheelMesh.transform.position = position;
            wheelMesh.transform.rotation = rotation * Quaternion.Inverse(wheelCollider.transform.parent.rotation) * this.transform.rotation;
        }

        #endregion

    }

}
