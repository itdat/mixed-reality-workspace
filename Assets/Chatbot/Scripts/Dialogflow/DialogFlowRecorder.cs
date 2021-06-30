using System;
using ChatBot;
using ChatBot.Utils.AudioUtils;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogflow {
    public class DialogFlowRecorder : AudioRecorder {
        public DialogFlowRecorder(string microphoneName) : base(microphoneName) { }

        [Serializable]
        public class DialogFlowRecordEvent : UnityEvent<AudioClipInfo, float[]> { }

        public DialogFlowRecordEvent OnStopRecord = new DialogFlowRecordEvent();

        public override AudioClip StopRecord() {
            Microphone.End(microphoneName);
            isRecording = false;
            //Trim the audioclip by the length of the recording
            var recordingNew = AudioClip.Create(_recordedAudioClip.name,
                (int) ((Time.time - _startRecordingTime) * _recordedAudioClip.frequency), _recordedAudioClip.channels,
                _recordedAudioClip.frequency, false);
            var data = new float[(int) ((Time.time - _startRecordingTime) * _recordedAudioClip.frequency)];
            _recordedAudioClip.GetData(data, 0);
            var audioClipInfo = new AudioClipInfo(recordingNew.samples, recordingNew.channels, recordingNew.frequency);
            OnStopRecord.Invoke(audioClipInfo, data);
            onStateRecordChange.Invoke(isRecording);
            return null;
        }
    }
}
