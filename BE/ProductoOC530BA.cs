using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class ProductoOC530BA
    {
        public int IdOrden { get; set; }
        public int codProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public long DVH { get; set; }

        // Propiedades de navegación / UI
        public string NombreProducto { get; set; }
    }
}
