using BE;
using BE.Enum;
using BLL;
using Services;
using Services_530BA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Servicios
{
    public partial class PagarOrdenCompra : Form
    {
        BLLOrdenCompra530BA bllOrden = new BLLOrdenCompra530BA();

        private readonly int idSolicitud;
        private readonly List<ProductoCotizacion530BA> lineas;

        public PagarOrdenCompra(int idSolicitud, string proveedor, List<ProductoCotizacion530BA> lineas)
        {
            InitializeComponent();

            this.idSolicitud = idSolicitud;
            this.lineas = lineas ?? new List<ProductoCotizacion530BA>();

            CargarMetodosPago();
            CargarDatos(idSolicitud, proveedor);
            ActualizarIdioma();
        }

        private void CargarMetodosPago()
        {
            cboMetodoPago.DataSource = Enum.GetValues(typeof(MetodoPago530BA));
            cboMetodoPago.SelectedIndex = -1;
        }

        private void CargarDatos(int idSolicitud, string proveedor)
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            lblSolicitud.Text = string.Format(t.Translate("PagarOC.lblSolicitud"), idSolicitud, proveedor);

            dgvDetalle.AutoGenerateColumns = false;
            dgvDetalle.Columns.Clear();
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("PagarOC.colProducto"), DataPropertyName = "NombreProducto" });
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("PagarOC.colCantidad"), DataPropertyName = "Cantidad" });
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("PagarOC.colPrecioUnitario"), DataPropertyName = "PrecioUnitario" });
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("PagarOC.colSubtotal"), DataPropertyName = "Subtotal" });

            dgvDetalle.DataSource = null;
            dgvDetalle.DataSource = lineas;

            decimal total = lineas.Sum(l => l.Cantidad * (l.PrecioUnitario ?? 0m));
            lblImporte.Text = string.Format(t.Translate("PagarOC.lblTotal"), total);
        }

        private void btnPagar_Click(object sender, EventArgs e)
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;
            lblError.Text = "";

            if (cboMetodoPago.SelectedItem == null)
            {
                lblError.Text = t.Translate("PagarOC.msgFaltaMetodoPago");
                return;
            }

            MetodoPago530BA metodo = (MetodoPago530BA)cboMetodoPago.SelectedItem;

            string titular = txtNombreTitular.Text.Trim();

            if (titular == "")
            {
                MessageBox.Show(t.Translate("PagarOC.msgFaltaTitular"));
                txtNombreTitular.Focus();
                return;
            }

            string vencimiento = txtVencimiento.Text.Trim();
            if (!EsVencimientoValido(vencimiento))
            {
                MessageBox.Show(t.Translate("PagarOC.msgVencimientoInvalido"));
                txtVencimiento.Focus();
                return;
            }

            // El PAN completo se usa solo para leer los ultimos 4 digitos y no se persiste.
            if (!EsNumeroTarjetaValido(txtNumeroTarjeta.Text))
            {
                MessageBox.Show(t.Translate("PagarOC.msgTarjetaInvalida"));
                txtNumeroTarjeta.Focus();
                return;
            }

            decimal total = lineas.Sum(l => l.Cantidad * (l.PrecioUnitario ?? 0m));
            Pago530BA pago = new Pago530BA(metodo, ObtenerUltimos4(txtNumeroTarjeta.Text), titular, vencimiento, total);

            btnPagar.Enabled = false;

            try
            {
                int idOrden = bllOrden.GenerarOrdenCompra(idSolicitud, lineas, pago);

                MessageBox.Show(string.Format(t.Translate("PagarOC.msgPagoExitoso"), idOrden), t.Translate("GenerarOC.titleExito"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtNumeroTarjeta.Text = "";
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(t.Translate("PagarOC.msgErrorPago") + t.Translate(ex.Message), t.Translate("GenerarOC.titleError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnPagar.Enabled = true;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            // Se descarta el pago: la solicitud sigue existiendo en la BD.
            txtNumeroTarjeta.Text = "";
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ActualizarIdioma()
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            this.Text = t.Translate("PagarOC.formTitle");
            gbDetalle.Text = t.Translate("PagarOC.gbDetalle");
            lblMetodoPago.Text = t.Translate("PagarOC.lblMetodoPago");
            lblNumeroTarjeta.Text = t.Translate("PagarOC.lblNumeroTarjeta");
            lblNombreTitular.Text = t.Translate("PagarOC.lblNombreTitular");
            lblVencimiento.Text = t.Translate("PagarOC.lblVencimiento");
            btnPagar.Text = t.Translate("PagarOC.btnPagar");
            btnCancelar.Text = t.Translate("PagarOC.btnCancelar");
        }

        public static bool EsVencimientoValido(string vencimiento)
        {
            // formato MM/AA
            if (string.IsNullOrWhiteSpace(vencimiento) || vencimiento.Length != 5 || vencimiento[2] != '/')
                return false;

            string mesStr = vencimiento.Substring(0, 2);
            string anioStr = vencimiento.Substring(3, 2);

            if (!mesStr.All(char.IsDigit) || !anioStr.All(char.IsDigit))
                return false;

            int mes = int.Parse(mesStr);
            int anio = int.Parse(anioStr) + 2000; // Convierte "26" a 2026

            if (mes < 1 || mes > 12)
                return false;

            int mesActual = DateTime.Now.Month;
            int anioActual = DateTime.Now.Year;

            // Si el año es mayor al actual, es válida.
            // Si es el mismo año, el mes de vencimiento debe ser mayor o igual al mes actual.
            return anio > anioActual || (anio == anioActual && mes >= mesActual);
        }

        public static bool EsNumeroTarjetaValido(string numero)
        {
            string digitos = SoloDigitos(numero);
            return digitos.Length >= 13 && digitos.Length <= 19;
        }

        public static string ObtenerUltimos4(string numero)
        {
            string digitos = SoloDigitos(numero);
            return digitos.Length >= 4 ? digitos.Substring(digitos.Length - 4) : string.Empty;
        }

        private static string SoloDigitos(string valor)
        {
            return new string((valor ?? string.Empty).Where(char.IsDigit).ToArray());
        }
    }
}
