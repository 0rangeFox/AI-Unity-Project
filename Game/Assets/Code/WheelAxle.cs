using System;
using UnityEngine;

namespace Code {

    [Serializable]
    public struct WheelAxle {

        public WheelCollider wheelColliderLeft;
        public WheelCollider wheelColliderRight;
        public GameObject wheelMeshLeft;
        public GameObject wheelMeshRight;

        public bool motor;
        public bool steering;

    }

}
