using Expedientes.Application.DTOs;

namespace Expedientes.Application.Exporters
{
    public interface IInvoiceExporter
    {
        string ClientId { get; }
        string ExportType { get; }
        byte[] Export(InvoiceResultDto result);
    }
}
