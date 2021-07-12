using System;
using Dialogflow;
using UnityEngine;

public class RobotAnimation : MonoBehaviour {
    private Animator[] _animators;
    private ClientView _clientView;
    private bool _sayingState;
    private DialogFlowClient _client;

    private void OnEnable() {
        _clientView = FindObjectOfType<ClientView>();
        _animators = gameObject.GetComponentsInChildren<Animator>();
        _client = FindObjectOfType<DialogFlowClient>();
    }

    private void Start()
    {
        _client.GetAudioRecorder().onStateRecordChange.AddListener(UpdateListeningAnimation);
    }

    private void UpdateListeningAnimation(bool isRecording)
    {
        SetAnimate("isListening", isRecording);
    }

    private void Update() {
        var isSaying = _clientView.AudioPlay.isPlaying;
        if (isSaying == _sayingState) return;
        SetAnimate("isSaying", isSaying);
        _sayingState = isSaying;
    }

    private void SetAnimate(string param, bool value) {
        if (_animators == null) return;
        foreach (var anim in _animators) {
            anim.SetBool(param, value);
        }
    }
}
