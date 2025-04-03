using OpenCvSharp;
using OpenCvSharp.Extensions; // For BitmapConverter
using System;
using System.Drawing;         // For Bitmap
using DigitalWatermark.Models;  // Your VideoWatermarkModel class
using DigitalWatermark.Services; // Contains LSBWatermarkService with EmbedWatermark method

namespace DigitalWatermark.Services
{
    public class VideoWatermarkService
    {
        /// Processes the input video by embedding the watermark on each frame and writing the output video.
        /// <param name="model">The video watermark model containing file paths and watermark text.</param>
        public void ApplyWatermark(VideoWatermarkModel model)
        {
            // Open the video file.
            using (VideoCapture capture = new VideoCapture(model.InputVideoPath))
            {
                if (!capture.IsOpened())
                    throw new Exception("Cannot open video file");

                int frameWidth = capture.FrameWidth;
                int frameHeight = capture.FrameHeight;
                double fps = capture.Fps;

                // Prepare the VideoWriter to write the watermarked video.
                using (VideoWriter writer = new VideoWriter(model.OutputVideoPath,
                                                             FourCC.H264, fps,
                                                             new OpenCvSharp.Size(frameWidth, frameHeight)))
                {
                    Mat frame = new Mat();
                    while (capture.Read(frame))
                    {
                        if (frame.Empty())
                            break;

                        // Convert the frame from Mat to Bitmap.
                        Bitmap bitmapFrame = BitmapConverter.ToBitmap(frame);

                        // Apply LSB watermarking with bit stuffing on the Bitmap.
                        Bitmap watermarkedBitmap = LSBWatermarkService.EmbedWatermark(bitmapFrame, model.WatermarkText);

                        // Convert the watermarked Bitmap back to Mat.
                        Mat watermarkedFrame = BitmapConverter.ToMat(watermarkedBitmap);

                        // Write the watermarked frame to the output video.
                        writer.Write(watermarkedFrame);
                    }
                }
            }
        }
    }
}
