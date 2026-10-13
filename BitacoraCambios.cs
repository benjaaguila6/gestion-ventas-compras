using BE;
using BLL;
using Services_530BA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Servicios
{
    public partial class BitacoraCambios : Form
    {
        BLLProducto530BA bllProducto = new BLLProducto530BA();
        BLLProductoHistorial530BA bllProductoHistorial = new BLLProductoHistorial530BA();

        private Producto530BA productoSeleccionado = null;

        public BitacoraCambios()
        {
            InitializeComponent();
            CargarFiltroEstado();
            ConfigurarColumnasProductos();
            ConfigurarColumnasHistorial();
            actualizarIdioma();
        }

        private void BitacoraCambios_Load(object sender, EventArgs e)
        {
            txtBuscar.Focus();
        }

        private void CargarFiltroEstado()
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            var lista = new List<object>();
            lista.Add(new { Valor = 0, Nombre = t.Translate("BitacoraCambios.filtroTodos") });
            lista.Add(new { Valor = 1, Nombre = t.Translate("BitacoraCambios.filtroSoloActivos") });
            lista.Add(new { Valor = 2, Nombre = t.Translate("BitacoraCambios.filtroSoloInactivos") });

            cmbFiltroEstado.DataSource = lista;
            cmbFiltroEstado.DisplayMember = "Nombre";
            cmbFiltroEstado.ValueMember = "Valor";
            cmbFiltroEstado.SelectedIndex = 0;
        }

        private void ConfigurarColumnasProductos()
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            dgvProductos.AutoGenerateColumns = false;
            dgvProductos.Columns.Clear();

            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCodigo",
                HeaderText = t.Translate("BitacoraCambios.colCodigo"),
                DataPropertyName = "codProducto",
                FillWeight = 25
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNombre",
                HeaderText = t.Translate("BitacoraCambios.colNombre"),
                DataPropertyName = "nombre",
                FillWeight = 40
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colExistencia",
                HeaderText = t.Translate("BitacoraCambios.colExistencia"),
                DataPropertyName = "existencia",
                FillWeight = 20
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPrecio",
                HeaderText = t.Translate("BitacoraCambios.colPrecio"),
                DataPropertyName = "precioUnitario",
                FillWeight = 20
            });
            dgvProductos.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colActivo",
                HeaderText = t.Translate("BitacoraCambios.colActivo"),
                DataPropertyName = "Activo",
                FillWeight = 15
            });
        }

        private void ConfigurarColumnasHistorial()
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            dgvHistorial.AutoGenerateColumns = false;
            dgvHistorial.Columns.Clear();

            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colVersion",
                HeaderText = t.Translate("BitacoraCambios.colVersion"),
                DataPropertyName = "Version",
                FillWeight = 12
            });
            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFechaHora",
                HeaderText = t.Translate("BitacoraCambios.colFechaHora"),
                DataPropertyName = "FechaHora",
                FillWeight = 18
            });
            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTipoOperacion",
                HeaderText = t.Translate("BitacoraCambios.colTipoOperacion"),
                DataPropertyName = "TipoOperacion",
                FillWeight = 15
            });
            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNombre",
                HeaderText = t.Translate("BitacoraCambios.colNombre"),
                DataPropertyName = "nombre",
                FillWeight = 20
            });
            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colExistencia",
                HeaderText = t.Translate("BitacoraCambios.colExistencia"),
                DataPropertyName = "existencia",
                FillWeight = 12
            });
            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPrecio",
                HeaderText = t.Translate("BitacoraCambios.colPrecio"),
                DataPropertyName = "precioUnitario",
                FillWeight = 12
            });
            dgvHistorial.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colVersionActiva",
                HeaderText = t.Translate("BitacoraCambios.colVersionActiva"),
                DataPropertyName = "VersionActiva",
                FillWeight = 10
            });
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            try
            {
                string filtro = txtBuscar.Text.Trim();
                int estado = Convert.ToInt32(cmbFiltroEstado.SelectedValue ?? 0);

                List<Producto530BA> productos = bllProducto.ObtenerTodos();

                if (!string.IsNullOrEmpty(filtro))
                {
                    int codigoBuscado = 0;
                    bool esCodigo = int.TryParse(filtro, out codigoBuscado);

                    productos = productos
                        .Where(p => p.nombre.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0
                                    || (esCodigo && p.codProducto == codigoBuscado))
                        .ToList();
                }

                if (estado == 1)
                {
                    productos = productos.Where(p => p.Activo).ToList();
                }
                else if (estado == 2)
                {
                    productos = productos.Where(p => !p.Activo).ToList();
                }

                dgvProductos.DataSource = productos;

                productoSeleccionado = null;
                dgvHistorial.DataSource = null;
                lblProductoSeleccionado.Text = t.Translate("BitacoraCambios.lblSeleccioneProducto");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    t.Translate("BitacoraCambios.msgErrorOperacion") + t.Translate(ex.Message),
                    t.Translate("BitacoraCambios.titleError"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvProductos.CurrentRow?.DataBoundItem is Producto530BA producto)
            {
                CargarHistorialProducto(producto);
            }
        }

        private void CargarHistorialProducto(Producto530BA producto)
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            productoSeleccionado = producto;
            lblProductoSeleccionado.Text = string.Format(
                t.Translate("BitacoraCambios.lblProductoSeleccionado"),
                producto.codProducto,
                producto.nombre
            );

            dgvHistorial.DataSource = bllProductoHistorial.ObtenerHistorial(producto.codProducto);
        }

        private void btnRestaurarVersion_Click(object sender, EventArgs e)
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            try
            {
                if (productoSeleccionado == null)
                {
                    MessageBox.Show(
                        t.Translate("BitacoraCambios.msgSeleccionarProducto"),
                        t.Translate("BitacoraCambios.titleAviso"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                if (!(dgvHistorial.CurrentRow?.DataBoundItem is ProductoHistorial530BA version))
                {
                    MessageBox.Show(
                        t.Translate("BitacoraCambios.msgSeleccionarVersion"),
                        t.Translate("BitacoraCambios.titleAviso"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                int versionSeleccionada = version.Version;

                DialogResult respuesta = MessageBox.Show(
                    string.Format(t.Translate("BitacoraCambios.msgConfirmarRestauracion"), versionSeleccionada),
                    t.Translate("BitacoraCambios.titleConfirmarRestauracion"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (respuesta != DialogResult.Yes) return;

                bllProductoHistorial.RestaurarVersion(productoSeleccionado.codProducto, versionSeleccionada);

                MessageBox.Show(
                    t.Translate("BitacoraCambios.msgVersionRestaurada"),
                    t.Translate("BitacoraCambios.titleExito"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                dgvHistorial.DataSource = bllProductoHistorial.ObtenerHistorial(productoSeleccionado.codProducto);
                dgvProductos.DataSource = bllProducto.ObtenerTodos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    t.Translate("BitacoraCambios.msgErrorOperacion") + t.Translate(ex.Message),
                    t.Translate("BitacoraCambios.titleError"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        public void actualizarIdioma()
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            this.Text = t.Translate("BitacoraCambios.formTitle");
            label1.Text = t.Translate("BitacoraCambios.labelBuscar");
            label2.Text = t.Translate("BitacoraCambios.labelEstado");
            btnBuscar.Text = t.Translate("BitacoraCambios.btnBuscar");
            btnRestaurarVersion.Text = t.Translate("BitacoraCambios.btnRestaurarVersion");
            groupBox1.Text = t.Translate("BitacoraCambios.groupProductos");
            groupBox2.Text = t.Translate("BitacoraCambios.groupHistorial");
            lblProductoSeleccionado.Text = t.Translate("BitacoraCambios.lblSeleccioneProducto");
        }
    }
}