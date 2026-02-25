using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Expedientes.Domain.Interfaces;
using Expedientes.Domain.Entities;

namespace Expedientes.Application.Services
{
    public class InvoiceProcessingService
    {
        private readonly IFileImporter<ImportedInvoice> _invoicesPresentedImporter;
        private readonly IFileImporter<InvoiceMetadata> _invoicesInformationImporter;
        private readonly IFileImporter<AnmatData> _anmatImporter;

        public InvoiceProcessingService(
            IFileImporter<ImportedInvoice> invoicesPresentedImporter,
            IFileImporter<InvoiceMetadata> invoicesInformationImporter,
            IFileImporter<AnmatData> anmatImporter)
        {
            _invoicesPresentedImporter = invoicesPresentedImporter;
            _invoicesInformationImporter = invoicesInformationImporter;
            _anmatImporter = anmatImporter;
        }
        public List<ImportedInvoice> Process(
            string invoicesPresentedPath,
            string? invoicesInformationPath,
            string? anmatPath)
        {
            var invoices = _invoicesPresentedImporter.Import(invoicesPresentedPath);

            if (!string.IsNullOrEmpty(invoicesInformationPath))
            {
                var metadata = _invoicesInformationImporter.Import(invoicesInformationPath);
                MergeMetadata(invoices, metadata);
            }
            if (!string.IsNullOrEmpty(anmatPath))
            {
                var anmatData = _anmatImporter.Import(anmatPath);
                ApplyAnmatData(invoices, anmatData);   
            }
            return invoices;
        }

        private void MergeMetadata(
            List<ImportedInvoice> invoices,
            List<InvoiceMetadata> metadata)
        {
            //unir por número de factura
        }

        private void ApplyAnmatData(
            List<ImportedInvoice> invoices,
            List<AnmatData> anmatData)
        {
            //unir por número de remito y gtin.
        }
    }
}
