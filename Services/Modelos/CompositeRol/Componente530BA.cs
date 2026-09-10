using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Modelos
{
    public abstract class Componente530BA
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        
        public virtual void agregarHijos(Componente530BA c)
        {
            throw new NotImplementedException();
        }
        public virtual void eliminarHijo(Componente530BA c)
        {
            throw new NotImplementedException();
        }
        public virtual List<Componente530BA> obtenerPermisos()
        {
            throw new NotImplementedException();
        }
    }
}
