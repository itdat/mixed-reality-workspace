using System.Collections;
using Photon.Pun;
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

        [PunRPC]
        public void PlayTalking(float time) {
            StartCoroutine(PlayTalkingInternal(time));
        }

        [PunRPC]
        public void PlayWelcome() {
            StartCoroutine(PlayWelcomeInternal());
        }

        [PunRPC]
        public void PlayThank() {
            StartCoroutine(PlayThankInternal());
        }

        private IEnumerator PlayTalkingInternal(float time) {
            animator.SetBool("isTalking", true);
            yield return new WaitForSeconds(time - 1);
            animator.SetBool("isTalking", false);
        }

        private IEnumerator PlayWelcomeInternal() {
            animator.SetBool("isWelcome", true);
            yield return new WaitForSeconds(1);
            animator.SetBool("isWelcome", false);
        }


        private IEnumerator PlayThankInternal() {
            animator.SetBool("isThankful", true);
            yield return new WaitForSeconds(1);
            animator.SetBool("isThankful", false);
        }
    }
}
