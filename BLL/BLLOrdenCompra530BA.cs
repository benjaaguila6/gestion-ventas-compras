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

                producto.DVH = DigitoVerificador530BA.CalcularDVH(producto.IdOrden.ToString() + producto.codProducto.ToString() + producto.Cantidad.ToString() + producto.Precio.ToString());

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
    }
}
