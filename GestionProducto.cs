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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Servicios
{
    public partial class GestionProducto : Form
    {
        BLLProducto530BA bllProducto = new BLLProducto530BA();
        List<Producto530BA> listProducto = new List<Producto530BA>();

        private enum ModoOperacion
        {
            Ninguno,
            Crear,
            Modificar,
            Activar,
            Desactivar
        }
        private ModoOperacion modoActual = ModoOperacion.Ninguno;

        public GestionProducto()
        {
            InitializeComponent();
            CargarGrilla();
        }

        private void CargarGrilla()
        {
            dgvProductos.AutoGenerateColumns = false;

            dgvProductos.Columns.Clear();
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nombre", DataPropertyName = "nombre" });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Existencia", DataPropertyName = "existencia" });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Precio Unitario", DataPropertyName = "precioUnitario" });
            dgvProductos.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Activo", DataPropertyName = "Activo" });

            listProducto = bllProducto.ObtenerTodos();

            dgvProductos.DataSource = null;
            dgvProductos.DataSource = listProducto;
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            btnCrear.Enabled = false;
            btnModificar.Enabled = false;
            btnActivar.Enabled = false;
            btnDesactivar.Enabled = false;

            btnGuardar.Enabled = true;
            btnCancelar.Enabled = true;

            modoActual = ModoOperacion.Crear;

            groupBox2.Visible = true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            try
            {
                switch (modoActual)
                {
                    case ModoOperacion.Crear:

                        string nombre = txtNombre.Text;
                        int existencia = Convert.ToInt32(txtExistencia.Text);
                        decimal precioUnitario = decimal.Parse(txtPrecioUnitario.Text);

                        if(precioUnitario <= 0)
                        {
                            MessageBox.Show(t.Translate("GestionProducto.msgPrecioMayorCero"));
                            return;
                        }

                        if(existencia <= 0)
                        {
                            MessageBox.Show(t.Translate("GestionProducto.msgExistenciaMayorCero"));
                            return;
                        }

                        bllProducto.AgregarProducto(nombre, existencia, precioUnitario);
                        break;

                    case ModoOperacion.Modificar:

                        string nombreMod = txtNombre.Text;
                        decimal precioUnitarioMod = decimal.Parse(txtPrecioUnitario.Text);

                        if (precioUnitarioMod <= 0)
                        {
                            MessageBox.Show(t.Translate("GestionProducto.msgPrecioMayorCero"));
                            return;
                        }

                        Producto530BA productoSeleccionado = (Producto530BA)dgvProductos.CurrentRow.DataBoundItem;

                        bllProducto.ActualizarProducto(productoSeleccionado.codProducto, nombreMod, precioUnitarioMod);
                        break;

                    case ModoOperacion.Activar:

                        Producto530BA productoSeleccionadoActivar = (Producto530BA)dgvProductos.CurrentRow.DataBoundItem;

                        if(productoSeleccionadoActivar.Activo)
                        {
                            MessageBox.Show(t.Translate("GestionProducto.msgProductoYaActivo"));
                            return;
                        }

                        bllProducto.ActivarProducto(productoSeleccionadoActivar.codProducto);
                        break;

                    case ModoOperacion.Desactivar:

                        Producto530BA productoSeleccionadoDesactivar = (Producto530BA)dgvProductos.CurrentRow.DataBoundItem;

                        if (!productoSeleccionadoDesactivar.Activo)
                        {
                            MessageBox.Show(t.Translate("GestionProducto.msgProductoYaDesactivado"));
                            return;
                        }

                        bllProducto.DesactivarProducto(productoSeleccionadoDesactivar.codProducto);
                        break;
                }

                CargarGrilla();
                groupBox2.Visible = false;

                txtNombre.Text = "";
                txtExistencia.Text = "";
                txtPrecioUnitario.Text = "";

                btnCrear.Enabled = true;
                btnModificar.Enabled = true;
                btnActivar.Enabled = true;
                btnDesactivar.Enabled = true;

                btnGuardar.Enabled = false;
                btnCancelar.Enabled = false;

                modoActual = ModoOperacion.Ninguno;
            }
            catch (Exception ex)
            {
                MessageBox.Show(t.Translate("GestionProducto.msgErrorOperacion") + ex.Message);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if(dgvProductos.SelectedRows.Count > 0)
            {
                btnCrear.Enabled = false;
                btnModificar.Enabled = false;
                btnActivar.Enabled = false;
                btnDesactivar.Enabled = false;

                btnGuardar.Enabled = true;
                btnCancelar.Enabled = true;

                Producto530BA productoSeleccionado = (Producto530BA)dgvProductos.SelectedRows[0].DataBoundItem;
                txtNombre.Text = productoSeleccionado.nombre;
                txtPrecioUnitario.Text = productoSeleccionado.precioUnitario.ToString();
                txtExistencia.Text = productoSeleccionado.existencia.ToString();
                modoActual = ModoOperacion.Modificar;
                groupBox2.Visible = true;

                txtExistencia.Enabled = false; // La existencia no se puede modificar aquí
            }
            else
            {
                var t = ServiceSessionManager530BA.getIntancia().Idioma;
                MessageBox.Show(t.Translate("GestionProducto.msgSeleccionarProductoModificar"));
            }
        }

        private void btnActivar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count > 0)
            {
                btnCrear.Enabled = false;
                btnModificar.Enabled = false;
                btnActivar.Enabled = false;
                btnDesactivar.Enabled = false;

                btnGuardar.Enabled = true;
                btnCancelar.Enabled = true;

                modoActual = ModoOperacion.Activar;
            }
            else
            {
                var t = ServiceSessionManager530BA.getIntancia().Idioma;
                MessageBox.Show(t.Translate("GestionProducto.msgSeleccionarProductoModificar"));
            }
        }

        private void btnDesactivar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count > 0)
            {
                btnCrear.Enabled = false;
                btnModificar.Enabled = false;
                btnActivar.Enabled = false;
                btnDesactivar.Enabled = false;

                btnGuardar.Enabled = true;
                btnCancelar.Enabled = true;

                modoActual = ModoOperacion.Desactivar;
            }
            else
            {
                var t = ServiceSessionManager530BA.getIntancia().Idioma;
                MessageBox.Show(t.Translate("GestionProducto.msgSeleccionarProductoModificar"));
            }

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            txtNombre.Text = "";
            txtExistencia.Text = "";
            txtPrecioUnitario.Text = "";

            btnCrear.Enabled = true;
            btnModificar.Enabled = true;
            btnActivar.Enabled = true;
            btnDesactivar.Enabled = true;

            btnGuardar.Enabled = false;
            btnCancelar.Enabled = false;
            groupBox2.Visible = false;

            modoActual = ModoOperacion.Ninguno;
        }
    }
}
