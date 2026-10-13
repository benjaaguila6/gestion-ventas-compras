using DAL;
using Services_530BA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BackUpRestore530BA
    {
        DALBackUpRestore530BA _dal = new DALBackUpRestore530BA();

        private static string Traducir(string key)
        {
            return ServiceSessionManager530BA.getIntancia().Idioma.Translate(key);
        }

        public void realizarBackUp(string ruta)
        {
            if (!Directory.Exists(ruta))
            {
                throw new Exception(Traducir("Services.msgDirectorioNoExiste"));
            }

            string nombreArchivo = $"Backup_Sistema_{DateTime.Now:yyyyMMdd_HHmm}.bak";
            string rutaCompleta = Path.Combine(ruta, nombreArchivo);

            _dal.realizarBackUp(rutaCompleta);
        }

        public void realizarRestore(string ruta)
        {
            if (!File.Exists(ruta))
            {
                throw new Exception(Traducir("Services.msgArchivoBackupNoExiste"));
            }


            if (Path.GetExtension(ruta).ToLower() != ".bak")
            {
                throw new Exception(Traducir("Services.msgFormatoBackupInvalido"));
            }
                

            _dal.realizarRestore(ruta);
        }
    }
}
