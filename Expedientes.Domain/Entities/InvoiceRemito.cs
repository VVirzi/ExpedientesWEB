using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expedientes.Domain.Entities
{
    public class InvoiceRemito
    {
        public string RemitoNumber { get; set; }
        public List<InvoiceItem> Items { get; set; } = new();

        public InvoiceItem GetOrCreateItem(string article, string? lote)
        {
            var item = Items.FirstOrDefault(i =>
                i.Article == article &&
                i.Lote == lote);
            
            if (item == null)
            {
                item = Items.FirstOrDefault(j =>
                j.Article == article &&
                j.Lote == null);

                if(item == null)
                {
                    item = new InvoiceItem
                    {
                        Article = article,
                        Lote = lote
                    };
                    Items.Add(item);
                } 
            }
            return item;
        }
    }
}
