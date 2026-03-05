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
        public IActionResult Process(
        IFormFile invoicesFile,
        IFormFile? metadataFile,
        IFormFile? anmatFile)
        {
            if (invoicesFile == null || invoicesFile.Length == 0)
                return BadRequest("Debe enviar el archivo principal.");

            using var invoicesStream = invoicesFile.OpenReadStream();
            using var metadataStream = metadataFile?.OpenReadStream();
            using var anmatStream = anmatFile?.OpenReadStream();

            var result = _invoiceProcessingService.Process(
                invoicesStream,
                metadataStream,
                anmatStream);

            return Ok(result);
        }
    }
}
