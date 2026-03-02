using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Expedientes.Domain.Entities;

namespace Expedientes.Application.Mergin
{
    public interface IAnmatMerger
    {
        void Merge(
            List<ImportedInvoice> invoices,
            List<AnmatData> anmatData);
    }
}
