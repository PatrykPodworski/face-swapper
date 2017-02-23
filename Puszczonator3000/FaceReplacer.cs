using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Puszczonator3000
{
    public class FaceReplacer
    {
        private readonly CascadeClassifier _cascadeClassifier;
        private readonly List<Bitmap> _faces;
        private readonly Random _random;

        public FaceReplacer()
        {
            _cascadeClassifier = new CascadeClassifier("xml/haarcascade_frontalface_alt.xml");
            _random = new Random();

            _faces = new List<Bitmap>();

            var directories = Directory.GetFiles("facesClean", "*.png", SearchOption.AllDirectories).ToList();

            foreach (var dir in directories)
            {
                _faces.Add(new Bitmap(dir));
            }
        }

        public Bitmap FaceSwap(Bitmap bitmap)
        {
            var grayImage = new Image<Gray, byte>(bitmap);
            var faces = _cascadeClassifier.DetectMultiScale(grayImage, 1.1, 10, Size.Empty);
            var graphic = Graphics.FromImage(bitmap);

            foreach (var face in faces)
            {
                if (_faces.Count == 0)
                    break;

                const float scale = 0.3f;
                var x = face.X - face.Width * scale;
                var y = face.Y - face.Height * scale;
                var xSize = (int)(face.Width * (1 + 2 * scale));
                var ySize = (int)(face.Height * (1 + 2 * scale));
                var faceIndex = _random.Next(_faces.Count);
                var faceTodraw = ResizeImage(_faces[faceIndex], xSize, ySize);
                graphic.DrawImage(faceTodraw, x, y, xSize, ySize);
                //_faces.RemoveAt(faceIndex);
            }

            return bitmap;
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}