using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expedientes.Domain.Entities
{
    public class InvoiceMetadata
    {
        public string CAE { get; set; }
        public DateTime? CAEExpirationDate { get; set; }
    }
}
