using UnityEngine;

namespace AcquireChan.Scripts {
    public class MoveFollowCamera : MonoBehaviour {
        private AgentController agentController;
        public float dx = 1f;
        public float dy = 1f;
        public float dz = 1f;
        private Transform mainCam;
        private Vector3 lasPosition;

        private void OnEnable() {
            agentController = GetComponent<AgentController>();
            mainCam = Camera.main.transform;
            lasPosition = mainCam.position;
        }

        private void Update() {
            var pos = mainCam.position;
            if (Vector3.Distance(lasPosition, pos) < 1f) return;
            lasPosition = pos;
            pos.x += dx;
            pos.y += dy;
            pos.z += dz;
            agentController.MoveToPoint(pos);
            if (!agentController.isMove) {
                var rotation = mainCam.rotation;
                transform.rotation = new Quaternion(0.0f, rotation.y, 0.0f, rotation.w);
            }
        }
    }
}
