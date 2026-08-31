using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace Expedientes.Infrastructure.Pdf
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class QrGenerator
    {
        public byte[] GenerateQr(string content)
        {
            var hints = new Dictionary<EncodeHintType, object>
            {
                { EncodeHintType.MARGIN, 0 }
            };

            var writer = new QRCodeWriter();
            var bitMatrix = writer.encode(content, BarcodeFormat.QR_CODE, 300, 300, hints);

            int width = bitMatrix.Width;
            int height = bitMatrix.Height;

            using var bitmap = new System.Drawing.Bitmap(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bitmap.SetPixel(x, y, bitMatrix[x, y]
                        ? System.Drawing.Color.Black
                        : System.Drawing.Color.White);
                }
            }

            using var ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }
    }
}