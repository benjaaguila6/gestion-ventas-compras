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
        BLLProveedor530BA bllProveedor = new BLLProveedor530BA();
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
    }
}
