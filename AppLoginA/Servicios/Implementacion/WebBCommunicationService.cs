using AppLoginA.Models;
using AppLoginA.Servicios.Contrato;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppLoginA.Servicios.Implementacion
{
    public class WebBCommunicationService
    {
        private readonly HttpClient _httpClient;
        private readonly IUsuarioService _usuarioServicio;

        public WebBCommunicationService(HttpClient httpClient, IUsuarioService usuarioServicio)
        {
            _usuarioServicio = usuarioServicio;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<string> GetInfoFromWebB(string tokenJWT)
        {
            // Verifica que se haya proporcionado un token JWT
            if (string.IsNullOrWhiteSpace(tokenJWT))
            {
                throw new ArgumentException("Token JWT no válido", nameof(tokenJWT));
            }

            try
            {
                // Decodifica el token JWT para obtener los claims
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(tokenJWT);
                // Obtiene la fecha de expiración del token
                var expirationDate = token.ValidTo;

                // Verifica si el token ha expirado
                if (expirationDate < DateTime.UtcNow)
                {
                    // Si el token ha expirado, devuelve un mensaje de error
                    return "El token ha expirado";
                }

                // Obtiene el correo y la contraseña del token
                var correo = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                

                // Verifica la validez del correo y la contraseña, puedes usar tu lógica de validación aquí
                // Por ejemplo, puedes llamar al servicio de usuario para verificar las credenciale
                    Usuario usuario_encontrado = await _usuarioServicio.GetUsuarioTk(correo);
                    if (usuario_encontrado == null)
                    {
                        return "error";
                    }

                return "Información obtenida correctamente";

            }
            catch (Exception ex)
            {
                // Maneja cualquier excepción que pueda ocurrir durante la verificación del token
                throw new Exception("Error al verificar el token JWT", ex);
            }
        }
    }
}