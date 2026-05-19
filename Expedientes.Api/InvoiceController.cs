using Microsoft.AspNetCore.Mvc;
using Expedientes.Application.Services;
using Expedientes.Application.DTOs;

namespace Expedientes.Api
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceProcessingService _invoiceProcessingService;

        public InvoiceController(IInvoiceProcessingService processingService)
        {
            _invoiceProcessingService = processingService;
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
    }
}
