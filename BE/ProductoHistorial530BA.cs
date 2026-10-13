using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class ProductoHistorial530BA
    {
        public ProductoHistorial530BA()
        {
            
        }

        // Propiedades de bitacora
        public int IdHistorial { get; set; }
        public int codProducto { get; set; }
        public int Version { get; set; }
        public DateTime FechaHora { get; set; }
        public string TipoOperacion { get; set; }
        public string UsuarioId { get; set; }
        public bool VersionActiva { get; set; }

        // Valores del producto
        public string nombre { get; set; }
        public int existencia { get; set; }
        public decimal precioUnitario { get; set; }
        public bool Activo { get; set; }
        public long DVH { get; set; }
    }
}
