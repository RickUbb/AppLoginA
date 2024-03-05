using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using AppLoginA.Servicios.Contrato;
using AppLoginA.Models;
using AppLoginA.DBContext;

namespace AppLoginA.Servicios.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly BaseEFContext _dbContext;
        public UsuarioService(BaseEFContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Usuario> GetUsuario(string correo, string clave)
        {
            Usuario usuario_encontrado = await _dbContext.Usuarios.Where(u => u.Correo == correo && u.Password == clave)
                 .FirstOrDefaultAsync();

            return usuario_encontrado;
        }
        public async Task<Usuario> GetUsuarioTk(string correo)
        {
            Usuario usuario_encontrado = await _dbContext.Usuarios.Where(u => u.Correo == correo)
                 .FirstOrDefaultAsync();

            return usuario_encontrado;
        }
        public async Task<Usuario> SaveUsuario(Usuario modelo)
        {
            _dbContext.Usuarios.Add(modelo);
            await _dbContext.SaveChangesAsync();
            return modelo;
        }

        public async Task UpdateUsuario(Usuario usuario)
        {
            // Implementa la lógica para actualizar el usuario en la base de datos
            _dbContext.Entry(usuario).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// Este metodo necesita un atributo Tipo RegistroLogin, el cual lo manda
        /// a guardar a la base de datos con el DBContext en la tabla Logs
        /// y retorna el objeto para debuging.
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task<RegistroLogin> addUserLog(RegistroLogin log)
        {
            _dbContext.Logs.Add(log);
            await _dbContext.SaveChangesAsync();
            return log;
        }

        /// <summary>
        /// Este metodo consulta a la base de datos en la tabla Con el modelo Logs
        /// para sacar todos los datos en una lista y la devuelve.
        /// </summary>
        /// <returns></returns>
        public async Task<List<RegistroLogin>> ObtenerRegistrosLog()
        {
            List<RegistroLogin> registro;
            // Consulta la base de datos para obtener todos los registros de la tabla Logs
            registro = await _dbContext.Logs.ToListAsync();

            return registro;
        }
    }
}
