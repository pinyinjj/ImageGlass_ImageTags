using System.Text.Json.Serialization;

namespace ImageTagger
{
    // This class mirrors the JSON structure provided by ImageGlass.Tools for an image.
    // It's used to correctly parse the FilePath from the message data.
    public class ImageGlassImageData
    {
        [JsonPropertyName("FrameIndex")]
        public int FrameIndex { get; set; }

        [JsonPropertyName("IsError")]
        public bool IsError { get; set; }

        [JsonPropertyName("IsViewingSeparateFrame")]
        public bool IsViewingSeparateFrame { get; set; }

        [JsonPropertyName("Index")]
        public int Index { get; set; }

        [JsonPropertyName("FilePath")]
        public string? FilePath { get; set; }
    }
}
