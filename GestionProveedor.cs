using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BE;
using BLL;
using Services;
using Services.Modelos.Idioma;
using Services_530BA;

namespace Servicios
{
    public partial class GestionProveedor : Form, IIdiomaObserver
    {
        BLLProveedor530BA bllProveedor = new BLLProveedor530BA();
        List<Proveedor530BA> lista = new List<Proveedor530BA>();
        public GestionProveedor()
        {
            InitializeComponent();
            cargarGrilla();

            ServiceSessionManager530BA.getIntancia().Idioma.Suscribir(this);
            actualizarIdioma();
        }

        private void cargarGrilla()
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            lista = bllProveedor.ObtenerTodos();

            dgvProveedor.DataSource = null;
            dgvProveedor.DataSource = lista;

            dgvProveedor.Columns["Id"].Visible = false;
            dgvProveedor.Columns["EstaRegistradoDefinitivamente"].Visible = false;
            dgvProveedor.Columns["DVH"].Visible = false;

            dgvProveedor.Columns["RazonSocial"].DisplayIndex = 0;
            dgvProveedor.Columns["Cuit"].DisplayIndex = 5;
            dgvProveedor.Columns["Email"].DisplayIndex = 1;
            dgvProveedor.Columns["Telefono"].DisplayIndex = 2;
            dgvProveedor.Columns["Direccion"].DisplayIndex = 6;
            dgvProveedor.Columns["CVU"].DisplayIndex = 7;
            dgvProveedor.Columns["Estado"].DisplayIndex = 3;
            dgvProveedor.Columns["Activo"].DisplayIndex = 4;

            TraducirColumnasProveedor(t);
        }

        private enum ModoOperacion
        {
            Ninguno,
            Agregar,
            Modificar,
            ActivarDesactivar
        }

        private ModoOperacion ModoActual = ModoOperacion.Ninguno;

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            gbDatos.Visible = true;
            ModoActual = ModoOperacion.Agregar;

            btnAgregar.Enabled = false;
            btnModificar.Enabled = false;
            btnActDesact.Enabled = false;
            btnGuardar.Enabled = true;
            btnCancelar.Enabled = true; 

            if (rbPreRegistrar.Checked)
            {
                txtCuit.Enabled = false;
                txtDireccion.Enabled = false;
                txtCVU.Enabled = false;
            }

            if (rbRegistrar.Checked)
            {
                DataGridViewRow fila = dgvProveedor.CurrentRow;

                txtRazonSocial.Text = fila.Cells["RazonSocial"].Value.ToString();
                txtEmail.Text = fila.Cells["Email"].Value.ToString();
                txtTelefono.Text = fila.Cells["Telefono"].Value.ToString();
            }
            
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            gbDatos.Visible = true;
            ModoActual = ModoOperacion.Modificar;

            DataGridViewRow fila = dgvProveedor.CurrentRow;

            txtRazonSocial.Text = fila.Cells["RazonSocial"].Value.ToString();
            txtEmail.Text = fila.Cells["Email"].Value.ToString();
            txtTelefono.Text = fila.Cells["Telefono"].Value.ToString();

            
            //txtCuit.Text = fila.Cells["Cuit"].Value.ToString();
            //txtCVU.Text = fila.Cells["CVU"].Value.ToString();
            //txtDireccion.Text = fila.Cells["Direccion"].Value.ToString();

            btnAgregar.Enabled = false;
            btnModificar.Enabled = false;
            btnActDesact.Enabled = false;
            btnGuardar.Enabled = true;
            btnCancelar.Enabled = true;

            if (!rbRegistrar.Checked)
            {
                txtEmail.Enabled = false;
                txtRazonSocial.Enabled = false;
                txtCuit.Enabled = false;
            }
            
        }

        private void btnActDesact_Click(object sender, EventArgs e)
        {
            btnAgregar.Enabled = false;
            btnModificar.Enabled = false;
            btnActDesact.Enabled = false;
            btnGuardar.Enabled = true;
            btnCancelar.Enabled = true;

            ModoActual = ModoOperacion.ActivarDesactivar;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;
            try
            {
                switch (ModoActual)
                {
                    case ModoOperacion.Agregar:
                        string email = txtEmail.Text;
                        string razonSocial = txtRazonSocial.Text;
                        string telefono = txtTelefono.Text;

                        string cuit = txtCuit.Text;
                        string direccion = txtDireccion.Text;
                        string cvu = txtCVU.Text;

                        if (rbPreRegistrar.Checked)
                        {
                            bllProveedor.PreRegistrar(razonSocial, email, telefono);
                        }
                        else if (rbRegistrar.Checked)
                        {
                            Proveedor530BA proveedorSeleccionado = (Proveedor530BA)dgvProveedor.CurrentRow.DataBoundItem;

                            bllProveedor.CompletarRegistro(proveedorSeleccionado, cuit, direccion, cvu);
                        }

                        MessageBox.Show(idioma.Translate("GestionProveedor.proveedorExitoso"));
                        break;

                    case ModoOperacion.Modificar:
                        string telefonoModificado = txtTelefono.Text;
                        string direccionModificado = txtDireccion.Text;
                        string cvuModificado = txtCVU.Text;

                        Proveedor530BA proveedorModificado = (Proveedor530BA)dgvProveedor.CurrentRow.DataBoundItem;

                        bllProveedor.ModificarProveedor(proveedorModificado.Id, telefonoModificado, direccionModificado, cvuModificado);

                        MessageBox.Show(idioma.Translate("GestionProveedor.proveedorModificado"));
                        break;

                    case ModoOperacion.ActivarDesactivar:

                        Proveedor530BA proveedor = (Proveedor530BA)dgvProveedor.CurrentRow.DataBoundItem;

                        DialogResult r = MessageBox.Show(idioma.Translate("GestionProveedor.msgConfirmarCambioEstado"), idioma.Translate("GestionProveedor.msgConfirmar"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (r != DialogResult.Yes)
                        {
                            return;
                        }

                        bllProveedor.ActivarDesactivar(proveedor.Id);
                        break;
                }

                ModoActual = ModoOperacion.Ninguno;
                gbDatos.Visible = false;
                btnAgregar.Enabled = true;
                btnModificar.Enabled = true;
                btnActDesact.Enabled = true;
                btnGuardar.Enabled = false;
                btnCancelar.Enabled = false;

                cargarGrilla();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(idioma.Translate("GestionProveedor.msgError"), ex.Message), idioma.Translate("GestionProveedor.titleError"));
            }
        }

        public void actualizarIdioma()
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            this.Text = t.Translate("GestionProveedor.formTitle");
            gbProveedor.Text = t.Translate("GestionProveedor.gbProveedor");
            gbDatos.Text = t.Translate("GestionProveedor.gbDatos");
            groupBox1.Text = t.Translate("GestionProveedor.gbModoOperacion");
            label1.Text = t.Translate("GestionProveedor.labelEmail");
            label2.Text = t.Translate("GestionProveedor.labelRazonSocial");
            label3.Text = t.Translate("GestionProveedor.labelTelefono");
            label4.Text = t.Translate("GestionProveedor.labelCuit");
            label5.Text = t.Translate("GestionProveedor.labelDireccion");
            label6.Text = t.Translate("GestionProveedor.labelCvu");
            rbPreRegistrar.Text = t.Translate("GestionProveedor.rbPreRegistrar");
            rbRegistrar.Text = t.Translate("GestionProveedor.rbRegistrar");
            btnAgregar.Text = t.Translate("GestionProveedor.btnAgregar");
            btnModificar.Text = t.Translate("GestionProveedor.btnModificar");
            btnActDesact.Text = t.Translate("GestionProveedor.btnActDesact");
            btnGuardar.Text = t.Translate("GestionProveedor.btnGuardar");
            btnCancelar.Text = t.Translate("GestionProveedor.btnCancelar");

            TraducirColumnasProveedor(t);
        }

        private void TraducirColumnasProveedor(IdiomaManager idioma)
        {
            if (dgvProveedor.Columns["RazonSocial"] != null) dgvProveedor.Columns["RazonSocial"].HeaderText = idioma.Translate("Proveedor.colRazonSocial");
            if (dgvProveedor.Columns["Cuit"] != null) dgvProveedor.Columns["Cuit"].HeaderText = idioma.Translate("Proveedor.colCuit");
            if (dgvProveedor.Columns["Email"] != null) dgvProveedor.Columns["Email"].HeaderText = idioma.Translate("Proveedor.colEmail");
            if (dgvProveedor.Columns["Telefono"] != null) dgvProveedor.Columns["Telefono"].HeaderText = idioma.Translate("Proveedor.colTelefono");
            if (dgvProveedor.Columns["Direccion"] != null) dgvProveedor.Columns["Direccion"].HeaderText = idioma.Translate("Proveedor.colDireccion");
            if (dgvProveedor.Columns["CVU"] != null) dgvProveedor.Columns["CVU"].HeaderText = idioma.Translate("Proveedor.colCvu");
            if (dgvProveedor.Columns["Estado"] != null) dgvProveedor.Columns["Estado"].HeaderText = idioma.Translate("Proveedor.colEstado");
            if (dgvProveedor.Columns["Activo"] != null) dgvProveedor.Columns["Activo"].HeaderText = idioma.Translate("Proveedor.colActivo");
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            btnGuardar.Enabled = false;
            btnCancelar.Enabled = false;

            btnAgregar.Enabled = true;
            btnModificar.Enabled = true;
            btnActDesact.Enabled = true;

            gbDatos.Visible = false;

            txtCuit.Enabled = true;
            txtDireccion.Enabled = true;
            txtCVU.Enabled = true;
            txtRazonSocial.Enabled = true;
            txtTelefono.Enabled = true;
            txtEmail.Enabled = true;

            txtCuit.Text = "";
            txtDireccion.Text = "";
            txtCVU.Text = "";
            txtRazonSocial.Text = "";
            txtTelefono.Text = "";
            txtEmail.Text = "";
        }
    }
}