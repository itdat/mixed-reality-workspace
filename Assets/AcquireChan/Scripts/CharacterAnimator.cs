using System.Collections;
using UnityEngine;

namespace AcquireChan.Scripts {
    public class CharacterAnimator : MonoBehaviour {
        private Animator animator;
        private bool isMove;
        private bool isRun;
        private AgentController agentController;

        private void Start() {
            animator = GetComponent<Animator>();
            isMove = false;
            isRun = false;
            agentController = GetComponent<AgentController>();
        }

        // Update is called once per frame
        private void Update() {
            isMove = agentController.isMove;
            animator.SetBool("isMove", isMove);
            animator.SetBool("isRun", isRun);
        }

        public IEnumerator PlayTalking(float time) {
            animator.SetBool("isTalking", true);
            yield return new WaitForSeconds(time - 1);
            animator.SetBool("isTalking", false);
        }

        public IEnumerator PlayWelcome() {
            animator.SetBool("isWelcome", true);
            yield return new WaitForSeconds(1);
            animator.SetBool("isWelcome", false);
        }

        public IEnumerator PlayThank() {
            animator.SetBool("isThankful", true);
            yield return new WaitForSeconds(1);
            animator.SetBool("isThankful", false);
        }
    }
}
