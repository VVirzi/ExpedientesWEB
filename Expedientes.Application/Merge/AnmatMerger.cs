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
        public void Merge(
            List<ImportedInvoice> invoices,
            List<AnmatData> anmatData)
        {
            if(invoices == null || anmatData == null) return;

            var anmatDictionary = anmatData.
                Where(a =>
                    !string.IsNullOrWhiteSpace(a.RemitoNumber) &&
                    !string.IsNullOrWhiteSpace(a.Gtin) &&
                    !string.IsNullOrWhiteSpace(a.Lote))
                .GroupBy(a=> (
                    a.RemitoNumber.Trim(),
                    a.Gtin.Trim(),
                    a.Lote.Trim()))
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList());

            foreach( var invoice in invoices )
            {
                if( invoice == null ) continue;
                foreach( var remito in invoice.Remitos)
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
        }
    }
}
