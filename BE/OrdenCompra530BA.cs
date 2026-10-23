using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class OrdenCompra530BA
    {
        public int Id { get; set; }
        public int IdProveedor { get; set; }
        public bool Recibido { get; set; }
        public DateTime FechaEmision { get; set; }
        public long DVH { get; set; }

        // Propiedades de navegación / UI (No persisten directo en la tabla OrdenCompra)
        public string ProveedorRazonSocial { get; set; }
        public List<ProductoOC530BA> Productos { get; set; } = new List<ProductoOC530BA>();
    }
}
