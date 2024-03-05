using Microsoft.EntityFrameworkCore;
using AppLoginA.Models;

namespace AppLoginA.Servicios.Contrato
{
    public interface IUsuarioService
    {
        Task<Usuario> GetUsuario(string correo, string clave);
        Task<Usuario> GetUsuarioTk(string correo);
        Task<Usuario> SaveUsuario(Usuario modelo);
        Task UpdateUsuario(Usuario usuario);
        /// <summary>
        /// Este metodo necesita un objeto tipo RegistroLogin donde se especifica
        /// el correo del usuario que ha iniciado sesion o se a registrado en la aplicacion
        /// y tambien un bool para saber si es con token o no, 1 = token, 0 = sin token
        /// El metodo sirve para guardar en la base de datos en la tabla Logs
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        Task<RegistroLogin> addUserLog(RegistroLogin log);

        /// <summary>
        /// Interfaz para el Metodo que devuelva una lista de tipo Registro de Logs
        /// </summary>
        /// <returns></returns>
        Task<List<RegistroLogin>> ObtenerRegistrosLog();
    }
}
