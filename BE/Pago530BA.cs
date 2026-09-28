using System;

namespace BE
{
    public class Pago530BA
    {
        // ctor para crear un pago nuevo
        public Pago530BA(Enum.MetodoPago530BA metodo, string ultimos4, string nombreTitular, string vencimiento, decimal monto)
        {
            Metodo = metodo;
            Ultimos4 = ultimos4;
            NombreTitular = nombreTitular;
            Vencimiento = vencimiento;
            Monto = monto;
            Fecha = DateTime.Now;
        }

        // ctor para mapear desde la BD
        public Pago530BA(int id, int idFactura, Enum.MetodoPago530BA metodo, string ultimos4,
                         string nombreTitular, string vencimiento, decimal monto, DateTime fecha, long dvh)
        {
            Id = id;
            IdFactura = idFactura;
            Metodo = metodo;
            Ultimos4 = ultimos4;
            NombreTitular = nombreTitular;
            Vencimiento = vencimiento;
            Monto = monto;
            Fecha = fecha;
            DVH = dvh;
        }

        public int Id { get; set; }
        public int IdFactura { get; set; }
        public Enum.MetodoPago530BA Metodo { get; set; }
        public string Ultimos4 { get; set; }
        public string NombreTitular { get; set; }
        public string Vencimiento { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public long DVH { get; set; }
    }
}
