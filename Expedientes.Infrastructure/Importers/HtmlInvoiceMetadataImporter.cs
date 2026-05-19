using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Expedientes.Domain.Entities;
using Expedientes.Domain.Interfaces;
using HtmlAgilityPack;

namespace Expedientes.Domain.Importers
{
    public class HtmlInvoiceMetadataImporter : HtmlImporter, IFileImporter<InvoiceMetadata>
    {
        private int InvoiceNumberIndex = 1;
        private int RemitoNumberIndex = 3;
        private int ArticleIndex = 8;
        private int GtinIndex = 10;
        private int TroquelIndex = 11;
        private int QuantityIndex = 12;
        private int UnitPriceIndex = 13;
        private int LoteIndex = 21;
        private int ExpirationDateIndex = 22;
        private int CAEIndex = 25;
        private int CAEExpirationDateIndex = 26;
        public List<InvoiceMetadata> Import(Stream fileStream)
        {
            var invoices = new Dictionary<string, InvoiceMetadata>();

            var doc = LoadDocument(fileStream);
            var table = GetTable(doc);

            var rows = table.SelectNodes(".//tr");
            if (rows == null || rows.Count <= 1)
                return invoices.Values.ToList();
            for (int i = 1; i < rows.Count; i++)
            {
                var cells = rows[i].SelectNodes("th|td");
                if (cells == null || cells.Count < 26)
                    continue;

                string invoiceId = Clean(cells[InvoiceNumberIndex].InnerText);

                if (string.IsNullOrWhiteSpace(invoiceId))
                    continue;

                if (!invoices.TryGetValue(invoiceId, out var invoice))
                {
                    DateTime.TryParse(Clean(cells[CAEExpirationDateIndex].InnerText), out DateTime date);

                    invoice = new InvoiceMetadata
                    {
                        InvoiceNumber = invoiceId,
                        CAE = Clean(cells[CAEIndex].InnerText),
                        CAEExpirationDate = date
                    };
                    invoices.Add(invoiceId, invoice);
                }
                
                string remitoNumber = Clean(cells[RemitoNumberIndex].InnerText);
                var remito = invoice.GetOrCreateRemito(remitoNumber);

                string article = Clean(cells[ArticleIndex].InnerText);
                string lote = Clean(cells[LoteIndex].InnerText);
                var item = remito.GetOrCreateItem(article, lote);

                item.Gtin = "0" + Clean(cells[GtinIndex].InnerText);
                item.Troquel = Clean(cells[TroquelIndex].InnerText);
                item.Quantity += ParseInt(Clean(cells[QuantityIndex].InnerText));
                if (item.UnitPrice == 0)
                    item.UnitPrice = ParseAmount(Clean(cells[UnitPriceIndex].InnerText));

                if (!item.ExpirationDate.HasValue &&
                    DateTime.TryParse(Clean(cells[ExpirationDateIndex].InnerText), out DateTime exp))
                    item.ExpirationDate = exp;
            }
            return invoices.Values.ToList();
        }
    }
}
