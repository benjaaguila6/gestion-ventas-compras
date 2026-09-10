using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Modelos
{
    public class PermisoModelo530BA : Componente530BA
    {
        public override List<Componente530BA> obtenerPermisos()
        {
            return new List<Componente530BA> { this }; // se devuelve a si mismo
        }

        public override string ToString()
        {
            return $"Patente {this.Nombre}";
        }
    }
}
