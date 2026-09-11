using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL.Negocio
{
    public class DALCliente
    {
        DALAcceso530BA dal = new DALAcceso530BA();

        public DataTable ObtenerTodos()
        {
            string query = "SELECT * FROM Cliente";
            return dal.executeDataTable(query);
        }

        public DataRow ObtenerPorDNI(string dni)
        {
            string query = $"SELECT * FROM Cliente WHERE DNI = @dni";

            var parametros = new Dictionary<string, object>
            {
                {"@dni", dni }
            };

            DataTable dt = dal.executeDataTable(query, parametros);

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0];
            }
            else
            {
                return null;
            }
        }


        public void InsertCliente(Cliente530BA cliente, long dvh)
        {
            string query = @"INSERT INTO Cliente (NombreCompleto, DNI, Email, CodPostal, Localidad, Direccion, DVH) 
                             VALUES (@NombreCompleto, @DNI, @Email, @CodPostal, @Localidad, @Direccion, @DVH)";

            var parametros = new Dictionary<string, object>
            {
                {"@NombreCompleto", cliente.NombreCompleto },
                {"@DNI", cliente.DNI },
                {"@Email", cliente.Email },
                {"@CodPostal", cliente.CodPostal },
                {"@Localidad", cliente.Localidad },
                {"@Direccion", cliente.Direccion },
                {"@DVH", dvh }
            };

            dal.executeNonQuery(query, parametros);
        }

        public void DeleteCliente(string dni)
        {
            string query = @"DELETE FROM Cliente WHERE DNI = @dni";

            var parametros = new Dictionary<string, object>
            {
                {"@dni", dni }
            };

            dal.executeNonQuery(query, parametros);
        }

        public void UpdateCliente(Cliente530BA cliente, long dvh)
        {
            string query = @"UPDATE Cliente
                     SET Email = @Email,
                         CodPostal = @CodPostal,
                         Localidad = @Localidad,
                         Direccion = @Direccion,
                         DVH = @DVH
                     WHERE Id = @id";

            var parametros = new Dictionary<string, object>
                {
                    { "@id", cliente.Id },
                    { "@Email", cliente.Email },
                    { "@CodPostal", cliente.CodPostal },
                    { "@Localidad", cliente.Localidad },
                    { "@Direccion", cliente.Direccion },
                    { "@DVH", dvh }
                };

            dal.executeNonQuery(query, parametros);
        }
        public void ActualizarDVH(int id, long dvh)
        {
            string query = @"UPDATE Cliente
                     SET DVH = @dvh
                     WHERE Id = @id";

            var parametros = new Dictionary<string, object>
                {
                    { "@id", id },
                    { "@dvh", dvh }
                };

            dal.executeNonQuery(query, parametros);
        }
    }
}
