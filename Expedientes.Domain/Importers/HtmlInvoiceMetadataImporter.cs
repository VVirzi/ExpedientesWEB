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
                //Añadir los items al remito.
                //¿Debo crear una class RemitoMetadata o con la class InvoiceRemito está bien?
                //Datos a añadir: RemitoNumber, Article, Gtin, Troquel, Quantity, UnitPrice y Lote.
            }
            return invoices.Values.ToList();
        }
    }
}
