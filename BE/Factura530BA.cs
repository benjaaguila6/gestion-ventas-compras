using System;
using BE.Enum;
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
            Estado = EstadoFactura530BA.Pendiente;
        }

        // ctor para mapear desde la BD
        public Factura530BA(int id, string dni, DateTime fecha, decimal total, EstadoFactura530BA estado)
        {
            Id = id;
            DNI = dni;
            Fecha = fecha;
            Total = total;
            Estado = estado;
        }

        public int Id { get; set; }
        public string DNI { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public long DVH { get; set; }

        // Pendiente = emitida y todavia no cobrada.
        // Pagada    = cobrada; el stock ya fue descontado.
        public EstadoFactura530BA Estado { get; set; }
    }
}