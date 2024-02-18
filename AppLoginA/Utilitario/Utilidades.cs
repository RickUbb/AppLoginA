using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AppLoginA.Utilitario
{
    public class Utilidades
    {

        public static string EncriptarClave(string clave)
        {

            StringBuilder sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;

                byte[] result = hash.ComputeHash(enc.GetBytes(clave));

                foreach (byte b in result)
                    sb.Append(b.ToString("x2"));
            }

            return sb.ToString();

        }

        public static bool EsCorreoValido(string correo)
        {
            if (string.IsNullOrEmpty(correo))
                return false;

            // Utiliza una expresión regular para validar el formato del correo
            // Puedes ajustar esta expresión regular según tus necesidades
            string patron = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            return Regex.IsMatch(correo, patron);
        }

        public static bool EsPasswordValido(string password)
        {
            // Aquí puedes agregar tus propias reglas de validación para la contraseña
            // Por ejemplo, longitud mínima, mayúsculas, minúsculas, caracteres especiales, etc.
            return !string.IsNullOrEmpty(password) && password.Length >= 8;
        }

    }
}
