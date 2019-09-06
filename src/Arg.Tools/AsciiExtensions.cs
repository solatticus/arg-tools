using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Arg.Tools.Ascii
{
    public static class AsciiExtensions
    {
        private static readonly string[] _chars = { "#", "#", "@", "%", "=", "+", "*", ":", "-", ".", " " };

        public static string ToAscii(this Bitmap image)
        {
            var toggle = false;
            var sb = new StringBuilder();

            for (int h = 0; h < image.Height; h++)
            {
                for (int w = 0; w < image.Width; w++)
                {
                    var pixelColor = image.GetPixel(w, h);
                    
                    var red = (pixelColor.R + pixelColor.G + pixelColor.B) / 3; //Average out the RGB components to find the Gray Color
                    var green = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    var blue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    
                    var grayColor = Color.FromArgb(red, green, blue);

                    if (!toggle) //Use the toggle flag to minimize height-wise stretch
                    {
                        int index = (grayColor.R * 10) / 255;
                        sb.Append(_chars[index]);
                    }
                }

                if (!toggle)
                {
                    sb.Append("<BR>");
                    toggle = true;
                }
                else
                {
                    toggle = false;
                }
            }

            return sb.ToString();
        }

        public static Image Resize(this Image inputBitmap, int asciiWidth)
        {
            int asciiHeight = (int)Math.Ceiling((double)inputBitmap.Height * asciiWidth / inputBitmap.Width);

            //Create a new Bitmap and define its resolution
            var result = new Bitmap(asciiWidth, asciiHeight);
            using var g = Graphics.FromImage(result);

            //The interpolation mode produces high quality images
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(inputBitmap, 0, 0, asciiWidth, asciiHeight);
            g.Dispose();

            return result;
        }
    }
}
