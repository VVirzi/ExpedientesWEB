using Expedientes.Application.Exporters;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;


namespace Expedientes.Infrastructure.Pdf
{
    public class QrPdfExporter : IQrPdfExporter
    {
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        private readonly QrGenerator _qrGenerator = new();

        public byte[] Export(List<(string content, string label)> qrItems)
        {
            var ms = new MemoryStream();
            using (var writer = new PdfWriter(ms))
            using (var pdf = new PdfDocument(writer))
            using (var document = new Document(pdf, PageSize.A4))
            {
                document.SetMargins(MmToPt(20), MmToPt(20), MmToPt(20), MmToPt(20));

                const float qrSize = 170f;
                float usableWidth = MmToPt(170);

                var table = new Table(UnitValue.CreatePointArray(new float[] { usableWidth }));

                foreach (var (content, label) in qrItems)
                {
                    byte[] qrBytes = _qrGenerator.GenerateQr(content);

                    var qrImage = new Image(ImageDataFactory.Create(qrBytes))
                        .ScaleAbsolute(qrSize, qrSize)
                        .SetHorizontalAlignment(HorizontalAlignment.CENTER);

                    var labelParagraph = new Paragraph(label)
                        .SetFontSize(10)
                        .SetMargin(4)
                        .SetTextAlignment(TextAlignment.CENTER);

                    var cell = new Cell()
                        .Add(qrImage)
                        .Add(labelParagraph)
                        .SetPadding(8)
                        .SetTextAlignment(TextAlignment.CENTER);

                    table.AddCell(cell);
                }
                document.Add(table);
            }
            return ms.ToArray();
        }

        private float MmToPt(float mm) => mm * 72f / 25.4f;
    }
}
