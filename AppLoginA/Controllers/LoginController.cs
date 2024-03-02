using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AppLoginA.Servicios.Contrato;
using AppLoginA.Models;
using AppLoginA.Utilitario;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppLoginA.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioService _usuarioServicio;
        private readonly ITokenService _tokenService;

        public LoginController(IUsuarioService usuarioServicio, ITokenService tokenService)
        {
            _usuarioServicio = usuarioServicio;
            _tokenService = tokenService;
        }

        public IActionResult Registrarse()
        {
            return View();
        }

        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrarse(Usuario usuario)
        {
            try
            {
                if (!Utilidades.EsCorreoValido(usuario.Correo) || !Utilidades.EsPasswordValido(usuario.Password))
                {
                    ViewData["Mensaje"] = "Correo y contraseña inválidos, correo debe contener '@' '.' y contraseña debe tener al menos 8 caracteres.";
                    return View("Registrarse", usuario);
                }

                usuario.idRol = 1; // Asignar el rol adecuado para el usuario

                usuario.Password = Utilidades.EncriptarClave(usuario.Password);

                Usuario usuario_creado = await _usuarioServicio.SaveUsuario(usuario);

                if (usuario_creado.idUsuario > 0)
                    return RedirectToAction("IniciarSesion");

                ViewData["Mensaje"] = "No se pudo crear el usuario";
                return View("Registrarse", usuario);
            }
            catch (Exception ex)
            {
                ViewData["Mensaje"] = "Error al registrar el usuario: " + ex.Message;
                return View("Registrarse", usuario);
            }
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(string correo, string clave)
        {
            string claveEncriptada = Utilidades.EncriptarClave(clave);

            Usuario usuario_encontrado = await _usuarioServicio.GetUsuario(correo, claveEncriptada);

            if (usuario_encontrado == null)
            {
                ViewBag.Mensaje = "No se encontraron coincidencias";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario_encontrado.Correo)
                // Puedes agregar más reclamaciones según necesites
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(1) // Por ejemplo, ajusta el tiempo de vida del token según tus requisitos
                // Puedes configurar otras propiedades de autenticación según necesites
            };


            // Crear la cookie de autenticación
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Generar el token JWT
            string token = _tokenService.GenerateToken(usuario_encontrado);

            // Asignar el token a TempData
            TempData["Token"] = token;

            // Almacenar el token en una cookie de autenticación
            Response.Cookies.Append("Token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(1) // El mismo tiempo de vida que la propiedad ExpiresUtc
            });

            // Redireccionar al usuario a la página de inicio o a otra página de tu aplicación
            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> CerrarSesion()
        {
            // Cerrar la sesión del usuario
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Limpiar la cookie de token
            Response.Cookies.Delete("Token");

            // Redireccionar al usuario a la página de inicio de sesión
            return RedirectToAction("IniciarSesion");
        }

    }
}