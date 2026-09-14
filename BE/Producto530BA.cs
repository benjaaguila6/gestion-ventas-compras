using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Producto530BA
    {
        //ctor para crear un producto nuevo
        public Producto530BA(string nombre, int existencia, decimal precioUnitario)
        {
            this.nombre = nombre;
            this.existencia = existencia;
            this.precioUnitario = precioUnitario;
        }


        //ctor para la bd
        public Producto530BA(int codProducto, string nombre, int existencia, decimal precioUnitario, long dvh)
        {
            this.codProducto = codProducto;
            this.nombre = nombre;
            this.existencia = existencia;
            this.precioUnitario = precioUnitario;
            this.DVH = dvh;
        }


        public Producto530BA()
        {
            
        }
        public int codProducto { get; set; }
        public string nombre { get; set; }
        public int existencia { get; set; }
        public decimal  precioUnitario { get; set; }
        public long DVH { get; set; }
        public bool Activo { get; set; } = true;
    }
}
