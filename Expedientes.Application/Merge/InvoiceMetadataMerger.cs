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

            foreach (var invoice in invoices)
            {
                var invoiceMetadata = metadata.FirstOrDefault(m =>
                    string.Equals(m.InvoiceNumber?.Trim(),
                                  invoice.InvoiceNumber?.Trim(),
                                  StringComparison.OrdinalIgnoreCase));

                if (invoiceMetadata == null) continue;

                invoice.Metadata = new InvoiceMetadata
                {
                    InvoiceNumber = invoiceMetadata.InvoiceNumber,
                    CAE = invoiceMetadata.CAE,
                    CAEExpirationDate = invoiceMetadata.CAEExpirationDate
                };

                foreach (var remito in invoice.Remitos)
                {
                    var metadataRemito = invoiceMetadata.Remitos.FirstOrDefault(r =>
                        RemitoMatches(remito.RemitoNumber, r.RemitoNumber));

                    if (metadataRemito == null) continue;

                    foreach (var item in remito.Items)
                    {
                        var metadataItem = metadataRemito.Items.FirstOrDefault(i =>
                            string.Equals(i.Article?.Trim(),
                                          item.Article?.Trim(),
                                          StringComparison.OrdinalIgnoreCase));

                        if (metadataItem == null) continue;

                        item.Gtin = metadataItem.Gtin;
                        item.Troquel = metadataItem.Troquel;
                        item.Lote = metadataItem.Lote;
                        item.ExpirationDate = metadataItem.ExpirationDate;
                    }
                }
            }
        }

        private bool RemitoMatches(string invoiceRemito, string metadataRemito)
        {
            if (string.IsNullOrWhiteSpace(invoiceRemito) ||
                string.IsNullOrWhiteSpace(metadataRemito)) return false;

            var metaNormalized = metadataRemito.Trim();

            return invoiceRemito.Trim().EndsWith(metaNormalized,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
