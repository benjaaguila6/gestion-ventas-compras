using BE;
using BLL;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Services.Modelos.Idioma;
using Services_530BA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Font = iTextSharp.text.Font;
using Rectangle = iTextSharp.text.Rectangle;

namespace Servicios
{
    public partial class VerRecibos : Form, IIdiomaObserver
    {
        BLLOrdenCompra530BA bllOrden = new BLLOrdenCompra530BA();
        BLLProveedor530BA bllProveedor = new BLLProveedor530BA();
        private List<OrdenCompra530BA> recibosCargados = new List<OrdenCompra530BA>();
        private List<Proveedor530BA> proveedoresCargados = new List<Proveedor530BA>();

        public VerRecibos()
        {
            InitializeComponent();
            ConfigurarGrilla();
            CargarCombos();
            CargarRecibos();

            ServiceSessionManager530BA.getIntancia().Idioma.Suscribir(this);
            actualizarIdioma();
        }

        private void ConfigurarGrilla()
        {
            dgvRecibos.AutoGenerateColumns = false;
            dgvRecibos.Columns.Clear();

            dgvRecibos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                DataPropertyName = "Id",
                ReadOnly = true,
                Width = 90
            });
            dgvRecibos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FechaEmision",
                DataPropertyName = "FechaEmision",
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" },
                Width = 110
            });
            dgvRecibos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProveedorRazonSocial",
                DataPropertyName = "ProveedorRazonSocial",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dgvRecibos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProveedorCuit",
                DataPropertyName = "ProveedorCuit",
                ReadOnly = true,
                Width = 130
            });
            dgvRecibos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Estado",
                ReadOnly = true,
                Width = 150
            });
            dgvRecibos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "UnidadesRecibidas",
                DataPropertyName = "UnidadesRecibidas",
                ReadOnly = true,
                Width = 130
            });
        }

        private void CargarCombos()
        {
            proveedoresCargados = bllProveedor.ObtenerTodos();
            proveedoresCargados.Insert(0, new Proveedor530BA { Id = 0, RazonSocial = "Todos los proveedores" });
            
            cboProveedor.DisplayMember = "RazonSocial";
            cboProveedor.ValueMember = "Id";
            cboProveedor.DataSource = proveedoresCargados;


            cboEstado.Items.Clear();
            cboEstado.Items.Add("Todos");
            cboEstado.Items.Add("Entrega parcial");
            cboEstado.Items.Add("Entrega completa");
            cboEstado.SelectedIndex = 0;
        }

        private void CargarRecibos()
        {
            recibosCargados = bllOrden.ObtenerRecibos();
            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            IEnumerable<OrdenCompra530BA> filtrados = recibosCargados;

            string textoNumero = txtNumeroOrden.Text.Trim();
            if (!string.IsNullOrWhiteSpace(textoNumero))
            {
                if (int.TryParse(textoNumero, out int numero))
                {
                    filtrados = filtrados.Where(o => o.Id == numero);
                }
                else
                {
                    filtrados = Enumerable.Empty<OrdenCompra530BA>();
                }
            }

            if (cboProveedor.SelectedItem is Proveedor530BA proveedor && proveedor.Id > 0)
            {
                filtrados = filtrados.Where(o => o.IdProveedor == proveedor.Id);
            }

            if (chkUsarFecha.Checked)
            {
                DateTime desde = dtpFechaDesde.Value.Date;
                DateTime hasta = dtpFechaHasta.Value.Date;

                filtrados = filtrados.Where(o => o.FechaEmision.Date >= desde && o.FechaEmision.Date <= hasta);
            }

            int estado = cboEstado.SelectedIndex;

            if (estado == 1)
            {
                filtrados = filtrados.Where(o => o.EstadoRecepcion == 1);
            }
            else if (estado == 2)
            {
                filtrados = filtrados.Where(o => o.EstadoRecepcion == 2);
            }

            dgvRecibos.DataSource = null;
            dgvRecibos.DataSource = new BindingList<OrdenCompra530BA>(filtrados.ToList());
        }

        private void chkUsarFecha_CheckedChanged(object sender, EventArgs e)
        {
            dtpFechaDesde.Enabled = chkUsarFecha.Checked;
            dtpFechaHasta.Enabled = chkUsarFecha.Checked;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            string textoNumero = txtNumeroOrden.Text.Trim();

            if (!string.IsNullOrWhiteSpace(textoNumero) && !int.TryParse(textoNumero, out _))
            {
                MessageBox.Show(t.Translate("VerRecibos.msgNumeroInvalido"),
                    t.Translate("VerRecibos.titleAtencion"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (chkUsarFecha.Checked && dtpFechaDesde.Value.Date > dtpFechaHasta.Value.Date)
            {
                MessageBox.Show(t.Translate("VerRecibos.msgFechaInvalida"),
                    t.Translate("VerRecibos.titleAtencion"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CargarRecibos();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtNumeroOrden.Text = string.Empty;
            cboProveedor.SelectedIndex = 0;
            chkUsarFecha.Checked = false;
            dtpFechaDesde.Value = DateTime.Today;
            dtpFechaHasta.Value = DateTime.Today;
            cboEstado.SelectedIndex = 0;

            CargarRecibos();
        }

        private void dgvRecibos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || dgvRecibos.Columns[e.ColumnIndex].Name != "Estado")
            {
                return;
            }

            if (dgvRecibos.Rows[e.RowIndex].DataBoundItem is OrdenCompra530BA orden)
            {
                var t = ServiceSessionManager530BA.getIntancia().Idioma;
                e.Value = t.Translate(orden.EstadoRecepcion == 2 ? "VerRecibos.estadoCompleta" : "VerRecibos.estadoParcial");
                e.FormattingApplied = true;
            }
        }

        private void dgvRecibos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void GenerarRemitoPDF(int idOrden)
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            OrdenCompra530BA orden = bllOrden.ObtenerReciboPorId(idOrden);

            if (orden == null)
            {
                MessageBox.Show(t.Translate("VerRecibos.msgSinSeleccion"),
                    t.Translate("VerRecibos.titleAtencion"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<ProductoOC530BA> productos = bllOrden.ObtenerProductosRecibidosPorOrden(idOrden);

            if (productos == null || productos.Count == 0)
            {
                MessageBox.Show(t.Translate("VerRecibos.msgSinProductos"),
                    t.Translate("VerRecibos.titleAtencion"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(orden.ProveedorCuit) || string.IsNullOrWhiteSpace(orden.ProveedorDireccion))
            {
                DialogResult continuar = MessageBox.Show(t.Translate("VerRecibos.msgProveedorIncompleto"),
                    t.Translate("VerRecibos.titleAtencion"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (continuar != DialogResult.Yes)
                {
                    return;
                }
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Archivos PDF (*.pdf)|*.pdf";
            saveFileDialog.FileName = $"Remito_{orden.Id:D8}.pdf";

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                GenerarDocumentoRemitoPDF(orden, productos, saveFileDialog.FileName);

                MessageBox.Show(t.Translate("VerRecibos.msgPdfGenerado"), t.Translate("VerRecibos.titleExito"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(t.Translate("VerRecibos.msgError") + " " + ex.Message,
                    t.Translate("VerRecibos.titleError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerarDocumentoRemitoPDF(OrdenCompra530BA orden, List<ProductoOC530BA> productos, string fileName)
        {
            // Tamaño A4 con márgenes compactos para mantener la proporción del talonario
            Document doc = new Document(PageSize.A4, 25, 25, 25, 25);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(fileName, FileMode.Create));
            doc.Open();

            // Fuentes
            Font fontR = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 22, BaseColor.BLACK);
            Font fontRCod = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7, BaseColor.BLACK);
            Font fontTituloRemito = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 15, BaseColor.BLACK);
            Font fontSubTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7f, BaseColor.BLACK);
            Font fontNumero = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
            Font fontNegrita = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8.5f, BaseColor.BLACK);
            Font fontNormal = FontFactory.GetFont(FontFactory.HELVETICA, 8.5f, BaseColor.BLACK);
            Font fontEmpresa = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
            Font fontSlogan = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.DARK_GRAY);
            Font fontPie = FontFactory.GetFont(FontFactory.HELVETICA, 6.5f, BaseColor.BLACK);
            Font fontPieBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7.5f, BaseColor.BLACK);
            Font fontSello = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.BLACK);

            // ==========================================
            // 1. ENCABEZADO TIPO TALONARIO CON LETRA "R"
            // ==========================================
            PdfPTable tablaCabecera = new PdfPTable(3);
            tablaCabecera.WidthPercentage = 100;
            tablaCabecera.SetWidths(new float[] { 45f, 10f, 45f });

            // --- Columna Izquierda: Emisor ---
            PdfPCell celdaIzq = new PdfPCell();
            celdaIzq.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
            celdaIzq.Padding = 6f;
            celdaIzq.FixedHeight = 115f;

            try
            {
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance("logoaqua.jpeg");
                logo.ScaleToFit(85, 38);
                celdaIzq.AddElement(logo);
            }
            catch { }

            celdaIzq.AddElement(new Paragraph("Agua - Soda", fontEmpresa));
            celdaIzq.AddElement(new Paragraph("Pureza Natural", fontSlogan));
            celdaIzq.AddElement(new Paragraph("Venta y Distribución de Agua y Soda envasada", fontSubTitulo));
            celdaIzq.AddElement(new Paragraph("Domicilio Comercial - Teléfono\nE-mail: contacto@aguapureza.com", fontSubTitulo));

            Paragraph pCondIva = new Paragraph("IVA RESPONSABLE INSCRIPTO", fontSubTitulo);
            pCondIva.Alignment = Element.ALIGN_CENTER;
            pCondIva.SpacingBefore = 4f;
            celdaIzq.AddElement(pCondIva);

            // --- Columna Centro: Cuadro "R" y divisor vertical ---
            PdfPCell celdaCentro = new PdfPCell();
            celdaCentro.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
            celdaCentro.HorizontalAlignment = Element.ALIGN_CENTER;
            celdaCentro.Padding = 0f;
            celdaCentro.FixedHeight = 115f;

            PdfPTable boxR = new PdfPTable(1);
            boxR.WidthPercentage = 80;
            PdfPCell cLetra = new PdfPCell(new Phrase("R", fontR))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER,
                BorderWidth = 1.2f,
                PaddingBottom = 1f
            };
            PdfPCell cCod = new PdfPCell(new Phrase("Cod. 91", fontRCod))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER,
                BorderWidth = 1.2f,
                PaddingBottom = 2f
            };
            boxR.AddCell(cLetra);
            boxR.AddCell(cCod);
            celdaCentro.AddElement(boxR);

            // Línea divisoria central hacia abajo
            PdfPTable tablaDivisoria = new PdfPTable(2);
            tablaDivisoria.WidthPercentage = 100;
            PdfPCell cDivIzq = new PdfPCell { Border = Rectangle.RIGHT_BORDER, BorderWidth = 0.8f, FixedHeight = 65f };
            PdfPCell cDivDer = new PdfPCell { Border = Rectangle.NO_BORDER, FixedHeight = 65f };
            tablaDivisoria.AddCell(cDivIzq);
            tablaDivisoria.AddCell(cDivDer);
            celdaCentro.AddElement(tablaDivisoria);

            // --- Columna Derecha: Número, Fecha y Datos Impositivos ---
            PdfPCell celdaDer = new PdfPCell();
            celdaDer.Border = Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
            celdaDer.Padding = 6f;
            celdaDer.FixedHeight = 115f;

            Paragraph pRemito = new Paragraph("REMITO", fontTituloRemito);
            pRemito.Alignment = Element.ALIGN_CENTER;
            celdaDer.AddElement(pRemito);

            Paragraph pNoValido = new Paragraph("DOCUMENTO NO VÁLIDO\nCOMO FACTURA", fontSubTitulo);
            pNoValido.Alignment = Element.ALIGN_CENTER;
            celdaDer.AddElement(pNoValido);

            Paragraph pNum = new Paragraph($"N° 0001 - {orden.Id:D8}", fontNumero);
            pNum.Alignment = Element.ALIGN_CENTER;
            pNum.SpacingBefore = 3f;
            celdaDer.AddElement(pNum);

            Paragraph pFecha = new Paragraph($"Fecha:   {DateTime.Now:dd / MM / yyyy}", fontNegrita);
            pFecha.SpacingBefore = 2f;
            celdaDer.AddElement(pFecha);

            Paragraph pAfip = new Paragraph("CUIT: 30-71234567-8\nIng. Brutos C.M.: 901-123456-1\nInicio Actividades: 01/01/2020", fontSubTitulo);
            pAfip.SpacingBefore = 2f;
            celdaDer.AddElement(pAfip);

            tablaCabecera.AddCell(celdaIzq);
            tablaCabecera.AddCell(celdaCentro);
            tablaCabecera.AddCell(celdaDer);
            doc.Add(tablaCabecera);

            // ==========================================
            // 2. DATOS DEL CLIENTE / PROVEEDOR (SR./ES)
            // ==========================================
            PdfPTable tablaCliente = new PdfPTable(2);
            tablaCliente.WidthPercentage = 100;
            tablaCliente.SetWidths(new float[] { 68f, 32f });
            tablaCliente.SpacingBefore = 2f;

            // Fila 1: Razón Social
            PdfPCell cCliente = new PdfPCell();
            cCliente.Colspan = 2;
            cCliente.Border = Rectangle.BOX;
            cCliente.Padding = 4f;
            cCliente.AddElement(new Phrase($"Sr./es:  {orden.ProveedorRazonSocial}", fontNormal));
            tablaCliente.AddCell(cCliente);

            // Fila 2: Domicilio y Localidad
            PdfPCell cDom = new PdfPCell(new Phrase($"Domicilio:  {orden.ProveedorDireccion}", fontNormal))
            {
                Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER,
                Padding = 4f
            };
            //PdfPCell cLoc = new PdfPCell(new Phrase("Localidad:  __________________", fontNormal))
            //{
            //    Border = Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER,
            //    Padding = 4f
            //};
            tablaCliente.AddCell(cDom);
            //tablaCliente.AddCell(cLoc);

            // Fila 3: Condición IVA y CUIT
            //PdfPCell cIva = new PdfPCell(new Phrase("IVA:   Responsable Inscripto  [  ]   Consumidor Final  [  ]", fontNormal))
            //{
            //    Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER,
            //    Padding = 4f
            //};
            PdfPCell cCuit = new PdfPCell(new Phrase($"CUIT:  {orden.ProveedorCuit}", fontNormal))
            {
                Border = Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER,
                Padding = 4f
            };
            //tablaCliente.AddCell(cIva);
            tablaCliente.AddCell(cCuit);

            // Fila 4: Condiciones de venta y Factura N°
            PdfPCell cCond = new PdfPCell(new Phrase("Condiciones de venta:    Contado  [ X ]    Cta. Cte.  [  ]", fontNormal))
            {
                Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER,
                Padding = 4f
            };
            PdfPCell cFact = new PdfPCell(new Phrase("Factura N°:  ________________", fontNormal))
            {
                Border = Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER,
                Padding = 4f
            };
            tablaCliente.AddCell(cCond);
            tablaCliente.AddCell(cFact);

            doc.Add(tablaCliente);

            // ==========================================
            // 3. TABLA DE PRODUCTOS (GRILLA COMPLETA)
            // ==========================================
            PdfPTable tablaProductos = new PdfPTable(2);
            tablaProductos.WidthPercentage = 100;
            tablaProductos.SetWidths(new float[] { 15f, 85f });
            tablaProductos.SpacingBefore = 2f;

            // Encabezados de la grilla
            PdfPCell cTHCant = new PdfPCell(new Phrase("CANTIDAD", fontNegrita))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Border = Rectangle.BOX,
                FixedHeight = 18f
            };
            PdfPCell cTHDesc = new PdfPCell(new Phrase("DESCRIPCIÓN", fontNegrita))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Border = Rectangle.BOX,
                FixedHeight = 18f
            };
            tablaProductos.AddCell(cTHCant);
            tablaProductos.AddCell(cTHDesc);

            // Renglones con datos reales
            foreach (ProductoOC530BA producto in productos)
            {
                PdfPCell cCant = new PdfPCell(new Phrase(producto.CantidadRecibida.ToString(), fontNormal))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Border = Rectangle.BOX,
                    FixedHeight = 18f
                };
                PdfPCell cDesc = new PdfPCell(new Phrase($"  {producto.NombreProducto ?? string.Empty}", fontNormal))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Border = Rectangle.BOX,
                    FixedHeight = 18f
                };
                tablaProductos.AddCell(cCant);
                tablaProductos.AddCell(cDesc);
            }

            // Renglones vacíos para completar el formato de talonario rayado (16 filas en total)
            int filasObjetivo = 16;
            int filasRestantes = Math.Max(0, filasObjetivo - productos.Count);

            for (int i = 0; i < filasRestantes; i++)
            {
                PdfPCell cVaciaCant = new PdfPCell(new Phrase(" ", fontNormal))
                {
                    Border = Rectangle.BOX,
                    FixedHeight = 18f
                };
                PdfPCell cVaciaDesc = new PdfPCell(new Phrase(" ", fontNormal))
                {
                    Border = Rectangle.BOX,
                    FixedHeight = 18f
                };
                tablaProductos.AddCell(cVaciaCant);
                tablaProductos.AddCell(cVaciaDesc);
            }

            doc.Add(tablaProductos);

            // ==========================================
            // 4. TRANSPORTE Y RECIBÍ CONFORME / SELLO
            // ==========================================
            PdfPTable tablaPie = new PdfPTable(2);
            tablaPie.WidthPercentage = 100;
            tablaPie.SetWidths(new float[] { 50f, 50f });
            tablaPie.SpacingBefore = 2f;

            // Columna Izquierda: Datos del Transporte
            PdfPCell celdaTransp = new PdfPCell();
            celdaTransp.Border = Rectangle.BOX;
            celdaTransp.Padding = 6f;
            celdaTransp.FixedHeight = 75f;
            //celdaTransp.AddElement(new Paragraph("Transporte: .....................................................................", fontNormal));
            //celdaTransp.AddElement(new Paragraph("Domicilio:   .....................................................................", fontNormal));
            //celdaTransp.AddElement(new Paragraph("CUIT:           .....................................................................", fontNormal));
            tablaPie.AddCell(celdaTransp);

            // Columna Derecha: Sello y Recibí Conforme
            PdfPCell celdaConforme = new PdfPCell();
            celdaConforme.Border = Rectangle.BOX;
            celdaConforme.Padding = 5f;
            celdaConforme.FixedHeight = 75f;
            celdaConforme.HorizontalAlignment = Element.ALIGN_CENTER;

            // Sello de recepción
            string textoSello = orden.EstadoRecepcion == 2 ? "[ ENTREGA COMPLETA ]" : "[ ENTREGA PARCIAL ]";
            Paragraph pSello = new Paragraph(textoSello, fontSello);
            pSello.Alignment = Element.ALIGN_CENTER;
            celdaConforme.AddElement(pSello);

            Paragraph pEspacio = new Paragraph("\n", fontNormal);
            celdaConforme.AddElement(pEspacio);

            Paragraph pFirmaLinea = new Paragraph("...........................................................................", fontNormal);
            pFirmaLinea.Alignment = Element.ALIGN_CENTER;
            celdaConforme.AddElement(pFirmaLinea);

            Paragraph pRecibi = new Paragraph("RECIBÍ CONFORME", fontNegrita);
            pRecibi.Alignment = Element.ALIGN_CENTER;
            celdaConforme.AddElement(pRecibi);

            tablaPie.AddCell(celdaConforme);
            doc.Add(tablaPie);

            // ==========================================
            // 5. PIE LEGAL Y DATOS DE IMPRENTA (CAI)
            // ==========================================
            PdfPTable tablaLegal = new PdfPTable(3);
            tablaLegal.WidthPercentage = 100;
            tablaLegal.SetWidths(new float[] { 45f, 25f, 30f });
            tablaLegal.SpacingBefore = 2f;

            PdfPCell cLegal1 = new PdfPCell();
            cLegal1.Border = Rectangle.BOX;
            cLegal1.Padding = 3f;
            cLegal1.AddElement(new Paragraph("Imprimió: AQUA Pureza Natural\nNumerado del N° 0001-00000001 al 0001-00010000", fontPie));

            PdfPCell cLegal2 = new PdfPCell();
            cLegal2.Border = Rectangle.BOX;
            cLegal2.Padding = 3f;
            cLegal2.HorizontalAlignment = Element.ALIGN_CENTER;
            Paragraph pDefensa = new Paragraph("147 Teléfono gratuito CABA\nÁrea de defensa y protección al consumidor", fontPie);
            pDefensa.Alignment = Element.ALIGN_CENTER;
            cLegal2.AddElement(pDefensa);

            PdfPCell cLegal3 = new PdfPCell();
            cLegal3.Border = Rectangle.BOX;
            cLegal3.Padding = 3f;
            cLegal3.HorizontalAlignment = Element.ALIGN_RIGHT;
            Paragraph pCai = new Paragraph("ORIGINAL blanco - DUPLICADO color\nCAI: 45000000000003\nF. VTO: ", fontPieBold);
            pCai.Alignment = Element.ALIGN_RIGHT;
            cLegal3.AddElement(pCai);

            tablaLegal.AddCell(cLegal1);
            tablaLegal.AddCell(cLegal2);
            tablaLegal.AddCell(cLegal3);
            doc.Add(tablaLegal);

            doc.Close();
            writer.Close();
        }


        public void actualizarIdioma()
        {
            // 1. Evitar ejecución si el formulario ya fue cerrado o liberado
            if (this.IsDisposed || this.Disposing) return;

            var t = ServiceSessionManager530BA.getIntancia().Idioma;
            if (t == null) return;

            // Traducción de textos básicos
            this.Text = t.Translate("VerRecibos.formTitle");
            lblInstruccion.Text = t.Translate("VerRecibos.lblInstruccion");
            gbFiltros.Text = t.Translate("VerRecibos.gbFiltros");
            lblNumeroOrden.Text = t.Translate("VerRecibos.lblNumeroOrden");
            lblProveedor.Text = t.Translate("VerRecibos.lblProveedor");
            chkUsarFecha.Text = t.Translate("VerRecibos.chkUsarFecha");
            lblEstado.Text = t.Translate("VerRecibos.lblEstado");
            lblFechaDesde.Text = t.Translate("VerRecibos.lblFechaDesde");
            lblFechaHasta.Text = t.Translate("VerRecibos.lblFechaHasta");
            btnBuscar.Text = t.Translate("VerRecibos.btnBuscar");
            btnLimpiar.Text = t.Translate("VerRecibos.btnLimpiar");
            gbRecibos.Text = t.Translate("VerRecibos.gbRecibos");
            btnImprimirRemito.Text = t.Translate("VerRecibos.btnImprimir");

            // 2. Traducción segura de columnas de la grilla
            ActualizarHeaderColumna(dgvRecibos, "Id", t.Translate("VerRecibos.colNumeroOrden"));
            ActualizarHeaderColumna(dgvRecibos, "FechaEmision", t.Translate("VerRecibos.colFecha"));
            ActualizarHeaderColumna(dgvRecibos, "ProveedorRazonSocial", t.Translate("VerRecibos.colProveedor"));
            ActualizarHeaderColumna(dgvRecibos, "ProveedorCuit", t.Translate("VerRecibos.colCuit"));
            ActualizarHeaderColumna(dgvRecibos, "Estado", t.Translate("VerRecibos.colEstadoEntrega"));
            ActualizarHeaderColumna(dgvRecibos, "UnidadesRecibidas", t.Translate("VerRecibos.colUnidadesRecibidas"));

            // 3. Traducción segura de cboEstado
            if (cboEstado.DataSource == null && cboEstado.Items.Count >= 3)
            {
                int indiceEstado = cboEstado.SelectedIndex < 0 ? 0 : cboEstado.SelectedIndex;
                cboEstado.Items[0] = t.Translate("VerRecibos.estadoTodos");
                cboEstado.Items[1] = t.Translate("VerRecibos.estadoParcial");
                cboEstado.Items[2] = t.Translate("VerRecibos.estadoCompleta");
                cboEstado.SelectedIndex = indiceEstado;
            }

            // 4. Traducción segura de proveedores (verificando null)
            if (proveedoresCargados != null && proveedoresCargados.Count > 0)
            {
                int idSeleccionado = cboProveedor.SelectedItem is Proveedor530BA seleccionado ? seleccionado.Id : 0;
                proveedoresCargados[0].RazonSocial = t.Translate("VerRecibos.proveedorTodos");
                cboProveedor.DataSource = null;
                cboProveedor.DataSource = proveedoresCargados;
                cboProveedor.SelectedValue = idSeleccionado;
            }

            dgvRecibos.Refresh();
        }

        private void ActualizarHeaderColumna(DataGridView dgv, string nombreColumna, string texto)
        {
            if (dgv.Columns.Contains(nombreColumna) && dgv.Columns[nombreColumna] != null)
            {
                dgv.Columns[nombreColumna].HeaderText = texto;
            }
        }

        private void btnImprimirRemito_Click(object sender, EventArgs e)
        {
            if (dgvRecibos.CurrentRow.DataBoundItem is OrdenCompra530BA orden)
            {
                GenerarRemitoPDF(orden.Id);
            }
        }
    }
}
