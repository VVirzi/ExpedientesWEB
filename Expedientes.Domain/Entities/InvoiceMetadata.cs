using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expedientes.Domain.Entities
{
    public class InvoiceMetadata
    {
        public string InvoiceNumber { get; set; }
        public string CAE { get; set; }
        public DateTime? CAEExpirationDate { get; set; }
        public List<InvoiceRemito> Remitos { get; set; } = new ();

        public InvoiceRemito GetOrCreateRemito(string remitoNumber)
        {
            var remito = Remitos.FirstOrDefault(r => r.RemitoNumber == remitoNumber);
            if (remito == null)
            {
                remito = new InvoiceRemito { RemitoNumber = remitoNumber };
                Remitos.Add(remito);
            }
            return remito;
        }
    }
}