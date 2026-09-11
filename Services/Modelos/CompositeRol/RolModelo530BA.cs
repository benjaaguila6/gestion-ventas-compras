using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Modelos
{
    public class RolModelo530BA
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<Componente530BA> Permisos { get; set; } = new List<Componente530BA>();

        public override string ToString()
        {
            return this.Nombre;
        }
        public List<Componente530BA> ObtenerPermisos()
        {
            List<Componente530BA> permisos = new List<Componente530BA >();

            foreach (Componente530BA hijo in this.Permisos)
            {
                permisos.AddRange(hijo.obtenerPermisos());
            }

            return permisos.GroupBy(p => p.Id).Select(grupo => grupo.First()).ToList(); // esto permite que no se le asignen permisos duplicados
        }
    }
}
