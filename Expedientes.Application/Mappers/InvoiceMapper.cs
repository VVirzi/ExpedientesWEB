using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Expedientes.Application.DTOs;
using Expedientes.Domain.Entities;

namespace Expedientes.Application.Mappers
{
    public static class InvoiceMapper
    {
        public static InvoiceResultDto ToDto(InvoiceProcessingResult result)
        {
            return new InvoiceResultDto
            {
                Invoices = result.Invoices.Select(ToDto).ToList(),
                Warnings = result.Warnings.Select(w => new ProcessingWarningDto
                {
                    InvoiceNumber= w.InvoiceNumber,
                    ItemGtin = w.ItemGtin,
                    Message = w.Message
                }).ToList()
            };
        }

        private static InvoiceDto ToDto(ImportedInvoice invoice)
        {
            return new InvoiceDto
            {
                InvoiceType = invoice.InvoiceType,
                InvoiceNumber = invoice.InvoiceNumber,
                Date = invoice.Date,
                RemitoNumber = invoice.Remitos.FirstOrDefault()?.RemitoNumber ?? string.Empty,
                AffiliateNumber = invoice.AffiliateNumber,
                AffiliateName = invoice.AffiliateName,
                InvoiceFile = invoice.InvoiceFile,
                PurchaseOrder = invoice.PurchaseOrder,
                TotalAmount = invoice.TotalAmount,
                CAE = invoice.Metadata?.CAE,
                CAEExpirationDate = invoice.Metadata?.CAEExpirationDate,
                Items = invoice.Remitos
                    .SelectMany(r => r.Items)
                    .Select(ToDto)
                    .ToList()
            };
        }

        private static InvoiceItemDto ToDto(InvoiceItem item)
        {
            return new InvoiceItemDto
            {
                Gtin = item.Gtin,
                Article = item.Article,
                Troquel = item.Troquel,
                Lote = item.Lote,
                ExpirationDate = item.ExpirationDate,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Total = item.Total,
                Traceabilities = item.Traceabilities.Select(t => new TraceabilityDto
                {
                    TransactionId = t.TransactionId,
                    Serie = t.Serie
                }).ToList()
            };
        }
    }
}
