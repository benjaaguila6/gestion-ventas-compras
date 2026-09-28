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
using Services;

namespace Servicios
{
    public partial class RepararInconsistencias : Form, IIdiomaObserver
    {
        private bool usuarioOk, rolOk, familiaOk, patenteOk, clienteOk, productoOk;

        private void btnRecalcular_Click(object sender, EventArgs e)
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;

            try
            {
                if (!usuarioOk) DigitoVerificador530BA.RepararUsuario();
                if (!rolOk) DigitoVerificador530BA.RepararRol();
                if (!familiaOk) DigitoVerificador530BA.RepararFamilia();
                if (!patenteOk) DigitoVerificador530BA.RepararPatente();
                if (!clienteOk) DigitoVerificador530BA.RepararCliente();
                if (!productoOk) DigitoVerificador530BA.RepararProducto();

                MessageBox.Show(idioma.Translate("MsgReparacionExitosa"));

                ServiceSessionManager530BA.getIntancia().Logout();

                this.Hide();
                Login login = new Login();
                login.FormClosed += (s, args) => this.Close();
                login.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(idioma.Translate("MsgErrorReparar") + ex.Message);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = idioma.Translate("FiltroBackup");
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        DigitoVerificador530BA.RealizarRestore(ofd.FileName);

                        MessageBox.Show(idioma.Translate("MsgRestoreExitoso"));
                        Application.Exit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(idioma.Translate("MsgErrorRestore") + ex.Message);
                    }
                }
            }
        }

        public RepararInconsistencias(bool usuarioOk, bool rolOk, bool familiaOk, bool patenteOk, bool clienteOk, bool productoOk)
        {
            InitializeComponent();

            this.usuarioOk = usuarioOk;
            this.rolOk = rolOk;
            this.familiaOk = familiaOk;
            this.patenteOk = patenteOk;
            this.clienteOk = clienteOk;
            this.productoOk = productoOk;

            ServiceSessionManager530BA.getIntancia().Idioma.Suscribir(this);

            actualizarIdioma();

            MostrarTablasConError();
        }

        public void actualizarIdioma()
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;

            this.Text = idioma.Translate("TituloRepararInconsistencias");
            btnRecalcular.Text = idioma.Translate("BtnRecalcular");
            btnRestore.Text = idioma.Translate("BtnRestore");
            btnSalir.Text = idioma.Translate("BtnSalir");

            MostrarTablasConError();
        }

        private void MostrarTablasConError()
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;

            string mensaje = idioma.Translate("MensajeInconsistenciasDetectadas") + "\n";

            if (!usuarioOk) mensaje += idioma.Translate("RepararInconsistencias.itemUsuario");
            if (!rolOk) mensaje += idioma.Translate("RepararInconsistencias.itemRol");
            if (!familiaOk) mensaje += idioma.Translate("RepararInconsistencias.itemFamilia");
            if (!patenteOk) mensaje += idioma.Translate("RepararInconsistencias.itemPatente");
            if (!clienteOk) mensaje += idioma.Translate("RepararInconsistencias.itemCliente");
            if (!productoOk) mensaje += idioma.Translate("RepararInconsistencias.itemProducto");

            lblMensaje.Text = mensaje;
        }


        
    }
}
