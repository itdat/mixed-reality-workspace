using System.Collections;
using System.Linq;
using Dialogflow;
using Dialogflow.Audio;
using Dialogflow.Handler;
using UnityEngine;

namespace ChatBot {
    public class AudioRecordHandler : MonoBehaviour {
        private GameObject audioSourcePrefab;
        public float minimumLevel = (float) 0.0001;
        public float timeOutStopRecord = (float) 0.5;

        public bool autoStopRecord;
        private AudioRecorder recorder;
        private AudioSource _audioSource;
        private DialogFlowClient _client;

        private void OnEnable() {
            audioSourcePrefab = Instantiate(Resources.Load<GameObject>("ChatBot\\AudioSourcePrefab"));
            _audioSource = audioSourcePrefab.GetComponent<AudioSource>();
        }

        private void Start() {
            _client = GetComponent<DialogFlowClient>();
            recorder = _client.GetAudioRecorder();

            if (autoStopRecord)
                recorder.onStartRecord.AddListener(CheckStopSpeak);
        }

        private void CheckStopSpeak(AudioClip audioClip) {
            while (!(Microphone.GetPosition(_client.GetMicrophoneName()) > 0)) { }

            _audioSource.clip = audioClip;

            _audioSource.Play();
            StartCoroutine(InternalCheckStopSpeak());
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private readonly float[] _clipSampleData = new float[4096];

        private IEnumerator InternalCheckStopSpeak() {
            float startSpeak = 0;
            while (recorder.isRecording) {
                _audioSource.GetSpectrumData(_clipSampleData, 0, FFTWindow.Rectangular);
                var currentAverageVolume = _clipSampleData.Average();
                if (currentAverageVolume < minimumLevel) {
                    var endSpeak = Time.time;
                    if (startSpeak != 0 && endSpeak - startSpeak >= timeOutStopRecord) {
                        recorder.StopRecord(out var info, out var data);
                        Debug.Log("User stop speak, end record!!!");
                    }
                    else startSpeak = 0;
                }
                else if (startSpeak == 0)
                    startSpeak = Time.time;

                yield return new WaitForSeconds((float) 0.5);
            }
        }

        private void OnDestroy() {
            Destroy(audioSourcePrefab);
        }
    }
}
