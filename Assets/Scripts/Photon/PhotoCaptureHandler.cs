using System.Collections;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Windows.WebCam;

namespace Photon {
    public class PhotoCaptureHandler : MonoBehaviourPunCallbacks {
        PhotoCapture photoCaptureObject;
        private Texture2D targetTexture;
        private bool isTaking;
        public AudioClip soundEffect;
        private AudioSource audioSource;

        public void TakePictureWithCountDown() {
            if (isTaking) return;
            StartCoroutine(TakePicture());
        }

        private IEnumerator TakePicture() {
            isTaking = true;
            var timer = Instantiate(Resources.Load<GameObject>("Timer"));
            var countDown = timer.GetComponent<CountDown>();
            countDown.StartCountDown(3);
            while (countDown.timerIsRunning) {
                yield return null;
            }

            Destroy(timer);

            var cameraResolution =
                PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

            // Create a PhotoCapture object
            PhotoCapture.CreateAsync(false, delegate(PhotoCapture captureObject) {
                photoCaptureObject = captureObject;
                var cameraParameters = new CameraParameters {
                    hologramOpacity = 0.0f,
                    cameraResolutionWidth = cameraResolution.width,
                    cameraResolutionHeight = cameraResolution.height,
                    pixelFormat = CapturePixelFormat.BGRA32
                };

                // Activate the camera
                photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate {
                    // Take a picture
                    photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
                });
            });

            if (soundEffect == null) yield break;
            if (audioSource == null)
                audioSource = FindObjectOfType<AudioSource>();
            audioSource.PlayOneShot(soundEffect);
        }

        private void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result,
            PhotoCaptureFrame photoCaptureFrame) {
            // Copy the raw image data into the target texture
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);
            photonView.RPC("ShowImage", RpcTarget.Others, targetTexture.EncodeToJPG(), targetTexture.width,
                targetTexture.height);
            photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
            isTaking = false;
        }

        private void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result) {
            // Shutdown the photo capture resource
            photoCaptureObject.Dispose();
            photoCaptureObject = null;
        }

        [PunRPC]
        private void ShowImage(byte[] data, int width, int height) {
            Debug.Log("ShowImage " + data.Length);
            var image = Instantiate(Resources.Load<GameObject>("ChatBot\\ImageResponsePlate"));
            var contentQuad = image.transform.Find("ContentQuad").gameObject;
            var meshRenderer = contentQuad.GetComponent<MeshRenderer>();
            var texture = new Texture2D(width, height);
            texture.LoadImage(data);
            meshRenderer.material.mainTexture = texture;
            image.SetActive(true);
        }
    }
}
