using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Expedientes.Domain.Entities;

namespace Expedientes.Domain.Interfaces
{
    public interface IFileImporter<T>
    {
        //bool CanHandle(string fileType);
        List<T> Import(Stream fileStream);    
    }
}
