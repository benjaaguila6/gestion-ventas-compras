using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Negocio
{
    public class DALSolicitudCotizacion
    {
        DALAcceso530BA _dal = new DALAcceso530BA();
        public int InsertSolicitudCotizacion(SolicitudCotizacion530BA cabecera, List<ProductoCotizacion530BA> items, List<KeyValuePair<int, int>> detalleIds)
        {
            using (SqlConnection conn = new SqlConnection(_dal.CadenaConexion))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    long idSolicitud;

                    // encabezado
                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO SolicitudCotizacion (IdProveedor, Fecha, DVH)
                        VALUES (@idProveedor, @fecha, @dvh);
                        SELECT CAST(SCOPE_IDENTITY() AS BIGINT);", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@idProveedor", cabecera.IdProveedor);
                        cmd.Parameters.AddWithValue("@fecha", cabecera.Fecha);
                        cmd.Parameters.AddWithValue("@dvh", cabecera.DVH);
                        idSolicitud = Convert.ToInt64(cmd.ExecuteScalar());
                    }

                    // detalle. El DVH necesita el IdSolicitud recien generado, por eso se
                    // inserta NULL y la BLL lo actualiza despues (igual que DetalleFactura).
                    foreach (ProductoCotizacion530BA item in items)
                    {
                        int idDetalle;

                        using (SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO ProductoCotizacion (IdSolicitud, codProducto, Cantidad, DVH)
                            VALUES (@idSolicitud, @codProducto, @cantidad, NULL);
                            SELECT CAST(SCOPE_IDENTITY() AS INT);", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@idSolicitud", idSolicitud);
                            cmd.Parameters.AddWithValue("@codProducto", item.codProducto);
                            cmd.Parameters.AddWithValue("@cantidad", item.Cantidad);
                            idDetalle = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        detalleIds.Add(new KeyValuePair<int, int>(idDetalle, item.codProducto));
                    }

                    tran.Commit();
                    return (int)idSolicitud;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public DataTable ObtenerSolicitudesPendientes()
        {
            string query = @"SELECT s.Id, s.IdProveedor, s.Fecha, s.DVH, p.RazonSocial 
                             FROM SolicitudCotizacion s
                             INNER JOIN Proveedor p ON s.IdProveedor = p.Id
                             ORDER BY s.Fecha DESC";

            return _dal.executeDataTable(query);
        }

        public DataTable ObtenerProductosPorSolicitud(int idSolicitud)
        {
            string query = @"SELECT pc.Id, pc.IdSolicitud, pc.codProducto, pc.Cantidad, pc.DVH, pr.Nombre as NombreProducto
                             FROM ProductoCotizacion pc
                             INNER JOIN Producto pr ON pc.codProducto = pr.codProducto
                             WHERE pc.IdSolicitud = @IdSolicitud";

            var parametros = new Dictionary<string, object>
            {
                { "@IdSolicitud", idSolicitud }
            };

            return _dal.executeDataTable(query, parametros);
        }

        public void ActualizarDVHProductoCotizacion(int idDetalle, long dvh)
        {
            string query = "UPDATE ProductoCotizacion SET DVH = @dvh WHERE Id = @id";
            var parametros = new Dictionary<string, object>
            {
                { "@id", idDetalle },
                { "@dvh", dvh }
            };
            _dal.executeNonQuery(query, parametros);
        }

        public void ActualizarDVHSolicitudCotizacion(int idSolicitud, long dvh)
        {
            string query = "UPDATE SolicitudCotizacion SET DVH = @dvh WHERE Id = @id";
            var parametros = new Dictionary<string, object>
            {
                { "@id", idSolicitud },
                { "@dvh", dvh }
            };
            _dal.executeNonQuery(query, parametros);
        }

        public DataTable ObtenerTodas()
        {
            return _dal.executeDataTable("SELECT * FROM SolicitudCotizacion");
        }

        public DataTable ObtenerDetalles()
        {
            return _dal.executeDataTable("SELECT * FROM ProductoCotizacion");
        }
    }
}
