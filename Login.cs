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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Services;

namespace Servicios
{
    public partial class Login : Form, IIdiomaObserver
    {
        UsuarioService _userService = new UsuarioService();
        BLLIdioma530BA _idiomaService = new BLLIdioma530BA();
        

        public Login()
        {
            InitializeComponent();
            ServiceSessionManager530BA.getIntancia().Idioma.Suscribir(this);

        }

        

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            
            string username = txtUser.Text;
            string password = txtPassword.Text;
            try
            {
                bool usaPasswordDefault = _userService.login(username, password);

                int idiomaUsuario = ServiceSessionManager530BA.getIntancia().usuarioActivo.IdIdioma;
                string codIdiomaUsuario = idiomaUsuario == 1 ? "es" : "en";
                ServiceSessionManager530BA.getIntancia().Idioma.CargarIdioma(codIdiomaUsuario);

                bool usuarioOk = DigitoVerificador530BA.VerificarUsuario();
                bool rolOk = DigitoVerificador530BA.VerificarRol();
                bool familiaOk = DigitoVerificador530BA.VerificarFamilia();
                bool patenteOk = DigitoVerificador530BA.VerificarPatente();
                bool clienteOk = DigitoVerificador530BA.VerificarCliente();
                bool productoOk = DigitoVerificador530BA.VerificarProducto();


                if (!usuarioOk || !rolOk || !familiaOk || !patenteOk || !clienteOk || !productoOk)
                {
                    if (ServiceSessionManager530BA.getIntancia().usuarioActivo.Rol.Id != 1)
                    {
                        MessageBox.Show("Se encontraron inconsistencias en la base de datos, contactese con un administrador");
                        txtUser.Text = "";
                        txtPassword.Text = "";
                        ServiceSessionManager530BA.getIntancia().Logout();
                        return;

                    }
                    this.Hide();
                    RepararInconsistencias pantalla = new RepararInconsistencias(usuarioOk, rolOk, familiaOk, patenteOk, clienteOk, productoOk);
                    pantalla.FormClosed += (s, args) => RestaurarIdiomaLogin();
                    pantalla.Show();
                    return;
                }
                
                txtUser.Text = "";
                txtPassword.Text = "";
                this.Hide();

                if (usaPasswordDefault)
                {
                    CambiarContraseña form = new CambiarContraseña();
                    form.FormClosed += (s, args) => RestaurarIdiomaLogin();
                    form.Show();
                }
                else
                {
                    MenuPrincipal menu = new MenuPrincipal();
                    menu.FormClosed += (s, args) => RestaurarIdiomaLogin();
                    menu.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RestaurarIdiomaLogin()
        {

            this.Show();
        }

        public void actualizarIdioma()
        {

        }

        
    }
}
