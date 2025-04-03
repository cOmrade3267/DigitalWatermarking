using System;
using System.Collections.Generic;
using System.Drawing;
using OpenCvSharp;
using static OpenCvSharp.LineIterator;

namespace DigitalWatermark.Services
{
    public class LSBWatermarkService
    {
        public static Bitmap EmbedWatermark(Bitmap original, string watermarkText)
        { 
            List<int> watermarkBits = new List<int>();
            int consecutiveOnce = 0;
            foreach (char c in watermarkText)
            {
                byte ascii = (byte)c;
                // Convert to 8 bits.
                for (int i = 7; i >= 0; i--)
                {
                    int bit = (ascii >> i) & 1;
                    watermarkBits.Add(bit);

                    watermarkBits.Add((ascii >> i) & 1);
                    if(bit == 1)
                    {
                        if(consecutiveOnce ==5)
                        {
                            watermarkBits.Add(0);
                            consecutiveOnce = 0;
                        }

                    }
                    else
                    {
                        consecutiveOnce = 0;
                    }
                }
            }
            List<int> flag = new List<int> { 0, 1, 1, 1, 1, 1, 1, 0 };
            List<int> stuffedWatermark = new List<int>();
            stuffedWatermark.AddRange(flag);       //performing bit stuffing
            stuffedWatermark.AddRange(watermarkBits);
            stuffedWatermark.AddRange(flag);

            Bitmap watermarked = new Bitmap(original); //copy of original image
            bool finished = false;
            int bitIndex = 0;
            for(int y=0; y< watermarked.Height && !finished;  y++)
            {
                for(int x=0; x< watermarked.Width && !finished; x++)
                {
                    Color pixel = watermarked.GetPixel(x, y);
                    int blue = pixel.B;
                    if(bitIndex < watermarkBits.Count)
                    {
                        blue = (blue & ~1) | stuffedWatermark[bitIndex]; //clearing LSB of image and set it as watermark bit 
                        bitIndex++;
                    }
                    else
                        finished = true;
                    Color newPixel = Color.FromArgb(pixel.R, pixel.G, blue);
                    watermarked.SetPixel(x, y, newPixel);
                }
            }
           return watermarked; 
        }

        public static string ExtractWatermark(Bitmap watermarked)
        {
            List<int> bits = new List<int>();

            for(int y=0; y<watermarked.Height; y++)
            {
                for(int x=0; x<watermarked.Width; x++)
                {
                    Color pixel = watermarked.GetPixel(x, y);
                    int bit = pixel.B & 1;
                    bits.Add(bit);
                }
            }
            // Now removing flag bit
            string watermark = "";

            for(int i=0; i<bits.Count; i += 8)
            {
                if (i + 8 < bits.Count)
                    break;
                int ascii = 0;
                for (int j = 0; j < 8; j++)
                {
                    ascii = (ascii << 1) | bits[i + j];
                }
                if (ascii == 0) // termination check
                    break;
                watermark += (char)ascii;
            }
            return watermark;
        }

    }
}

// @videh use: Bitmap watermarkedImage = LSBWatermarkService.EmbedWatermark(originalBitmap, "Your Watermark Text");

