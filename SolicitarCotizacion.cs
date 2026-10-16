using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;
using Services.Modelos.Idioma;
using Services_530BA;

namespace Servicios
{
    public partial class SolicitarCotizacion : Form, IIdiomaObserver
    {
        BLLProveedor530BA bllProveedor = new BLLProveedor530BA();
        BLLProducto530BA bllProducto = new BLLProducto530BA();
        BLLSolicitudCotizacion530BA bllSolicitud = new BLLSolicitudCotizacion530BA();
        List<ProductoCotizacion530BA> items = new List<ProductoCotizacion530BA>();

        public SolicitarCotizacion()
        {
            InitializeComponent();
            cargarProveedores();
            cargarProductos();
            refrescarGrilla();
            actualizarIdioma();
        }

        private void cargarProveedores()
        {
            List<Proveedor530BA> proveedores = bllProveedor.ObtenerTodos();

            dgvProveedores.DataSource = null;
            dgvProveedores.DataSource = proveedores.Where(p => p.EstaRegistradoDefinitivamente);

            dgvProveedores.Columns["Id"].Visible = false;
            dgvProveedores.Columns["EstaRegistradoDefinitivamente"].Visible = false;

            dgvProveedores.Columns["RazonSocial"].HeaderText = "Razón Social";
            dgvProveedores.Columns["Cuit"].HeaderText = "CUIT";
            dgvProveedores.Columns["Email"].HeaderText = "Correo Electrónico";
            dgvProveedores.Columns["Telefono"].HeaderText = "Teléfono";
            dgvProveedores.Columns["Direccion"].HeaderText = "Dirección";
            dgvProveedores.Columns["CVU"].HeaderText = "CVU";
            dgvProveedores.Columns["Estado"].HeaderText = "Fase de Registro";
            dgvProveedores.Columns["Activo"].HeaderText = "Activo";

            dgvProveedores.Columns["RazonSocial"].DisplayIndex = 0;
            dgvProveedores.Columns["Cuit"].DisplayIndex = 5;
            dgvProveedores.Columns["Email"].DisplayIndex = 1;
            dgvProveedores.Columns["Telefono"].DisplayIndex = 2;
            dgvProveedores.Columns["Direccion"].DisplayIndex = 6;
            dgvProveedores.Columns["CVU"].DisplayIndex = 7;
            dgvProveedores.Columns["Estado"].DisplayIndex = 3;
            dgvProveedores.Columns["Activo"].DisplayIndex = 4;

            
        }

        private void cargarProductos()
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            List<Producto530BA> productos = bllProducto.ObtenerTodos();

            dgvProductos.DataSource = null;
            dgvProductos.DataSource = productos;

            dgvProductos.AutoGenerateColumns = false;

            dgvProductos.Columns.Clear();
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("GestionProducto.colNombre"), DataPropertyName = "nombre" });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("GestionProducto.colExistencia"), DataPropertyName = "existencia" });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("GestionProducto.colPrecioUnitario"), DataPropertyName = "precioUnitario" });
            dgvProductos.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = t.Translate("GestionProducto.colActivo"), DataPropertyName = "Activo" });
        }

        private void btnAgregarItem_Click(object sender, EventArgs e)
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            try
            {
                var ProductoSeleccionado = (Producto530BA)dgvProductos.SelectedRows[0].DataBoundItem;

                if (ProductoSeleccionado == null)
                {
                    MessageBox.Show(t.Translate("SolicitarCotizacion.msgFaltaProducto"), t.Translate("SolicitarCotizacion.titleAviso"));
                    return;
                }

                int codProducto = ProductoSeleccionado.codProducto;
                string nombre = ProductoSeleccionado.nombre;

                int cantidad = Convert.ToInt32(txtCantidad.Text);

                if (cantidad <= 0)
                {
                    MessageBox.Show(t.Translate("SolicitarCotizacion.msgCantidadInvalida"), t.Translate("SolicitarCotizacion.titleAviso"));
                    return;
                }

                if (items.Any(i => i.codProducto == codProducto))
                {
                    MessageBox.Show(t.Translate("SolicitarCotizacion.msgProductoDuplicado"), t.Translate("SolicitarCotizacion.titleAviso"));
                    return;
                }

                items.Add(new ProductoCotizacion530BA { codProducto = codProducto, Nombre = nombre, Cantidad = cantidad });

                refrescarGrilla();
                txtCantidad.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(t.Translate("SolicitarCotizacion.titleError") + ": " + ex.Message);
            }
        }

        private void btnQuitarItem_Click(object sender, EventArgs e)
        {
            if (dgvItems.CurrentRow?.DataBoundItem is ProductoCotizacion530BA item)
            {
                items.Remove(item);
                refrescarGrilla();
            }
        }

        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            try
            {
                var proveedorSeleccionado = (Proveedor530BA)dgvProveedores.CurrentRow.DataBoundItem;

                if (proveedorSeleccionado == null)
                {
                    MessageBox.Show(t.Translate("SolicitarCotizacion.msgFaltaProveedor"), t.Translate("SolicitarCotizacion.titleAviso"));
                    return;
                }

                if (items.Count == 0)
                {
                    MessageBox.Show(t.Translate("SolicitarCotizacion.msgSinItems"), t.Translate("SolicitarCotizacion.titleAviso"));
                    return;
                }

                int idProveedor = proveedorSeleccionado.Id;

                btnFinalizar.Enabled = false;

                int idSolicitud = bllSolicitud.GenerarSolicitud(idProveedor, items);

                MessageBox.Show(string.Format(t.Translate("SolicitarCotizacion.msgExito"), idSolicitud), t.Translate("SolicitarCotizacion.titleExito"));

                items.Clear();
                refrescarGrilla();
            }
            catch (Exception ex)
            {
                MessageBox.Show(t.Translate("SolicitarCotizacion.titleError") + ": " + ex.Message);
            }
        }

        private void refrescarGrilla()
        {
            dgvItems.DataSource = null;
            dgvItems.DataSource = items.ToList();
            dgvItems.CurrentCell = null;
        }


        public void actualizarIdioma()
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            this.Text = t.Translate("SolicitarCotizacion.formTitle");
            gbProveedor.Text = t.Translate("SolicitarCotizacion.gbProveedor");
            gbProducto.Text = t.Translate("SolicitarCotizacion.gbProducto");
            gbDetalle.Text = t.Translate("SolicitarCotizacion.gbDetalle");
            labelCantidad.Text = t.Translate("SolicitarCotizacion.labelCantidad");
            btnAgregarItem.Text = t.Translate("SolicitarCotizacion.btnAgregar");
            btnQuitarItem.Text = t.Translate("SolicitarCotizacion.btnQuitar");
            btnFinalizar.Text = t.Translate("SolicitarCotizacion.btnFinalizar");

            if (dgvItems.Columns["codProducto"] != null) dgvItems.Columns["codProducto"].HeaderText = t.Translate("SolicitarCotizacion.colCodProducto");
            if (dgvItems.Columns["Nombre"] != null) dgvItems.Columns["Nombre"].HeaderText = t.Translate("SolicitarCotizacion.colProducto");
            if (dgvItems.Columns["Cantidad"] != null) dgvItems.Columns["Cantidad"].HeaderText = t.Translate("SolicitarCotizacion.colCantidad");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GestionProveedor form = new GestionProveedor();
            
            form.FormClosed += (s, args) =>
            {
                cargarProveedores();
            };

            form.Show();
        }
    }
}
