using System;
using System.Collections.Generic;
using AudioUtils;
using ChatBot.ThreadHandler;
using ChatBot.Utils.AudioUtils;
using Dialogflow.Event;
using UnityEngine;
using UnityEngine.Events;
using AudioRecorder = Dialogflow.Audio.AudioRecorder;

namespace Dialogflow.Handler {
    [RequireComponent(typeof(SessionClient))]
    public sealed class DialogFlowClient : MonoBehaviour {
        private SessionClient SessionClient;
        private static string defaultSessionName = Guid.NewGuid().ToString();
        public AudioResponseEvent onAudioResponse = new AudioResponseEvent();
        public TextResponseEvent onTextResponse = new TextResponseEvent();
        public DetectIntentResponseEvent onDetectIntentResponse = new DetectIntentResponseEvent();
        private AudioRecorder recorder;
        public string microphoneName;

        private void OnEnable() {
            SessionClient = GetComponent<SessionClient>();
            // var settings = SessionClient.accessSettings;
            // var credentialsFilePath = $"Assets\\StreamingAssets\\{settings.CredentialsFileName}.json";
            // Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsFilePath);

            recorder = new AudioRecorder(microphoneName);
            recorder.OnStopRecord.AddListener(OnRecordEnd);
            SessionClient.ChatbotResponded += OnResponse;
            SessionClient.DetectIntentError += OnError;
        }

        private void OnResponse(Response response) {
            onDetectIntentResponse.Invoke(response);
            if (response == null) return;
            onTextResponse.Invoke(response.queryResult.fulfillmentText);
            if (response.OutputAudio.Length != 0) {
                var audioBytes = Convert.FromBase64String(response.OutputAudio);
                var audioClip = WavUtility.ToAudioClip(audioBytes);
                onAudioResponse.Invoke(audioClip);
            }

            var parameters = response.queryResult.parameters;
            if (parameters != null) {
                foreach (var param in response.queryResult.parameters.Keys) {
                    if (parameterCallbacks.ContainsKey(param))
                        parameterCallbacks[param].Invoke((string) parameters[param]);
                }
            }

            var action = response.queryResult.action;
            if (action != null && actionCallbacks.ContainsKey(action)) {
                actionCallbacks[response.queryResult.action].Invoke();
            }
        }

        private void OnError(ErrorResponse error) { }

        private string GetSessionName() {
            return defaultSessionName;
        }

        public void Clear() {
            SessionClient.ClearSession(GetSessionName());
            defaultSessionName = Guid.NewGuid().ToString();
        }

        private static bool IsNetworkAvailable => Application.internetReachability != NetworkReachability.NotReachable;

        private void OnRecordEnd(AudioClipInfo info, float[] data) { }

        public void SendAudio(string audioString) {
            SessionClient.DetectIntentFromAudio(audioString, GetSessionName());
        }

        public void SendText(string msg) {
            SessionClient.DetectIntentFromText(msg, GetSessionName());
        }

        private bool isRecording;

        public void OnButtonRecord() {
            if (!isRecording) {
                isRecording = true;
                recorder.StartRecord();
            }
            else {
                isRecording = false;
                recorder.StopRecord(out var info, out var data);

                if (!IsNetworkAvailable) {
                    OnResponse(null);
                    return;
                }

                Async.Run(() => {
                    var audioString = WavUtility.FromAudioClip(info, ref data);
                    return audioString;
                }).ContinueInMainThread(SendAudio);
            }
        }

        #region Parameter Callback Region

        private class ParameterCallback : UnityEvent<string> { }

        private Dictionary<string, ParameterCallback> parameterCallbacks;

        public void registerOnParameters(string paramName, UnityAction<string> action) {
            if (parameterCallbacks == null)
                parameterCallbacks = new Dictionary<string, ParameterCallback>();

            if (parameterCallbacks.ContainsKey(paramName)) {
                var parameterCallback = parameterCallbacks[paramName];
                parameterCallback.AddListener(action);
            }
            else {
                var parameterCallback = new ParameterCallback();
                parameterCallback.AddListener(action);
                parameterCallbacks.Add(paramName, parameterCallback);
            }
        }

        public void unregisterOnParameters<T>(string paramName, UnityAction<string> action) {
            if (!parameterCallbacks.ContainsKey(paramName)) return;
            var parameterCallback = parameterCallbacks[paramName];
            parameterCallback.RemoveListener(action);
        }

        #endregion

        #region Action Callback

        private class ActionCallback : UnityEvent { }

        private Dictionary<string, ActionCallback> actionCallbacks;

        public void registerOnAction(string paramName, UnityAction action) {
            if (actionCallbacks == null)
                actionCallbacks = new Dictionary<string, ActionCallback>();

            if (actionCallbacks.ContainsKey(paramName)) {
                var actionCallback = actionCallbacks[paramName];
                actionCallback.AddListener(action);
            }
            else {
                var actionCallback = new ActionCallback();
                actionCallback.AddListener(action);
                actionCallbacks.Add(paramName, actionCallback);
            }
        }

        public void unregisterOnAction(string paramName, UnityAction action) {
            if (!actionCallbacks.ContainsKey(paramName)) return;
            var actionCallback = actionCallbacks[paramName];
            actionCallback.RemoveListener(action);
        }

        #endregion

        public AudioRecorder GetAudioRecorder() {
            return recorder;
        }

        public string GetMicrophoneName() {
            return microphoneName;
        }
    }
}
