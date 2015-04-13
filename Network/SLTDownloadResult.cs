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
        }

        public SLTDownloadResult(WWW www)
            : this(www.error)
        {
            Text = www.text;
            Bytes = www.bytes;
            Texture = www.texture;
        }
    }
}