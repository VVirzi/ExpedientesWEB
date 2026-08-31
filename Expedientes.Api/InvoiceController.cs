using Microsoft.AspNetCore.Mvc;
using Expedientes.Application.Services;
using Expedientes.Application.DTOs;
using Expedientes.Application.Exporters;

namespace Expedientes.Api
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceProcessingService _invoiceProcessingService;
        private readonly IEnumerable<IInvoiceExporter> _exporters;

        public InvoiceController(
            IInvoiceProcessingService processingService,
            IEnumerable<IInvoiceExporter> exporters)
        {
            _invoiceProcessingService = processingService;
            _exporters = exporters;
        }

        [HttpPost("process")]
        [Consumes("multipart/form-data")]
        public IActionResult Process([FromForm] InvoiceProcessRequest request)
        {
            if (request.InvoicesFile == null || request.InvoicesFile.Length == 0)
                return BadRequest("Debe enviar el archivo principal.");

            using var invoicesStream = request.InvoicesFile.OpenReadStream();
            using var metadataStream = request.MetadataFile?.OpenReadStream();
            using var anmatStream = request.AnmatFile?.OpenReadStream();

            var result = _invoiceProcessingService.Process(
                invoicesStream,
                metadataStream,
                anmatStream);

            return Ok(result);
        }

        [HttpPost("export")]
        public IActionResult Export([FromBody] ExportRequest request)
        {
            if (request == null)
                return BadRequest("Request es null.");


            var exporter = _exporters.FirstOrDefault(e =>
                e.ClientId == request.ClientId &&
                e.ExportType == request.ExportType);

            if (exporter == null)
                return BadRequest($"Exportador no encontrado.");

            var fileBytes = exporter.Export(request.Result);

            string contentType = request.ExportType == "pdf"
                ? "application/pdf"
                : "text/plain";

            string fileName = $"export_{request.ClientId}_{request.ExportType}_{DateTime.Now:yyyyMMdd}.{(request.ExportType == "pdf" ? "pdf" : "txt")}";

            return File(fileBytes, contentType, fileName);
        }
    }
}
