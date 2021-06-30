namespace ChatBot {
    public interface ChatBotClient {
        void SendText(string text);
        AudioRecorder GetAudioRecorder();
        string GetMicrophoneName();
    }
}
