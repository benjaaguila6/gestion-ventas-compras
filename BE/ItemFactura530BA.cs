using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    // Línea de detalle de una factura (en memoria, mientras se carga).
    public class ItemFactura530BA
    {
        public int codProducto { get; set; }
        public string nombre { get; set; }
        public int cantidad { get; set; }
        public decimal precioUnitario { get; set; }

        public decimal Subtotal
        {
            get { return cantidad * precioUnitario; }
        }
    }
}