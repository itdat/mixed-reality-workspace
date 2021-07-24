using System.Collections;
using AcquireChan.Scripts;
using Dialogflow.Handler;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.Video;

public class MixedRealityView : MonoBehaviour {
    private const string imageResponsePrefab = "ChatBot\\ImageResponsePlate";
    private const string videoResponsePrefab = "ChatBot\\VideoResponse";
    private const string audioErr = "ChatBot\\networkerr";
    public AudioSource audioPlay;
    private DialogFlowClient dialogFlowClient;
    private GameObject image;
    private GameObject video;
    public KeyCode keyCode;
    public CharacterAnimator animator;

    [FormerlySerializedAs("isUseKeyDown")]
    public bool useKeyDown;

    private void OnEnable() {
        audioPlay = GetComponent<AudioSource>();
        dialogFlowClient = GetComponent<DialogFlowClient>();

        dialogFlowClient.onAudioResponse.AddListener(PlayAudio);

        dialogFlowClient.registerOnParameters("imageUrl", LoadImage);
        dialogFlowClient.registerOnParameters("videoUrl", PlayVideo);
        dialogFlowClient.onDetectIntentResponse.AddListener(response => {
            if (response == null) {
                var audioClip = Resources.Load<AudioClip>(audioErr);
                audioPlay.clip = audioClip;
                audioPlay.Play();

                return;
            }

            switch (response.queryResult.action) {
                case "input.welcome":
                    animator.PlayWelcome();
                    break;
                case "thank":
                    animator.PlayThank();
                    break;
            }
        });
        dialogFlowClient.registerOnAction("show_table", ShowTables);
    }

    private static void ShowTables() {
        var go = Resources.Load<GameObject>("Tables\\Tables");
        var mainCam = Camera.main.transform;
        var rotation = mainCam.rotation;
        var position = mainCam.position + rotation * Vector3.forward * 3;
        Instantiate(go, position, Quaternion.identity);
    }

    private void LoadImage(string url) {
        Debug.Log(url);
        StartCoroutine(GetTexture(url));
    }

    private void PlayAudio(AudioClip result) {
        audioPlay.clip = result;
        audioPlay.Play();
        animator.PlayTalking(result.length);
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

        audioPlay.Stop();
        dialogFlowClient.OnButtonRecord();
    }

    private string[] text = {
        "Chào bạn", "Cửa hàng có mặt hàng gì", "Cách để xem sản phẩm", "Cho mình xem các loại bàn",
        "Chiếc bàn thứ nhất có màu khác không", "Kích thước như thế nào", "Cho mình đặt chiếc bàn này", "Cảm ơn"
    };

    private int i = 0;

    private void Update() {
        if (!useKeyDown) return;
        if (Input.GetKeyDown(keyCode)) {
            OnButtonRecord();
            // dialogFlowClient.SendText(text[i++]);
        }
    }
}
