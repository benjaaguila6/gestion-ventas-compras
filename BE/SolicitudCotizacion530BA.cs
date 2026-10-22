using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class SolicitudCotizacion530BA
    {
        public int Id { get; set; }
        public int IdProveedor { get; set; }
        public DateTime Fecha { get; set; }
        public long DVH { get; set; }

        // Propiedades de navegación / UI (No persisten directo en la tabla Solicitud)
        public string ProveedorRazonSocial { get; set; }
        public List<ProductoCotizacion530BA> Productos { get; set; } = new List<ProductoCotizacion530BA>();

        private string _descripcionAmigable;

        // Propiedad calculada para el ComboBox de la UI.
        // La BLL la sobreescribe con la version localizada segun el idioma activo;
        // el fallback queda como red de seguridad para usos fuera de la BLL.
        public string DescripcionAmigable
        {
            get => _descripcionAmigable ?? $"Solicitud #{Id} - {ProveedorRazonSocial} ({Fecha:dd/MM/yyyy})";
            set => _descripcionAmigable = value;
        }
    }
}
