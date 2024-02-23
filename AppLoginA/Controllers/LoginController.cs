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
                ViewData["Mensaje"] = "No se encontraron coincidencias";
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
                AllowRefresh = true
                // Puedes configurar otras propiedades de autenticación según necesites
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }
    }
}
