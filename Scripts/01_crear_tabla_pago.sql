-- ============================================================
-- Caso de uso: Cobrar Venta
-- Tabla de pagos asociada a una factura.
--
-- Seguridad / PCI-DSS:
--   - NO se guarda el PAN (numero completo de tarjeta).
--   - NO se guarda el CVV/CVC bajo ninguna circunstancia.
--   - Solo ultimos 4 digitos + titular + vencimiento, que es
--     informacion que queda fuera del alcance del PAN.
--
-- La integridad se protege con DVH, igual que el resto de las
-- tablas del sistema (Factura, DetalleFactura, Producto, Cliente).
-- ============================================================

IF OBJECT_ID('dbo.Pago', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Pago
    (
        Id            INT            IDENTITY(1,1) NOT NULL,
        IdFactura     INT            NOT NULL,
        MetodoPago    TINYINT        NOT NULL,   -- 1 = Credito, 2 = Debito
        Ultimos4      CHAR(4)        NOT NULL,
        NombreTitular NVARCHAR(100)   NOT NULL,
        Vencimiento   CHAR(5)        NOT NULL,   -- formato MM/AA
        Monto         DECIMAL(18,2)  NOT NULL,
        Fecha         DATETIME       NOT NULL,
        DVH           BIGINT         NULL,
        CONSTRAINT PK_Pago PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Pago_Factura FOREIGN KEY (IdFactura)
            REFERENCES dbo.Factura (Id)
    );

    PRINT 'Tabla Pago creada.';
END
ELSE
BEGIN
    PRINT 'La tabla Pago ya existe.';
END
GO

-- ============================================================
-- Permiso de acceso al caso de uso (mismo criterio que
-- "Cargar Factura" y "Maestro Producto").
--
-- El DVH queda NULL a proposito: el digito verificador de
-- Patente se calcula en C# como SHA256(Id + Nombre)
-- (DigitoVerificador530BA.CalcularDVHFilaGenerica) y depende
-- del Id autogenerado. No se reimplementa aqui para que el
-- script no pueda divergir del algoritmo real.
--
-- Despues de correr este script hay que ejecutar la
-- reparacion desde el formulario "Reparar Inconsistencias"
-- (RepararPatente), que recalcula el DVH de la fila y el DVV
-- de la tabla completa.
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM dbo.Patente WHERE Nombre = 'Cobrar Venta')
BEGIN
    INSERT INTO dbo.Patente (Nombre, DVH)
    VALUES ('Cobrar Venta', NULL);

    PRINT 'Patente Cobrar Venta creada (DVH pendiente de reparacion).';
END
ELSE
BEGIN
    PRINT 'La patente Cobrar Venta ya existe.';
END
GO
