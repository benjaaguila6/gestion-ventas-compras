using BE;
using Services;
using Services.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services_530BA
{
    public sealed class ServiceSessionManager530BA
    {
        private ServiceSessionManager530BA() 
        {
            Idioma = new IdiomaManager();
        }

        private static ServiceSessionManager530BA _instancia;

        public UsuarioModelo530BA usuarioActivo { get; private set; }

        public static ServiceSessionManager530BA getIntancia()
        {
            if( _instancia == null)
            {
                _instancia = new ServiceSessionManager530BA();
            }

            return _instancia;
        }

        public void Login(UsuarioModelo530BA usuario)
        {
            usuarioActivo = usuario;
        }

        public void Logout()
        {
            usuarioActivo = null;
        }

        public bool estaLogueado()
        {
            return usuarioActivo != null;
        }


        public bool TienePermiso(string nombrePermiso)
        {
            List<Componente530BA> todosLosPermisos = usuarioActivo.Rol.ObtenerPermisos();

            //recorremos la lista buscando coincidencia por el nombre de la patente
            foreach (Componente530BA componente in todosLosPermisos)
            {
                if (string.Equals(componente.Nombre, nombrePermiso, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }


        public IdiomaManager Idioma { get; private set; }

    }
}
