using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Windows.WebCam;
using Utils;

namespace Photon {
    public class PhotoCaptureHandler : MonoBehaviourPunCallbacks {
        PhotoCapture photoCaptureObject;
        private Texture2D targetTexture;
        private bool isTaking;
        public AudioClip soundEffect;
        private AudioSource audioSource;
        public bool isCaptureScreen;

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
            if (isCaptureScreen) {
                var screenshot = ScreenCapture.CaptureScreenshotAsTexture();
                var size = ExpectSize(screenshot, targetTexture);
                var width = size.Key;
                var height = size.Value;
                if (width != screenshot.width || height != screenshot.height)
                    screenshot = ImageHelpers.ResampleAndCrop(screenshot, width, height);

                if (width != targetTexture.width || height != targetTexture.height)
                    targetTexture = ImageHelpers.ResampleAndCrop(targetTexture, width, height);

                targetTexture = ImageHelpers.AlphaBlend(targetTexture, screenshot);
            }

            photonView.RPC("ShowImage", RpcTarget.Others, targetTexture.EncodeToJPG(), targetTexture.width,
                targetTexture.height);
            photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
            isTaking = false;
        }

        private static KeyValuePair<int, int> ExpectSize(Texture texture1, Texture texture2) {
            var width = Math.Min(texture1.width, texture2.width);
            var height = Math.Min(texture1.height, texture2.height);
            var res = new KeyValuePair<int, int>(width, height);
            return res;
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
