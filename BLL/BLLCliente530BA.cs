using BE;
using DAL.Negocio;
using Services;
using Services_530BA;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLLCliente530BA
    {
        DALCliente dal = new DALCliente();
        BitacoraEventosService bit = new BitacoraEventosService();
        public List<Cliente530BA> ObtenerTodos()
        {
            DataTable dt = dal.ObtenerTodos();

            List<Cliente530BA> list = new List<Cliente530BA>();

            foreach(DataRow dr in dt.Rows)
            {
                list.Add(MapearCliente(dr));
            }

            return list;
        }
        public void crearCliente(string nombreCompleto, string dNI, string email, int codPostal, string localidad, string direccion)
        {
            if(dal.ObtenerPorDNI(dNI) != null)
            {
                throw new Exception("Ya existe un cliente con ese DNI"); // falta traduccion
            }

            Cliente530BA cliente = new Cliente530BA(nombreCompleto, dNI, email, codPostal, localidad, direccion);

            long dvh = CalcularDVHCliente(nombreCompleto, dNI, email, codPostal, localidad, direccion);

            dal.InsertCliente(cliente, dvh);

            Services.DigitoVerificador530BA.ActualizarDVVCliente();

            string dniAutor = ServiceSessionManager530BA.getIntancia().usuarioActivo.DNI;
            bit.registrarEvento(dniAutor, "Se creo un nuevo cliente con DNI: " + dNI, BE.Enum.Criticidad530BA.Medio, BE.Enum.Modulos530BA.Cliente);
            
            //mas adelante bitacora de cambios.
        }

        public void eliminarCliente(string dNI)
        {
            DataRow dr = dal.ObtenerPorDNI(dNI);
            
            if (dr == null)
            {
                throw new Exception("No existe un cliente con ese DNI"); // falta traduccion
            }

            dal.DeleteCliente(dNI);
            Services.DigitoVerificador530BA.ActualizarDVVCliente();

            string dniAutor = ServiceSessionManager530BA.getIntancia().usuarioActivo.DNI;
            bit.registrarEvento(dniAutor, "Se eliminó un cliente con DNI: " + dNI, BE.Enum.Criticidad530BA.Medio, BE.Enum.Modulos530BA.Cliente);
        }

        public void modificarCliente(string dni, string email, int codPostal, string localidad, string direccion)
        {
            DataRow dr = dal.ObtenerPorDNI(dni);

            if (dr == null)
            {
                throw new Exception("No existe un cliente con ese DNI"); // falta traduccion
            }
            Cliente530BA cliente = MapearCliente(dr);

            cliente.Email = email;
            cliente.CodPostal = codPostal;
            cliente.Localidad = localidad;
            cliente.Direccion = direccion;

            long dvh = CalcularDVHCliente(cliente.NombreCompleto, cliente.DNI, email, codPostal, localidad, direccion);
            dal.UpdateCliente(cliente, dvh);

            Services.DigitoVerificador530BA.ActualizarDVVCliente();
            string dniAutor = ServiceSessionManager530BA.getIntancia().usuarioActivo.DNI;
            bit.registrarEvento(dniAutor, "Se modificó un cliente" , BE.Enum.Criticidad530BA.Medio, BE.Enum.Modulos530BA.Cliente);
        }
        public Cliente530BA MapearCliente(DataRow row)
        {

            if(row == null)
            {
                return null;
            }

            int id = Convert.ToInt32(row["Id"]);
            string nombreCompleto = row["NombreCompleto"].ToString();
            string dNI = row["DNI"].ToString();
            string email = row["Email"].ToString();
            int codPostal = Convert.ToInt32(row["CodPostal"]);
            string localidad = row["Localidad"].ToString();
            string direccion = row["Direccion"].ToString();
            return new Cliente530BA(id, nombreCompleto, dNI, email, codPostal, localidad, direccion);
        }

        #region Digito Verificador
        private long CalcularDVHCliente(string nombreCompleto, string dNI, string email, int codPostal, string localidad, string direccion)
        {
            string cadena = nombreCompleto + dNI + email + codPostal + localidad + direccion;

            return Services.DigitoVerificador530BA.CalcularDVH(cadena);
        }

        #endregion Digito Verificador
    }
}
