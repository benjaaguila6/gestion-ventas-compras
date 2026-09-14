using BE;
using DAL.Negocio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLLProducto530BA
    {
        DALProducto530BA dal = new DALProducto530BA();

        public List<Producto530BA> ObtenerTodos()
        {
            DataTable dt = dal.ObtenerTodos();
            List<Producto530BA> productos = new List<Producto530BA>();
            
            foreach (DataRow dr in dt.Rows)
            {
                productos.Add(MapearProducto(dr));
            }

            return productos;
        }
        public void AgregarProducto(string nombre, int existencia, decimal precioUnitario)
        {
            var idioma = Services_530BA.ServiceSessionManager530BA.getIntancia().Idioma;

            if(dal.ObtenerPorNombre(nombre) != null)
            {
                throw new Exception(idioma.Translate("ExcProductoNombreExistente"));
            }

            Producto530BA producto = new Producto530BA(nombre, existencia, precioUnitario);

            long dvh = CalcularDVHProducto(nombre, existencia, precioUnitario);

            dal.InsertarProducto(producto, dvh);

            Services.DigitoVerificador530BA.ActualizarDVVProducto();
        }

        public void ActualizarProducto(int codProducto, string nombre, decimal precioUnitario)
        {
            Producto530BA producto = new Producto530BA
            {
                codProducto = codProducto,
                nombre = nombre,
                precioUnitario = precioUnitario
            };

            dal.UpdateProducto(producto);
            RecalcularDVHProducto(codProducto);
        }

        public void ActivarProducto(int codProducto)
        {
            dal.ActivarProducto(codProducto);
        }

        public void DesactivarProducto(int codProducto)
        {
            dal.DesactivarProducto(codProducto);
        }

        public void ActualizarExistencia(int codProducto, int nuevaExistencia)
        {
            dal.ActualizarExistencia(codProducto, nuevaExistencia);
            RecalcularDVHProducto(codProducto);
        }


        public Producto530BA MapearProducto(DataRow dr)
        {
            return new Producto530BA
            {
                codProducto = Convert.ToInt32(dr["codProducto"]),
                nombre = Convert.ToString(dr["nombre"]),
                existencia = Convert.ToInt32(dr["existencia"]),
                precioUnitario = Convert.ToDecimal(dr["precioUnitario"]),
                Activo = Convert.ToBoolean(dr["Activo"])
            };
        }

        #region Digito Verificador

        private long CalcularDVHProducto(string nombre, int existencia, decimal precioUnitario)
        {
            string cadena = nombre + existencia + precioUnitario;

            return Services.DigitoVerificador530BA.CalcularDVH(cadena);
        }

        public void RecalcularDVHProducto(int codProducto)
        {
            DataRow row = dal.ObtenerPorCodProducto(codProducto);
            if (row == null) return;

            string nombre = Convert.ToString(row["nombre"]);
            int existencia = Convert.ToInt32(row["existencia"]);
            decimal precioUnitario = Convert.ToDecimal(row["precioUnitario"]);

            long nuevoDVH = CalcularDVHProducto(nombre, existencia, precioUnitario);

            dal.ActualizarDVH(codProducto, nuevoDVH);
            Services.DigitoVerificador530BA.ActualizarDVVProducto();
        }

        #endregion Digito Verificador
    }
}
