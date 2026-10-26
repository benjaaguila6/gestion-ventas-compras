using BE;
using BE.Enum;
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
    public class BLLOrdenCompra530BA
    {
        DALOrdenCompra dalOrden = new DALOrdenCompra();
        BLLSolicitudCotizacion530BA bllSolicitud = new BLLSolicitudCotizacion530BA();
        BLLProveedor530BA bllProveedor = new BLLProveedor530BA();
        BLLProducto530BA bllProducto = new BLLProducto530BA();
        BitacoraEventosService bit = new BitacoraEventosService();

        public int GenerarOrdenCompra(int idSolicitud, List<ProductoCotizacion530BA> items, Pago530BA pago)
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;

            if (items == null || items.Count == 0)
            {
                throw new Exception(idioma.Translate("ExcOCSinItems"));
            }

            if (items.Any(i => i.Cantidad <= 0))
            {
                throw new Exception(idioma.Translate("ExcCantidadInvalida"));
            }

            if (items.GroupBy(i => i.codProducto).Any(g => g.Count() > 1))
            {
                throw new Exception(idioma.Translate("ExcProductoDuplicado"));
            }

            if (pago == null)
            {
                throw new Exception(idioma.Translate("ExcOCPagoObligatorio"));
            }

            if (string.IsNullOrWhiteSpace(pago.NombreTitular))
            {
                throw new Exception(idioma.Translate("ExcOCPagoTitularObligatorio"));
            }

            if (string.IsNullOrWhiteSpace(pago.Ultimos4) || pago.Ultimos4.Length != 4)
            {
                throw new Exception(idioma.Translate("ExcOCPagoUltimos4Invalidos"));
            }

            SolicitudCotizacion530BA solicitud = bllSolicitud.ObtenerPorId(idSolicitud);

            if (solicitud == null)
            {
                throw new Exception(idioma.Translate("ExcSolicitudNoExiste"));
            }

            Proveedor530BA proveedor = bllProveedor.ObtenerPorId(solicitud.IdProveedor);

            if (proveedor == null || !proveedor.EstaRegistradoDefinitivamente)
            {
                throw new Exception(idioma.Translate("ExcProveedorNoRegistrado"));
            }

            // El precio vigente lo define la BD. El operador solo puede ajustar cantidades.
            List<ProductoCotizacion530BA> productosCotizados = bllSolicitud.ObtenerProductosPorSolicitud(idSolicitud);

            foreach (ProductoCotizacion530BA item in items)
            {
                ProductoCotizacion530BA cotizado = productosCotizados.FirstOrDefault(p => p.codProducto == item.codProducto);

                if (cotizado == null)
                {
                    throw new Exception(idioma.Translate("ExcOCProductoNoPertenece"));
                }

                if (cotizado.PrecioUnitario == null || cotizado.PrecioUnitario <= 0)
                {
                    throw new Exception(idioma.Translate("ExcOCPrecioInvalido"));
                }

                item.PrecioUnitario = cotizado.PrecioUnitario;
                item.NombreProducto = cotizado.NombreProducto;
            }

            decimal total = items.Sum(i => i.Cantidad * i.PrecioUnitario.Value);

            if (pago.Monto != total)
            {
                throw new Exception(idioma.Translate("ExcOCPagoMontoInvalido"));
            }

            DateTime ahora = DateTime.Now;
            DateTime fechaEmision = new DateTime(ahora.Year, ahora.Month, ahora.Day, ahora.Hour, ahora.Minute, ahora.Second);

            OrdenCompra530BA orden = new OrdenCompra530BA
            {
                IdProveedor = solicitud.IdProveedor,
                Recibido = false,
                FechaEmision = fechaEmision
            };

            orden.DVH = DigitoVerificador530BA.CalcularDVH(orden.IdProveedor.ToString() + orden.Recibido.ToString() + orden.FechaEmision.ToString());

            int idOrden = dalOrden.InsertarOrdenCompra(orden);

            List<ProductoOC530BA> productosOC = new List<ProductoOC530BA>();

            foreach (ProductoCotizacion530BA item in items)
            {
                ProductoOC530BA producto = new ProductoOC530BA
                {
                    IdOrden = idOrden,
                    codProducto = item.codProducto,
                    Cantidad = item.Cantidad,
                    Precio = item.PrecioUnitario.Value
                };

                producto.DVH = DigitoVerificador530BA.CalcularDVH(producto.IdOrden.ToString() + producto.codProducto.ToString() + producto.Cantidad.ToString() + producto.CantidadRecibida.ToString() + producto.Precio.ToString());

                productosOC.Add(producto);
            }

            dalOrden.InsertarProductosOC(idOrden, productosOC);

            // La solicitud se reemplaza por la orden: recién ahora se elimina.
            bllSolicitud.EliminarSolicitud(idSolicitud);

            DigitoVerificador530BA.ActualizarDVVSolicitudCotizacion();
            DigitoVerificador530BA.ActualizarDVVProductoCotizacion();
            DigitoVerificador530BA.ActualizarDVVOrdenCompra();
            DigitoVerificador530BA.ActualizarDVVProductoOC();

            string dniAutor = ServiceSessionManager530BA.getIntancia().usuarioActivo.DNI;
            bit.registrarEvento(dniAutor, string.Format(idioma.Translate("BLLOrdenCompra.eventoGenerada"), idSolicitud, idOrden, total), Criticidad530BA.Medio, Modulos530BA.Compras);

            return idOrden;
        }

        public List<OrdenCompra530BA> ObtenerTodas()
        {
            DataTable dt = dalOrden.ObtenerOrdenes();
            List<OrdenCompra530BA> ordenes = new List<OrdenCompra530BA>();

            foreach (DataRow row in dt.Rows)
            {
                ordenes.Add(MapearOrden(row));
            }

            return ordenes;
        }

        public OrdenCompra530BA ObtenerPorId(int idOrden)
        {
            DataTable dt = dalOrden.ObtenerOrdenPorId(idOrden);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return MapearOrden(dt.Rows[0]);
        }

        public List<ProductoOC530BA> ObtenerProductosPorOrden(int idOrden)
        {
            DataTable dt = dalOrden.ObtenerProductosPorOrden(idOrden);
            List<ProductoOC530BA> productos = new List<ProductoOC530BA>();

            foreach (DataRow row in dt.Rows)
            {
                productos.Add(MapearProductoOC(row));
            }

            return productos;
        }

        public List<OrdenCompra530BA> ObtenerRecibos()
        {
            DataTable dt = dalOrden.ObtenerRecibos();
            List<OrdenCompra530BA> recibos = new List<OrdenCompra530BA>();

            foreach (DataRow row in dt.Rows)
            {
                recibos.Add(MapearRecibo(row));
            }

            return recibos;
        }

        public OrdenCompra530BA ObtenerReciboPorId(int idOrden)
        {
            DataTable dt = dalOrden.ObtenerReciboPorId(idOrden);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return MapearRecibo(dt.Rows[0]);
        }

        public List<ProductoOC530BA> ObtenerProductosRecibidosPorOrden(int idOrden)
        {
            return ObtenerProductosPorOrden(idOrden)
                .Where(p => p.CantidadRecibida > 0)
                .ToList();
        }

        public bool RegistrarRecepcion(int idOrden, List<ProductoOC530BA> entregas)
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;

            if (idOrden <= 0)
            {
                throw new Exception(idioma.Translate("ExcRecepcionSinSeleccion"));
            }

            OrdenCompra530BA orden = ObtenerPorId(idOrden);

            if (orden == null)
            {
                throw new Exception(idioma.Translate("ExcRecepcionSinSeleccion"));
            }

            if (orden.Recibido)
            {
                throw new Exception(idioma.Translate("ExcRecepcionOrdenYaRecibida"));
            }

            List<ProductoOC530BA> persistidos = ObtenerProductosPorOrden(idOrden);

            if (persistidos == null || persistidos.Count == 0)
            {
                throw new Exception(idioma.Translate("ExcRecepcionCantidadInvalida"));
            }

            List<ProductoOC530BA> cambios = new List<ProductoOC530BA>();

            foreach (ProductoOC530BA persistido in persistidos)
            {
                ProductoOC530BA entrega = entregas == null
                    ? null
                    : entregas.FirstOrDefault(e => e.codProducto == persistido.codProducto);

                int cantidad = entrega == null ? 0 : entrega.CantidadEstaEntrega;

                if (cantidad < 0 || cantidad > persistido.Pendiente)
                {
                    throw new Exception(idioma.Translate("ExcRecepcionCantidadInvalida"));
                }

                if (cantidad == 0)
                {
                    continue;
                }

                cambios.Add(new ProductoOC530BA
                {
                    IdOrden = persistido.IdOrden,
                    codProducto = persistido.codProducto,
                    Cantidad = persistido.Cantidad,
                    CantidadRecibida = persistido.CantidadRecibida + cantidad,
                    CantidadEstaEntrega = cantidad,
                    Precio = persistido.Precio,
                    NombreProducto = persistido.NombreProducto
                });
            }

            if (cambios.Count == 0)
            {
                throw new Exception(idioma.Translate("ExcRecepcionSinCantidades"));
            }

            string dniAutor = ServiceSessionManager530BA.getIntancia().usuarioActivo.DNI;

            try
            {
                foreach (ProductoOC530BA cambio in cambios)
                {
                    long dvh = DigitoVerificador530BA.CalcularDVH(
                        cambio.IdOrden.ToString() + cambio.codProducto.ToString() + cambio.Cantidad.ToString()
                        + cambio.CantidadRecibida.ToString() + cambio.Precio.ToString());

                    int filas = dalOrden.ActualizarCantidadRecibidaProductoOC(
                        cambio.IdOrden, cambio.codProducto, cambio.CantidadRecibida, dvh);

                    if (filas != 1)
                    {
                        throw new Exception(idioma.Translate("ExcRecepcionCantidadInvalida"));
                    }
                }

                // El stock suma solo el delta de esta entrega, nunca el acumulado.
                foreach (ProductoOC530BA cambio in cambios)
                {
                    bllProducto.AumentarStock(cambio.codProducto, cambio.CantidadEstaEntrega);
                }

                List<ProductoOC530BA> actualizados = ObtenerProductosPorOrden(idOrden);
                bool completa = actualizados.All(p => p.CantidadRecibida == p.Cantidad);

                if (completa)
                {
                    long dvhOrden = DigitoVerificador530BA.CalcularDVH( orden.IdProveedor.ToString() + true.ToString() + orden.FechaEmision.ToString());

                    dalOrden.MarcarComoRecibida(idOrden, dvhOrden);
                }

                DigitoVerificador530BA.ActualizarDVVOrdenCompra();
                DigitoVerificador530BA.ActualizarDVVProductoOC();

                bit.registrarEvento(dniAutor, DescribirRecepcion(idOrden, cambios, actualizados, completa), Criticidad530BA.Medio, Modulos530BA.Compras);

                return completa;
            }
            catch (Exception ex)
            {
                bit.registrarEvento(dniAutor, string.Format("Error al registrar la recepción de la orden de compra N° {0}: {1}", idOrden, ex.Message), Criticidad530BA.Alto, Modulos530BA.Compras);
                throw;
            }
        }

        private string DescribirRecepcion(int idOrden, List<ProductoOC530BA> cambios, List<ProductoOC530BA> actualizados, bool completa)
        {
            List<string> partes = new List<string>();

            foreach (ProductoOC530BA cambio in cambios)
            {
                ProductoOC530BA actual = actualizados.FirstOrDefault(p => p.codProducto == cambio.codProducto);
                int pendiente = actual == null ? 0 : actual.Pendiente;

                partes.Add(string.Format("{0} (cód. {1}): +{2}, pendiente {3}", cambio.NombreProducto, cambio.codProducto, cambio.CantidadEstaEntrega, pendiente));
            }

            string estado = completa ? "La orden quedó completa." : "La orden queda pendiente de recepción.";

            return string.Format("Se registró la recepción de la orden de compra N° {0}. {1} Detalle: {2}",
                idOrden, estado, string.Join("; ", partes));
        }

        private OrdenCompra530BA MapearOrden(DataRow row)
        {
            return new OrdenCompra530BA
            {
                Id = Convert.ToInt32(row["Id"]),
                IdProveedor = Convert.ToInt32(row["IdProveedor"]),
                Recibido = Convert.ToBoolean(row["Recibido"]),
                FechaEmision = Convert.ToDateTime(row["FechaEmision"]),
                DVH = row["DVH"] != DBNull.Value ? Convert.ToInt64(row["DVH"]) : 0,
                ProveedorRazonSocial = row["RazonSocial"].ToString(),
                EstadoRecepcion = row["EstadoRecepcion"] != DBNull.Value ? Convert.ToInt32(row["EstadoRecepcion"]) : 0
            };
        }

        private ProductoOC530BA MapearProductoOC(DataRow row)
        {
            return new ProductoOC530BA
            {
                IdOrden = Convert.ToInt32(row["IdOrden"]),
                codProducto = Convert.ToInt32(row["codProducto"]),
                Cantidad = Convert.ToInt32(row["Cantidad"]),
                CantidadRecibida = Convert.ToInt32(row["CantidadRecibida"]),
                Precio = Convert.ToDecimal(row["Precio"]),
                DVH = row["DVH"] != DBNull.Value ? Convert.ToInt64(row["DVH"]) : 0,
                NombreProducto = row["NombreProducto"].ToString()
            };
        }

        private OrdenCompra530BA MapearRecibo(DataRow row)
        {
            return new OrdenCompra530BA
            {
                Id = Convert.ToInt32(row["Id"]),
                IdProveedor = Convert.ToInt32(row["IdProveedor"]),
                Recibido = Convert.ToBoolean(row["Recibido"]),
                FechaEmision = Convert.ToDateTime(row["FechaEmision"]),
                DVH = row["DVH"] != DBNull.Value ? Convert.ToInt64(row["DVH"]) : 0,
                ProveedorRazonSocial = row["RazonSocial"].ToString(),
                ProveedorCuit = row["ProveedorCuit"] != DBNull.Value ? row["ProveedorCuit"].ToString() : null,
                ProveedorDireccion = row["ProveedorDireccion"] != DBNull.Value ? row["ProveedorDireccion"].ToString() : null,
                UnidadesRecibidas = row["UnidadesRecibidas"] != DBNull.Value ? Convert.ToInt32(row["UnidadesRecibidas"]) : 0,
                EstadoRecepcion = row["EstadoRecepcion"] != DBNull.Value ? Convert.ToInt32(row["EstadoRecepcion"]) : 0
            };
        }
    }
}
