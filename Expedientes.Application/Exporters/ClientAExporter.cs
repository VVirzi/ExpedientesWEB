using Expedientes.Application.DTOs;

namespace Expedientes.Application.Exporters
{
    public class ClientAExporter : IInvoiceExporter
    {
        private readonly IQrPdfExporter _pdfExporter;
        public string ClientId => "ClientA";
        public string ExportType => "pdf";

        private const string requestType = "01";
        private const string supplierCode = "9002";
        public ClientAExporter(IQrPdfExporter pdfExporter)
        {
            _pdfExporter = pdfExporter;
        }
        public byte[] Export(InvoiceResultDto result)
        {
            var qrItems = new List<(string content, string label)>();

            foreach (var invoice in result.Invoices)
            {
                var remito = FormatRemito(invoice.RemitoNumber);
                var invoiceNumber =FormatInvoice(invoice.InvoiceNumber);
                var totalAmount = FormatAmount(invoice.TotalAmount);
                var date = FormatDate(invoice.Date);
                var requestNumber = (invoice.PurchaseOrder ?? "").Replace("/", "");
                var purchaseOrder = FormatPurchaseOrder(invoice.PurchaseOrder);
                var cae = invoice.CAE ?? "";
                var caeExpirationDate = FormatDate(invoice.CAEExpirationDate);

                string content =
                    remito +
                    invoiceNumber +
                    totalAmount +
                    date +
                    requestType +
                    requestNumber +
                    supplierCode +
                    purchaseOrder +
                    cae +
                    caeExpirationDate;

                qrItems.Add((content, invoice.InvoiceNumber));
            }
            return _pdfExporter.Export(qrItems);
        }

        private string FormatRemito(string remito)
        {
            string rto = (remito ?? "").Replace("-", "").Replace("R", "");
            return rto.Length > 12 ? rto.Substring(rto.Length - 12) : rto;
        }

        private string FormatInvoice(string invoice)
        {
            string fc = (invoice ?? "").Replace("-", "");
            return fc.PadLeft(12, '0');
        }

        private string FormatAmount(decimal price)
        {
            string raw = ((long)(price)).ToString();
            return raw.Length > 9 ? raw.Substring(raw.Length - 9) : raw.PadLeft(9, '0');
        }

        private string FormatDate(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("ddMMyyyy") : string.Empty;
        }

        private string FormatPurchaseOrder(string purchaseOrder)
        {
            string cleaned = (purchaseOrder ?? "").Replace("/", "");
            return cleaned.Length < 10 ? cleaned.PadLeft(10, '0') : cleaned;
        }
    }
}
