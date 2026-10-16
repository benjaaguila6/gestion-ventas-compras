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
        }

        private void cargarGrilla()
        {

            lista = bllProveedor.ObtenerTodos();

            dgvProveedor.DataSource = null;
            dgvProveedor.DataSource = lista;

            dgvProveedor.Columns["Id"].Visible = false;
            dgvProveedor.Columns["EstaRegistradoDefinitivamente"].Visible = false;

            dgvProveedor.Columns["RazonSocial"].HeaderText = "Razón Social";
            dgvProveedor.Columns["Cuit"].HeaderText = "CUIT";
            dgvProveedor.Columns["Email"].HeaderText = "Correo Electrónico";
            dgvProveedor.Columns["Telefono"].HeaderText = "Teléfono";
            dgvProveedor.Columns["Direccion"].HeaderText = "Dirección";
            dgvProveedor.Columns["CVU"].HeaderText = "CVU";
            dgvProveedor.Columns["Estado"].HeaderText = "Fase de Registro";
            dgvProveedor.Columns["Activo"].HeaderText = "Activo";

            dgvProveedor.Columns["RazonSocial"].DisplayIndex = 0;
            dgvProveedor.Columns["Cuit"].DisplayIndex = 5;
            dgvProveedor.Columns["Email"].DisplayIndex = 1;
            dgvProveedor.Columns["Telefono"].DisplayIndex = 2;
            dgvProveedor.Columns["Direccion"].DisplayIndex = 6;
            dgvProveedor.Columns["CVU"].DisplayIndex = 7;
            dgvProveedor.Columns["Estado"].DisplayIndex = 3;
            dgvProveedor.Columns["Activo"].DisplayIndex = 4;
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
            
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            gbDatos.Visible = true;
            ModoActual = ModoOperacion.Modificar;

            btnAgregar.Enabled = false;
            btnModificar.Enabled = false;
            btnActDesact.Enabled = false;
            btnGuardar.Enabled = true;
            btnCancelar.Enabled = true;

            txtEmail.Enabled = false;
            txtRazonSocial.Enabled = false;
            txtCuit.Enabled = false;
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
                MessageBox.Show($"Error: " + ex.Message);
            }
        }

        public void actualizarIdioma()
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            this.Text = t.Translate("GestionProveedor.formTitle");
            gbProveedor.Text = t.Translate("GestionProveedor.gbProveedor");
            gbDatos.Text = t.Translate("GestionProveedor.gbDatos");
            label1.Text = t.Translate("GestionProveedor.labelEmail");
            label2.Text = t.Translate("GestionProveedor.labelRazonSocial");
            label3.Text = t.Translate("GestionProveedor.labelTelefono");
            label4.Text = t.Translate("GestionProveedor.labelCuit");
            label5.Text = t.Translate("GestionProveedor.labelDireccion");
            label6.Text = t.Translate("GestionProveedor.labelCvu");
            btnAgregar.Text = t.Translate("GestionProveedor.btnAgregar");
            btnModificar.Text = t.Translate("GestionProveedor.btnModificar");
            btnActDesact.Text = t.Translate("GestionProveedor.btnActDesact");
            btnGuardar.Text = t.Translate("GestionProveedor.btnGuardar");
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            btnGuardar.Enabled = false;
            btnCancelar.Enabled = false;

            btnAgregar.Enabled = true;
            btnModificar.Enabled = true;
            btnActDesact.Enabled = true;

            gbDatos.Visible = false;   
        }
    }
}