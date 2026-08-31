using Expedientes.Application.DTOs;
using System.Text;

namespace Expedientes.Application.Exporters
{
    public class ClientCExporter : IInvoiceExporter
    {
        public string ClientId => "ClientC";
        public string ExportType => "txt";

        private const string GLN = "7798374190009";

        public byte[] Export(InvoiceResultDto result)
        {
            var sb = new StringBuilder();
            int itemNumber = 1;

            foreach (var invoice in result.Invoices)
            {
                var date = invoice.Date.ToString("yyyyMMdd");
                var deliveryDate = GetNextBusinessDay(invoice.Date).ToString("yyyyMMdd");
                var nroF5 = PadRight(invoice.InvoiceFile ?? "",' ', 15);
                var nroAfiliate = PadRight(invoice.AffiliateNumber ?? "", ' ', 15);
                var remito = PadLeft((invoice.RemitoNumber ?? "").Replace("R", ""), '0', 15);

                foreach (var item in invoice.Items)
                {
                    for (int i = 0; i < item.Quantity; i++)
                    {
                        var traceability = item.Traceabilities.ElementAtOrDefault(i);

                        var line = string.Concat(
                            "1",
                            invoice.InvoiceNumber,
                            date,
                            nroF5,
                            nroAfiliate,
                            remito,
                            PadLeft(itemNumber.ToString(), '0', 15),
                            PadLeft(item.Gtin ?? "", '0', 14),
                            PadRight(item.Article ?? "", ' ', 60),
                            PadLeft(item.Troquel ?? "", '0', 8),
                            deliveryDate,
                            "001",
                            FormatPrice(item.UnitPrice),
                            PadRight(traceability?.Serie ?? "", ' ', 20),
                            PadRight(item.Lote ?? "", ' ', 20),
                            FormatExpirationDate(item.ExpirationDate),
                            GLN,
                            PadRight("", ' ', 10),
                            PadRight(traceability?.TransactionId ?? "", ' ', 18)
                        );

                        sb.AppendLine(line);
                        itemNumber++;
                    }
                }
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private DateTime GetNextBusinessDay(DateTime date)
        {
            var next = date.AddDays(1);
            while (next.DayOfWeek == DayOfWeek.Saturday || next.DayOfWeek == DayOfWeek.Sunday)
                next = next.AddDays(1);
            return next;
        }

        private string FormatPrice(decimal unitPrice)
        {
            long centavos = (long)(unitPrice);
            return PadLeft(centavos.ToString(), '0', 12);
        }

        private string FormatExpirationDate(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("yyyyMMdd") : PadRight("", ' ', 8);
        }

        private string PadLeft(string value, char pad, int length)
        {
            if (value.Length >= length) return value.Substring(value.Length - length);
            return value.PadLeft(length, pad);
        }

        private string PadRight(string value, char pad, int length)
        {
            if (value.Length >= length) return value.Substring(0, length);
            return value.PadRight(length, pad);
        }
    }
}