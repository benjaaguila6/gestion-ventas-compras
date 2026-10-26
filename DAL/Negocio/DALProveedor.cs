using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Negocio
{
    public class DALProveedor
    {
        DALAcceso530BA dal = new DALAcceso530BA();

        public DataTable ObtenerTodos()
        {
            string query = "SELECT * FROM Proveedor";

            return dal.executeDataTable(query);
        }

        public DataRow ObtenerPorEmail(string email)
        {
            string query = "SELECT Id, Email, RazonSocial, Telefono, Cuit, Direccion, CVU, Estado FROM Proveedor WHERE Email = @Email";
            
            var parametros = new Dictionary<string, object> { { "@Email", email } };

            DataTable dt = dal.executeDataTable(query, parametros);
            
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0];
            }

            return null; // Inexistente
        }

        public DataRow ObtenerPorId(int id)
        {
            string query = "SELECT Id, Email, RazonSocial, Telefono, Cuit, Direccion, CVU, Estado, Activo FROM Proveedor WHERE Id = @Id";
            var param = new Dictionary<string, object> { { "@Id", id } };

            DataTable dt = dal.executeDataTable(query, param);

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0];
            }

                return null; // Inexistente
        }

        public void PreRegistrarProveedor(Proveedor530BA proveedor)
        {
            string query = @"INSERT INTO Proveedor (Email, RazonSocial, Telefono, Estado) VALUES (@Email, @RazonSocial, @Telefono, 'PreRegistrado');";

            var parametros = new Dictionary<string, object>
            {
                { "@Email", proveedor.Email },
                { "@RazonSocial", proveedor.RazonSocial },
                { "@Telefono", proveedor.Telefono }
            };

            dal.executeScalar(query, parametros);
        }

        public void RegistrarDefinitivo(Proveedor530BA proveedor)
        {
            string query = @"UPDATE Proveedor SET RazonSocial = @RazonSocial, Cuit = @Cuit, Direccion = @Direccion, CVU = @CVU, Estado = 'Registrado' WHERE Id = @Id AND Estado = 'PreRegistrado'";

            var parametros = new Dictionary<string, object>
            {
                { "@Id", proveedor.Id },
                { "@RazonSocial", proveedor.RazonSocial },
                { "@Cuit", proveedor.Cuit },
                { "@Direccion", proveedor.Direccion },
                { "@CVU", proveedor.CVU }
            };

            dal.executeNonQuery(query, parametros);
        }

        public void ModificarProveedor(int idProveedor, string telefono, string direccion, string cvu)
        {
            string query = @"UPDATE Proveedor SET Telefono = @telefono, Direccion = @direccion, CVU = @cvu WHERE Id = @id";

            var parametros = new Dictionary<string, object>
            {
                {"@telefono", telefono },
                {"@direccion", direccion },
                {"@cvu", cvu },
                {"@id", idProveedor }
            };

            dal.executeNonQuery(query, parametros);
        }

        public void CambiarEstadoActivo(int idProveedor)
        {
            string query = @"UPDATE Proveedor SET Activo = CASE WHEN Activo = 1 THEN 0 ELSE 1 END WHERE Id = @Id";

            var parametros = new Dictionary<string, object>
            {
                { "@Id", idProveedor }
            };

            dal.executeNonQuery(query, parametros);
        }

        public void ActualizarDVH(int id, long dvh)
        {
            string query = "UPDATE Proveedor SET DVH = @DVH WHERE Id = @Id";

            var parametros = new Dictionary<string, object>
            {
                {"@DVH", dvh },
                {"@Id", id }
            };

            dal.executeNonQuery(query, parametros);
        }
    }
}
