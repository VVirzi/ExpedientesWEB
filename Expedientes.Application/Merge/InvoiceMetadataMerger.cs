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

                    var expandedItems = new List<InvoiceItem>();

                    foreach (var item in remito.Items)
                    {
                        var metadataItems = metadataRemito.Items
                            .Where(i => string.Equals(i.Article?.Trim(),
                                                      item.Article?.Trim(),
                                                      StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        if (!metadataItems.Any())
                        {
                            expandedItems.Add(item);
                            continue;
                        }

                        if (metadataItems.Count == 1)
                        {
                            var meta = metadataItems[0];
                            item.Gtin = meta.Gtin;
                            item.Troquel = meta.Troquel;
                            item.Lote = meta.Lote;
                            item.ExpirationDate = meta.ExpirationDate;
                            expandedItems.Add(item);
                            continue;
                        }

                        foreach (var meta in metadataItems)
                        {
                            var newItem = new InvoiceItem
                            {
                                Article = item.Article,
                                Gtin = meta.Gtin,
                                Troquel = meta.Troquel,
                                Lote = meta.Lote,
                                ExpirationDate = meta.ExpirationDate,
                                Quantity = (int)meta.Quantity,
                                UnitPrice = meta.UnitPrice > 0 ? meta.UnitPrice : item.UnitPrice
                            };
                            expandedItems.Add(newItem);
                        }
                    }

                    remito.Items.Clear();
                    foreach (var expanded in expandedItems)
                        remito.Items.Add(expanded);
                }
            }
        }

        private bool RemitoMatches(string invoiceRemito, string metadataRemito)
        {
            if (string.IsNullOrWhiteSpace(invoiceRemito) ||
                string.IsNullOrWhiteSpace(metadataRemito)) return false;

            /*var metaNormalized = metadataRemito.Trim();

            return invoiceRemito.Trim().EndsWith(metaNormalized,
                StringComparison.OrdinalIgnoreCase);*/
            var normalized = invoiceRemito.Trim().EndsWith(metadataRemito.Trim(),
                StringComparison.OrdinalIgnoreCase);
            return normalized;
        }
    }
}
