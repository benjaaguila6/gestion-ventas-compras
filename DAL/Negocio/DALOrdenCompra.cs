using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Negocio
{
    public class DALOrdenCompra
    {
        DALAcceso530BA _dal = new DALAcceso530BA();

        public int InsertarOrdenCompra(OrdenCompra530BA orden)
        {
            string query = @"INSERT INTO OrdenCompra (IdProveedor, Recibido, FechaEmision, DVH)
                             VALUES (@idProveedor, @recibido, @fechaEmision, @dvh);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var parametros = new Dictionary<string, object>
            {
                { "@idProveedor", orden.IdProveedor },
                { "@recibido", orden.Recibido },
                { "@fechaEmision", orden.FechaEmision },
                { "@dvh", orden.DVH }
            };

            return Convert.ToInt32(_dal.executeScalar(query, parametros));
        }

        public void InsertarProductosOC(int idOrden, List<ProductoOC530BA> productos)
        {
            string query = @"INSERT INTO Producto_OC (IdOrden, codProducto, Cantidad, Precio, DVH)
                             VALUES (@idOrden, @codProducto, @cantidad, @precio, @dvh);";

            foreach (ProductoOC530BA producto in productos)
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@idOrden", idOrden },
                    { "@codProducto", producto.codProducto },
                    { "@cantidad", producto.Cantidad },
                    { "@precio", producto.Precio },
                    { "@dvh", producto.DVH }
                };

                _dal.executeNonQuery(query, parametros);
            }
        }

        public void ActualizarDVHOrdenCompra(int idOrden, long dvh)
        {
            string query = "UPDATE OrdenCompra SET DVH = @dvh WHERE Id = @id";
            var parametros = new Dictionary<string, object>
            {
                { "@id", idOrden },
                { "@dvh", dvh }
            };
            _dal.executeNonQuery(query, parametros);
        }

        public void ActualizarDVHProductoOC(int idOrden, int codProducto, long dvh)
        {
            string query = "UPDATE Producto_OC SET DVH = @dvh WHERE IdOrden = @idOrden AND codProducto = @codProducto";
            var parametros = new Dictionary<string, object>
            {
                { "@idOrden", idOrden },
                { "@codProducto", codProducto },
                { "@dvh", dvh }
            };
            _dal.executeNonQuery(query, parametros);
        }

        public DataTable ObtenerTodas()
        {
            return _dal.executeDataTable("SELECT * FROM OrdenCompra");
        }

        public DataTable ObtenerProductos()
        {
            return _dal.executeDataTable("SELECT * FROM Producto_OC");
        }
    }
}
