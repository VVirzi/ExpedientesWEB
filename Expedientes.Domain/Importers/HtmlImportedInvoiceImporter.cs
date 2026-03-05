using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Expedientes.Domain.Entities;
using Expedientes.Domain.Interfaces;

namespace Expedientes.Domain.Importers
{
    public class HtmlImportedInvoiceImporter : IFileImporter<ImportedInvoice>
    {
        private int InvoiceTypeIndex = 0;
        private int InvoiceNumberIndex = 1;
        private int DateIndex = 2;
        private int RemitoNumberIndex = 3;
        private int InvoiceFileIndex = 5;
        private int AffiliateNameIndex = 13;
        private int AffiliateNumberIndex = 14;
        private int PurchaseOrderIndex = 19;
        private int TotalAmountIndex = 16;
        private int ArticleIndex = 9;
        private int QuantityIndex = 8;
        private int UnitPriceIndex = 12;
        public List<ImportedInvoice> Import(Stream fileStream)
        {
            var invoices = new Dictionary<string, ImportedInvoice>();
            var doc = new HtmlDocument();
            doc.Load(fileStream);

            var table = doc.DocumentNode.SelectSingleNode("//table");
            if (table == null)
                throw new Exception("No se encontró ninguna tabla en el archivo HTML.");

            var rows = table.SelectNodes(".//tr");
            if (rows == null || rows.Count <= 1)
                return invoices.Values.ToList(); //Testear

            for (int i = 1; i < rows.Count; i++)
            {
                var cells = rows[i].SelectNodes("th|td");
                if (cells == null || cells.Count < 20)
                    continue;

                string invoiceId = Clean(cells[InvoiceNumberIndex].InnerText);

                if (string.IsNullOrWhiteSpace(invoiceId))
                    continue;

                if (!invoices.TryGetValue(invoiceId, out var invoice))
                {
                    DateTime.TryParse(Clean(cells[DateIndex].InnerText), out DateTime date);

                    invoice = new ImportedInvoice
                    {
                        InvoiceType = Clean(cells[InvoiceTypeIndex].InnerText),
                        InvoiceNumber = invoiceId,
                        Date = date,
                        RemitoNumber = Clean(cells[RemitoNumberIndex].InnerText),
                        InvoiceFile = Clean(cells[InvoiceFileIndex].InnerText),
                        AffiliateName = Clean(cells[AffiliateNameIndex].InnerText),
                        AffiliateNumber = Clean(cells[AffiliateNumberIndex].InnerText),
                        PurchaseOrder = Clean(cells[PurchaseOrderIndex].InnerText),
                        TotalAmount = ParseAmount(Clean(cells[TotalAmountIndex].InnerText)),
                    };
                invoices.Add(invoiceId, invoice);
                }
                invoice.AddOrUpdateItem(
                    Clean(cells[ArticleIndex].InnerText), 
                    ParseInt(Clean(cells[QuantityIndex].InnerText)), 
                    ParseAmount(Clean(cells[UnitPriceIndex].InnerText)));
            }

            return invoices.Values.ToList();
        }
        private string Clean(string value)
        {
            return HtmlEntity.DeEntitize(value)?.Trim() ?? string.Empty;
        }

        private decimal ParseAmount(string value)
        {
            value = value.Replace(".", "").Replace(",", ".");
            decimal.TryParse(value, out decimal result);
            return result;
        }
        private int ParseInt(string value)
        {
            int.TryParse(value.Replace(".", ""), out int result);
            return result;
        }
    }
}

