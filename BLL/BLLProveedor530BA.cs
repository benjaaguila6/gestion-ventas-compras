using BE;
using DAL.Negocio;
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
        }

        public void CompletarRegistro(Proveedor530BA proveedor, string cuit, string direccion, string cvu)
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;

            if (string.IsNullOrWhiteSpace(proveedor.Cuit) || proveedor.Cuit.Length != 11)
            {
                throw new Exception(idioma.Translate("ExcProveedorCuitInvalido"));
            }
                

            if (string.IsNullOrWhiteSpace(proveedor.CVU) || proveedor.CVU.Length != 22)
            {
                throw new Exception(idioma.Translate("ExcProveedorCvuInvalido"));
            }
                

            if (string.IsNullOrWhiteSpace(proveedor.Direccion))
            {
                throw new Exception(idioma.Translate("ExcProveedorDireccionObligatoria"));
            }

            var provExistente = MapearProveedor(dal.ObtenerPorId(proveedor.Id));

            if (provExistente.EstaRegistradoDefinitivamente)
            {
                throw new Exception(idioma.Translate("ExcProveedorYaRegistrado"));
            }

            // 3. Actualizar
            dal.RegistrarDefinitivo(proveedor);
        }

        public void ModificarProveedor(int idProveedor, string telefono, string direccion, string cvu)
        {
            var idioma = ServiceSessionManager530BA.getIntancia().Idioma;

            if (string.IsNullOrWhiteSpace(cvu) || cvu.Length != 22)
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
        }

        public void ActivarDesactivar(int idProveedor)
        {
            dal.CambiarEstadoActivo(idProveedor);
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
