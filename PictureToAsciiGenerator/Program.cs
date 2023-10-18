using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Bitte geben Sie die URL des Bildes ein:");
        string url = Console.ReadLine();

        Console.WriteLine("Bild wird heruntergeladen...");
        var image = await DownloadImageFromUrl(url);

        Console.WriteLine("Bild wird verarbeitet...");
        image = ResizeImage(image, (int)(image.Width * 0.8), (int)(image.Height * 0.3)); // Breite um 20% reduziert und Höhe um 70% reduziert
        var coloredAsciiArt = ConvertImageToColoredAscii(image);

        Console.WriteLine("ASCII-Art generiert:");
        Console.WriteLine(coloredAsciiArt);
    }

    static async Task<System.Drawing.Image> DownloadImageFromUrl(string url)
    {
        using var httpClient = new HttpClient();
        var imageData = await httpClient.GetByteArrayAsync(url);

        using var memoryStream = new MemoryStream(imageData);
        return System.Drawing.Image.FromStream(memoryStream);
    }

    static System.Drawing.Image ResizeImage(System.Drawing.Image image, int width, int height)
    {
        var resizedImage = new Bitmap(image, new Size(width, height));
        return resizedImage;
    }

    static string ConvertImageToColoredAscii(System.Drawing.Image image)
    {
        var coloredAsciiArt = "";
        string asciiChars = "@%#*+=-:. ";
        string colorCodes = "\u001b[48;2;{0};{1};{2}m"; // Farb-ANSI-Code

        int totalPixels = image.Width * image.Height;
        int processedPixels = 0;

        for (int h = 0; h < image.Height; h++)
        {
            for (int w = 0; w < image.Width; w++)
            {
                var pixelColor = ((Bitmap)image).GetPixel(w, h);
                int grayScale = (int)((pixelColor.R + pixelColor.G + pixelColor.B) / 3.0);

                int index = (grayScale * (asciiChars.Length - 1)) / 255;
                coloredAsciiArt += string.Format(colorCodes, pixelColor.R, pixelColor.G, pixelColor.B) + asciiChars[index];

                processedPixels++;
                int progress = (int)(((double)processedPixels / totalPixels) * 100);
                Console.Write($"\rLadebalken: [{new string('#', progress / 2)}{new string(' ', 50 - progress / 2)}] {progress}%");
            }

            coloredAsciiArt += "\u001b[0m\n";
        }

        return coloredAsciiArt;
    }
}