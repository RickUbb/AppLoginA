using Microsoft.AspNetCore.Authentication.Cookies; // Importa el espacio de nombres para la autenticación basada en cookies.
using Microsoft.AspNetCore.Authentication; // Importa el espacio de nombres para la autenticación.
using Microsoft.AspNetCore.Mvc; // Importa el espacio de nombres para los controladores MVC.
using System.Security.Claims; // Importa el espacio de nombres para las reclamaciones de identidad.
using AppLoginA.Servicios.Contrato; // Importa el espacio de nombres para los contratos de servicios.
using AppLoginA.Models; // Importa el espacio de nombres para los modelos de la aplicación.
using AppLoginA.Utilitario; // Importa el espacio de nombres para las utilidades de la aplicación.

namespace AppLoginA.Controllers // Define el espacio de nombres y comienza la declaración del controlador.
{
    public class LoginController : Controller // Define la clase LoginController como un controlador MVC.
    {

        private readonly IUsuarioService _usuarioServicio; // Define una instancia de la interfaz IUsuarioService para acceder al servicio de usuarios.

        public LoginController(IUsuarioService usuarioServicio) // Constructor de la clase LoginController que recibe una instancia de IUsuarioService.
        {
            _usuarioServicio = usuarioServicio; // Asigna la instancia del servicio de usuarios al campo privado _usuarioServicio.
        }

        public IActionResult Registrarse() // Método de acción para mostrar la vista de registro.
        {
            return View(); // Retorna la vista de registro.
        }

        public IActionResult IniciarSesion()
        {
            return View(); // Retorna la vista de login.
        }


        [HttpPost] // Atributo que indica que este método responde a las solicitudes POST.
        public async Task<IActionResult> Registrarse(Usuario usuario)
        {
            try
            {
                // Validar el correo electrónico y la contraseña
                if (!Utilidades.EsCorreoValido(usuario.Correo) || !Utilidades.EsPasswordValido(usuario.Password))
                {
                    ViewData["Mensaje"] = "Correo inválido o contraseña inválida.";
                    return View("Registrarse", usuario);
                }

                // Asignar el rol adecuado para el usuario
                usuario.idRol = 1; // Por ejemplo, asignamos el rol de 1 para Paciente

                // Encriptar la contraseña
                usuario.Password = Utilidades.EncriptarClave(usuario.Password);

                // Guardar el usuario en la base de datos
                Usuario usuario_creado = await _usuarioServicio.SaveUsuario(usuario);

                // Verificar si el usuario se creó correctamente
                if (usuario_creado.idUsuario > 0)
                    return RedirectToAction("IniciarSesion"); // Redireccionar al usuario a la página de inicio de sesión

                ViewData["Mensaje"] = "No se pudo crear el usuario";
                return View("Registrarse", usuario);
            }
            catch (Exception ex)
            {
                ViewData["Mensaje"] = "Error al registrar el usuario: " + ex.Message;
                return View("Registrarse", usuario);
            }
        }

        [HttpPost] // Atributo que indica que este método responde a las solicitudes POST.
        public async Task<IActionResult> IniciarSesion(string correo, string clave) // Método de acción para procesar el inicio de sesión del usuario.
        {
            string claveEncriptada = Utilidades.EncriptarClave(clave); // Encripta la contraseña proporcionada por el usuario.

            Usuario usuario_encontrado = await _usuarioServicio.GetUsuario(correo, claveEncriptada); // Busca al usuario en la base de datos.

            if (usuario_encontrado == null) // Si no se encuentra al usuario.
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias"; // Establece un mensaje de error en la vista.
                return View(); // Retorna la vista de inicio de sesión.
            }

            List<Claim> claims = new List<Claim>() { // Crea una lista de reclamaciones para la identidad del usuario.
                new Claim(ClaimTypes.Name, usuario_encontrado.Correo) // Agrega la reclamación del nombre de usuario.
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); // Crea una identidad de reclamaciones para el usuario.
            AuthenticationProperties properties = new AuthenticationProperties() // Crea propiedades de autenticación.
            {
                AllowRefresh = true // Permite actualizar la sesión de autenticación.
            };

            await HttpContext.SignInAsync( // Inicia sesión del usuario.
                CookieAuthenticationDefaults.AuthenticationScheme, // Esquema de autenticación basado en cookies.
                new ClaimsPrincipal(claimsIdentity), // Principal de reclamaciones del usuario.
                properties // Propiedades de autenticación.
                );

            return RedirectToAction("Index", "Home"); // Redirige al usuario a la página de inicio.
        }

    }
}