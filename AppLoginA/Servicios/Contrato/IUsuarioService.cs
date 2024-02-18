using Microsoft.EntityFrameworkCore;
using AppLoginA.Models;

namespace AppLoginA.Servicios.Contrato
{
    public interface IUsuarioService
    {
        Task<Usuario> GetUsuario(string correo, string clave);
        Task<Usuario> SaveUsuario(Usuario modelo);

    }
}
