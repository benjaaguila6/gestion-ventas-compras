using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Modelos
{
    public class FamiliaModelo530BA : Componente530BA
    {
        private List<Componente530BA> hijos = new List<Componente530BA>();

        public override void agregarHijos(Componente530BA c)
        {
            hijos.Add(c);
        }

        public override void eliminarHijo(Componente530BA c)
        {
            hijos.Remove(c);
        }

        public override List<Componente530BA> obtenerPermisos()
        {
            List<Componente530BA> permisos = new List<Componente530BA>();

            foreach (Componente530BA hijo in hijos)
            {
                permisos.AddRange(hijo.obtenerPermisos());
            }

            return permisos;
        }

        public override string ToString()
        {
            return $"Familia {this.Nombre}";
        }
    }
}
