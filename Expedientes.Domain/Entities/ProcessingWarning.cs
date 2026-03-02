using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expedientes.Domain.Entities
{
    public class ProcessingWarning
    {
        public string InvoiceNumber { get; set; }
        public string? ItemGtin {  get; set; }
        public string Message { get; set; }
    }
}
