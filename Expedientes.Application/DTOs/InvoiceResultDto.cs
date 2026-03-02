using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Expedientes.Domain.Entities;

namespace Expedientes.Application.DTOs
{
    public class InvoiceResultDto
    {
        public List<InvoiceDto> Invoices { get; set; } = new();
        public List<ProcessingWarningDto> Warnings { get; set; } = new();
    }
}
