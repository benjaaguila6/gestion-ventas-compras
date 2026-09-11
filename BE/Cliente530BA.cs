using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Cliente530BA
    {

        // ctor para crear
        public Cliente530BA(string nombreCompleto, string dNI, string email, int codPostal, string localidad, string direccion)
        {
            NombreCompleto = nombreCompleto;
            DNI = dNI;
            Email = email;
            CodPostal = codPostal;
            Localidad = localidad;
            Direccion = direccion;
        }

        // ctor para mapear
        public Cliente530BA(int id, string nombreCompleto, string dNI, string email, int codPostal, string localidad, string direccion)
        {
            Id = id;
            NombreCompleto = nombreCompleto;
            DNI = dNI;
            Email = email;
            CodPostal = codPostal;
            Localidad = localidad;
            Direccion = direccion;
        }

        public int Id { get; set; }
        public long DVH { get; set; }
        public string NombreCompleto { get; set; }
        public string DNI { get; set; }
        public string Email { get; set; }
        public int CodPostal { get; set; }
        public string Localidad { get; set; }
        public string Direccion { get; set; }
    }
}
