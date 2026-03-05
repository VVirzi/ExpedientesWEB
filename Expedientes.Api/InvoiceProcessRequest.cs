namespace Expedientes.Api
{
    public class InvoiceProcessRequest
    {
        public IFormFile InvoicesFile { get; set; }
        public IFormFile? MetadataFile { get; set; }
        public IFormFile? AnmatFile { get; set; }
    }
}