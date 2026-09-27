using System.ComponentModel;

namespace BE.Enum
{
    // Método con el que se abona una factura.
    // Los valores se persisten en Pago.MetodoPago (TINYINT).
    public enum MetodoPago530BA
    {
        [Description("Tarjeta de Credito")]
        Credito = 1,

        [Description("Tarjeta de Debito")]
        Debito = 2
    }
}
