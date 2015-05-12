using UnityEngine;

namespace Saltr.UnitySdk.Network
{
    public class SLTDownloadResult
    {
        public string Text { get; set; }

        public byte[] Bytes { get; set; }

        public string Error { get; set; }

        public Texture2D Texture { get; set; }

        public object StateObject { get; set; }

        public SLTDownloadResult(string error)
        {
            Error = error;
            Text = string.Empty;
        }

        public SLTDownloadResult(string error, string text, byte[] bytes = null, Texture2D texture = null)
            : this(error)
        {
            Text = text ?? string.Empty;
            Bytes = bytes;
            Texture = texture;
        }
    }
}