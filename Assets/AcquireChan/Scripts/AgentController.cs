using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

// Walk to a random position and repeat
namespace AcquireChan.Scripts {
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentController : MonoBehaviour {
        public float m_Range = 10.0f;

        [SerializeField]
        protected NavMeshAgent m_Agent;

        private bool isRelax;

        [SerializeField]
        private float timeRelax = 0.5f;

        private float relaxTimer = -1;

        [HideInInspector]
        public bool isMove;

        // [HideInInspector]
        public bool isAutoMove = true;

        private void Start() {
            m_Agent = GetComponent<NavMeshAgent>();
            isMove = false;
            isRelax = true;
        }

        private void Update() {
            if (m_Agent.pathPending || m_Agent.remainingDistance > 0.1f) {
                relaxTimer = timeRelax;
                isRelax = true;
                isMove = true;
                return;
            }

            if (!isRelax && isAutoMove) {
                var newDes = RandomNavSphere(transform.position, -1);
                m_Agent.destination = newDes;
            }
            else {
                isMove = false;
                relaxProcessing();
            }
        }

        private void relaxProcessing() {
            relaxTimer -= Time.deltaTime;
            if (relaxTimer < 0) {
                isRelax = false;
            }
        }

        private Vector3 RandomNavSphere(Vector3 originPos, int layerMask) {
            var randDir = Random.insideUnitSphere * m_Range;
            randDir += originPos;
            NavMesh.SamplePosition(randDir, out var navMeshHit, m_Range, layerMask);
            return navMeshHit.position;
        }

        public GameObject PrefabToSpawn;

        public void MoveToPoint(MixedRealityPointerEventData eventData) {
            if (isAutoMove) return;
            var sourceName = eventData.InputSource.SourceName;
            if (!sourceName.Contains("Right")) return;
            var result = eventData.Pointer.Result;
            MoveToPoint(result.Details.Point);
        }

      public void MoveToPoint(Vector3 des) {
            m_Agent.SetDestination(des);
            // if (PrefabToSpawn != null) {
            //     Instantiate(PrefabToSpawn, des, Quaternion.identity);
            // }
        }

        public void ComeHere() {
            isAutoMove = false;
            var cameraTransform = Camera.main.transform;
            m_Agent.SetDestination(cameraTransform.position);
            if (PrefabToSpawn != null) {
                Instantiate(PrefabToSpawn, cameraTransform.position, Quaternion.identity);
            }
        }

        public void SetAutoMove(bool value) {
            isAutoMove = value;
        }
    }
}
