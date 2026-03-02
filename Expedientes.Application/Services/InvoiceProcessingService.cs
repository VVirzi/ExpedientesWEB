using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Expedientes.Domain.Interfaces;
using Expedientes.Domain.Entities;
using Expedientes.Application.Mergin;
using Expedientes.Application.DTOs;
using Expedientes.Application.Mappers;

namespace Expedientes.Application.Services
{
    public class InvoiceProcessingService
    {
        private readonly IFileImporter<ImportedInvoice> _invoicesPresentedImporter;
        private readonly IFileImporter<InvoiceMetadata> _invoicesInformationImporter;
        private readonly IFileImporter<AnmatData> _anmatImporter;
        private readonly IInvoiceMetadataMerger _invoiceMetadataMerger;
        private readonly IAnmatMerger _anmatMerger;

        public InvoiceProcessingService(
            IFileImporter<ImportedInvoice> invoicesPresentedImporter,
            IFileImporter<InvoiceMetadata> invoicesInformationImporter,
            IFileImporter<AnmatData> anmatImporter,
            IInvoiceMetadataMerger invoiceMetadataMerger,
            IAnmatMerger anmatMerger)
        {
            _invoicesPresentedImporter = invoicesPresentedImporter;
            _invoicesInformationImporter = invoicesInformationImporter;
            _anmatImporter = anmatImporter;
            _invoiceMetadataMerger = invoiceMetadataMerger;
            _anmatMerger = anmatMerger;
        }
        public InvoiceResultDto Process(
            string invoicesPresentedPath,
            string? invoicesInformationPath,
            string? anmatPath)
        {
            var result = new InvoiceProcessingResult();
            var invoices = _invoicesPresentedImporter.Import(invoicesPresentedPath);

            if (!string.IsNullOrEmpty(invoicesInformationPath))
            {
                var metadata = _invoicesInformationImporter.Import(invoicesInformationPath);
                _invoiceMetadataMerger.Merge(invoices, metadata);
            }
            if (!string.IsNullOrEmpty(anmatPath))
            {
                var anmatData = _anmatImporter.Import(anmatPath);
                _anmatMerger.Merge(invoices, anmatData);   
            }
            result.Invoices = invoices;
            result.Warnings = ValidateInvoices(invoices);

            return InvoiceMapper.ToDto(result);
        }

        private List<ProcessingWarning> ValidateInvoices(List<ImportedInvoice> invoices)
        {
            var warnings = new List<ProcessingWarning>();
            foreach (var invoice in invoices)
            {
                foreach(var item in invoice.Items)
                {
                    if (string.IsNullOrWhiteSpace(item.Gtin))
                    {
                        warnings.Add(new ProcessingWarning
                        {
                            InvoiceNumber = invoice.InvoiceNumber,
                            ItemGtin = null,
                            Message = "El item no tiene GTIN"
                        });
                    }
                    if (string.IsNullOrWhiteSpace(item.Troquel))
                    {
                        warnings.Add(new ProcessingWarning
                        {
                            InvoiceNumber = invoice.InvoiceNumber,
                            ItemGtin = item.Gtin,
                            Message = "El item no tiene Troquel"
                        });
                    }
                }
            }
            return warnings;
        }
    }
}
