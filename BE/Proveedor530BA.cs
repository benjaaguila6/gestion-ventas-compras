using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Proveedor530BA
    {
        public Proveedor530BA()
        {

        }

        // Constructor para el pre-registro
        public Proveedor530BA(string email, string razonSocial, string telefono)
        {
            Email = email;
            RazonSocial = razonSocial;
            Telefono = telefono;
            Estado = "PreRegistrado";
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string RazonSocial { get; set; }
        public string Telefono { get; set; }
        public bool Activo { get; set; }
        public long DVH { get; set; }

        // Estos campos admiten null
        public string Cuit { get; set; }
        public string Direccion { get; set; }
        public string CVU { get; set; }

        public string Estado { get; set; }

        // Propiedad calculada útil para validaciones rápidas en UI o BLL
        public bool EstaRegistradoDefinitivamente => Estado == "Registrado";

        public override string ToString()
        {
            return RazonSocial ?? string.Empty;
        }
    }
}
