using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Expedientes.Domain.Entities;

namespace Expedientes.Application.Mergin
{
    public class AnmatMerger : IAnmatMerger
    {
        public List<ProcessingWarning> Merge(
            List<ImportedInvoice> invoices,
            List<AnmatData> anmatData)
        {
            var warnings = new List<ProcessingWarning>();
            if (invoices == null || anmatData == null) return warnings;

            var anmatDictionary = anmatData.
                Where(a =>
                    !string.IsNullOrWhiteSpace(a.RemitoNumber) &&
                    !string.IsNullOrWhiteSpace(a.Gtin) &&
                    !string.IsNullOrWhiteSpace(a.Lote))
                .GroupBy(a => (
                    a.RemitoNumber.Trim(),
                    a.Gtin.Trim(),
                    a.Lote.Trim()))
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList());

            foreach (var invoice in invoices)
            {
                if (invoice == null) continue;
                foreach (var remito in invoice.Remitos)
                {
                    foreach (var item in remito.Items)
                    {
                        if (string.IsNullOrWhiteSpace(remito.RemitoNumber) ||
                           string.IsNullOrWhiteSpace(item.Gtin) ||
                           string.IsNullOrWhiteSpace(item.Lote)
                            ) continue;
                        var key = (
                            remito.RemitoNumber?.Trim(),
                            item.Gtin?.Trim(),
                            item.Lote?.Trim()
                        );
                        if (anmatDictionary.TryGetValue(key, out var matches))
                        {
                            foreach (var match in matches)
                            {
                                if (!string.IsNullOrWhiteSpace(match.IDTransction))
                                {
                                    item.AddTraceability(
                                        match.IDTransction,
                                        match.Serie
                                    );
                                }
                            }
                        }
                    }
                }
            }
            var allGtinsInInvoices = invoices
                .SelectMany(i => i.Remitos)
                .SelectMany(r => r.Items)
                .Where(item => !string.IsNullOrWhiteSpace(item.Gtin))
                .Select(item => item.Gtin.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var gtinsNotFound = anmatData
                .Where(a => !string.IsNullOrWhiteSpace(a.Gtin) &&
                            !allGtinsInInvoices.Contains(a.Gtin.Trim()))
                .Select(a => a.Gtin.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase);

            foreach (var gtin in gtinsNotFound)
            {
                warnings.Add(new ProcessingWarning
                {
                    InvoiceNumber = null,
                    ItemGtin = gtin,
                    Message = $"El GTIN {gtin} existe en ANMAT pero no fue encontrado en ninguna factura."
                });
            }

            return warnings;
        }
    }
}
