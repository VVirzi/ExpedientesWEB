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
    public class HtmlImportedInvoiceImporter : HtmlImporter, IFileImporter<ImportedInvoice>
    {
        private int InvoiceTypeIndex = 0;
        private int InvoiceNumberIndex = 1;
        private int DateIndex = 2;
        private int RemitoNumberIndex = 3;
        private int InvoiceFileIndex = 5;
        private int PurchaseOrderIndex = 19;
        private int TotalAmountIndex = 16;
        private int AffiliateNameIndex = 13;
        private int AffiliateNumberIndex = 14;
        private int ArticleIndex = 9;
        private int QuantityIndex = 8;
        private int UnitPriceIndex = 12;
        public List<ImportedInvoice> Import(Stream fileStream)
        {
            var invoices = new Dictionary<string, ImportedInvoice>();

            var doc = LoadDocument(fileStream);
            var table = GetTable(doc);

            var rows = table.SelectNodes(".//tr"); //Testear
            if (rows == null || rows.Count <= 1)
                return invoices.Values.ToList(); 

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

                    var purchaseOrder = Clean(cells[PurchaseOrderIndex].InnerText);
                    if (string.IsNullOrWhiteSpace(purchaseOrder))
                        purchaseOrder = Clean(cells[InvoiceFileIndex].InnerText);

                    invoice = new ImportedInvoice
                    {
                        InvoiceType = Clean(cells[InvoiceTypeIndex].InnerText),
                        InvoiceNumber = invoiceId,
                        Date = date,
                        InvoiceFile = Clean(cells[InvoiceFileIndex].InnerText),
                        AffiliateName = Clean(cells[AffiliateNameIndex].InnerText),
                        AffiliateNumber = Clean(cells[AffiliateNumberIndex].InnerText),
                        PurchaseOrder = purchaseOrder,
                        TotalAmount = ParseAmount(Clean(cells[TotalAmountIndex].InnerText)),
                    };
                    invoices.Add(invoiceId, invoice);
                }
                var remito = invoice.GetOrCreateRemito(Clean(cells[RemitoNumberIndex].InnerText));

                var item = remito.GetOrCreateItem(Clean(cells[ArticleIndex].InnerText), null);
                item.Quantity += ParseInt(Clean(cells[QuantityIndex].InnerText));
                if (item.UnitPrice == 0) item.UnitPrice = ParseAmount(Clean(cells[UnitPriceIndex].InnerText));
            }
            return invoices.Values.ToList();
        }
    }
}

