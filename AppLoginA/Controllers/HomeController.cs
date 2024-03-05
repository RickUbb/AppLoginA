using AppLoginA.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AppLoginA.Servicios.Implementacion;
using AppLoginA.Servicios.Contrato;

namespace AppLoginA.Controllers
{
    [Authorize] //solo accede si el usuario esta autorizado
    public class HomeController : Controller
    {
        private readonly IUsuarioService _usuarioServicio; // Define una instancia de la interfaz IUsuarioService para acceder al servicio de usuarios.
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger, IUsuarioService usuarioServicio)
        {
            _logger = logger;
            _usuarioServicio = usuarioServicio;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("IniciarSesion", "Login");
        }

        /// <summary>
        /// Constrolador para que utiliza el servicio de usuario para obtener
        /// el metodos para consultar los logs en la base y retorne una lista de logs
        /// y se la devuelve enla vista
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> VerLogs()
        {
            List<RegistroLogin> listLogs;
            listLogs = await _usuarioServicio.ObtenerRegistrosLog();
            return View(listLogs);
        }
    }
}