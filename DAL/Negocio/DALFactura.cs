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
    public class DALFactura
    {
        DALAcceso530BA _dal = new DALAcceso530BA();

        
        public int InsertFactura(Factura530BA factura, List<ItemFactura530BA> items, List<KeyValuePair<int, int>> detalleIds) // preserva orden: par = (idDetalle, codProducto) por cada ítem
        {
            using (SqlConnection conn = new SqlConnection(_dal.CadenaConexion))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    long idFactura;

                    // encabezado
                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO Factura (DNI, Fecha, Total, Estado, DVH)
                        VALUES (@dni, @fecha, @total, @estado, @dvh);
                        SELECT CAST(SCOPE_IDENTITY() AS BIGINT);", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@dni", factura.DNI);
                        cmd.Parameters.AddWithValue("@fecha", factura.Fecha);
                        cmd.Parameters.AddWithValue("@total", factura.Total);
                        cmd.Parameters.AddWithValue("@estado", (int)factura.Estado);
                        cmd.Parameters.AddWithValue("@dvh", factura.DVH);
                        idFactura = Convert.ToInt64(cmd.ExecuteScalar());
                    }

                    // detalle. El stock NO se toca todavia.
                    foreach (ItemFactura530BA item in items)
                    {
                        int idDetalle;

                        using (SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO DetalleFactura (IdFactura, codProducto, Cantidad, PrecioUnitario, Subtotal, DVH)
                            VALUES (@idFactura, @codProducto, @cantidad, @precioUnitario, @subtotal, NULL);
                            SELECT CAST(SCOPE_IDENTITY() AS INT);", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@idFactura", idFactura);
                            cmd.Parameters.AddWithValue("@codProducto", item.codProducto);
                            cmd.Parameters.AddWithValue("@cantidad", item.cantidad);
                            cmd.Parameters.AddWithValue("@precioUnitario", item.precioUnitario);
                            cmd.Parameters.AddWithValue("@subtotal", item.Subtotal);
                            idDetalle = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        detalleIds.Add(new KeyValuePair<int, int>(idDetalle, item.codProducto));
                    }

                    tran.Commit();
                    return (int)idFactura;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public void CambiarEstado(int idFactura)
        {
            string query = @"
                UPDATE Factura
                SET Estado = @estadoPagada
                WHERE Id = @idFactura AND Estado = @estadoPendiente;";

            var parametros = new Dictionary<string, object>
            {
                { "@idFactura", idFactura },
                { "@estadoPagada", (int)BE.Enum.EstadoFactura530BA.Pagada },
                { "@estadoPendiente", (int)BE.Enum.EstadoFactura530BA.Pendiente }
            };

            _dal.executeNonQuery(query, parametros);
        }

        public decimal ObtenerTotalFactura(int idFactura)
        {
            string query = "SELECT Total FROM Factura WHERE Id = @idFactura";

            var parametros = new Dictionary<string, object>
            {
                { "@idFactura", idFactura }
            };

            var resultado = _dal.executeScalar(query, parametros);

            return Convert.ToDecimal(resultado);
        }

        // Detalle persistido de la factura: pares (codProducto, cantidad) a descontar.
        public List<ItemFactura530BA> ObtenerDetalleFactura(int idFactura)
        {
            List<ItemFactura530BA> lista = new List<ItemFactura530BA>();

            string query = "SELECT CodProducto, Cantidad FROM DetalleFactura WHERE IdFactura = @IdFactura";

            var parametros = new Dictionary<string, object>
            {
                { "@idFactura", idFactura }
            };

            DataTable dt = _dal.executeDataTable(query, parametros);


            foreach (DataRow row in dt.Rows)
            {
                ItemFactura530BA item = new ItemFactura530BA();
                item.codProducto = Convert.ToInt32(row["CodProducto"]);
                item.cantidad = Convert.ToInt32(row["Cantidad"]);

                lista.Add(item);
            }

            return lista;
        }

        public void ActualizarDVHDetalle(int idDetalle, long dvh)
        {
            string query = "UPDATE DetalleFactura SET DVH = @dvh WHERE Id = @id";
            var parametros = new Dictionary<string, object>
            {
                { "@id", idDetalle },
                { "@dvh", dvh }
            };
            _dal.executeNonQuery(query, parametros);
        }
        public void ActualizarDVHFactura(int idFactura, long dvh)
        {
            string query = "UPDATE Factura SET DVH = @dvh WHERE Id = @id";
            var parametros = new Dictionary<string, object>
            {
                { "@id", idFactura },
                { "@dvh", dvh }
            };
            _dal.executeNonQuery(query, parametros);
        }

        public DataRow ObtenerPagoPorFactura(int idFactura)
        {
            string query = "SELECT * FROM Pago WHERE IdFactura = @idFactura";

            var parametros = new Dictionary<string, object>
            {
                {"@idFactura", idFactura }
            };

            DataTable dt = _dal.executeDataTable(query, parametros);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public DataTable ObtenerTodas()
        {
            return _dal.executeDataTable("SELECT * FROM Factura");
        }

        public DataTable ObtenerDetalles()
        {
            return _dal.executeDataTable("SELECT * FROM DetalleFactura");
        }
    }
}