using Expedientes.Application.DTOs;

namespace Expedientes.Api
{
    public class ExportRequest
    {
        public string ClientId { get; set; }
        public string ExportType { get; set; }
        public InvoiceResultDto Result { get; set; }
    }
}
