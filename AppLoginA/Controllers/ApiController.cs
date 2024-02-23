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

        public ApiController(IUsuarioService usuarioServicio)
        {
            _usuarioServicio = usuarioServicio;
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
                // Crear Token
                return Ok("User Login Successful");
            }
            else
            {
                // Si las credenciales no son válidas, retornar un mensaje de error
                return BadRequest("Invalid credentials");
            }
        }
    }
}
