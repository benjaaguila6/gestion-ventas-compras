using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class ProductoCotizacion530BA
    {
        public int Id { get; set; }
        public int IdSolicitud { get; set; }
        public int codProducto { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public long DVH { get; set; }

        // Propiedad de navegación UI
        public string NombreProducto { get; set; }
    }
}
