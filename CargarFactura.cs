using BE;
using BLL;
using iTextSharp.text;
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
    public partial class CargarFactura : Form
    {
        BLLCliente530BA bllCliente = new BLLCliente530BA();
        List<ItemFactura530BA> items = new List<ItemFactura530BA>();
        private string dniClienteSeleccionado = string.Empty;

        public CargarFactura()
        {
            InitializeComponent();

            dgvLineas.AllowUserToAddRows = false;
            dgvLineas.AllowUserToDeleteRows = false;
            dgvLineas.ReadOnly = true;
            dgvLineas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLineas.MultiSelect = false;

            CargarGrilla();

            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            this.Text = t.Translate("CargarFactura.formTitle");
            groupBox1.Text = t.Translate("CargarFactura.groupBoxCliente");
            label2.Text = t.Translate("CargarFactura.labelCliente");
            label1.Text = t.Translate("CargarFactura.labelDni");
            label3.Text = t.Translate("CargarFactura.labelDni");
            groupBox2.Text = t.Translate("CargarFactura.groupBoxDetalle");
            btnBuscarCliente.Text = t.Translate("CargarFactura.btnBuscarCliente");
            btnAgregarProducto.Text = t.Translate("CargarFactura.btnAgregarProducto");
            btnQuitarProducto.Text = t.Translate("CargarFactura.btnQuitarProducto");
            btnFinalizar.Text = t.Translate("CargarFactura.btnFinalizar");
            btnCancelar.Text = t.Translate("CargarFactura.btnCancelar");
        }

        private void CargarGrilla()
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            dgvLineas.AutoGenerateColumns = false;

            dgvLineas.Columns.Clear();
            dgvLineas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("CargarFactura.colProducto"), DataPropertyName = "nombre" });
            dgvLineas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("CargarFactura.colCantidad"), DataPropertyName = "cantidad" });
            dgvLineas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("CargarFactura.colPrecioUnitario"), DataPropertyName = "precioUnitario" });
            dgvLineas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("CargarFactura.colSubtotal"), DataPropertyName = "Subtotal" });

            
            dgvLineas.DataSource = items;

            if (dgvLineas.BindingContext[items] is CurrencyManager cm)
            {
                cm.Refresh();
            }

            dgvLineas.CurrentCell = null;

            lblTotal.Text = string.Format(t.Translate("CargarFactura.msgTotalFactura"), items.Sum(i => i.Subtotal).ToString());
        }

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            try
            {
                if(txtDNI.Text.Trim() == "")
                {
                    MessageBox.Show(t.Translate("CargarFactura.msgDniRequerido"));
                    return;
                }

                Cliente530BA cliente = bllCliente.ObtenerPorDNI(txtDNI.Text.Trim());
                lblNombreCliente.Text = cliente.NombreCompleto;

                dniClienteSeleccionado = cliente.DNI;

                MessageBox.Show(string.Format(t.Translate("CargarFactura.msgClienteAsignado"), cliente.NombreCompleto));
            }
            catch (Exception ex)
            {
                lblNombreCliente.Text = "";

                DialogResult respuesta = MessageBox.Show(t.Translate(ex.Message) + t.Translate("CargarFactura.msgRegistrarCliente"), t.Translate("CargarFactura.titleBuscarCliente"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (respuesta == DialogResult.Yes)
                {
                    using (GestionClientes form = new GestionClientes())
                    {
                        form.ShowDialog();
                    }
                }
            }
        }

        private void btnAgregarProducto_Click(object sender, EventArgs e)
        {
            using (SeleccionarProducto form = new SeleccionarProducto())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Producto530BA producto = form.ProductoSeleccionado;
                    int cantidad = form.CantidadSeleccionada;

                    ItemFactura530BA item = items.FirstOrDefault(i => i.codProducto == producto.codProducto);

                    if (item == null)
                    {
                        items.Add(new ItemFactura530BA
                        {
                            codProducto = producto.codProducto,
                            nombre = producto.nombre,
                            cantidad = cantidad,
                            precioUnitario = producto.precioUnitario
                        });
                    }
                    else
                    {
                        item.cantidad += cantidad;
                    }

                    CargarGrilla();
                }
            }
        }

        private void btnQuitarProducto_Click(object sender, EventArgs e)
        {
            if (dgvLineas.CurrentRow?.DataBoundItem is ItemFactura530BA item)
            {
                items.Remove(item);
                CargarGrilla();
            }
            else
            {
                var t = ServiceSessionManager530BA.getIntancia().Idioma;
                MessageBox.Show(t.Translate("GestionFactura.msgSeleccionarLinea"));
            }
        }

        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            if (dniClienteSeleccionado == "")
            {
                MessageBox.Show(t.Translate("GestionFactura.msgFaltaCliente"));
                return;
            }

            if (items.Count == 0)
            {
                MessageBox.Show(t.Translate("ExcFacturaSinItems"));
                return;
            }

            if (!ServiceSessionManager530BA.getIntancia().TienePermiso("Cobrar Venta"))
            {
                MessageBox.Show(t.Translate("CobrarVenta.msgSinPermiso"));
                return;
            }

            BLLFactura530BA bllFactura = new BLLFactura530BA();
            int idFactura;

            // se retienen los datos del cliente antes de limpiar el formulario
            string dni = dniClienteSeleccionado;
            string nombre = lblNombreCliente.Text;
            List<ItemFactura530BA> lineas = new List<ItemFactura530BA>(items);

            try
            {
                idFactura = bllFactura.GuardarFactura(dni, lineas);
            }
            catch (Exception ex)
            {
                MessageBox.Show(t.Translate("GestionFactura.msgErrorOperacion") + t.Translate(ex.Message));
                return;
            }

            MessageBox.Show(string.Format(t.Translate("GestionFactura.msgFacturaCreada"), idFactura) + "\n\n" + t.Translate("GestionFactura.msgDebeCobrar"));

            // El carrito ya quedo persistido en la base: se limpia siempre, se cobre o no. Si el operador no cobra ahora, la factura queda en estado 0 y habria que cobrarla despues por otro lado.
            items.Clear();
            txtDNI.Text = "";
            lblNombreCliente.Text = "";
            dniClienteSeleccionado = string.Empty;
            CargarGrilla();

            using (CobrarVenta form = new CobrarVenta())
            {
                form.CargarDatosDeFactura(idFactura, dni, nombre, lineas);
                form.ShowDialog();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dgvLineas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dgvLineas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
        }
    }
}