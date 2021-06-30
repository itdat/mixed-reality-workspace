using System;
using System.Collections.Generic;
using ChatBot;
using ChatBot.ThreadHandler;
using ChatBot.Utils.AudioUtils;
using Dialogflow.Event;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogflow {
    public class DialogFlowClient : MonoBehaviour, ChatBotClient {
        private string _sessionId = Guid.NewGuid().ToString();
        private SessionsClientImpl _sessionsClient;
        private SessionName _sessionName;
        private DialogFlowRecorder _recorder;

        private static bool IsNetworkAvailable => Application.internetReachability != NetworkReachability.NotReachable;

        public string microphoneName;
        public DialogFlowSettings settings;
        public AudioResponseEvent onAudioResponse = new AudioResponseEvent();
        public TextResponseEvent onTextResponse = new TextResponseEvent();
        public DetectIntentResponseEvent onDetectIntentResponse = new DetectIntentResponseEvent();

        private void OnEnable() {
            var credentialsFilePath = $"Assets\\StreamingAssets\\{settings.CredentialsFileName}.json";
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsFilePath);

            _sessionsClient = (SessionsClientImpl) SessionsClient.Create();
            _sessionName = SessionName.FromProjectSession(settings.ProjectId, _sessionId);

            _recorder = new DialogFlowRecorder(microphoneName);
            _recorder.OnStopRecord.AddListener(OnRecordEnd);
        }


        protected void OnRecordEnd(AudioClipInfo info, float[] data) {
            if (!IsNetworkAvailable) {
                UpdateResponse(null);
                return;
            }

            Async.Run(() => {
                var byteString = AudioUtils.FromAudioClip(info, ref data);
                var result = SendAudio(byteString);
                return result;
            }).ContinueInMainThread(UpdateResponse);
        }


        private void UpdateResponse(DetectIntentResponse response) {
            onDetectIntentResponse.Invoke(response);
            if (response == null) return;
            onTextResponse.Invoke(response.QueryResult.FulfillmentText);
            if (!response.OutputAudio.IsEmpty) {
                var audioClip = WavUtility.ToAudioClip(response.OutputAudio.ToByteArray());
                onAudioResponse.Invoke(audioClip);
            }

            if (response.QueryResult.Parameters != null) {
                foreach (var keyValuePair in response.QueryResult.Parameters.Fields) {
                    if (_callbacks.ContainsKey(keyValuePair.Key))
                        _callbacks[keyValuePair.Key].Invoke(keyValuePair.Value);
                }
            }
        }

        public void SendText(string text) {
            if (!IsNetworkAvailable) {
                UpdateResponse(null);
                return;
            }

            Async.Run(() => {
                var textInput = new TextInput {
                    Text = text,
                    LanguageCode = settings.LanguageCode
                };
                var queryInput = new QueryInput { Text = textInput };

                var request = new DetectIntentRequest {
                    SessionAsSessionName = _sessionName,
                    QueryInput = queryInput
                };
                var response = _sessionsClient.DetectIntent(request);
                return response;
            }).ContinueInMainThread(UpdateResponse);
        }

        public AudioRecorder GetAudioRecorder() {
            return _recorder;
        }

        public string GetMicrophoneName() {
            return microphoneName;
        }

        private DetectIntentResponse SendAudio(ByteString data) {
            var config = new InputAudioConfig { LanguageCode = settings.LanguageCode };
            var queryInput = new QueryInput { AudioConfig = config };
            var request = new DetectIntentRequest {
                InputAudio = data, QueryInput = queryInput, Session = _sessionName.ToString()
            };
            var response = _sessionsClient.DetectIntent(request);
            return response;
        }

        public void Clear() {
            _sessionId = Guid.NewGuid().ToString();
            _sessionName = SessionName.FromProjectSession(settings.ProjectId, _sessionId);
        }

        public void OnButtonRecord() {
            _recorder.OnButtonRecord();
        }

        #region Parameter Callback Region

        private class ParameterCallback : UnityEvent<Value> { }

        private Dictionary<string, ParameterCallback> _callbacks;

        public void registerOnParameters(string paramName, UnityAction<Value> action) {
            if (_callbacks == null)
                _callbacks = new Dictionary<string, ParameterCallback>();

            if (_callbacks.ContainsKey(paramName)) {
                var parameterCallback = _callbacks[paramName];
                parameterCallback.AddListener(action);
            }
            else {
                var parameterCallback = new ParameterCallback();
                parameterCallback.AddListener(action);
                _callbacks.Add(paramName, parameterCallback);
            }
        }

        public void unregisterOnParameters<T>(string paramName, UnityAction<Value> action) {
            if (!_callbacks.ContainsKey(paramName)) return;
            var parameterCallback = _callbacks[paramName];
            parameterCallback.RemoveListener(action);
        }

        #endregion
    }
}
