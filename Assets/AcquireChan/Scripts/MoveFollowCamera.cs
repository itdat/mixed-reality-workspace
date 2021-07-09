using UnityEngine;

namespace AcquireChan.Scripts {
    public class MoveFollowCamera : MonoBehaviour {
        private AgentController agentController;

        private void OnEnable() {
            agentController = GetComponent<AgentController>();
        }

        private void Update() {
            if (Camera.main is null) return;
            var pos = Camera.main.transform.position;
            pos.x += 1f;
            pos.z += 1f;
            agentController.MoveToPoint(pos);
        }
    }
}
