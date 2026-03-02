using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expedientes.Application.DTOs
{
    public class InvoiceItemDto
    {
        public string Gtin { get; set; }
        public string Article { get; set; }
        public string Troquel { get; set; }
        public string Lote { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public List<TraceabilityDto> Traceabilities { get; set; } = new();
    }
}
