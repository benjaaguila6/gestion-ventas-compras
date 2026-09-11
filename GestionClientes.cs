using BE;
using BLL;
using Services.Modelos.Idioma;
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
    public partial class GestionClientes : Form, IIdiomaObserver
    {
        BLLCliente530BA bllCliente = new BLLCliente530BA();
        List<Cliente530BA> listClientes = new List<Cliente530BA>();
        public GestionClientes()
        {
            InitializeComponent();
            cargarGrilla();
        }

        private void cargarGrilla()
        {
            dgvClientes.AutoGenerateColumns = false;
            dgvClientes.Columns.Clear();


            //para que no muestre el id y el dvh en la grilla, solo los datos visibles para el usuario
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nombre Completo", DataPropertyName = "NombreCompleto" });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "DNI", DataPropertyName = "DNI" });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Email", DataPropertyName = "Email" });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Código Postal", DataPropertyName = "CodPostal" });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Localidad", DataPropertyName = "Localidad" });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Dirección", DataPropertyName = "Direccion" });

            listClientes = bllCliente.ObtenerTodos();

            dgvClientes.DataSource = null;
            dgvClientes.DataSource = listClientes;
        }

        public void actualizarIdioma()
        {
            throw new NotImplementedException();
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

                        MessageBox.Show("Cliente creado correctamente"); // despues traducir
                        cargarGrilla();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message); // traducir
                    }
                    break;

                case ModoOperacion.Eliminar:
                    try
                    {
                        if (dgvClientes.SelectedRows.Count > 0)
                        {
                            Cliente530BA clienteSeleccionado = (Cliente530BA)dgvClientes.SelectedRows[0].DataBoundItem;
                            bllCliente.eliminarCliente(clienteSeleccionado.DNI);
                            MessageBox.Show("Cliente eliminado correctamente"); // despues traducir
                            cargarGrilla();
                        }
                        else
                        {
                            MessageBox.Show("Seleccione un cliente para eliminar"); // despues traducir
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message); // traudcir
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

                            MessageBox.Show("Cliente modificado correctamente"); // despues traducir
                            cargarGrilla();
                        }
                        else
                        {
                            MessageBox.Show("Seleccione un cliente para modificar"); // despues traducir
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message); // traducir
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
                MessageBox.Show("Seleccione un cliente para eliminar"); // despues traducir
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
    }
}
