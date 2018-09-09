using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BingeBuddyNg.Functions
{
    public static class ImageResizingFunction
    {
        private const int ResizeMaxWidth = 1024;

        [FunctionName("ImageResizingFunction")]
        public static void Run(
            [BlobTrigger("img/{name}", Connection = "AzureWebJobsStorage")]Stream strm, string name,
            [Blob("imgresized/{name}", FileAccess.Write, Connection = "AzureWebJobsStorage")]Stream resizedStream,
            ILogger log)
        {
            var img = Image.Load(strm);

            if (img.Width > ResizeMaxWidth)
            {
                double ratio = img.Width / (double)img.Height;
                int height = (int)(img.Height / ratio);
                img.Mutate(x => x.Resize(new ResizeOptions() { Size = new SixLabors.Primitives.Size(ResizeMaxWidth, height) }));
            }

            IImageEncoder encoder = GetEncoderFromFileName(name);
            img.Save(resizedStream, encoder);
        }

        private static IImageEncoder GetEncoderFromFileName(string name)
        {
            string ext = Path.GetExtension(name);
            switch (ext)
            {
                case ".png":
                    return new PngEncoder();
                case ".jpg":
                case ".jpeg":
                    return new JpegEncoder();
                case ".gif":
                    return new GifEncoder();
                default:
                    throw new InvalidOperationException($"Unsupported file format {ext}");
            }
        }
    }
}
