using UnityEngine;

    [DefaultExecutionOrder(-5)]
    public class TrainingMapReplicator : MonoBehaviour {

        public GameObject baseArea;
        public int numAreas = 1;
        public float separation = 10f;

        public void OnEnable() => this.AddEnvironments();

        private void AddEnvironments() {
            for (var i = 0; i < this.numAreas; i++) {
                var area = Instantiate(baseArea, new Vector3(i * separation, 0, 0), Quaternion.identity, this.transform);
                area.name = $"{baseArea.name} #{i}";
            }
        }

    }
