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
    public class BLLSolicitudCotizacion530BA
    {
        DALSolicitudCotizacion dal = new DALSolicitudCotizacion();
        BitacoraEventosService bit = new BitacoraEventosService();

        public int GenerarSolicitud(int idProveedor, List<ProductoCotizacion530BA> items)
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;


            if (items == null || items.Count == 0)
            {
                throw new Exception(idioma.Translate("ExcSolicitudSinItems"));
            }

            if (items.Any(i => i.Cantidad <= 0))
            {
                throw new Exception(idioma.Translate("ExcCantidadInvalida"));
            }

            if (items.GroupBy(i => i.codProducto).Any(g => g.Count() > 1))
            {
                throw new Exception(idioma.Translate("ExcProductoDuplicado"));
            }

            DateTime ahora = DateTime.Now;
            DateTime fecha = new DateTime(ahora.Year, ahora.Month, ahora.Day, ahora.Hour, ahora.Minute, ahora.Second);

            SolicitudCotizacion530BA cabecera = new SolicitudCotizacion530BA
            {
                IdProveedor = idProveedor,
                Fecha = fecha
            };

            cabecera.DVH = DigitoVerificador530BA.CalcularDVH(cabecera.IdProveedor.ToString() + cabecera.Fecha.ToString());

            List<KeyValuePair<int, int>> detalleIds = new List<KeyValuePair<int, int>>();

            int idSolicitud = dal.InsertSolicitudCotizacion(cabecera, items, detalleIds);

            for (int i = 0; i < detalleIds.Count; i++)
            {
                ProductoCotizacion530BA item = items[i];

                long dvhDetalle = DigitoVerificador530BA.CalcularDVH(idSolicitud.ToString() + item.codProducto + item.Cantidad);

                dal.ActualizarDVHProductoCotizacion(detalleIds[i].Key, dvhDetalle);
            }

            DigitoVerificador530BA.ActualizarDVVSolicitudCotizacion();
            DigitoVerificador530BA.ActualizarDVVProductoCotizacion();

            string dniAutor = ServiceSessionManager530BA.getIntancia().usuarioActivo.DNI;
            bit.registrarEvento(dniAutor, "Se generó la solicitud de cotización N° " + idSolicitud, Criticidad530BA.Medio, Modulos530BA.Compras);

            return idSolicitud;
        }

        public List<SolicitudCotizacion530BA> ObtenerSolicitudesPendientes()
        {
            DataTable dt = dal.ObtenerSolicitudesPendientes();
            List<SolicitudCotizacion530BA> lista = new List<SolicitudCotizacion530BA>();

            foreach (DataRow row in dt.Rows)
            {
                lista.Add(MapearSolicitud(row));
            }
            return lista;
        }

        /// <summary>
        /// Arma el texto que se muestra en el ComboBox de solicitudes, usando el idioma activo.
        /// </summary>
        public string DescribirSolicitud(SolicitudCotizacion530BA solicitud)
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;

            return string.Format(
                idioma.Translate("Solicitud.formatoDescripcion"),
                solicitud.Id,
                solicitud.ProveedorRazonSocial,
                solicitud.Fecha.ToString("dd/MM/yyyy"));
        }

        public List<ProductoCotizacion530BA> ObtenerProductosPorSolicitud(int idSolicitud)
        {
            DataTable dt = dal.ObtenerProductosPorSolicitud(idSolicitud);
            List<ProductoCotizacion530BA> lista = new List<ProductoCotizacion530BA>();

            foreach (DataRow row in dt.Rows)
            {
                lista.Add(MapearProducto(row));
            }
            return lista;
        }

        public SolicitudCotizacion530BA ObtenerPorId(int idSolicitud)
        {
            DataTable dt = dal.ObtenerSolicitudPorId(idSolicitud);

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            return MapearSolicitud(dt.Rows[0]);
        }

        /// <summary>
        /// Elimina la solicitud porque ya fue convertida en orden de compra.
        /// Debe llamarse después de persistir la orden: si la solicitud ya no
        /// existe, se informa para no duplicar órdenes por un doble clic.
        /// </summary>
        public void EliminarSolicitud(int idSolicitud)
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;

            int filas = dal.EliminarSolicitud(idSolicitud);

            if (filas != 1)
            {
                throw new Exception(idioma.Translate("ExcSolicitudYaProcesada"));
            }
        }

        private SolicitudCotizacion530BA MapearSolicitud(DataRow row)
        {
            SolicitudCotizacion530BA solicitud = new SolicitudCotizacion530BA
            {
                Id = Convert.ToInt32(row["Id"]),
                IdProveedor = Convert.ToInt32(row["IdProveedor"]),
                Fecha = Convert.ToDateTime(row["Fecha"]),
                DVH = row["DVH"] != DBNull.Value ? Convert.ToInt64(row["DVH"]) : 0,
                ProveedorRazonSocial = row["RazonSocial"].ToString()
            };

            solicitud.DescripcionAmigable = DescribirSolicitud(solicitud);

            return solicitud;
        }

        private ProductoCotizacion530BA MapearProducto(DataRow row)
        {
            return new ProductoCotizacion530BA
            {
                Id = Convert.ToInt32(row["Id"]),
                IdSolicitud = Convert.ToInt32(row["IdSolicitud"]),
                codProducto = Convert.ToInt32(row["codProducto"]),
                Cantidad = Convert.ToInt32(row["Cantidad"]),
                DVH = row["DVH"] != DBNull.Value ? Convert.ToInt64(row["DVH"]) : 0,
                NombreProducto = row["NombreProducto"].ToString(),
                PrecioUnitario = row["PrecioUnitario"] != DBNull.Value ? Convert.ToDecimal(row["PrecioUnitario"]) : (decimal?)null
            };
        }
    }
}
