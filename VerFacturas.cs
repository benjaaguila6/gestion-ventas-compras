using BE;
using BE.Enum;
using BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Font = iTextSharp.text.Font;
using Rectangle = iTextSharp.text.Rectangle;
using Chunk = iTextSharp.text.Chunk;

namespace Servicios
{
    public partial class VerFacturas : Form
    {
        BLLFactura530BA bllFactura = new BLLFactura530BA();
        List<Factura530BA> listaFacturas = new List<Factura530BA>();
        public VerFacturas()
        {
            InitializeComponent();
            var t = Services_530BA.ServiceSessionManager530BA.getIntancia().Idioma;
            this.Text = t.Translate("VerFacturas.formTitle");
            btnCobrarFactura.Text = t.Translate("VerFacturas.btnCobrarFactura");
            button1.Text = t.Translate("VerFacturas.btnImprimirFactura");
            cargarGrilla();
        }

        private void cargarGrilla()
        {
            listaFacturas = bllFactura.ObtenerTodas();
            dgvFacturas.DataSource = null;
            dgvFacturas.DataSource = listaFacturas;
        }

        private void btnCobrarFactura_Click(object sender, EventArgs e)
        {
            var t = Services_530BA.ServiceSessionManager530BA.getIntancia().Idioma;

            // 1. Validar selección
            if (dgvFacturas.SelectedRows.Count == 0)
            {
                MessageBox.Show(t.Translate("VerFacturas.msgSeleccionarFactura"));
                return;
            }

            Factura530BA facturaSeleccionada = (Factura530BA)dgvFacturas.CurrentRow.DataBoundItem;

            // 2. Validar que la factura esté pendiente y no haya sido pagada ya
            if (facturaSeleccionada.Estado == EstadoFactura530BA.Pagada)
            {
                // Agrega esta key a tu JSON ("Esta factura ya se encuentra pagada.")
                MessageBox.Show(t.Translate("VerFacturas.msgFacturaYaCobrada"));
                return;
            }

            try
            {
                int idFactura = facturaSeleccionada.Id;
                string dni = facturaSeleccionada.DNI;

                // 3. Buscar el nombre del cliente utilizando su BLL
                BLLCliente530BA bllCliente = new BLLCliente530BA();
                Cliente530BA cliente = bllCliente.ObtenerPorDNI(dni);
                string nombre = cliente.NombreCompleto;

                List<ItemFactura530BA> lineas = bllFactura.ObtenerDetalles(idFactura);

                if (lineas == null || lineas.Count == 0)
                {
                    MessageBox.Show(t.Translate("ExcFacturaSinItems"));
                    return;
                }

                // 5. Cargar datos en el formulario de cobro
                using (CobrarVenta form = new CobrarVenta())
                {
                    form.CargarDatosDeFactura(idFactura, dni, nombre, lineas);
                    form.ShowDialog();
                }

                // 6. Refrescar la grilla para actualizar el estado visual de la factura si fue pagada
                cargarGrilla();
            }
            catch (Exception ex)
            {
                MessageBox.Show(t.Translate("GestionFactura.msgErrorOperacion") + t.Translate(ex.Message));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var t = Services_530BA.ServiceSessionManager530BA.getIntancia().Idioma;

            if (dgvFacturas.SelectedRows.Count == 0)
            {
                MessageBox.Show(t.Translate("VerFacturas.msgSeleccionarFactura"));
                return;
            }

            Factura530BA facturaSeleccionada = (Factura530BA)dgvFacturas.SelectedRows[0].DataBoundItem;

            try
            {
                // 1. Obtener datos del cliente
                BLLCliente530BA bllCliente = new BLLCliente530BA();
                Cliente530BA cliente = bllCliente.ObtenerPorDNI(facturaSeleccionada.DNI);

                // 2. Obtener el detalle de la factura
                List<ItemFactura530BA> lineas = bllFactura.ObtenerDetalles(facturaSeleccionada.Id);

                if (lineas == null || lineas.Count == 0)
                {
                    MessageBox.Show(t.Translate("ExcFacturaSinItems"));
                    return;
                }

                // 3. NUEVO: Obtener los datos del pago (si la factura está pagada)
                Pago530BA pago = null;
                if (facturaSeleccionada.Estado == EstadoFactura530BA.Pagada)
                {
                    pago = bllFactura.ObtenerPagoPorFactura(facturaSeleccionada.Id);
                }

                // 4. Dialogo para guardar el archivo
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Archivos PDF (*.pdf)|*.pdf";
                saveFileDialog.FileName = $"Factura_{facturaSeleccionada.Id:D8}.pdf";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // 5. Llamar al generador enviando también el objeto pago
                    GenerarDocumentoPDF(facturaSeleccionada, cliente, lineas, pago, saveFileDialog.FileName);

                    MessageBox.Show(t.Translate("VerFacturas.msgPdfGenerado"), t.Translate("VerFacturas.titleExito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(t.Translate("GestionFactura.msgErrorOperacion") + "\n" + t.Translate(ex.Message));
            }
        }

        private void GenerarDocumentoPDF(Factura530BA facturaSeleccionada, Cliente530BA cliente, List<ItemFactura530BA> lineas, Pago530BA pago, string fileName)
        {
            Document doc = new Document(PageSize.A4, 40, 40, 40, 40);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(fileName, FileMode.Create));
            doc.Open();

            BaseColor colorPrimario = new BaseColor(30, 100, 180);
            BaseColor colorFondoCabecera = new BaseColor(180, 235, 125);
            BaseColor colorPagado = new BaseColor(46, 139, 87);

            Font fontTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 22, colorPrimario);
            Font fontSubtitulo = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
            Font fontFacturaCabecera = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, colorPrimario);
            Font fontNegrita = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.BLACK);
            Font fontNormal = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
            Font fontTotal = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, colorPrimario);

            // --- CABECERA ---
            PdfPTable tablaCabecera = new PdfPTable(2);
            tablaCabecera.WidthPercentage = 100;
            tablaCabecera.SetWidths(new float[] { 60f, 40f });

            PdfPCell celdaEmpresa = new PdfPCell();
            celdaEmpresa.Border = Rectangle.NO_BORDER;

            try
            {
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance("logoaqua.jpeg");
                logo.ScaleAbsolute(100, 60);
                celdaEmpresa.AddElement(logo);
            }
            catch { }

            celdaEmpresa.AddElement(new Phrase("Agua - Soda", fontTitulo));
            celdaEmpresa.AddElement(new Phrase("Pureza Natural", fontSubtitulo));
            tablaCabecera.AddCell(celdaEmpresa);

            PdfPCell celdaInfoFactura = new PdfPCell();
            celdaInfoFactura.Border = Rectangle.NO_BORDER;
            celdaInfoFactura.HorizontalAlignment = Element.ALIGN_RIGHT;

            Paragraph pFactura = new Paragraph("FACTURA\n", fontFacturaCabecera);
            pFactura.Alignment = Element.ALIGN_RIGHT;
            Paragraph pNumero = new Paragraph($"N° {facturaSeleccionada.Id:D8}\n", fontNegrita);
            pNumero.Alignment = Element.ALIGN_RIGHT;
            Paragraph pFecha = new Paragraph($"Fecha: {facturaSeleccionada.Fecha:dd/MM/yyyy}   Hora: {facturaSeleccionada.Fecha:HH:mm}", fontNormal);
            pFecha.Alignment = Element.ALIGN_RIGHT;

            celdaInfoFactura.AddElement(pFactura);
            celdaInfoFactura.AddElement(pNumero);
            celdaInfoFactura.AddElement(pFecha);
            tablaCabecera.AddCell(celdaInfoFactura);

            doc.Add(tablaCabecera);
            doc.Add(new Chunk(new LineSeparator(1f, 100f, BaseColor.GRAY, Element.ALIGN_CENTER, -1)));
            doc.Add(new Paragraph("\n"));

            // --- CLIENTE ---
            PdfPTable tablaCliente = new PdfPTable(2);
            tablaCliente.WidthPercentage = 100;

            PdfPCell celdaNomCliente = new PdfPCell(new Phrase($"Cliente: {cliente.NombreCompleto}", fontNegrita));
            celdaNomCliente.Border = Rectangle.NO_BORDER;

            PdfPCell celdaDniCliente = new PdfPCell(new Phrase($"DNI: {cliente.DNI}", fontNormal));
            celdaDniCliente.Border = Rectangle.NO_BORDER;
            celdaDniCliente.HorizontalAlignment = Element.ALIGN_RIGHT;

            tablaCliente.AddCell(celdaNomCliente);
            tablaCliente.AddCell(celdaDniCliente);
            doc.Add(tablaCliente);
            doc.Add(new Paragraph("\n"));

            // --- PRODUCTOS ---
            PdfPTable tablaItems = new PdfPTable(4);
            tablaItems.WidthPercentage = 100;
            tablaItems.SetWidths(new float[] { 45f, 15f, 20f, 20f });

            string[] cabeceras = { "Producto", "Cantidad", "Precio unit.", "Subtotal" };
            foreach (string cabecera in cabeceras)
            {
                PdfPCell celda = new PdfPCell(new Phrase(cabecera, fontNegrita));
                celda.BackgroundColor = colorFondoCabecera;
                celda.Border = Rectangle.NO_BORDER;
                celda.PaddingBottom = 5f;
                if (cabecera != "Producto") celda.HorizontalAlignment = Element.ALIGN_RIGHT;
                tablaItems.AddCell(celda);
            }

            foreach (var item in lineas)
            {
                PdfPCell cProd = new PdfPCell(new Phrase(item.nombre, fontNormal)) { Border = Rectangle.NO_BORDER };
                PdfPCell cCant = new PdfPCell(new Phrase(item.cantidad.ToString(), fontNormal)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
                PdfPCell cPrecio = new PdfPCell(new Phrase($"$ {item.precioUnitario:N2}", fontNormal)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
                PdfPCell cSub = new PdfPCell(new Phrase($"$ {item.Subtotal:N2}", fontNormal)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };

                tablaItems.AddCell(cProd);
                tablaItems.AddCell(cCant);
                tablaItems.AddCell(cPrecio);
                tablaItems.AddCell(cSub);
            }
            doc.Add(tablaItems);
            doc.Add(new Chunk(new LineSeparator(1f, 100f, BaseColor.GRAY, Element.ALIGN_CENTER, -1)));

            // --- TOTAL ---
            Paragraph pTotal = new Paragraph($"TOTAL $ {facturaSeleccionada.Total:N2}", fontTotal);
            pTotal.Alignment = Element.ALIGN_RIGHT;
            pTotal.SpacingBefore = 10f;
            doc.Add(pTotal);

            doc.Add(new Paragraph("\n"));

            // --- PIE DE PÁGINA (Pago) ---
            PdfPTable tablaPie = new PdfPTable(2);
            tablaPie.WidthPercentage = 100;

            PdfPCell celdaPago = new PdfPCell();
            celdaPago.Border = Rectangle.NO_BORDER;

            // Validamos si hay un pago registrado asociado a esta factura
            if (pago != null)
            {
                celdaPago.AddElement(new Phrase($"Forma de pago: {pago.Metodo.ToString()}", fontNormal));
                celdaPago.AddElement(new Phrase($"Fecha de pago: {pago.Fecha:dd/MM/yyyy HH:mm}", fontNormal));
            }
            else
            {
                celdaPago.AddElement(new Phrase("Forma de pago: Pendiente", fontNormal));
            }

            tablaPie.AddCell(celdaPago);

            // Sello PAGADA
            PdfPCell celdaSelloContenedor = new PdfPCell();
            celdaSelloContenedor.Border = Rectangle.NO_BORDER;
            celdaSelloContenedor.HorizontalAlignment = Element.ALIGN_RIGHT;

            if (facturaSeleccionada.Estado == EstadoFactura530BA.Pagada)
            {
                PdfPTable tablaSello = new PdfPTable(1);
                tablaSello.WidthPercentage = 40;
                tablaSello.HorizontalAlignment = Element.ALIGN_RIGHT;

                PdfPCell celdaSello = new PdfPCell(new Phrase("PAGADA", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, colorPagado)));
                celdaSello.BorderColor = colorPagado;
                celdaSello.BorderWidth = 2f;
                celdaSello.Padding = 5f;
                celdaSello.HorizontalAlignment = Element.ALIGN_CENTER;

                tablaSello.AddCell(celdaSello);
                celdaSelloContenedor.AddElement(tablaSello);
            }
            tablaPie.AddCell(celdaSelloContenedor);

            doc.Add(tablaPie);

            Paragraph pGracias = new Paragraph("\n\n\n¡Gracias por tu compra!", FontFactory.GetFont(FontFactory.HELVETICA, 10, colorPrimario));
            pGracias.Alignment = Element.ALIGN_CENTER;
            doc.Add(pGracias);

            doc.Close();
            writer.Close();
        }
    }
}
