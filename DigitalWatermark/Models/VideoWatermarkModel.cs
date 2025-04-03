namespace DigitalWatermark.Models
{
    public class VideoWatermarkModel
    {
        public string InputVideoPath { get; set; }
        public string OutputVideoPath { get; set; }
        public string WatermarkText { get; set; }
        public int FrameRate { get; set; } // Optional: may be auto-detected.
    }
}
