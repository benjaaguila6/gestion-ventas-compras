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
        }

        private void CargarGrilla()
        {
            dgvProductos.AutoGenerateColumns = false;

            dgvProductos.Columns.Clear();
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nombre", DataPropertyName = "nombre" });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Stock", DataPropertyName = "existencia" });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Precio Unitario", DataPropertyName = "precioUnitario" });

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

                string input = Interaction.InputBox("Ingrese la cantidad a seleccionar:", "Cantidad","1");

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
                        MessageBox.Show("Por favor ingrese un número entero mayor a 0.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
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