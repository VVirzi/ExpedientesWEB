using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Expedientes.Domain.Entities;

namespace Expedientes.Domain.Interfaces
{
    public interface IFileImporter
    {
        bool CanHandle(string fileType);
        List<ImportedInvoice> Import(Stream fileStream);    
    }
}
