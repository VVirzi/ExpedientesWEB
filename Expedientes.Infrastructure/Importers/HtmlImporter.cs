using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Expedientes.Domain.Importers
{
    public abstract class HtmlImporter
    {
        protected HtmlDocument LoadDocument(Stream stream)
        {
            var doc = new HtmlDocument();
            doc.Load(stream, System.Text.Encoding.Latin1);
            return doc;
        }

        protected HtmlNode GetTable(HtmlDocument doc)
        {
            var table = doc.DocumentNode.SelectSingleNode("//table");
            if (table == null)
                throw new Exception("No se encontró ninguna tabla en el archivo HTML.");
            return table;
        }

        protected string Clean(string value)
        {
            return HtmlEntity.DeEntitize(value)?.Trim() ?? string.Empty;
        }
        protected decimal ParseAmount(string value)
        {
            value = value.Replace(".", "").Replace(",", ".");
            decimal.TryParse(value, out decimal result);
            return result;
        }
        protected int ParseInt(string value)
        {
            int.TryParse(value.Replace(".", ""), out int result);
            return result;
        }
    }
}
