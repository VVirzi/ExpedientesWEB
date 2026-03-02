using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expedientes.Domain.Entities
{
    public class InvoiceProcessingResult
    {
        public List<ImportedInvoice> Invoices { get; set; } = new();
        public List<ProcessingWarning> Warnings { get; set; } = new();
    }
}
