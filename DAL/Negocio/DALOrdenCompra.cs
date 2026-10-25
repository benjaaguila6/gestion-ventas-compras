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

        public DataTable ObtenerOrdenes()
        {
            string query = @"SELECT o.Id, o.IdProveedor, o.Recibido, o.FechaEmision, o.DVH, p.RazonSocial,
                                    CASE WHEN o.Recibido = 1 THEN 2
                                         WHEN EXISTS (SELECT 1 FROM Producto_OC po
                                                      WHERE po.IdOrden = o.Id AND po.CantidadRecibida > 0) THEN 1
                                         ELSE 0 END AS EstadoRecepcion
                             FROM OrdenCompra o
                             INNER JOIN Proveedor p ON o.IdProveedor = p.Id
                             ORDER BY o.FechaEmision DESC, o.Id DESC";

            return _dal.executeDataTable(query);
        }

        public DataTable ObtenerOrdenPorId(int idOrden)
        {
            string query = @"SELECT o.Id, o.IdProveedor, o.Recibido, o.FechaEmision, o.DVH, p.RazonSocial,
                                    CASE WHEN o.Recibido = 1 THEN 2
                                         WHEN EXISTS (SELECT 1 FROM Producto_OC po
                                                      WHERE po.IdOrden = o.Id AND po.CantidadRecibida > 0) THEN 1
                                         ELSE 0 END AS EstadoRecepcion
                             FROM OrdenCompra o
                             INNER JOIN Proveedor p ON o.IdProveedor = p.Id
                             WHERE o.Id = @idOrden";

            var parametros = new Dictionary<string, object>
            {
                { "@idOrden", idOrden }
            };

            DataTable dt = _dal.executeDataTable(query, parametros);

            return dt.Rows.Count > 0 ? dt : null;
        }

        public DataTable ObtenerProductosPorOrden(int idOrden)
        {
            string query = @"SELECT po.IdOrden, po.codProducto, po.Cantidad, po.CantidadRecibida, po.Precio, po.DVH,
                                    pr.Nombre AS NombreProducto
                             FROM Producto_OC po
                             INNER JOIN Producto pr ON po.codProducto = pr.codProducto
                             WHERE po.IdOrden = @idOrden
                             ORDER BY pr.Nombre";

            var parametros = new Dictionary<string, object>
            {
                { "@idOrden", idOrden }
            };

            return _dal.executeDataTable(query, parametros);
        }

        public int ActualizarCantidadRecibidaProductoOC(int idOrden, int codProducto, int cantidadRecibida, long dvh)
        {
            string query = @"UPDATE Producto_OC
                             SET CantidadRecibida = @cantidadRecibida, DVH = @dvh
                             WHERE IdOrden = @idOrden AND codProducto = @codProducto
                               AND @cantidadRecibida <= Cantidad";

            var parametros = new Dictionary<string, object>
            {
                { "@idOrden", idOrden },
                { "@codProducto", codProducto },
                { "@cantidadRecibida", cantidadRecibida },
                { "@dvh", dvh }
            };

            return _dal.executeNonQuery(query, parametros);
        }

        public int MarcarComoRecibida(int idOrden, long dvh)
        {
            string query = @"UPDATE OrdenCompra
                             SET Recibido = 1, DVH = @dvh
                             WHERE Id = @idOrden AND Recibido = 0";

            var parametros = new Dictionary<string, object>
            {
                { "@idOrden", idOrden },
                { "@dvh", dvh }
            };

            return _dal.executeNonQuery(query, parametros);
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
