using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace DataReef.TM.Services.Extensions
{
    public static class ImageExtensions
    {
        public static byte[] ToByteArray(this Image image)
        {
            var myImageCodecInfo = GetEncoderInfo("image/jpeg");
            var encoder = Encoder.Quality;
            var encoderParameter = new EncoderParameter(encoder, 50L);

            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = encoderParameter;

            var ms = new MemoryStream();
            image.Save(ms, myImageCodecInfo, encoderParameters);

            return ms.ToArray();
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            return ImageCodecInfo
                .GetImageDecoders()
                .FirstOrDefault(c => string.Equals(c.MimeType, mimeType, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
