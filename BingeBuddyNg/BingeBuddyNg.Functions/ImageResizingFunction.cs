using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.IO;

namespace BingeBuddyNg.Functions
{
    public class ImageResizingFunction
    {
        private const int ResizeMaxWidth = 1024;
        private const int ResizeMaxHeight = 768;

        [FunctionName(nameof(ImageResizingFunction))]
        public void Run([BlobTrigger("img/{name}", Connection = "AzureWebJobsStorage")] Stream strm, string name,
                        [Blob("img/{name}", FileAccess.Write, Connection = "AzureWebJobsStorage")] out byte[] resizedData,
                        ILogger log)
        {
            Image<SixLabors.ImageSharp.PixelFormats.Rgba32> img = null;

            try
            {
                img = Image.Load(strm);
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Error processing image [{name}]");
                resizedData = null;
                return;
            }


            if (img.Width > ResizeMaxWidth || img.Height > ResizeMaxHeight)
            {
                double ratio = img.Width / (double)img.Height;

                // landscape
                if (ratio > 1)
                {
                    log.LogInformation($"Resizing image with width {img.Width} and height {img.Height} ...");
                    int height = (int)(ResizeMaxHeight / ratio);
                    img.Mutate(x => x.AutoOrient().Resize(new ResizeOptions() { Size = new Size(ResizeMaxWidth, height), Mode = ResizeMode.Max }));
                }
                else
                {
                    int width = (int)(ResizeMaxWidth * ratio);
                    img.Mutate(x => x.AutoOrient().Resize(new ResizeOptions() { Size = new Size(width, ResizeMaxHeight), Mode = ResizeMode.Max }));
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


        private IImageEncoder GetEncoderFromFileName(string name)
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
