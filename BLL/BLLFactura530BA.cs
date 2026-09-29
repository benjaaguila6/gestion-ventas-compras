using BE;
using BE.Enum;
using DAL;
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
        DALPago dalPago = new DALPago();
        BLLCliente530BA bllCliente = new BLLCliente530BA();
        BLLProducto530BA bllProducto = new BLLProducto530BA();
        BitacoraEventosService bit = new BitacoraEventosService();

        public List<Factura530BA> ObtenerTodas()
        {
            DataTable dt = dalFactura.ObtenerTodas();
            List<Factura530BA> lista = new List<Factura530BA>();

            foreach(DataRow dr in dt.Rows)
            {
                lista.Add(MapearFactura(dr));
            }

            return lista;
        }

        public List<ItemFactura530BA> ObtenerDetalles(int idFactura)
        {
            List<ItemFactura530BA> lista = dalFactura.ObtenerDetalleFactura(idFactura);

            BLLProducto530BA bllProducto = new BLLProducto530BA();

            foreach (ItemFactura530BA item in lista)
            {
                Producto530BA producto = bllProducto.ObtenerPorID(item.codProducto);

                item.nombre = producto.nombre;
                item.precioUnitario = producto.precioUnitario;
            }

            return lista;
        }

        private Factura530BA MapearFactura(DataRow row)
        {
            if (row == null)
            {
                return null;
            }

            int id = Convert.ToInt32(row["ID"]);
            string DNI = row["DNI"].ToString();
            DateTime fecha = Convert.ToDateTime(row["Fecha"]);
            decimal total = Convert.ToDecimal(row["Total"]);
            EstadoFactura530BA estado = (EstadoFactura530BA)Convert.ToInt32(row["Estado"]);

            return new Factura530BA(id, DNI, fecha, total, estado);
        }

        public int GuardarFactura(string dniCliente, List<ItemFactura530BA> items)
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;

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

            factura.DVH = DigitoVerificador530BA.CalcularDVH(factura.DNI + factura.Fecha.ToString() + factura.Total.ToString());

            List<KeyValuePair<int, int>> detalleIds = new List<KeyValuePair<int, int>>();

            int idFactura = dalFactura.InsertFactura(factura, items, detalleIds);

            // el DVH del detalle necesita el IdFactura recién generado
            for (int i = 0; i < detalleIds.Count; i++)
            {
                ItemFactura530BA item = items[i];

                long dvhDetalle = DigitoVerificador530BA.CalcularDVH(idFactura.ToString() + item.codProducto + item.cantidad + item.precioUnitario + item.Subtotal);

                dalFactura.ActualizarDVHDetalle(detalleIds[i].Key, dvhDetalle);
            }

            DigitoVerificador530BA.ActualizarDVVFactura();
            DigitoVerificador530BA.ActualizarDVVDetalleFactura();

            string dniAutor = ServiceSessionManager530BA.getIntancia().usuarioActivo.DNI;
            bit.registrarEvento(dniAutor, "Se emitió la factura N° " + idFactura + " pendiente de cobro", Criticidad530BA.Medio, Modulos530BA.Factura);

            return idFactura;
        }

        public void CobrarFactura(int idFactura, Pago530BA pago)
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;
            ValidarPago(pago, idioma);

            int idPago = 0;

            try
            {
                dalFactura.CambiarEstado(idFactura);

                decimal totalFactura = dalFactura.ObtenerTotalFactura(idFactura);

                if (pago.Monto != totalFactura)
                {
                    throw new Exception(idioma.Translate("CobrarVenta.msgMontoInvalido"));
                }

                List<ItemFactura530BA> lineas = dalFactura.ObtenerDetalleFactura(idFactura);

                foreach (ItemFactura530BA linea in lineas)
                {
                    bllProducto.DescontarStock(linea.codProducto, linea.cantidad);
                }

                // 5. Registrar el pago.
                idPago = dalPago.RegistrarPago(idFactura, pago);
            }
            catch
            {
                throw; // Mantenemos la propagación del error limpia
            }


            long dvhPago = DigitoVerificador530BA.CalcularDVH(idFactura.ToString() + (int)pago.Metodo + pago.Ultimos4 + pago.NombreTitular + pago.Vencimiento + pago.Monto + pago.Fecha);
            dalPago.ActualizarDVHPago(idPago, dvhPago);

            string dniAutor = ServiceSessionManager530BA.getIntancia().usuarioActivo.DNI;
            bit.registrarEvento(dniAutor, "Se cobró la factura N° " + idFactura, Criticidad530BA.Medio, Modulos530BA.Factura);
        }


        // reglas que debe cumplir el pago antes de tocar la base.
        private void ValidarPago(Pago530BA pago, IdiomaManager idioma)
        {
            if (pago == null)
            {
                throw new Exception(idioma.Translate("CobrarVenta.msgFaltaPago"));
            }

            if (string.IsNullOrWhiteSpace(pago.NombreTitular))
            {
                throw new Exception(idioma.Translate("CobrarVenta.msgFaltaTitular"));
            }

            // solo los ultimos 4 dígitos: el PAN completo nunca se persiste
            if (string.IsNullOrWhiteSpace(pago.Ultimos4) || pago.Ultimos4.Trim().Length != 4 || !pago.Ultimos4.Trim().All(char.IsDigit))
            {
                throw new Exception(idioma.Translate("CobrarVenta.msgUltimos4Invalido"));
            }
        }

        public Pago530BA ObtenerPagoPorFactura(int idFactura)
        {
            // Se asume que DALPago tiene un método que devuelve un DataRow buscando por IdFactura
            DataRow dr = dalFactura.ObtenerPagoPorFactura(idFactura);

            // Si la factura no tiene pago asociado (o aún está pendiente en la base, aunque no debería si el estado es pagado)
            if (dr == null)
            {
                return null;
            }

            return MapearPago(dr);
        }

        public Pago530BA MapearPago(DataRow row)
        {
            if (row == null)
            {
                return null;
            }

            Pago530BA pago = new Pago530BA();

            pago.Id = Convert.ToInt32(row["Id"]);
            pago.IdFactura = Convert.ToInt32(row["IdFactura"]);

            // Casteo explícito del entero guardado en BD hacia tu enumerador
            pago.Metodo = (BE.Enum.MetodoPago530BA)Convert.ToInt32(row["MetodoPago"]);

            // Validación de nulos (DBNull) para los campos de tarjeta, ya que si paga en efectivo pueden venir nulos desde SQL
            pago.Ultimos4 = row["Ultimos4"] != DBNull.Value ? row["Ultimos4"].ToString() : string.Empty;
            pago.NombreTitular = row["NombreTitular"] != DBNull.Value ? row["NombreTitular"].ToString() : string.Empty;
            pago.Vencimiento = row["Vencimiento"] != DBNull.Value ? row["Vencimiento"].ToString() : string.Empty;

            pago.Monto = Convert.ToDecimal(row["Monto"]);
            pago.Fecha = Convert.ToDateTime(row["Fecha"]);
            pago.DVH = Convert.ToInt64(row["DVH"]);

            return pago;
        }
    }
}