using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services_530BA
{
    public class ServiceSeguridad530BA
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("ClaveSecretaParaSistema530BA2026");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("VectorInicial530");

        public static string Hashear(string input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }
        public static string EncriptarAES(string textoPlano)
        {
            if (string.IsNullOrEmpty(textoPlano)) return textoPlano;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(textoPlano);
                    }
                    // Retorna el texto cifrado convertido a Base64 (listo para la BD)
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string DesencriptarAES(string textoCifrado)
        {
            if (string.IsNullOrEmpty(textoCifrado)) return textoCifrado;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // Convertimos el texto cifrado de la BD (Base64) de vuelta a bytes
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(textoCifrado)))
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd(); // Retorna el email original
                }
            }
        }
    }
}
