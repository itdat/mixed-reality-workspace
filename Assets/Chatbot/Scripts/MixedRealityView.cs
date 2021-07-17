using System.Collections;
using Dialogflow;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class MixedRealityView : MonoBehaviour {
    private const string imageResponsePrefab = "ChatBot\\ImageResponsePlate";
    private const string videoResponsePrefab = "ChatBot\\VideoResponse";
    private const string audioErr = "ChatBot\\networkerr";
    private AudioSource _audioPlay;
    private DialogFlowClient _client;
    private GameObject image;
    private GameObject video;

    [SerializeField] private GameObject target;

    private void OnEnable() {
        _audioPlay = GetComponent<AudioSource>();
        _client = GetComponent<DialogFlowClient>();

        _client.onAudioResponse.AddListener(result => {
            _audioPlay.clip = result;
            _audioPlay.Play();
        });

        _client.registerOnParameters("imageUrl", value => {
            var url = value.StringValue;
            Debug.Log(url);
            StartCoroutine(GetTexture(url));
        });
        _client.registerOnParameters("videoUrl", value => { PlayVideo(value.StringValue); });
        _client.onDetectIntentResponse.AddListener(response => {
            if (response != null) return;
            var audioClip = Resources.Load<AudioClip>(audioErr);
            _audioPlay.clip = audioClip;
            _audioPlay.Play();
        });
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
}
