using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expedientes.Domain.Entities
{
    public class AnmatData
    {
        public string RemitoNumber { get; set; }
        public string Gtin {  get; set; }
        public string Lote { get; set; }
        public string Serie { get; set; }
        public string IDTransction { get; set; }
    }
}
