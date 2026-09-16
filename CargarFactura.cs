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

namespace Servicios
{
    public partial class CargarFactura : Form
    {
        BLLFactura530BA bllFactura = new BLLFactura530BA();
        BLLCliente530BA bllCliente = new BLLCliente530BA();
        List<ItemFactura530BA> items = new List<ItemFactura530BA>();
        private string dniClienteSeleccionado = string.Empty;

        public CargarFactura()
        {
            InitializeComponent();
            CargarGrilla();
        }

        private void CargarGrilla()
        {
            dgvLineas.AutoGenerateColumns = false;

            dgvLineas.Columns.Clear();
            dgvLineas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Producto", DataPropertyName = "nombre" });
            dgvLineas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cantidad", DataPropertyName = "cantidad" });
            dgvLineas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Precio Unitario", DataPropertyName = "precioUnitario" });
            dgvLineas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Subtotal", DataPropertyName = "Subtotal" });

            dgvLineas.DataSource = null;
            dgvLineas.DataSource = items;

            lblTotal.Text = "Total: " + items.Sum(i => i.Subtotal).ToString();
        }

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            try
            {
                if(txtDNI.Text.Trim() == "")
                {
                    MessageBox.Show("Debe ingresar un DNI para buscar el cliente."); //falta traudcir
                    return;
                }

                Cliente530BA cliente = bllCliente.ObtenerPorDNI(txtDNI.Text.Trim());
                lblNombreCliente.Text = cliente.NombreCompleto;

                dniClienteSeleccionado = cliente.DNI;

                MessageBox.Show("Cliente asignado: " + cliente.NombreCompleto);
            }
            catch (Exception ex)
            {
                lblNombreCliente.Text = "";

                DialogResult respuesta = MessageBox.Show(ex.Message + ".   ¿Quiere registrar el cliente?", "Buscar Cliente", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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
            if (dgvLineas.SelectedRows.Count > 0 && dgvLineas.SelectedRows[0].Index >= 0)
            {
                ItemFactura530BA item = (ItemFactura530BA)dgvLineas.CurrentRow.DataBoundItem;
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

            try
            {
                int idFactura = bllFactura.GuardarFactura(dniClienteSeleccionado, items);

                MessageBox.Show(string.Format(t.Translate("GestionFactura.msgFacturaCreada"), idFactura));

                items.Clear();
                txtDNI.Text = "";
                lblNombreCliente.Text = "";
                CargarGrilla();
            }
            catch (Exception ex)
            {
                MessageBox.Show(t.Translate("GestionFactura.msgErrorOperacion") + ex.Message);
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
            if (dgvLineas.SelectedRows.Count == 0 || dgvLineas.CurrentRow == null || dgvLineas.CurrentRow.Index < 0)
            {
                return;
            }
        }
    }
}