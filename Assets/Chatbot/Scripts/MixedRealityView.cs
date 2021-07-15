using System;
using System.Collections;
using Dialogflow;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class MixedRealityView : MonoBehaviour {
    private const string imageResponsePrefab = "ChatBot\\ImageResponsePlate";
    private const string videoResponsePrefab = "ChatBot\\VideoResponse";
    private const string audioErr = "ChatBot\\networkerr";
    private AudioSource _audioPlay;
    private DialogFlowClient _client;
    private GameObject image;
    private GameObject video;
    public KeyCode keyCode;

    [FormerlySerializedAs("isUseKeyDown")]
    public bool useKeyDown;

    private void OnEnable() {
        _audioPlay = GetComponent<AudioSource>();
        _client = GetComponent<DialogFlowClient>();

        _client.onAudioResponse.AddListener(PlayAudio);

        _client.registerOnParameters("imageUrl", LoadImage);
        _client.registerOnParameters("videoUrl", value => { PlayVideo(value.StringValue); });
        _client.onDetectIntentResponse.AddListener(response => {
            if (response == null) {
                var audioClip = Resources.Load<AudioClip>(audioErr);
                _audioPlay.clip = audioClip;
                _audioPlay.Play();
                return;
            }

            if (response.QueryResult.Action == "show_table") {
                ShowTables();
            }
        });
    }

    private static void ShowTables() {
        var go = Resources.Load<GameObject>("Tables\\Tables");
        var mainCam = Camera.main.transform;
        var rotation = mainCam.rotation;
        var position = mainCam.position + rotation * Vector3.forward * 3;
        Instantiate(go, position, Quaternion.identity);
    }

    private void LoadImage(Value value) {
        var url = value.StringValue;
        Debug.Log(url);
        StartCoroutine(GetTexture(url));
    }

    private void PlayAudio(AudioClip result) {
        _audioPlay.clip = result;
        _audioPlay.Play();
    }

    private IEnumerator GetTexture(string url) {
        image = Instantiate(Resources.Load<GameObject>(imageResponsePrefab));
        image.SetActive(false);
        var request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else {
            var texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
            var contentQuad = image.transform.Find("ContentQuad").gameObject;
            var meshRenderer = contentQuad.GetComponent<MeshRenderer>();
            meshRenderer.material.mainTexture = texture;
            image.SetActive(true);
        }
    }

    private void PlayVideo(string url) {
        video = Instantiate(Resources.Load<GameObject>(videoResponsePrefab));
        var videoPlayer = video.GetComponent<VideoPlayer>();
        videoPlayer.url = url;
        videoPlayer.Play();
    }

    public void OnButtonRecord() {
        Debug.Log("OnButtonRecord");
        if (image != null) Destroy(image);
        if (video != null) Destroy(video);

        _audioPlay.Stop();
        _client.OnButtonRecord();
    }

    private void Update() {
        if (!useKeyDown) return;
        if (Input.GetKeyDown(keyCode)) {
            OnButtonRecord();
        }
    }
}
