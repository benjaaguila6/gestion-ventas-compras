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

        public void AgregarProducto(string nombre, int existencia, decimal precioUnitario)
        {
            if(dal.ObtenerPorNombre(nombre) != null)
            {
                throw new Exception("Ya existe un producto con ese nombre"); // falta traducir
            }

            Producto530BA producto = new Producto530BA(nombre, existencia, precioUnitario);

            dal.InsertarProducto(producto);
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
    }
}
