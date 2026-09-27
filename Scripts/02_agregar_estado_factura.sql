-- ============================================================
-- Caso de uso: Cobrar Venta
--
-- Se agrega el estado de la factura:
--   0 = Pendiente (emitida, todavia no cobrada)
--   1 = Pagada    (cobrada; el stock ya fue descontado)
--
-- El descuento de stock NO ocurre al insertar la factura, sino
-- cuando el pago confirma el cambio 0 -> 1. Por eso el estado
-- tiene que estar protegido: el paso de 0 a 1 es lo que hace
-- legitimo el descuento, y solo puede ocurrir una vez.
-- ============================================================

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'dbo.Factura' AND COLUMN_NAME = 'Estado'
)
BEGIN
    ALTER TABLE dbo.Factura
        ADD Estado TINYINT NOT NULL
        CONSTRAINT DF_Factura_Estado DEFAULT 0;

    PRINT 'Columna Factura.Estado agregada.';
END
ELSE
BEGIN
    PRINT 'La columna Factura.Estado ya existe.';
END
GO

-- Las facturas que ya existian se consideran pagadas: fueron creadas
-- por el flujo anterior, que guardaba y descontaba stock en la misma
-- transaccion. Marcarlas como 0 las volveria cobrables de nuevo y
-- permitiria un segundo descuento de stock.
UPDATE dbo.Factura SET Estado = 1;

PRINT 'Facturas preexistentes marcadas como pagadas (Estado = 1).';
GO
