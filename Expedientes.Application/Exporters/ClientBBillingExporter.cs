using System.Globalization;
using System.Text;
using Expedientes.Application.DTOs;

namespace Expedientes.Application.Exporters
{
    public class ClientBBillingExporter : IInvoiceExporter
    {
        public string ClientId => "ClientB";
        public string ExportType => "billing";

        public byte[] Export(InvoiceResultDto result)
        {
            var sb = new StringBuilder();
            foreach (var invoice in result.Invoices)
            {
                var reference = FormatReference(invoice.InvoiceType, invoice.InvoiceNumber);
                var date = FormatDate(invoice.Date);
                var text = $"{invoice.AffiliateNumber} {invoice.AffiliateName} {invoice.RemitoNumber}";

                foreach (var item in invoice.Items)
                {
                    var units = item.Quantity.ToString("F3", CultureInfo.InvariantCulture);
                    var unitPrice = item.UnitPrice / 100m;
                    var unitTotalPrice = (unitPrice * item.Quantity).ToString("F2", CultureInfo.InvariantCulture);
                    var importeTotal = (invoice.TotalAmount / 100m).ToString("F2", CultureInfo.InvariantCulture);

                    var line = string.Join("\t",
                        "1",
                        "135835",
                        date,
                        reference,
                        "RE",
                        "0001",
                        text,
                        "0001",
                        invoice.PurchaseOrder ?? "",
                        "101",
                        units,
                        unitTotalPrice,
                        importeTotal
                    );

                    sb.AppendLine(line);
                }
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }
        private string FormatReference(string invoiceType, string invoiceNumber)
        {
            string tipo = invoiceType?.Trim() ?? "";
            string lastChar = tipo.Length > 0 ? tipo.Substring(tipo.Length - 1) : "";

            return invoiceNumber.Replace("-", lastChar);
        }

        private string FormatDate(DateTime date)
        {
            return date.ToString("yyyyMMdd");
        }
    }
}
