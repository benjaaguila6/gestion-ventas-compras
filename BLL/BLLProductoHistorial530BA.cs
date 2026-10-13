using BE;
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
    public class BLLProductoHistorial530BA
    {
        DALProductoHistorial dal = new DALProductoHistorial();

        BitacoraEventosService bit = new BitacoraEventosService();

        public List<ProductoHistorial530BA> ObtenerHistorial(int codProducto)
        {
            DataTable dt = dal.ObtenerHistorialProducto(codProducto);

            List<ProductoHistorial530BA> historial = new List<ProductoHistorial530BA>();

            foreach(DataRow dr in dt.Rows)
            {
                ProductoHistorial530BA productoHistorial = MapearProductoHistorial(dr);
                historial.Add(productoHistorial);
            }

            return historial;
        }

        public void RestaurarVersion(int codProducto, int idVersion)
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;

            List<ProductoHistorial530BA> historial = ObtenerHistorial(codProducto);
            ProductoHistorial530BA versionSeleccionada = historial.FirstOrDefault(h => h.codProducto == codProducto && h.Version == idVersion);

            if(versionSeleccionada == null)
            {
                throw new Exception(idioma.Translate("ExcVersionNoEncontrada"));
            }

            if(versionSeleccionada.VersionActiva)
            {
                throw new Exception(idioma.Translate("ExcVersionYaActiva"));
            }

            string dni = ServiceSessionManager530BA.getIntancia().usuarioActivo.DNI;
            
            dal.RestaurarVersion(versionSeleccionada, dni);

            BLLProducto530BA bllProducto = new BLLProducto530BA();
            bllProducto.RecalcularDVHProducto(codProducto);

            DigitoVerificador530BA.ActualizarDVVProducto();

            bit.registrarEvento(dni, $"Restauración de versión de producto, Se restauró la versión {versionSeleccionada.Version} del producto con código {versionSeleccionada.codProducto}.", BE.Enum.Criticidad530BA.Alto, BE.Enum.Modulos530BA.Producto);
        }

        private ProductoHistorial530BA MapearProductoHistorial(DataRow row)
        {
            return new ProductoHistorial530BA
            {
                IdHistorial = Convert.ToInt32(row["IdHistorial"]),
                codProducto = Convert.ToInt32(row["codProducto"]),
                Version = Convert.ToInt32(row["Version"]),
                FechaHora = Convert.ToDateTime(row["FechaHora"]),
                TipoOperacion = row["TipoOperacion"].ToString(),
                UsuarioId = row["UsuarioDNI"].ToString(),
                VersionActiva = Convert.ToBoolean(row["VersionActiva"]),
                nombre = row["Nombre"].ToString(),
                existencia = Convert.ToInt32(row["Existencia"]),
                precioUnitario = Convert.ToDecimal(row["PrecioUnitario"]),
                Activo = Convert.ToBoolean(row["Activo"]),
                DVH = Convert.ToInt64(row["DVH"])
            };
        }
    }
}
