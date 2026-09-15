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
        DALAcceso530BA acceso = new DALAcceso530BA();

        public int InsertFactura(Factura530BA factura, List<ItemFactura530BA> items, List<KeyValuePair<int, int>> detalleIds) // preserva orden: par = (idDetalle, codProducto) por cada ítem
        {
            using (SqlConnection conn = new SqlConnection(acceso.CadenaConexion))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    long idFactura;

                    // encabezado
                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO Factura (DNI, Fecha, Total, DVH)
                        VALUES (@dni, @fecha, @total, @dvh);
                        SELECT CAST(SCOPE_IDENTITY() AS BIGINT);", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@dni", factura.DNI);
                        cmd.Parameters.AddWithValue("@fecha", factura.Fecha);
                        cmd.Parameters.AddWithValue("@total", factura.Total);
                        cmd.Parameters.AddWithValue("@dvh", factura.DVH);
                        idFactura = Convert.ToInt64(cmd.ExecuteScalar());
                    }

                    // detalle + descuento de stock por cada ítem
                    foreach (ItemFactura530BA item in items)
                    {
                        int idDetalle;

                        // Insertar línea de detalle
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

                        // descontar stock condicional (ATOÓMICO con rowcount)
                        using (SqlCommand cmd = new SqlCommand(@"
                            UPDATE Producto
                            SET Existencia = Existencia - @cantidad
                            WHERE codProducto = @codProducto AND Existencia >= @cantidad;", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@codProducto", item.codProducto);
                            cmd.Parameters.AddWithValue("@cantidad", item.cantidad);

                            int filas = cmd.ExecuteNonQuery();
                            if (filas == 0)
                            {
                                throw new InvalidOperationException("Stock insuficiente");
                            }
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

        public void ActualizarDVHDetalle(int idDetalle, long dvh)
        {
            string query = "UPDATE DetalleFactura SET DVH = @dvh WHERE Id = @id";
            var parametros = new Dictionary<string, object>
            {
                { "@id", idDetalle },
                { "@dvh", dvh }
            };
            acceso.executeNonQuery(query, parametros);
        }
        public void ActualizarDVHFactura(int idFactura, long dvh)
        {
            string query = "UPDATE Factura SET DVH = @dvh WHERE Id = @id";
            var parametros = new Dictionary<string, object>
            {
                { "@id", idFactura },
                { "@dvh", dvh }
            };
            acceso.executeNonQuery(query, parametros);
        }

        public DataTable ObtenerTodas()
        {
            return acceso.executeDataTable("SELECT * FROM Factura");
        }

        public DataTable ObtenerDetalles()
        {
            return acceso.executeDataTable("SELECT * FROM DetalleFactura");
        }
    }
}