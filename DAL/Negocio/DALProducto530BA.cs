using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Negocio
{
    public class DALProducto530BA
    {
        DALAcceso530BA dal = new DALAcceso530BA();

        public DataTable ObtenerTodos()
        {
            string query = "SELECT * FROM Producto";

            return dal.executeDataTable(query);
        }

        public DataRow ObtenerPorNombre(string nombre)
        {
            string query = "SELECT * FROM Producto WHERE Nombre = @nombre";

            var parametros = new Dictionary<string, object>
            {
                { "@nombre", nombre }
            };

            DataTable dt = dal.executeDataTable(query, parametros);

            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public void InsertarProducto(Producto530BA producto)
        {
            string query = "INSERT INTO Producto (Nombre, Existencia, PrecioUnitario) VALUES (@nombre, @existencia, @precioUnitario)";
            
            var parametros = new Dictionary<string, object>
            {
                { "@nombre", producto.nombre },
                { "@existencia", producto.existencia },
                { "@precioUnitario", producto.precioUnitario }
            };

            dal.executeNonQuery(query, parametros);
        }

        public void UpdateProducto(Producto530BA producto)
        {
            string query = "UPDATE Producto SET Nombre = @nombre, PrecioUnitario = @precioUnitario WHERE codProducto = @codProducto";
            var parametros = new Dictionary<string, object>
            {
                { "@codProducto", producto.codProducto },
                { "@nombre", producto.nombre },
                { "@precioUnitario", producto.precioUnitario }
            };

            dal.executeNonQuery(query, parametros);
        }

        public void ActualizarExistencia(int codProducto, int nuevaExistencia)
        {
            string query = "UPDATE Producto SET Existencia = @nuevaExistencia WHERE codProducto = @codProducto";
            
            var parametros = new Dictionary<string, object>
            {
                { "@codProducto", codProducto },
                { "@nuevaExistencia", nuevaExistencia }
            };

            dal.executeNonQuery(query, parametros);
        }

        public void ActivarProducto(int codProducto)
        {
            string query = "UPDATE Producto SET Activo = 1 WHERE codProducto = @codProducto";

            var parametros = new Dictionary<string, object>
            {
                { "@codProducto", codProducto }
            };

            dal.executeNonQuery(query, parametros);
        }

        public void DesactivarProducto(int codProducto)
        {
            string query = "UPDATE Producto SET Activo = 0 WHERE codProducto = @codProducto";
            
            var parametros = new Dictionary<string, object>
            {
                { "@codProducto", codProducto }
            };

            dal.executeNonQuery(query, parametros);
        }
    }
}
