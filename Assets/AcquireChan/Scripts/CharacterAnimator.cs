using AcquireChan.Scripts;
using UnityEngine;

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
}
