using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expedientes.Application.DTOs
{
    public class InvoiceMetadataDto
    {
        public string InvoiceNumber { get; set; }
        public string CAE { get; set; }
        public DateTime? CAEExpirationDate { get; set; }
    }
}
