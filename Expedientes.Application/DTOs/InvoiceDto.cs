using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Expedientes.Domain.Entities;

namespace Expedientes.Application.DTOs
{
    public class InvoiceDto
    {
        public string InvoiceNumber { get; set; }
        public DateTime Date { get; set; }
        public string RemitoNumber { get; set; }
        public string AffiliateNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string? CAE { get; set; }
        public DateTime? CAEExpirationDate { get; set; }
        public List<InvoiceItemDto> Items { get; set; } = new();
    }
}
