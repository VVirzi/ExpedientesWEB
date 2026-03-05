using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expedientes.Domain.Entities
{
    public class InvoiceItem
    {
        public string Gtin {  get; set; }
        public string Article { get; set; }
        public string Troquel {  get; set; }
        public string Lote { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total => Quantity * UnitPrice;
        private readonly List<ItemTraceability> _traceabilities = new();
        public IReadOnlyCollection<ItemTraceability> Traceabilities => _traceabilities;

        public void AddTraceability(string transactionId, string serie)
        {
            if (_traceabilities.Any(t => t.TransactionId == transactionId)) return;

            _traceabilities.Add(new ItemTraceability
            {
                TransactionId = transactionId,
                Serie = serie
            });
        }
    }
}
