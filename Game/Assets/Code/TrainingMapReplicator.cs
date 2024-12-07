using System.Collections.Generic;
using UnityEngine;

namespace Code {

    [DefaultExecutionOrder(-5)]
    public class TrainingMapReplicator : MonoBehaviour {

        public new Camera camera;
        public GameObject baseArea;
        public int numAreas = 1;
        public float separation = 10f;

        public List<GameObject> areas;

        public void OnEnable() => this.AddEnvironments();

        private void CenterCamera() {
            if (this.areas.Count == 0) return;

            Vector3 min = Vector3.zero, max = Vector3.zero;

            foreach (var area in this.areas) {
                var position = area.transform.position;
                min = Vector3.Min(min, position);
                max = Vector3.Max(max, position);
            }

            var centerPoint = (min + max) / 2;
            var height = max.y - min.y + 2f;
            var distance = this.areas.Count * height;

            this.camera.transform.position = new Vector3(centerPoint.x, centerPoint.y + distance, this.camera.transform.position.z);
            this.camera.transform.LookAt(centerPoint);

            if (this.camera.orthographic)
                this.camera.orthographicSize = (max.y - min.y) / 2 + 1f; // Adjust orthographic size based on bounds
            else
                this.camera.fieldOfView = 2f * Mathf.Rad2Deg * Mathf.Atan(height) + distance;
        }

        private void AddEnvironments() {
            for (var i = 0; i < this.numAreas; i++) {
                var area = Instantiate(baseArea, new Vector3(i * separation, 0, 0), Quaternion.identity, this.transform);
                area.name = $"{baseArea.name} #{i}";
                this.areas.Add(area);
            }

            this.CenterCamera();
        }

    }

}
