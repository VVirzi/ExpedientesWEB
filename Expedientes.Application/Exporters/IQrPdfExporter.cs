namespace Expedientes.Application.Exporters
{
    public interface IQrPdfExporter
    {
        byte[] Export(List<(string content, string label)> qrIterms);
    }
}
