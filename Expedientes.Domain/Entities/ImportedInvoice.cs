using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expedientes.Domain.Entities
{
    public class ImportedInvoice
    {
        public string InvoiceNumber { get; set; }
        public DateTime Date {  get; set; }
        public string RemitoNumber { get; set; }
        public string AffiliateNumber { get; set; }
        public List<InvoiceItem> Items { get; set; } = new();
        public decimal TotalAmount => Items.Sum(x => x.Total);
        public InvoiceMetadata? Metadata { get; set; }
    }
}
