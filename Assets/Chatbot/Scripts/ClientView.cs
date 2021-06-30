using System;
using System.Collections;
using Dialogflow;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ClientView : MonoBehaviour {
    public InputField content;
    public Text outputText;
    public Text recordButtonText;
    private AudioSource _audioPlay;
    private DialogFlowClient _client;
    public RawImage imageResult;
    public AudioSource AudioPlay => _audioPlay;

    protected void OnEnable() {
        _audioPlay = GetComponent<AudioSource>();
        _client = GetComponent<DialogFlowClient>();
        _client.onTextResponse.AddListener(result => {
            outputText.text = result;
            Debug.Log(result);
        });
        _client.onAudioResponse.AddListener(result => {
            _audioPlay.clip = result;
            _audioPlay.Play();
        });
        _client.registerOnParameters("imageUrl", value => {
            var url = value.StringValue;
            Debug.Log(url);
            // StartCoroutine(GetTexture(url));
        });
        _client.onDetectIntentResponse.AddListener(response => {
            if (response != null) return;
            string audioClipPath = "ChatBot\\networkerr";
            var audioClip = Resources.Load<AudioClip>(audioClipPath);
            _audioPlay.clip = audioClip;
            _audioPlay.Play();
        });
    }

    IEnumerator GetTexture(string url) {
        imageResult.gameObject.SetActive(true);
        var request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);

        else {
            imageResult.texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
        }
    }

    private void Start() {
        _client.GetAudioRecorder().onStateRecordChange.AddListener(updateRecordButton);
    }

    public void SendText() {
        imageResult.gameObject.SetActive(false);
        _client.SendText(content.text);
    }

    public void Clear() {
        _client.Clear();
    }

    void updateRecordButton(bool isRecording) {
        recordButtonText.text = isRecording ? "Stop Record" : "Start Record";
    }

    public void OnButtonRecord() {
        _audioPlay.Stop();
        imageResult.gameObject.SetActive(false);
        _client.OnButtonRecord();
    }
}
