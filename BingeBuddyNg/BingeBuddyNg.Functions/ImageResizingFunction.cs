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

namespace BingeBuddyNg.Functions
{
    public static class ImageResizingFunction
    {
        private const int ResizeMaxWidth = 1024;
        private const int ResizeMaxHeight = 768;

        [FunctionName("ImageResizingFunction")]
        public static void Run(
            [BlobTrigger("img/{name}", Connection = "AzureWebJobsStorage")]Stream strm, string name,
             [Blob("img/{name}", FileAccess.Write, Connection = "AzureWebJobsStorage")]out byte[] resizedData,
            ILogger log)
        {
            var img = Image.Load(strm);
            
            if (img.Width > ResizeMaxWidth || img.Height > ResizeMaxHeight)
            {
                double ratio = img.Width / (double)img.Height;

                // landscape
                if (ratio > 1)
                {
                    log.LogInformation($"Resizing image with width {img.Width} and height {img.Height} ...");
                    int height = (int)(ResizeMaxHeight / ratio);
                    img.Mutate(x => x.AutoOrient().Resize(ResizeMaxWidth, height));
                }
                else
                {
                    int width = (int)(ResizeMaxWidth * ratio);
                    img.Mutate(x => x.AutoOrient().Resize(width, ResizeMaxHeight));
                }


                IImageEncoder encoder = GetEncoderFromFileName(name);
                MemoryStream resizedStream = new MemoryStream();
                img.Save(resizedStream, encoder);
                resizedData = resizedStream.ToArray();
            }
            else
            {
                log.LogInformation($"Image with measuremnts width {img.Width} and height {img.Height}. No resizing necessary.");
                resizedData = null;
            }

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
