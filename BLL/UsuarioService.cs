using BE;
using BE.Enum;
using DAL;
using Services;
using Services.Modelos;
//using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class UsuarioService
    {
        DALUsuario530BA dal = new DALUsuario530BA();
        BitacoraEventosService bit = new BitacoraEventosService();

        public List<UsuarioModelo530BA> obtenerTodos()
        {
            var dt = dal.obtenerTodos();
            List<UsuarioModelo530BA> lista = new List<UsuarioModelo530BA>();

            foreach (DataRow row in dt.Rows)
            {
                lista.Add(MapearUsuario(row));
            }

            return lista;
        }

        public bool login(string user, string password)
        {
            var idioma = Services_530BA.ServiceSessionManager530BA.getIntancia().Idioma;

            var usuario = MapearUsuario(dal.obtenerPorUser(user));

            //validaciones
            if (Services_530BA.ServiceSessionManager530BA.getIntancia().estaLogueado())
            {
                throw new Exception(idioma.Translate("ExcSesionActiva"));
            }

            if (usuario == null)
            {
                throw new Exception(idioma.Translate("ExcUsuarioNoExiste"));
            }

            if (usuario.Bloqueo == true)
            {
                throw new Exception(idioma.Translate("ExcUsuarioBloqueado"));
            }

            if (usuario.Activo == false)
            {
                throw new Exception(idioma.Translate("ExcUsuarioInactivo"));
            }

            if (usuario.UltimoIntentoFallido.HasValue)
            {
                TimeSpan tiempo = DateTime.Now - usuario.UltimoIntentoFallido.Value;

                // si pasaron más de 30 minutos se reinicia
                if (tiempo.TotalMinutes >= 30)
                {
                    dal.reiniciarIntentos(usuario.DNI);

                    usuario.Intentos = 0;
                }
            }

            string passwordHash = Services_530BA.ServiceSeguridad530BA.Hashear(password);

            if (usuario.Password != passwordHash)
            {
                dal.aumentarIntento(usuario.DNI);

                usuario.Intentos++;

                if (usuario.Intentos >= 4)
                {
                    dal.bloquearUsuario(usuario.DNI);
                    bit.registrarEvento(usuario.DNI, $"Usuario {usuario.User} bloqueado.", Criticidad530BA.Alto, Modulos530BA.Seguridad);

                    throw new Exception(idioma.Translate("ExcCuentaBloqueada"));
                }

                throw new Exception(string.Format(idioma.Translate("ExcPasswordIncorrecta"), usuario.Intentos));
            }
            BLLRol gestorRol = new BLLRol();

            List<RolModelo530BA> roles = gestorRol.ObtenerRolesConJerarquia();

            var rolConPermisos = roles.FirstOrDefault(r => r.Id == usuario.Rol.Id);

            if(rolConPermisos != null)
            {
                usuario.Rol = rolConPermisos;
            }

            //login ok
            Services_530BA.ServiceSessionManager530BA.getIntancia().Login(usuario);

            bit.registrarEvento(usuario.DNI, $"Realizo login exitoso.", Criticidad530BA.Medio, Modulos530BA.Usuario);

            dal.reiniciarIntentos(usuario.DNI);

            // verificamos si sigue usando password por defecto
            string passwordDefault = GenerarPassword(usuario.Apellido, usuario.DNI);

            string passwordDefaultHash = Services_530BA.ServiceSeguridad530BA.Hashear(passwordDefault);

            bool usaPasswordDefault =
                usuario.Password == passwordDefaultHash;

            return usaPasswordDefault;
        }

        public void CrearUsuario(string dni, string nombre, string apellido, string email, RolModelo530BA rol)
        {
            var idioma = Services_530BA.ServiceSessionManager530BA.getIntancia().Idioma;


            if (dal.obtenerPorDNI(dni) != null)
            {
                throw new Exception(idioma.Translate("ExcUsuarioDniExistente"));
            }

            string user = GenerarUsuario(nombre, dni);
            string password = GenerarPassword(apellido, dni);
            string passwordHash = Services_530BA.ServiceSeguridad530BA.Hashear(password);

            long dvh = CalcularDVHUsuario(
                dni,
                nombre,
                apellido,
                email,
                rol.Id,
                user,
                passwordHash
            );
            Services.DigitoVerificador530BA.ActualizarDVVUsuario();

            dal.InsertarUsuario(dni, nombre, apellido, email, rol.Id, user, passwordHash, dvh);

            string dniAutor = Services_530BA.ServiceSessionManager530BA.getIntancia().usuarioActivo.DNI;

            bit.registrarEvento(dniAutor, "Se creo un usuario nuevo", Criticidad530BA.Medio, Modulos530BA.Usuario);
        }

        public void activarDesactivar(string dni)
        {
            List<UsuarioModelo530BA> todosLosUsuarios = obtenerTodos();

            UsuarioModelo530BA usuario = todosLosUsuarios.FirstOrDefault(u => u.DNI == dni);

            string evento = "";

            if (usuario.Activo == true)
            {
                dal.DesactivarUsuario(dni);
                RecalcularDVHUsuario(dni);
                evento = $"Se desactivó la cuenta del usuario: {usuario.User}";
            }
            else
            {
                dal.ActivarUsuario(dni);
                RecalcularDVHUsuario(dni);
                evento = $"Se activó la cuenta del usuario: {usuario.User}";
            }

            string dniAutor = Services_530BA.ServiceSessionManager530BA.getIntancia().usuarioActivo.DNI;

            bit.registrarEvento(dniAutor, evento, Criticidad530BA.Alto, Modulos530BA.Usuario);
        }

        public void ModificarUsuario(string dni, string email, RolModelo530BA rol)
        {
            dal.ModificarUsuario(dni, email, rol.Id);
            RecalcularDVHUsuario(dni);

            string dniAutor = Services_530BA.ServiceSessionManager530BA.getIntancia().usuarioActivo.DNI;

            bit.registrarEvento(dniAutor, $"Se modificó usuario DNI {dni}", Criticidad530BA.Medio, Modulos530BA.Usuario);
        }

        public bool cambiarPassword(string passwordActual, string passwordNueva)
        {
            var idioma = Services_530BA.ServiceSessionManager530BA.getIntancia().Idioma;

            string passwordActualHash = Services_530BA.ServiceSeguridad530BA.Hashear(passwordActual);

            UsuarioModelo530BA usuarioActivo = Services_530BA.ServiceSessionManager530BA.getIntancia().usuarioActivo;

            if (passwordActualHash != usuarioActivo.Password)
            {
                throw new Exception(idioma.Translate("ExcPasswordActualIncorrecta"));
            }

            string passwordNuevaHash = Services_530BA.ServiceSeguridad530BA.Hashear(passwordNueva);

            if (passwordActualHash == passwordNuevaHash)
            {
                throw new Exception(idioma.Translate("ExcPasswordIgualAnterior"));
            }

            dal.CambiarPassword(passwordNuevaHash, usuarioActivo.DNI);
            RecalcularDVHUsuario(usuarioActivo.DNI);
            

            return true;

        }

        private UsuarioModelo530BA MapearUsuario(DataRow row)
        {
            if (row == null)
            {
                return null;
            }
            return new UsuarioModelo530BA
            {
                DNI = row["DNI"].ToString(),
                Nombre = row["Nombre"].ToString(),
                Apellido = row["Apellido"].ToString(),
                Email = row["Email"].ToString(),
                Rol = new RolModelo530BA { Id = Convert.ToInt32(row["IdRol"]), Nombre = row["NombreRol"].ToString() },
                User = row["Username"].ToString(),
                Password = row["PasswordHash"].ToString(),
                Intentos = Convert.ToInt32(row["Intentos"]),
                Bloqueo = Convert.ToBoolean(row["Bloqueo"]),
                Activo = Convert.ToBoolean(row["Activo"]),
                UltimoIntentoFallido = row["UltimoIntentoFallido"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["UltimoIntentoFallido"]),
                IdIdioma = Convert.ToInt32(row["IdIdioma"]),
                //DVH = row["DVH"] == DBNull.Value ? 0 : Convert.ToInt64(row["DVH"]),

            };
        }

        public void DesbloquearUsuario(string dni)
        {
            var idioma = Services_530BA.ServiceSessionManager530BA.getIntancia().Idioma;

            var row = dal.obtenerPorDNI(dni);
            var usuario = MapearUsuario(row);

            if (usuario == null)
                throw new Exception(idioma.Translate("ExcUsuarioNoEncontrado"));

            if (!usuario.Bloqueo)
                throw new Exception(idioma.Translate("ExcUsuarioNoBloqueado"));

            // password default
            string nuevaPass = GenerarPassword(usuario.Apellido, usuario.DNI);
            string nuevaPassHash = Services_530BA.ServiceSeguridad530BA.Hashear(nuevaPass);

            // desbloqueo
            dal.desbloquearUsuario(dni, nuevaPassHash);
            RecalcularDVHUsuario(dni);

            // bitácora
            string dniAutor = Services_530BA.ServiceSessionManager530BA.getIntancia().usuarioActivo.DNI;

            bit.registrarEvento(
                dniAutor,
                $"Se desbloqueó el usuario: {usuario.User}",
                Criticidad530BA.Alto,
                Modulos530BA.Usuario
            );
        }

        public void GuardarIdioma(int idIdioma)
        {
            string dni = Services_530BA.ServiceSessionManager530BA.getIntancia().usuarioActivo.DNI;
            dal.GuardarIdioma(dni, idIdioma);
            RecalcularDVHUsuario(dni);

            Services_530BA.ServiceSessionManager530BA.getIntancia().usuarioActivo.IdIdioma = idIdioma;
        }

        private long CalcularDVHUsuario(


            string dni,
            string nombre,
            string apellido,
            string email,
            int idRol,
            string user,
            string passwordHash)
            
        {
                    string cadena =
                    dni +
                    nombre +
                    apellido +
                    email +
                    idRol +
                    user +
                    passwordHash;

            return Services.DigitoVerificador530BA.CalcularDVH(cadena);
        }

        private void RecalcularDVHUsuario(string dni)
        {
            var row = dal.obtenerPorDNI(dni);

            UsuarioModelo530BA usuario = MapearUsuario(row);

            long nuevoDVH = CalcularDVHUsuario(
                usuario.DNI,
                usuario.Nombre,
                usuario.Apellido,
                usuario.Email,
                usuario.Rol.Id,
                usuario.User,
                usuario.Password
            );

            dal.ActualizarDVH(dni, nuevoDVH);
            Services.DigitoVerificador530BA.ActualizarDVVUsuario();

        }


        public void RepararDVH(string dni)
        {
            RecalcularDVHUsuario(dni);
        }

        #region Credenciales
        public string GenerarUsuario(string nombre, string dni)
        {
            return nombre.Trim().ToLower() + dni;
        }

        public string GenerarPassword(string apellido, string dni)
        {
            return apellido.Trim().ToLower() + dni;
        }
        #endregion Credenciales
    }
}
