using UnityEngine;
using ZXing;

namespace Sotbify.MobileApp.Services.QRScanner
{
    public class Color32LuminanceSource : BaseLuminanceSource
    {
        public Color32LuminanceSource(int width, int height)
            : base(width, height)
        {
        }

        public Color32LuminanceSource(Color32[] pixels, int width, int height)
            : base(width, height)
        {
            SetPixels(pixels);
        }

        public void SetPixels(Color32[] pixels)
        {
            var num = 0;
            for (var index1 = Height - 1; index1 >= 0; --index1)
            {
                for (var index2 = 0; index2 < Width; ++index2)
                {
                    var color32 = pixels[index1 * Width + index2];
                    luminances[num++] = (byte) (color32.r + color32.g + color32.g + color32.b >> 2);
                }
            }
        }

        protected override LuminanceSource CreateLuminanceSource(
            byte[] newLuminances,
            int width,
            int height)
        {
            return new Color32LuminanceSource(width, height)
            {
                luminances = newLuminances
            };
        }
    }
}