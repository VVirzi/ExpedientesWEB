using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expedientes.Domain.Entities
{
    public class ImportedInvoice
    {
        public string InvoiceType { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime Date {  get; set; }
        public string RemitoNumber { get; set; }
        public string AffiliateNumber { get; set; }
        public string AffiliateName { get; set; }
        public string PurchaseOrder {  get; set; }
        public string InvoiceFile { get; set; }

        public List<InvoiceItem> Items { get; set; } = new();
        //public decimal TotalAmount => Items.Sum(x => x.Total);
        public decimal TotalAmount { get; set; }
        public InvoiceMetadata? Metadata { get; set; }

        public void AddOrUpdateItem(string article, int quantity, decimal unitPrice)
        {
            var existingItem = Items
                .FirstOrDefault(i => i.Article == article
                                  && i.UnitPrice == unitPrice);

            if (existingItem != null)
                existingItem.Quantity += quantity;
            else
                Items.Add(new InvoiceItem
                {
                    Article = article,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                });
        }
    }
}
