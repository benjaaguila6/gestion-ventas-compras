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

        // Precio vigente del producto al momento de generar la orden de compra.
        // No pertenece a ProductoCotizacion: se toma de Producto.PrecioUnitario.
        public decimal? PrecioUnitario { get; set; }

        // Propiedad calculada para la grilla de pago.
        public decimal Subtotal => Cantidad * (PrecioUnitario ?? 0m);
    }
}
