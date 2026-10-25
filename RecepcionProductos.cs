using BE;
using BLL;
using Microsoft.VisualBasic;
using Services.Modelos.Idioma;
using Services_530BA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Servicios
{
    public partial class RecepcionProductos : Form, IIdiomaObserver
    {
        BLLOrdenCompra530BA bllOrden = new BLLOrdenCompra530BA();
        private BindingList<ProductoOC530BA> listaProductos;
        private List<OrdenCompra530BA> ordenesCargadas;
        private OrdenCompra530BA ordenSeleccionada;

        public RecepcionProductos()
        {
            InitializeComponent();
            ConfigurarGrillas();
            CargarOrdenes();
            ActualizarEstadoControles();

            ServiceSessionManager530BA.getIntancia().Idioma.Suscribir(this);
            actualizarIdioma();
        }

        private void ConfigurarGrillas()
        {
            dgvOrdenes.AutoGenerateColumns = false;
            dgvOrdenes.Columns.Clear();
            dgvOrdenes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                DataPropertyName = "Id",
                ReadOnly = true
            });
            dgvOrdenes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProveedorRazonSocial",
                DataPropertyName = "ProveedorRazonSocial",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dgvOrdenes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FechaEmision",
                DataPropertyName = "FechaEmision",
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });
            dgvOrdenes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Estado",
                ReadOnly = true
            });

            dgvProductos.AutoGenerateColumns = false;
            dgvProductos.Columns.Clear();
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "codProducto",
                DataPropertyName = "codProducto",
                ReadOnly = true
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NombreProducto",
                DataPropertyName = "NombreProducto",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cantidad",
                DataPropertyName = "Cantidad",
                ReadOnly = true
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CantidadRecibida",
                DataPropertyName = "CantidadRecibida",
                ReadOnly = true
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Pendiente",
                DataPropertyName = "Pendiente",
                ReadOnly = true
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Precio",
                DataPropertyName = "Precio",
                ReadOnly = true
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CantidadEstaEntrega",
                DataPropertyName = "CantidadEstaEntrega",
                ReadOnly = false
            });
        }

        private void CargarOrdenes(int? idSeleccionar = null)
        {
            ordenesCargadas = bllOrden.ObtenerTodas();
            dgvOrdenes.SelectionChanged -= dgvOrdenes_SelectionChanged;
            dgvOrdenes.DataSource = null;
            dgvOrdenes.DataSource = new BindingList<OrdenCompra530BA>(ordenesCargadas);

            foreach (DataGridViewRow row in dgvOrdenes.Rows)
            {
                if (row.DataBoundItem is OrdenCompra530BA o && o.Recibido)
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                }
            }

            if (idSeleccionar.HasValue)
            {
                foreach (DataGridViewRow row in dgvOrdenes.Rows)
                {
                    if (row.DataBoundItem is OrdenCompra530BA o && o.Id == idSeleccionar.Value)
                    {
                        row.Selected = true;
                        dgvOrdenes.CurrentCell = row.Cells["Id"];
                        break;
                    }
                }
            }

            dgvOrdenes.SelectionChanged += dgvOrdenes_SelectionChanged;
            dgvOrdenes_SelectionChanged(dgvOrdenes, EventArgs.Empty);
        }

        private void dgvOrdenes_SelectionChanged(object sender, EventArgs e)
        {
            ordenSeleccionada = dgvOrdenes.CurrentRow?.DataBoundItem as OrdenCompra530BA;
            CargarProductos();
            ActualizarEstadoControles();
        }

        private void CargarProductos()
        {
            if (ordenSeleccionada == null)
            {
                listaProductos = null;
                dgvProductos.DataSource = null;
                return;
            }

            List<ProductoOC530BA> productos = bllOrden.ObtenerProductosPorOrden(ordenSeleccionada.Id);

            foreach (ProductoOC530BA producto in productos)
            {
                producto.CantidadEstaEntrega = 0;
            }

            listaProductos = new BindingList<ProductoOC530BA>(productos);
            dgvProductos.DataSource = null;
            dgvProductos.DataSource = listaProductos;
            dgvProductos.ReadOnly = ordenSeleccionada.Recibido;
        }

        private void ActualizarEstadoControles()
        {
            bool hayOrdenPendiente = ordenSeleccionada != null && !ordenSeleccionada.Recibido;
            bool hayProducto = dgvProductos.CurrentRow != null;

            btnModificarEntrega.Enabled = hayOrdenPendiente && hayProducto;
            btnRegistrarRecepcion.Enabled = hayOrdenPendiente;
        }

        private void dgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            ActualizarEstadoControles();
        }

        private void dgvOrdenes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvOrdenes.Columns[e.ColumnIndex].Name != "Estado")
            {
                return;
            }

            if (dgvOrdenes.Rows[e.RowIndex].DataBoundItem is OrdenCompra530BA orden)
            {
                var t = ServiceSessionManager530BA.getIntancia().Idioma;
                e.Value = t.Translate(EstadoKey(orden.EstadoRecepcion));
                e.FormattingApplied = true;
            }
        }

        private string EstadoKey(int estado)
        {
            switch (estado)
            {
                case 2:
                    return "RecepcionProductos.estadoRecibido";
                case 1:
                    return "RecepcionProductos.estadoParcial";
                default:
                    return "RecepcionProductos.estadoPendiente";
            }
        }

        private void dgvProductos_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void btnModificarEntrega_Click(object sender, EventArgs e)
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            if (ordenSeleccionada == null)
            {
                MessageBox.Show(t.Translate("RecepcionProductos.msgSinSeleccion"),
                    t.Translate("RecepcionProductos.titleAtencion"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ordenSeleccionada.Recibido)
            {
                MessageBox.Show(t.Translate("RecepcionProductos.msgOrdenYaRecibida"),
                    t.Translate("RecepcionProductos.titleAtencion"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvProductos.CurrentRow == null)
            {
                MessageBox.Show(t.Translate("RecepcionProductos.msgSinProducto"),
                    t.Translate("RecepcionProductos.titleAtencion"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dgvProductos.EndEdit();
            ProductoOC530BA producto = dgvProductos.CurrentRow.DataBoundItem as ProductoOC530BA;

            if (producto == null)
            {
                return;
            }

            string entrada = Interaction.InputBox(t.Translate("RecepcionProductos.msgIngresarCantidad"), t.Translate("RecepcionProductos.titleCantidad"), producto.CantidadEstaEntrega.ToString());

            if (string.IsNullOrWhiteSpace(entrada))
            {
                return;
            }

            if (!int.TryParse(entrada, out int cantidad) || cantidad < 0 || cantidad > producto.Pendiente)
            {
                MessageBox.Show(t.Translate("RecepcionProductos.msgCantidadInvalida"), t.Translate("RecepcionProductos.titleAtencion"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            producto.CantidadEstaEntrega = cantidad;
            dgvProductos.Refresh();
        }

        private void btnRegistrarRecepcion_Click(object sender, EventArgs e)
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            if (ordenSeleccionada == null)
            {
                MessageBox.Show(t.Translate("RecepcionProductos.msgSinSeleccion"), t.Translate("RecepcionProductos.titleAtencion"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ordenSeleccionada.Recibido)
            {
                MessageBox.Show(t.Translate("RecepcionProductos.msgOrdenYaRecibida"), t.Translate("RecepcionProductos.titleAtencion"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dgvProductos.EndEdit();

            if (listaProductos == null || listaProductos.Count == 0)
            {
                MessageBox.Show(t.Translate("RecepcionProductos.msgSinCantidades"), t.Translate("RecepcionProductos.titleAtencion"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (ProductoOC530BA producto in listaProductos)
            {
                if (producto.CantidadEstaEntrega < 0 || producto.CantidadEstaEntrega > producto.Pendiente)
                {
                    MessageBox.Show(t.Translate("RecepcionProductos.msgCantidadInvalida"), t.Translate("RecepcionProductos.titleAtencion"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (!listaProductos.Any(p => p.CantidadEstaEntrega > 0))
            {
                MessageBox.Show(t.Translate("RecepcionProductos.msgSinCantidades"), t.Translate("RecepcionProductos.titleAtencion"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<string> pendientes = listaProductos
                .Where(p => p.CantidadRecibida + p.CantidadEstaEntrega < p.Cantidad)
                .Select(p => string.Format(t.Translate("RecepcionProductos.msgItemPendiente"), p.NombreProducto,
                    p.Cantidad - (p.CantidadRecibida + p.CantidadEstaEntrega)))
                .ToList();

            string mensaje = pendientes.Count > 0
                ? t.Translate("RecepcionProductos.msgConfirmarParcial") + Environment.NewLine + Environment.NewLine +
                  string.Format(t.Translate("RecepcionProductos.msgPendientes"), string.Join(", ", pendientes))
                : t.Translate("RecepcionProductos.msgConfirmarRecepcion");

            if (MessageBox.Show(mensaje, t.Translate("RecepcionProductos.titleConfirmacion"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            int idOrden = ordenSeleccionada.Id;
            btnRegistrarRecepcion.Enabled = false;

            try
            {
                bool completa = bllOrden.RegistrarRecepcion(idOrden, listaProductos.ToList());

                MessageBox.Show(completa ? t.Translate("RecepcionProductos.msgExito") : t.Translate("RecepcionProductos.msgRecepcionParcial"), t.Translate("RecepcionProductos.titleExito"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarOrdenes(idOrden);
            }
            catch (Exception ex)
            {
                MessageBox.Show(t.Translate("RecepcionProductos.msgError") + " " + t.Translate(ex.Message), t.Translate("RecepcionProductos.titleError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                CargarOrdenes(idOrden);
            }
            finally
            {
                ActualizarEstadoControles();
            }
        }

        public void actualizarIdioma()
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            this.Text = t.Translate("RecepcionProductos.formTitle");
            lblInstruccion.Text = t.Translate("RecepcionProductos.lblInstruccion");
            gbOrdenes.Text = t.Translate("RecepcionProductos.gbOrdenes");
            gbProductos.Text = t.Translate("RecepcionProductos.gbProductos");
            btnModificarEntrega.Text = t.Translate("RecepcionProductos.btnModificarEntrega");
            btnRegistrarRecepcion.Text = t.Translate("RecepcionProductos.btnRegistrarRecepcion");

            dgvOrdenes.Columns["Id"].HeaderText = t.Translate("RecepcionProductos.colIdOrden");
            dgvOrdenes.Columns["ProveedorRazonSocial"].HeaderText = t.Translate("RecepcionProductos.colProveedor");
            dgvOrdenes.Columns["FechaEmision"].HeaderText = t.Translate("RecepcionProductos.colFechaEmision");
            dgvOrdenes.Columns["Estado"].HeaderText = t.Translate("RecepcionProductos.colEstado");

            dgvProductos.Columns["codProducto"].HeaderText = t.Translate("RecepcionProductos.colCodProducto");
            dgvProductos.Columns["NombreProducto"].HeaderText = t.Translate("RecepcionProductos.colProducto");
            dgvProductos.Columns["Cantidad"].HeaderText = t.Translate("RecepcionProductos.colCantSolicitada");
            dgvProductos.Columns["CantidadRecibida"].HeaderText = t.Translate("RecepcionProductos.colCantRecibida");
            dgvProductos.Columns["Pendiente"].HeaderText = t.Translate("RecepcionProductos.colPendiente");
            dgvProductos.Columns["Precio"].HeaderText = t.Translate("RecepcionProductos.colPrecio");
            dgvProductos.Columns["CantidadEstaEntrega"].HeaderText = t.Translate("RecepcionProductos.colCantEntrega");

            dgvOrdenes.Refresh();
        }
    }
}
