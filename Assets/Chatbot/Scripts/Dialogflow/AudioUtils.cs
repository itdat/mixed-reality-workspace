using System;
using System.IO;
using System.Text;
using ChatBot.Utils.AudioUtils;
using Google.Protobuf;
using UnityEngine;

namespace Dialogflow {
    public class AudioUtils : WavUtility {
        public static ByteString FromAudioClip(AudioClipInfo audioClip, ref float[] data) {
            var stream = new MemoryStream();
            const int headerSize = 44;

            ushort bitDepth = 16; //BitDepth (audioClip);

            var fileSize = audioClip.Samples * BlockSize_16Bit + headerSize; // BlockSize (bitDepth)

            WriteFileHeader(ref stream, fileSize);
            WriteFileFormat(ref stream, audioClip.Channels, audioClip.Frequency, bitDepth);
            WriteFileData(ref stream, audioClip, ref data);

            var result = ByteString.CopyFrom(stream.ToArray());
            stream.Dispose();
            return result;
        }

        private static int WriteFileData(ref MemoryStream stream, AudioClipInfo audioClip, ref float[] data) {
            int count = 0;
            int total = 8;

            byte[] bytes = ConvertAudioClipDataToInt16ByteArray(data);

            byte[] id = Encoding.ASCII.GetBytes("data");
            count += WriteBytesToMemoryStream(ref stream, id, "DATA_ID");

            int subchunk2Size = Convert.ToInt32(audioClip.Samples * BlockSize_16Bit); // BlockSize (bitDepth)
            count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(subchunk2Size), "SAMPLES");

            // Validate header
            Debug.AssertFormat(count == total, "Unexpected wav data id byte count: {0} == {1}", count, total);

            // Write bytes to stream
            count += WriteBytesToMemoryStream(ref stream, bytes, "DATA");

            // Validate audio data
            Debug.AssertFormat(bytes.Length == subchunk2Size, "Unexpected AudioClip to wav subchunk2 size: {0} == {1}",
                bytes.Length, subchunk2Size);

            return count;
        }
    }
}
