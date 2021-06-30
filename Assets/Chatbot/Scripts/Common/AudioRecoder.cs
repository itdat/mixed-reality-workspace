using UnityEngine;
using UnityEngine.Events;

namespace ChatBot {
    public class AudioRecorder {
        protected readonly string microphoneName;

        public class AudioRecordEvent : UnityEvent<AudioClip> { }

        public class AudioRecordStateEvent : UnityEvent<bool> { }

        public readonly AudioRecordEvent onStartRecord;
        public readonly AudioRecordStateEvent onStateRecordChange;

        public AudioRecorder(string microphoneName) {
            this.microphoneName = microphoneName;
            onStartRecord = new AudioRecordEvent();
            onStateRecordChange = new AudioRecordStateEvent();
        }


        protected AudioClip _recordedAudioClip;

        protected float _startRecordingTime;

        public bool isRecording;

        public void OnButtonRecord() {
            if (!isRecording)
                StartRecord();
            else
                StopRecord();
        }

        public virtual AudioClip StopRecord() {
            Microphone.End(microphoneName);
            isRecording = false;
            //Trim the audioclip by the length of the recording
            AudioClip recordingNew = AudioClip.Create(_recordedAudioClip.name,
                (int) ((Time.time - _startRecordingTime) * _recordedAudioClip.frequency), _recordedAudioClip.channels,
                _recordedAudioClip.frequency, false);
            var data = new float[(int) ((Time.time - _startRecordingTime) * _recordedAudioClip.frequency)];
            _recordedAudioClip.GetData(data, 0);
            recordingNew.SetData(data, 0);
            onStateRecordChange.Invoke(isRecording);
            return recordingNew;
        }

        protected void StartRecord() {
            _recordedAudioClip = Microphone.Start(microphoneName, false, 60, 44100);
            _startRecordingTime = Time.time;
            isRecording = true;
            onStartRecord.Invoke(_recordedAudioClip);
            onStateRecordChange.Invoke(isRecording);
        }
    }
}
