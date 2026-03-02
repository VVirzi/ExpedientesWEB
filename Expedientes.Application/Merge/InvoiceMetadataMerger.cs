using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Expedientes.Domain.Entities;

namespace Expedientes.Application.Mergin
{
    public class InvoiceMetadataMerger: IInvoiceMetadataMerger
    {
        public void Merge(
            List<ImportedInvoice> invoices,
            List<InvoiceMetadata> metadata)
        {
            if (invoices == null || metadata == null) return;

            var metadataDictionary = metadata
                .Where(m => !string.IsNullOrWhiteSpace(m.InvoiceNumber))
                .ToDictionary(
                    m => m.InvoiceNumber, 
                    m => m, 
                    StringComparer.OrdinalIgnoreCase);

            foreach ( var invoice in invoices)
            {
                if (invoice == null || !string.IsNullOrWhiteSpace(invoice.InvoiceNumber)) continue;
                
                if(metadataDictionary.TryGetValue(
                    invoice.InvoiceNumber, 
                    out var invoiceMetadata))
                {
                    invoice.Metadata = new InvoiceMetadata
                    {
                        InvoiceNumber = invoiceMetadata.InvoiceNumber,
                        CAE = invoiceMetadata.CAE,
                        CAEExpirationDate = invoiceMetadata.CAEExpirationDate
                    };
                }
            }
        }
    }
}
