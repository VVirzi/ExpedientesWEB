using System.Globalization;
using System.Text;
using Expedientes.Application.DTOs;

namespace Expedientes.Application.Exporters
{
    public class ClientBSettlementsExporter : IInvoiceExporter
    {
        public string ClientId => "ClientB";
        public string ExportType => "settlements";

        public byte[] Export(InvoiceResultDto result)
        {
            var sb = new StringBuilder();

            foreach (var invoice in result.Invoices)
            {
                var fecha = invoice.Date.ToString("dd/MM/yyyy");
                var factura = FormatInvoice(invoice.InvoiceType, invoice.InvoiceNumber);
                var importe = FormatPrice(invoice.TotalAmount);

                var line = string.Join("\t",
                    fecha,
                    factura,
                    invoice.RemitoNumber,
                    invoice.PurchaseOrder ?? "",
                    importe
                );

                sb.AppendLine(line);
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private string FormatInvoice(string invoiceType, string invoiceNumber)
        {
            string tipo = invoiceType?.Trim() ?? "";
            string lastChar = tipo.Length > 0 ? tipo.Substring(tipo.Length - 1) : "";

            return $"{lastChar}-{invoiceNumber}";
        }

        private string FormatPrice(decimal price)
        {
            decimal value = price / 100m;
            return value.ToString("F2", CultureInfo.InvariantCulture).Replace(".", ",");
        }
    }
}
