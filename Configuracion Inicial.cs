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
    public partial class Configuracion_Inicial : Form
    {
        public Configuracion_Inicial()
        {
            InitializeComponent();

            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            this.Text = t.Translate("ConfiguracionInicial.formTitle");
            label1.Text = t.Translate("ConfiguracionInicial.labelTitulo");
            label2.Text = t.Translate("ConfiguracionInicial.labelDescripcion");
            label3.Text = t.Translate("ConfiguracionInicial.labelInstancia");
            button1.Text = t.Translate("ConfiguracionInicial.btnConectar");
            button2.Text = t.Translate("ConfiguracionInicial.btnContinuar");
            button3.Text = t.Translate("ConfiguracionInicial.btnCancelar");
        }
    }
}
