using AppLoginA.Models;
using AppLoginA.Servicios.Contrato;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AppLoginA.Utilitario; // Asegúrate de importar el espacio de nombres de tus utilidades

namespace AppLoginA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IUsuarioService _usuarioServicio;
        private readonly ITokenService _tokenService; // Agrega la inyección de dependencia para ITokenService

        public ApiController(IUsuarioService usuarioServicio, ITokenService tokenService)
        {
            _usuarioServicio = usuarioServicio;
            _tokenService = tokenService; // Inyecta ITokenService en el constructor
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(Usuario userLogin)
        {
            // Encripta la contraseña proporcionada por el usuario
            string claveEncriptada = Utilidades.EncriptarClave(userLogin.Password);

            // Ahora puedes validar las credenciales en la base de datos
            var usuarioValido = await _usuarioServicio.GetUsuario(userLogin.Correo, claveEncriptada);

            if (usuarioValido != null)
            {
                // Si las credenciales son válidas, retornar un mensaje de éxito
                var token = _tokenService.GenerateToken(usuarioValido); // Utiliza ITokenService para generar el token
                return Ok(token);
            }
            else
            {
                // Si las credenciales no son válidas, retornar un mensaje de error
                return BadRequest("Invalid credentials");
            }
        }
    }
}
