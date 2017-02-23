using System.Drawing;
using System.IO;
using Puszczonator3000;

namespace Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // loading an image
            const string source = "src/image.png";
            Bitmap bitmap;

            try
            {
                bitmap = new Bitmap(source);
            }
            catch (FileNotFoundException)
            {
                System.Console.WriteLine(source + " is not a valid image!");
                System.Console.ReadKey();
                return;
            }

            // initalization of FaceReplacer
            var faceReplacer = new FaceReplacer();

            // face swaping
            var result = faceReplacer.FaceSwap(bitmap);

            // saving result
            const string destination = "src/result.png";
            result.Save(destination);
        }
    }
}
