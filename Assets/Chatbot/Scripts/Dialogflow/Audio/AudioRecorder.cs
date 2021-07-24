using System;
using ChatBot.Utils.AudioUtils;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogflow.Audio {
    public class AudioRecorder {
        private readonly string microphoneName;

        public class AudioRecordEvent : UnityEvent<AudioClip> { }

        public class AudioRecordStateEvent : UnityEvent<bool> { }

        public readonly AudioRecordEvent onStartRecord;
        public readonly AudioRecordStateEvent onStateRecordChange;

        public AudioRecorder(string microphoneName) {
            this.microphoneName = microphoneName;
            onStartRecord = new AudioRecordEvent();
            onStateRecordChange = new AudioRecordStateEvent();
        }

        [Serializable]
        public class DialogFlowRecordEvent : UnityEvent<AudioClipInfo, float[]> { }

        public DialogFlowRecordEvent OnStopRecord = new DialogFlowRecordEvent();
        private AudioClip _recordedAudioClip;

        private float _startRecordingTime;

        public bool isRecording;

        public AudioClip StartRecord() {
            isRecording = true;
            _recordedAudioClip = Microphone.Start(microphoneName, false, 60, 44100);
            _startRecordingTime = Time.time;
            onStateRecordChange.Invoke(true);
            return _recordedAudioClip;
        }

        public AudioClip StopRecord(out AudioClipInfo audioClipInfo, out float[] data) {
            isRecording = false;
            Debug.Log("Stop record");
            Microphone.End(microphoneName);
            //Trim the audioclip by the length of the recording
            var recordingNew = AudioClip.Create(_recordedAudioClip.name,
                (int) ((Time.time - _startRecordingTime) * _recordedAudioClip.frequency), _recordedAudioClip.channels,
                _recordedAudioClip.frequency, false);
            data = new float[(int) ((Time.time - _startRecordingTime) * _recordedAudioClip.frequency)];
            _recordedAudioClip.GetData(data, 0);
            audioClipInfo = new AudioClipInfo(recordingNew.samples, recordingNew.channels, recordingNew.frequency);
            OnStopRecord.Invoke(audioClipInfo, data);
            onStateRecordChange.Invoke(default);
            return _recordedAudioClip;
        }
    }
}
