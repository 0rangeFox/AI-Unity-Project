using System;
using UnityEngine;

namespace Code {

    [Serializable]
    public class CarSettings {

        public float mass = 1500f;
        public float drag = .05f;
        public Vector3 centerOfMass = new(0, -1f, 0);
        public float motorTorque = 1200f;
        public float steeringAngle = 50f;

    }

}
