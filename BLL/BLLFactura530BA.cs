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
    public class BLLFactura530BA
    {
        DALFactura dalFactura = new DALFactura();
        DALCliente dalCliente = new DALCliente();
        DALProducto530BA dalProducto = new DALProducto530BA();
        BLLCliente530BA bllCliente = new BLLCliente530BA();
        BLLProducto530BA bllProducto = new BLLProducto530BA();
        BitacoraEventosService bit = new BitacoraEventosService();


        
        public int GuardarFactura(string dniCliente, List<ItemFactura530BA> items)
        {
            var idioma = Services_530BA.ServiceSessionManager530BA.getIntancia().Idioma;

            // el cliente debe existir
            bllCliente.ObtenerPorDNI(dniCliente);

            if (items == null || items.Count == 0)
            {
                throw new Exception(idioma.Translate("ExcFacturaSinItems"));
            }

            if (items.Any(i => i.cantidad <= 0))
            {
                throw new Exception(idioma.Translate("ExcCantidadInvalida"));
            }

            Factura530BA factura = new Factura530BA(dniCliente, items.Sum(i => i.Subtotal));

            factura.DVH = Services.DigitoVerificador530BA.CalcularDVH(factura.DNI + factura.Fecha.ToString() + factura.Total.ToString());

            List<KeyValuePair<int, int>> detalleIds = new List<KeyValuePair<int, int>>();
            
            int idFactura = 0;

            try
            {
                idFactura = dalFactura.InsertFactura(factura, items, detalleIds);
            }
            catch (InvalidOperationException)
            {
                // el stock cambió entre la validación y la transacción
                throw new Exception(idioma.Translate("ExcStockInsuficiente"));
            }

            // el DVH del detalle necesita el IdFactura recién generado
            for (int i = 0; i < detalleIds.Count; i++)
            {
                ItemFactura530BA item = items[i];

                long dvhDetalle = Services.DigitoVerificador530BA.CalcularDVH(idFactura.ToString() + item.codProducto + item.cantidad + item.precioUnitario + item.Subtotal);

                dalFactura.ActualizarDVHDetalle(detalleIds[i].Key, dvhDetalle);

                // se descontó stock del producto, recalcular su DVH
                bllProducto.RecalcularDVHProducto(item.codProducto);
            }

            Services.DigitoVerificador530BA.ActualizarDVVFactura();
            Services.DigitoVerificador530BA.ActualizarDVVDetalleFactura();

            string dniAutor = Services_530BA.ServiceSessionManager530BA.getIntancia().usuarioActivo.DNI;
            bit.registrarEvento(dniAutor, "Se creó una factura con N° " + idFactura, Criticidad530BA.Medio, Modulos530BA.Factura);

            return idFactura;
        }
    }
}