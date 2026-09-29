using BE;
using BLL;
using Services.Modelos.Idioma;
using Services_530BA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Servicios
{
    public partial class GestionClientes : Form, IIdiomaObserver
    {
        BLLCliente530BA bllCliente = new BLLCliente530BA();
        List<Cliente530BA> listClientes = new List<Cliente530BA>();
        public GestionClientes()
        {
            InitializeComponent();
            cargarGrilla();

            ServiceSessionManager530BA.getIntancia().Idioma.Suscribir(this);
            actualizarIdioma();

        }

        private void cargarGrilla()
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            dgvClientes.AutoGenerateColumns = false;
            dgvClientes.Columns.Clear();


            //para que no muestre el id y el dvh en la grilla, solo los datos visibles para el usuario
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("GestionClientes.colNombreCompleto"), DataPropertyName = "NombreCompleto" });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("GestionClientes.colDni"), DataPropertyName = "DNI" });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("GestionClientes.colEmail"), DataPropertyName = "Email" });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("GestionClientes.colCodPostal"), DataPropertyName = "CodPostal" });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("GestionClientes.colLocalidad"), DataPropertyName = "Localidad" });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = t.Translate("GestionClientes.colDireccion"), DataPropertyName = "Direccion" });

            listClientes = bllCliente.ObtenerTodos();

            dgvClientes.DataSource = null;
            dgvClientes.DataSource = listClientes;
        }

        public void actualizarIdioma()
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            this.Text = t.Translate("GestionClientes.formTitle");
            groupBox1.Text = t.Translate("GestionClientes.groupBoxDatosPersonales");
            groupBox2.Text = t.Translate("GestionClientes.groupBoxDatosEntrega");
            groupBox3.Text = t.Translate("GestionClientes.groupBoxClientes");
            label1.Text = t.Translate("GestionClientes.labelDni");
            label2.Text = t.Translate("GestionClientes.labelNombre");
            label3.Text = t.Translate("GestionClientes.labelEmail");
            label4.Text = t.Translate("GestionClientes.labelCodPostal");
            label5.Text = t.Translate("GestionClientes.labelDireccion");
            label6.Text = t.Translate("GestionClientes.labelLocalidad");
            btnCrear.Text = t.Translate("GestionClientes.btnCrear");
            btnModificar.Text = t.Translate("GestionClientes.btnModificar");
            btnEliminar.Text = t.Translate("GestionClientes.btnEliminar");
            btnGuardar.Text = t.Translate("GestionClientes.btnGuardar");
            btnCancelar.Text = t.Translate("GestionClientes.btnCancelar");
            btnSerializar.Text = t.Translate("GestionClientes.btnSerializar");
            btnDeserializar.Text = t.Translate("GestionClientes.btnDeserializar");
            btnLimpiar.Text = t.Translate("GestionClientes.btnLimpiar");
        }


        //un enum para que el boton guardar sepa que hacer
        private enum ModoOperacion
        {
            Ninguno,
            Crear,
            Modificar,
            Eliminar
        }

        private ModoOperacion modoActual = ModoOperacion.Ninguno;

        private void btnCrear_Click(object sender, EventArgs e)
        {
            modoActual = ModoOperacion.Crear;

            //habilitar los campos de texto para crear un nuevo cliente
            groupBox1.Visible = true;
            groupBox2.Visible = true;

            //habilitar boton guardar y cancelar y deshabilitar los demas botones
            btnGuardar.Enabled = true;
            btnCancelar.Enabled = true;
            btnEliminar.Enabled = false;
            btnModificar.Enabled = false;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;

            switch (modoActual)
            {
                case ModoOperacion.Crear:
                    try
                    {
                        string nombre = txtNombre.Text;
                        string dni = txtDNI.Text;
                        string email = txtEmail.Text;
                        int codPostal = Convert.ToInt32(txtCodPostal.Text);
                        string localidad = txtLocalidad.Text;
                        string direccion = txtDireccion.Text;

                        bllCliente.crearCliente(nombre, dni, email, codPostal, localidad, direccion);

                        MessageBox.Show(t.Translate("GestionClientes.msgClienteCreado"));
                        cargarGrilla();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(t.Translate(ex.Message));
                    }
                    break;

                case ModoOperacion.Eliminar:
                    try
                    {
                        if (dgvClientes.SelectedRows.Count > 0)
                        {
                            Cliente530BA clienteSeleccionado = (Cliente530BA)dgvClientes.SelectedRows[0].DataBoundItem;
                            bllCliente.eliminarCliente(clienteSeleccionado.DNI);
                            MessageBox.Show(t.Translate("GestionClientes.msgClienteEliminado"));
                            cargarGrilla();
                        }
                        else
                        {
                            MessageBox.Show(t.Translate("GestionClientes.msgSeleccionarClienteEliminar"));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(t.Translate(ex.Message));
                    }
                    break;

                case ModoOperacion.Modificar:
                    try
                    {
                        if (dgvClientes.SelectedRows.Count > 0)
                        {
                            Cliente530BA clienteSeleccionado = (Cliente530BA)dgvClientes.SelectedRows[0].DataBoundItem;

                            string email = txtEmail.Text;
                            int codPostal = Convert.ToInt32(txtCodPostal.Text);
                            string localidad = txtLocalidad.Text;
                            string direccion = txtDireccion.Text;

                            bllCliente.modificarCliente(clienteSeleccionado.DNI, email, codPostal, localidad, direccion);

                            MessageBox.Show(t.Translate("GestionClientes.msgClienteModificado"));
                            cargarGrilla();
                        }
                        else
                        {
                            MessageBox.Show(t.Translate("GestionClientes.msgSeleccionarClienteModificar"));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(t.Translate(ex.Message));
                    }
                    break;
            }

            //restaurar botones y txtbox a su estado original
            btnCrear.Enabled = true;
            btnEliminar.Enabled = true;
            btnModificar.Enabled = true;

            btnGuardar.Enabled = false;
            btnCancelar.Enabled = false;
            groupBox1.Visible = false;
            groupBox2.Visible = false;

            txtNombre.Text = null;
            txtDNI.Text = null;
            txtEmail.Text = null;
            txtCodPostal.Text = null;
            txtLocalidad.Text = null;
            txtDireccion.Text = null;

            modoActual = ModoOperacion.Ninguno;
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count > 0)
            {
                Cliente530BA clienteSeleccionado = (Cliente530BA)dgvClientes.SelectedRows[0].DataBoundItem;

                modoActual = ModoOperacion.Modificar;
                groupBox1.Visible = true;
                groupBox2.Visible = true;
                btnGuardar.Enabled = true;
                btnCancelar.Enabled = true;
                btnCrear.Enabled = false;
                btnModificar.Enabled = false;
                btnEliminar.Enabled = false;

                //completar textbox con los datos del cliente seleccionado
                txtNombre.Text = clienteSeleccionado.NombreCompleto;
                txtDNI.Text = clienteSeleccionado.DNI;
                txtEmail.Text = clienteSeleccionado.Email;
                txtCodPostal.Text = clienteSeleccionado.CodPostal.ToString();
                txtLocalidad.Text = clienteSeleccionado.Localidad;
                txtDireccion.Text = clienteSeleccionado.Direccion;

                //solo los modificables se habilitan, el nombre y el dni no se pueden modificar
                txtNombre.Enabled = false;
                txtDNI.Enabled = false;

            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count > 0)
            {
                Cliente530BA clienteSeleccionado = (Cliente530BA)dgvClientes.SelectedRows[0].DataBoundItem;

                modoActual = ModoOperacion.Eliminar;

                groupBox1.Visible = true;
                groupBox2.Visible = true;
                btnGuardar.Enabled = true;
                btnCancelar.Enabled = true;
                btnCrear.Enabled = false;
                btnModificar.Enabled = false;
                btnEliminar.Enabled = false;

            }
            else
            {
                var t = ServiceSessionManager530BA.getIntancia().Idioma;
                MessageBox.Show(t.Translate("GestionClientes.msgSeleccionarClienteEliminar"));
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            btnCrear.Enabled = true;
            btnEliminar.Enabled = true;
            btnModificar.Enabled = true;

            btnGuardar.Enabled = false;
            btnCancelar.Enabled = false;
            groupBox1.Visible = false;
            groupBox2.Visible = false;

            txtNombre.Text = null;
            txtDNI.Text = null;
            txtEmail.Text = null;
            txtCodPostal.Text = null;
            txtLocalidad.Text = null;
            txtDireccion.Text = null;

            modoActual = ModoOperacion.Ninguno;
        }

        private void btnSerializar_Click(object sender, EventArgs e)
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;
            try
            {
                if (dgvClientes.SelectedRows.Count == 0)
                {
                    MessageBox.Show(t.Translate("GestionClientes.msgSeleccionarClienteSerializar"), t.Translate("GestionClientes.msgAtencion"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<Cliente530BA> clientesASerializar = new List<Cliente530BA>();
                foreach (DataGridViewRow row in dgvClientes.SelectedRows)
                {
                    clientesASerializar.Add((Cliente530BA)row.DataBoundItem);
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Archivos XML (*.xml)|*.xml";
                saveFileDialog.Title = t.Translate("GestionClientes.titleGuardarXml");
                saveFileDialog.FileName = "clientes_seleccionados.xml"; // Nombre por defecto

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Cliente530BA>));

                    using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        serializer.Serialize(fs, clientesASerializar);
                    }

                    MessageBox.Show(t.Translate("GestionClientes.msgSerializacionExito"), t.Translate("GestionClientes.msgExito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(t.Translate("GestionClientes.msgSerializacionError") + t.Translate(ex.Message), t.Translate("GestionClientes.msgError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            cargarGrilla();
        }

        private void btnDeserializar_Click(object sender, EventArgs e)
        {
            var t = ServiceSessionManager530BA.getIntancia().Idioma;
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Archivos XML (*.xml)|*.xml";
                openFileDialog.Title = t.Translate("GestionClientes.titleAbrirXml");

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Cliente530BA>));
                    List<Cliente530BA> clientesRecuperados;

                    using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open))
                    {
                        clientesRecuperados = (List<Cliente530BA>)serializer.Deserialize(fs);
                    }

                    if (clientesRecuperados == null || clientesRecuperados.Count == 0)
                    {
                        MessageBox.Show(t.Translate("GestionClientes.msgDeserializacionVacia"), t.Translate("GestionClientes.msgAdvertencia"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    dgvClientes.DataSource = null;
                    dgvClientes.DataSource = clientesRecuperados;

                    MessageBox.Show(t.Translate("GestionClientes.msgDeserializacionExito"), t.Translate("GestionClientes.msgExito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (InvalidOperationException) // Excepción típica de XmlSerializer cuando el formato falla
            {
                MessageBox.Show(t.Translate("GestionClientes.msgDeserializacionErrorFormato"), t.Translate("GestionClientes.msgErrorFormato"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception)
            {
                // Manejo de otros errores (permisos de lectura, archivo en uso, etc.)
                MessageBox.Show(t.Translate("GestionClientes.msgDeserializacionErrorGeneral"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
