using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Factura530BA
    {
        // ctor para crear una factura nueva
        public Factura530BA(string dni, decimal total)
        {
            DNI = dni;
            Total = total;
            Fecha = DateTime.Now;
        }

        // ctor para mapear desde la BD
        public Factura530BA(int id, string dni, DateTime fecha, decimal total, long dvh)
        {
            Id = id;
            DNI = dni;
            Fecha = fecha;
            Total = total;
            DVH = dvh;
        }

        public int Id { get; set; }
        public string DNI { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public long DVH { get; set; }
    }
}