using Microsoft.AspNetCore.Authentication.Cookies; // Importa el espacio de nombres para la autenticación basada en cookies.
using Microsoft.AspNetCore.Authentication; // Importa el espacio de nombres para la autenticación.
using Microsoft.AspNetCore.Mvc; // Importa el espacio de nombres para los controladores MVC.
using System.Security.Claims; // Importa el espacio de nombres para las reclamaciones de identidad.
using AppLoginA.Servicios.Contrato; // Importa el espacio de nombres para los contratos de servicios.
using AppLoginA.Models; // Importa el espacio de nombres para los modelos de la aplicación.
using AppLoginA.Utilitario; // Importa el espacio de nombres para las utilidades de la aplicación.
using Microsoft.AspNetCore.Authorization; // Agrega el espacio de nombres para el atributo [Authorize]
using AppLoginA.Servicios.Implementacion;
using System.IdentityModel.Tokens.Jwt;

namespace AppLoginA.Controllers // Define el espacio de nombres y comienza la declaración del controlador.
{
    /*[Authorize]*/
    public class LoginController : Controller // Define la clase LoginController como un controlador MVC.
    {

        private readonly IUsuarioService _usuarioServicio; // Define una instancia de la interfaz IUsuarioService para acceder al servicio de usuarios.
        private readonly WebBCommunicationService _webBCommunicationService;
        public LoginController(IUsuarioService usuarioServicio, WebBCommunicationService webBCommunicationService) // Constructor de la clase LoginController que recibe una instancia de IUsuarioService.
        {
            _usuarioServicio = usuarioServicio; // Asigna la instancia del servicio de usuarios al campo privado _usuarioServicio.
            _webBCommunicationService = webBCommunicationService;
        }

        public IActionResult Registrarse() // Método de acción para mostrar la vista de registro.
        {
            return View(); // Retorna la vista de registro.
        }

        public IActionResult IniciarSesion()
        {
            return View(); // Retorna la vista de login.
        }
        public IActionResult Token()
        {
            return View(); // Retorna la vista de login.
        }


        [HttpPost] // Atributo que indica que este método responde a las solicitudes POST.
        public async Task<IActionResult> Registrarse(Usuario usuario)
        {
            try
            {
                // Validar el correo electrónico y la contraseña
                if (!Utilidades.EsCorreoValido(usuario.Correo) && (!Utilidades.EsPasswordValido(usuario.Password)))
                {
                    ViewData["Mensaje"] = "Correo y contraseña inválidos , correo debe contener ‘@’ ‘.’ y contraseña debe contener al menos 8 dígitos.";
                    return View("Registrarse", usuario);
                }

                if (!Utilidades.EsCorreoValido(usuario.Correo))
                {
                    ViewData["Mensaje"] = "Correo inválido, correo debe contener ‘@’ ‘.’";
                    return View("Registrarse", usuario);
                }

                // Validar el correo electrónico y la contraseña
                if (!Utilidades.EsPasswordValido(usuario.Password))
                {
                    ViewData["Mensaje"] = "Contraseña inválida, contraseña debe contener al menos 8 dígitos.";
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

                ///Mando el usurio creado a registrarlo en los Logs
                RegistroLogin userLog = new RegistroLogin();
                userLog.Correo = usuario_creado.Correo;
                userLog.IsRegisteredToken = false;
                RegistroLogin RegistroUser = await _usuarioServicio.addUserLog(userLog);

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
            ///Encriptar
            string claveEncriptada = Utilidades.EncriptarClave(clave); // Encripta la contraseña proporcionada por el usuario.

            Usuario usuario_encontrado = await _usuarioServicio.GetUsuario(correo, claveEncriptada); // Busca al usuario en la base de datos.

            if (usuario_encontrado == null) // Si no se encuentra al usuario.
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias"; // Establece un mensaje de error en la vista.
                return View(); // Retorna la vista de inicio de sesión.
            }

            // Registra el tipo de acceso utilizado (en este caso, "Normal")
            //usuario_encontrado.TipoAcceso = TipoAcceso.Normal;
            RegistroLogin log = new RegistroLogin();
            log.Correo = correo;
            log.IsRegisteredToken = false;
            RegistroLogin usuario_log = await _usuarioServicio.addUserLog(log);

            // Guarda el usuario actualizado en la base de datos
            //await _usuarioServicio.UpdateUsuario(usuario_encontrado);

           List <Claim> claims = new List<Claim>() { // Crea una lista de reclamaciones para la identidad del usuario.
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
        [HttpPost]
        public async Task<IActionResult> LoginToken(string tokenJWT)
        {
            try
            {
                // Llama al método GetInfoFromWebB de WebBCommunicationService
                var informacion = await _webBCommunicationService.GetInfoFromWebB(tokenJWT);

                // Verifica si la verificación del token fue exitosa
                if (informacion == "Información obtenida correctamente")
                {
                    // Aquí es donde iniciarás sesión después de verificar el token correctamente

                    // Obtiene las reclamaciones del usuario (correo y otros detalles si es necesario)
                    var claims = new List<Claim>
                {
                new Claim(ClaimTypes.Name, "nombreUsuario"), // Por ejemplo, podrías usar el correo como nombre de usuario
                // Puedes agregar más reclamaciones según sea necesario
                };

                    // Crea una identidad de reclamaciones para el usuario
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Crea propiedades de autenticación
                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        // Aquí puedes agregar más propiedades de autenticación según sea necesario
                    };

                    // Inicia sesión del usuario
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    /// Mandamos el registro del ususrio positivo con token en la tabla logs
                    string emailString;
                    emailString = ObtenerCorreoDesdeTokenJWT(tokenJWT);
                    RegistroLogin userLog = new RegistroLogin();
                    userLog.Correo = emailString;
                    userLog.IsRegisteredToken = true;
                    RegistroLogin logged = await _usuarioServicio.addUserLog(userLog);
                    //==================================================================

                    // Redirige al usuario a la página de inicio
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Si hay un problema con el token, establece el mensaje en ViewData
                    ViewData["Mensaje"] = informacion;
                    return View("Token"); // Puedes redirigir a una vista de error o a la vista de inicio de sesión según sea necesario
                }

            }
            catch (Exception ex)
            {
                // Maneja cualquier excepción que pueda ocurrir al llamar al método
                ViewData["Mensaje"] = "Error al iniciar";
                return BadRequest($"Error al obtener información de WebB: {ex.Message}");
            }

        }

        /// <summary>
        /// El metodo necesita un token para poder extraer el correo
        /// y es lo qeu devuelve en tipo string 
        /// </summary>
        /// <param name="tokenJWT"></param>
        /// <returns></returns>
        public string ObtenerCorreoDesdeTokenJWT(string tokenJWT)
        {
            // Decodifica el token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(tokenJWT);

            // Busca la reclamación que contiene el correo electrónico
            var correoClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

            // Si se encuentra la reclamación del correo electrónico, devuelve su valor
            if (correoClaim != null)
            {
                return correoClaim.Value;
            }

            // Si no se encuentra la reclamación del correo electrónico, devuelve null o una cadena vacía, según sea necesario
            return null; // O puedes lanzar una excepción o manejarlo de otra manera según tus necesidades
        }
    }
}