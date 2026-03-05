using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Expedientes.Application.DTOs;

namespace Expedientes.Application.Services
{
    public interface IInvoiceProcessingService
    {
        InvoiceResultDto Process(
            Stream invoicesPath,
            Stream? metadataPath,
            Stream? anmatPath);
    }
}
