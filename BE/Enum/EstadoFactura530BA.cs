namespace BE.Enum
{
    // Estado de una factura. Se persiste en Factura.Estado (TINYINT).
    //
    // La factura se inserta como Pendiente. El paso a Pagada ocurre
    // unicamente cuando el cobro se confirma, y es ese paso el que
    // habilita el descuento de stock. Por eso solo puede ocurrir una vez.
    public enum EstadoFactura530BA
    {
        Pendiente = 0,
        Pagada = 1
    }
}
