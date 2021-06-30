namespace ChatBot.Utils.AudioUtils {
    public readonly struct AudioClipInfo {
        public readonly int Samples;
        public readonly int Channels;
        public readonly int Frequency;

        public AudioClipInfo(int samples, int channels, int frequency) {
            Samples = samples;
            Channels = channels;
            Frequency = frequency;
        }
    }
}
