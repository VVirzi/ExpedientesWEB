using System.Text;
using Expedientes.Domain.Entities;
using Expedientes.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Expedientes.Infrastructure.Importers
{
    public class AnmatImporter : IFileImporter<AnmatData>
    {
        private readonly string _companyFilter;

        public AnmatImporter(IConfiguration configuration)
        {
            _companyFilter = configuration["AnmatImporter:CompanyFilter"]
                ?? throw new InvalidOperationException(
                    "La configuración 'AnmatImporter:CompanyFilter' no está definida en appsettings.json");
        }
        public List<AnmatData> Import(Stream fileStream)
        {
            var result = new List<AnmatData>();

            using var reader = new StreamReader(fileStream, Encoding.Latin1);
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length < 1374) continue;

                // Filtrar solo registros de nuestra empresa
                string companyName = line.Substring(894, 15).Trim();
                if (!companyName.Equals(_companyFilter, StringComparison.OrdinalIgnoreCase))
                    continue;

                var remito = line.Substring(1334, 20).Trim();
                var gtin = line.Substring(526, 19).Trim();
                var lote = line.Substring(831, 20).Trim();
                var serie = line.Substring(810, 21).Trim();
                var idTx = line.Substring(0, 15).Trim();

                if (string.IsNullOrWhiteSpace(remito) || string.IsNullOrWhiteSpace(gtin))
                    continue;

                result.Add(new AnmatData
                {
                    RemitoNumber = remito,
                    Gtin = gtin,
                    Lote = lote,
                    Serie = serie,
                    IDTransction = idTx
                });
            }

            return result;
        }
    }
}
