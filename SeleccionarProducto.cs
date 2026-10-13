using BE;
using BLL;
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
using Microsoft.VisualBasic;

namespace Servicios
{
    public partial class SeleccionarProducto : Form
    {
        BLLFactura530BA bllFactura = new BLLFactura530BA();
        List<Producto530BA> listProducto = new List<Producto530BA>();
        BLLProducto530BA bllProducto = new BLLProducto530BA();

        public Producto530BA ProductoSeleccionado { get; private set; }
        public int CantidadSeleccionada { get; private set; }

        public SeleccionarProducto()
        {
            InitializeComponent();
            CargarGrilla();

            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            this.Text = t.Translate("SeleccionarProducto.formTitle");
            groupBox1.Text = t.Translate("SeleccionarProducto.groupBoxProductos");
            label1.Text = t.Translate("SeleccionarProducto.labelBuscar");
            btnSeleccionar.Text = t.Translate("SeleccionarProducto.btnSeleccionar");
            btnCancelar.Text = t.Translate("SeleccionarProducto.btnCancelar");
        }

        private void CargarGrilla()
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            dgvProductos.AutoGenerateColumns = false;

            dgvProductos.Columns.Clear();
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("SeleccionarProducto.colNombre"), DataPropertyName = "nombre" });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("SeleccionarProducto.colStock"), DataPropertyName = "existencia" });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("SeleccionarProducto.colPrecioUnitario"), DataPropertyName = "precioUnitario" });

            listProducto = bllProducto.BuscarProductos(txtBuscar.Text.Trim());

            dgvProductos.DataSource = null;
            dgvProductos.DataSource = listProducto;
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarGrilla();
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count > 0)
            {
                var t = ServiceSessionManager530BA.getIntancia().Idioma;

                string input = Interaction.InputBox(t.Translate("SeleccionarProducto.msgIngresarCantidad"), t.Translate("SeleccionarProducto.titleCantidad"),"1");

                // Si cancela o deja vacío, no cerramos el formulario
                if (string.IsNullOrWhiteSpace(input)) return;

                try
                {
                    if (int.TryParse(input, out int cantidad) && cantidad > 0)
                    {
                        ProductoSeleccionado = (Producto530BA)dgvProductos.SelectedRows[0].DataBoundItem;
                        CantidadSeleccionada = cantidad;

                        bllProducto.ValidarStock(ProductoSeleccionado.codProducto, cantidad);

                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show(t.Translate("SeleccionarProducto.msgCantidadInvalida"));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ServiceSessionManager530BA.getIntancia().Idioma.Translate(ex.Message));
                }
            }
            else
            {
                var t = ServiceSessionManager530BA.getIntancia().Idioma;
                MessageBox.Show(t.Translate("SeleccionarProducto.msgSeleccionar"));
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}