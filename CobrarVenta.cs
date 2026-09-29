using BE;
using BE.Enum;
using BLL;
using Services_530BA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Servicios
{
    public partial class CobrarVenta : Form
    {
        BLLFactura530BA bllFactura = new BLLFactura530BA();
        BLLCliente530BA bllCliente = new BLLCliente530BA();

        private List<ItemFactura530BA> items = new List<ItemFactura530BA>();
        private string nombreCliente = string.Empty;
        private int idFactura = 0;

        public CobrarVenta()
        {
            InitializeComponent();
            CargarMetodosPago();

            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            this.Text = t.Translate("CobrarVenta.formTitle");
            lblDetalle.Text = t.Translate("CobrarVenta.lblDetalle");
            lblMetodoPago.Text = t.Translate("CobrarVenta.lblMetodoPago");
            lblNumeroTarjeta.Text = t.Translate("CobrarVenta.lblNumeroTarjeta");
            lblNombreTitular.Text = t.Translate("CobrarVenta.lblNombreTitular");
            lblVencimiento.Text = t.Translate("CobrarVenta.lblVencimiento");
            lblCliente.Text = t.Translate("CobrarVenta.lblCliente");
            btnCobrar.Text = t.Translate("CobrarVenta.btnCobrar");
            btnCancelar.Text = t.Translate("CobrarVenta.btnCancelar");
        }


        // Carga los datos de la factura a cobrar. La factura ya fue emitida por
        // CargarFactura en estado pendiente (0); aqui se la cobra.
        public void CargarDatosDeFactura(int idFactura, string dni, string nombre, List<ItemFactura530BA> lineas)
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            this.idFactura = idFactura;
            nombreCliente = nombre;
            items = lineas ?? new List<ItemFactura530BA>();

            lblCliente.Text = string.Format(t.Translate("CobrarVenta.msgDetalleFactura"), idFactura, nombreCliente);

            dgvDetalle.AutoGenerateColumns = false;
            dgvDetalle.Columns.Clear();
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("CobrarVenta.colProducto"), DataPropertyName = "nombre" });
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("CobrarVenta.colCantidad"), DataPropertyName = "cantidad" });
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("CobrarVenta.colPrecioUnitario"), DataPropertyName = "precioUnitario" });
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("CobrarVenta.colSubtotal"), DataPropertyName = "Subtotal" });

            dgvDetalle.DataSource = null;
            dgvDetalle.DataSource = items;

            lblImporte.Text = string.Format(t.Translate("CobrarVenta.msgTotalACobrar"), items.Sum(i => i.Subtotal));
        }


        private void CargarMetodosPago()
        {
            cboMetodoPago.DataSource = Enum.GetValues(typeof(MetodoPago530BA));
            cboMetodoPago.SelectedIndex = -1;
        }


        private void btnCobrar_Click(object sender, EventArgs e)
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;
            lblError.Text = "";

            if (cboMetodoPago.SelectedItem == null)
            {
                lblError.Text = t.Translate("CobrarVenta.lblErrorFaltaMetodoPago");
                return;
            }

            MetodoPago530BA metodo = (MetodoPago530BA)cboMetodoPago.SelectedItem;

            string titular = txtNombreTitular.Text.Trim();
            if (titular == "")
            {
                MessageBox.Show(t.Translate("CobrarVenta.msgFaltaTitular"));
                txtNombreTitular.Focus();
                return;
            }

            string vencimiento = txtVencimiento.Text.Trim();
            if (!EsVencimientoValido(vencimiento))
            {
                MessageBox.Show(t.Translate("CobrarVenta.msgVencimientoInvalido"));
                txtVencimiento.Focus();
                return;
            }

            // El PAN completo se usa solo para leer los ultimos 4 digitos.
            string digitos = new string(txtNumeroTarjeta.Text.Where(char.IsDigit).ToArray());
            if (digitos.Length < 13 || digitos.Length > 19)
            {
                MessageBox.Show(t.Translate("CobrarVenta.msgTarjetaInvalida"));
                txtNumeroTarjeta.Focus();
                return;
            }

            decimal total = items.Sum(i => i.Subtotal);
            Pago530BA pago = new Pago530BA(metodo, digitos.Substring(digitos.Length - 4), titular, vencimiento, total);

            try
            {
                bllFactura.CobrarFactura(idFactura, pago);

                MessageBox.Show(string.Format(t.Translate("CobrarVenta.msgVentaCobrada"), idFactura));

                LimpiarFormulario();
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(t.Translate("CobrarVenta.msgErrorCobro") + t.Translate(ex.Message));
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            // se descarta el cobro: la factura YA existe en estado pendiente (0)
            // y el stock NO fue descontado. Queda pendiente de cobro.
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private static bool EsVencimientoValido(string vencimiento)
        {
            // formato MM/AA
            if (vencimiento.Length != 5 || vencimiento[2] != '/') return false;

            string mes = vencimiento.Substring(0, 2);
            string anio = vencimiento.Substring(3, 2);

            if (!mes.All(char.IsDigit) || !anio.All(char.IsDigit)) return false;

            int m = int.Parse(mes);
            return m >= 1 && m <= 12;
        }


        private void LimpiarFormulario()
        {
            items = new List<ItemFactura530BA>();
            dgvDetalle.DataSource = null;
            txtNumeroTarjeta.Text = "";
            txtNombreTitular.Text = "";
            txtVencimiento.Text = "";
            cboMetodoPago.SelectedIndex = -1;
            lblError.Text = "";
        }

    }
}
