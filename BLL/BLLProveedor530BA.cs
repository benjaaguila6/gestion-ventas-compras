using BE;
using BE.Enum;
using DAL.Negocio;
using Services;
using Services_530BA;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BLL
{
    public class BLLProveedor530BA
    {
        DALProveedor dal = new DALProveedor();
        BitacoraEventosService bit = new BitacoraEventosService();

        public List<Proveedor530BA> ObtenerTodos()
        {
            List<Proveedor530BA> proveedores = new List<Proveedor530BA>();

            DataTable dt = dal.ObtenerTodos();

            foreach(DataRow dr in dt.Rows)
            {
                proveedores.Add(MapearProveedor(dr));
            }

            return proveedores;
        }

        public void PreRegistrar(string razonsocial, string email, string telefono)
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;

            if (string.IsNullOrWhiteSpace(razonsocial))
            {
                throw new Exception(idioma.Translate("ExcProveedorRazonSocialObligatoria"));
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new Exception(idioma.Translate("ExcProveedorEmailFormatoInvalido"));
            }

            if (string.IsNullOrWhiteSpace(telefono))
            {
                throw new Exception(idioma.Translate("ExcTelefonoObligatorio"));
            }

            if (dal.ObtenerPorEmail(email) != null)
            {
                throw new Exception(idioma.Translate("ExcProveedorEmailExistente"));
            }

            
            Proveedor530BA proveedor = new Proveedor530BA
            {
                RazonSocial = razonsocial,
                Email = email,
                Telefono = telefono
            };

            dal.PreRegistrarProveedor(proveedor);

            long dvh = CalcularDVHProveedor(razonsocial, email, telefono, null, null, null);

            DigitoVerificador530BA.ActualizarDVVProveedor();

            string dniAutor = ServiceSessionManager530BA.getIntancia().usuarioActivo.DNI;
            bit.registrarEvento(dniAutor, "Se preregistro un proveedor", Criticidad530BA.Medio, Modulos530BA.Compras);
        }

        public void CompletarRegistro(Proveedor530BA proveedor, string cuit, string direccion, string cvu)
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;

            if (string.IsNullOrWhiteSpace(cuit))
            {
                throw new Exception(idioma.Translate("ExcProveedorCuitInvalido"));
            }
                

            if (string.IsNullOrWhiteSpace(cvu))
            {
                throw new Exception(idioma.Translate("ExcProveedorCvuInvalido"));
            }
                

            if (string.IsNullOrWhiteSpace(direccion))
            {
                throw new Exception(idioma.Translate("ExcProveedorDireccionObligatoria"));
            }

            var provExistente = MapearProveedor(dal.ObtenerPorId(proveedor.Id));

            if (provExistente.EstaRegistradoDefinitivamente)
            {
                throw new Exception(idioma.Translate("ExcProveedorYaRegistrado"));
            }

            // El DAL persiste estos datos desde la entidad: hay que volcarlos aca,
            // porque el proveedor viene de la grilla con esos campos en null.
            proveedor.Cuit = cuit;
            proveedor.Direccion = direccion;
            proveedor.CVU = cvu;

            // 3. Actualizar
            dal.RegistrarDefinitivo(proveedor);

            long dvh = CalcularDVHProveedor(provExistente.RazonSocial, provExistente.Email, provExistente.Telefono, cuit, direccion, cvu);

            DigitoVerificador530BA.ActualizarDVVProveedor();

            string dniAutor = ServiceSessionManager530BA.getIntancia().usuarioActivo.DNI;
            bit.registrarEvento(dniAutor, "Se completo el registro del proveedor: " + proveedor.Id, Criticidad530BA.Alto, Modulos530BA.Compras);

        }

        public void ModificarProveedor(int idProveedor, string telefono, string direccion, string cvu)
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;

            if (string.IsNullOrWhiteSpace(cvu))
            {
                throw new Exception(idioma.Translate("ExcProveedorCvuInvalido"));
            }

            if (string.IsNullOrWhiteSpace(direccion))
            {
                throw new Exception(idioma.Translate("ExcProveedorDireccionObligatoria"));
            }

            if (string.IsNullOrWhiteSpace(telefono))
            {
                throw new Exception(idioma.Translate("ExcTelefonoObligatorio"));
            }

            dal.ModificarProveedor(idProveedor, telefono, direccion, cvu);

            RecalcularDVHProveedor(idProveedor);

            string dniAutor = ServiceSessionManager530BA.getIntancia().usuarioActivo.DNI;
            bit.registrarEvento(dniAutor, "Se modifico el proveedor: " + idProveedor, Criticidad530BA.Medio, Modulos530BA.Compras);

        }

        public void ActivarDesactivar(int idProveedor)
        {
            dal.CambiarEstadoActivo(idProveedor);

            string dniAutor = ServiceSessionManager530BA.getIntancia().usuarioActivo.DNI;
            bit.registrarEvento(dniAutor, "Se cambio el estado a el proveedor: " + idProveedor, Criticidad530BA.Alto, Modulos530BA.Compras);
        }

        public Proveedor530BA ObtenerPorId(int idProveedor)
        {
            DataRow dr = dal.ObtenerPorId(idProveedor);

            if (dr == null)
            {
                return null;
            }

            return MapearProveedor(dr);
        }

        private long CalcularDVHProveedor(string razonSocial, string email, string telefono, string cuit, string direccion, string cvu)
        {
            string cadena = (razonSocial ?? "")
                          + (email ?? "")
                          + (telefono ?? "")
                          + (cuit ?? "")
                          + (direccion ?? "")
                          + (cvu ?? "");

            return DigitoVerificador530BA.CalcularDVH(cadena);
        }

        public void RecalcularDVHProveedor(int idProveedor)
        {
            DataRow row = dal.ObtenerPorId(idProveedor);
            if (row == null) return;

            string razonSocial = Convert.ToString(row["RazonSocial"]);
            string email = Convert.ToString(row["Email"]);
            string telefono = Convert.ToString(row["Telefono"]);
            string cuit = row["Cuit"] != DBNull.Value ? Convert.ToString(row["Cuit"]) : null;
            string direccion = row["Direccion"] != DBNull.Value ? Convert.ToString(row["Direccion"]) : null;
            string cvu = row["CVU"] != DBNull.Value ? Convert.ToString(row["CVU"]) : null;

            long nuevoDVH = CalcularDVHProveedor(razonSocial, email, telefono, cuit, direccion, cvu);

            dal.ActualizarDVH(idProveedor, nuevoDVH);
            Services.DigitoVerificador530BA.ActualizarDVVProveedor();
        }

        private Proveedor530BA MapearProveedor(DataRow row)
        {
            return new Proveedor530BA
            {
                Id = Convert.ToInt32(row["Id"]),
                Email = row["Email"].ToString(),
                RazonSocial = row["RazonSocial"].ToString(),
                Telefono = row["Telefono"].ToString(),
                Estado = row["Estado"].ToString(),
                // Manejo seguro de nulos desde SQL
                Cuit = row["Cuit"] != DBNull.Value ? row["Cuit"].ToString() : null,
                Direccion = row["Direccion"] != DBNull.Value ? row["Direccion"].ToString() : null,
                CVU = row["CVU"] != DBNull.Value ? row["CVU"].ToString() : null,

                Activo = row["Activo"] != DBNull.Value ? Convert.ToBoolean(row["Activo"]) : true
            };
        }
    }
}
