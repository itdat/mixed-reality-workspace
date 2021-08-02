using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSetter : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;

    private bool prevSayingState;

    public void isSaying(bool isSaying)
    {
        animator.SetBool("isSaying", isSaying);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (audioSource.isPlaying == prevSayingState) return;
        isSaying(audioSource.isPlaying);
        prevSayingState = audioSource.isPlaying;
    }
}
