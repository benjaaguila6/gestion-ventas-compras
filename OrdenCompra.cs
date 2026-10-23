using BE;
using BLL;
using Services.Modelos.Idioma;
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
    public partial class OrdenCompra : Form, IIdiomaObserver
    {
        BLLProveedor530BA bllProveedor = new BLLProveedor530BA();
        BLLSolicitudCotizacion530BA bllSolicitud = new BLLSolicitudCotizacion530BA();
        private BindingList<ProductoCotizacion530BA> listaProductosMemoria;
        private List<SolicitudCotizacion530BA> solicitudesCargadas;

        public OrdenCompra()
        {
            InitializeComponent();
            ConfigurarGrilla();
            CargarSolicitudes();
            ActualizarEstadoBotones();

            ServiceSessionManager530BA.getIntancia().Idioma.Suscribir(this);
            actualizarIdioma();
        }

        private void ConfigurarGrilla()
        {
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "codProducto",
                DataPropertyName = "codProducto",
                HeaderText = "Código",
                ReadOnly = true
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NombreProducto",
                DataPropertyName = "NombreProducto",
                HeaderText = "Producto",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            // Solo "Cantidad" queda editable: el bloqueo va por columna, no por grilla.
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cantidad",
                DataPropertyName = "Cantidad",
                HeaderText = "Cant. Solicitada",
                ReadOnly = false
            });
        }

        private void CargarSolicitudes()
        {
            solicitudesCargadas = bllSolicitud.ObtenerSolicitudesPendientes();

            // Desenlazamos el evento temporalmente para que no dispare búsquedas mientras se llena
            cmbSolicitudes.SelectedIndexChanged -= cmbSolicitudes_SelectedIndexChanged;

            cmbSolicitudes.DataSource = solicitudesCargadas;
            cmbSolicitudes.DisplayMember = "DescripcionAmigable";
            cmbSolicitudes.ValueMember = "Id";
            cmbSolicitudes.SelectedIndex = -1; // Arranca vacío

            cmbSolicitudes.SelectedIndexChanged += cmbSolicitudes_SelectedIndexChanged;
        }

        private void cmbSolicitudes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cmbSolicitudes.SelectedItem is SolicitudCotizacion530BA solicitudSeleccionada))
            {
                return;
            }

            var productos = bllSolicitud.ObtenerProductosPorSolicitud(solicitudSeleccionada.Id);

            listaProductosMemoria = new BindingList<ProductoCotizacion530BA>(productos);
            dgvProductos.DataSource = listaProductosMemoria;

            ActualizarEstadoBotones();
        }

        private void ActualizarEstadoBotones()
        {
            bool hayFilaActual = dgvProductos.CurrentRow != null;
            bool hayProductos = listaProductosMemoria != null && listaProductosMemoria.Any();

            btnEliminar.Enabled = hayFilaActual;
            btnModificar.Enabled = hayFilaActual;
            btnPagar.Enabled = hayProductos;
        }

        private void dgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            ActualizarEstadoBotones();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;

            if (dgvProductos.CurrentRow == null)
            {
                MessageBox.Show(idioma.Translate("GenerarOC.msgSeleccioneProducto"), idioma.Translate("GenerarOC.titleAtencion"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var productoAQuitar = (ProductoCotizacion530BA)dgvProductos.CurrentRow.DataBoundItem;
            listaProductosMemoria.Remove(productoAQuitar); // Se quita de memoria, actualiza la grilla sola

            ActualizarEstadoBotones();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;

            if (dgvProductos.CurrentRow == null)
            {
                MessageBox.Show(idioma.Translate("GenerarOC.msgSeleccioneProducto"), idioma.Translate("GenerarOC.titleAtencion"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewCell celdaCantidad = dgvProductos.CurrentRow.Cells["Cantidad"];

            if (celdaCantidad == null)
            {
                return;
            }

            dgvProductos.CurrentCell = celdaCantidad;
            dgvProductos.EndEdit();
            dgvProductos.BeginEdit(true);
        }

        private void dgvProductos_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Cantidad no numerica: se revierte la celda al valor anterior sin romper el form.
            e.ThrowException = false;
        }

        private void btnPagar_Click(object sender, EventArgs e)
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;

            if (!(cmbSolicitudes.SelectedItem is SolicitudCotizacion530BA solicitudSeleccionada))
                return;

            if (listaProductosMemoria == null || !listaProductosMemoria.Any())
            {
                MessageBox.Show(idioma.Translate("GenerarOC.msgSinProductos"), idioma.Translate("GenerarOC.titleAtencion"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. Verificamos el estado del proveedor
            var proveedor = bllProveedor.ObtenerPorId(solicitudSeleccionada.IdProveedor);

            if (proveedor == null)
            {
                MessageBox.Show(idioma.Translate("GenerarOC.msgProveedorIncompleto"), idioma.Translate("GenerarOC.titleAtencion"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Si no está registrado definitivamente, disparamos el flujo existente
            if (!proveedor.EstaRegistradoDefinitivamente)
            {
                MessageBox.Show(idioma.Translate("GenerarOC.msgProveedorIncompleto"), idioma.Translate("GenerarOC.titleAtencion"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reutilizamos la pantalla de registro definitivo existente pasándole el proveedor pre-registrado
                GestionProveedor frmRegistroDefinitivo = new GestionProveedor();
                frmRegistroDefinitivo.ShowDialog();

                // Al volver, re-consultamos a la BD para asegurar que el registro se completó con éxito
                proveedor = bllProveedor.ObtenerPorId(solicitudSeleccionada.IdProveedor);

                // Si el usuario canceló la pantalla o falló, frenamos el flujo de pago
                if (proveedor == null || !proveedor.EstaRegistradoDefinitivamente)
                {
                    return;
                }
            }

            // 3. Confirmamos la edición pendiente de la grilla antes de leer las cantidades
            dgvProductos.EndEdit();

            List<ProductoCotizacion530BA> lineas = listaProductosMemoria.ToList();

            try
            {
                using (PagarOrdenCompra frmPago = new PagarOrdenCompra(solicitudSeleccionada.Id, proveedor.RazonSocial, lineas))
                {
                    if (frmPago.ShowDialog(this) == DialogResult.OK)
                    {
                        // La solicitud fue reemplazada por la orden: recargamos el combo.
                        listaProductosMemoria = null;
                        dgvProductos.DataSource = null;
                        CargarSolicitudes();
                    }
                }
            }
            finally
            {
                ActualizarEstadoBotones();
            }
        }

        public void actualizarIdioma()
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            this.Text = t.Translate("GenerarOC.formTitle");
            lblInstruccion.Text = t.Translate("GenerarOC.lblInstruccion");
            gbCotizaciones.Text = t.Translate("GenerarOC.gbCotizaciones");
            gbDetalles.Text = t.Translate("GenerarOC.gbDetalles");
            btnEliminar.Text = t.Translate("GenerarOC.btnEliminar");
            btnModificar.Text = t.Translate("GenerarOC.btnModificar");
            btnPagar.Text = t.Translate("GenerarOC.btnPagar");

            if (dgvProductos.Columns["codProducto"] != null) dgvProductos.Columns["codProducto"].HeaderText = t.Translate("GenerarOC.colCodProducto");
            if (dgvProductos.Columns["NombreProducto"] != null) dgvProductos.Columns["NombreProducto"].HeaderText = t.Translate("GenerarOC.colProducto");
            if (dgvProductos.Columns["Cantidad"] != null) dgvProductos.Columns["Cantidad"].HeaderText = t.Translate("GenerarOC.colCantidad");

            ActualizarDescripcionesSolicitudes();
        }
        private void ActualizarDescripcionesSolicitudes()
        {
            if (solicitudesCargadas == null || solicitudesCargadas.Count == 0)
            {
                return;
            }

            foreach (SolicitudCotizacion530BA solicitud in solicitudesCargadas)
            {
                solicitud.DescripcionAmigable = bllSolicitud.DescribirSolicitud(solicitud);
            }

            int indiceSeleccionado = cmbSolicitudes.SelectedIndex;

            cmbSolicitudes.SelectedIndexChanged -= cmbSolicitudes_SelectedIndexChanged;

            cmbSolicitudes.DataSource = null;
            cmbSolicitudes.DataSource = solicitudesCargadas;
            cmbSolicitudes.DisplayMember = "DescripcionAmigable";
            cmbSolicitudes.ValueMember = "Id";
            cmbSolicitudes.SelectedIndex = indiceSeleccionado;

            cmbSolicitudes.SelectedIndexChanged += cmbSolicitudes_SelectedIndexChanged;
        }

        private void OrdenCompra_Load(object sender, EventArgs e)
        {

        }
    }
}
