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

        public async Task<Usuario> SaveUsuario(Usuario modelo)
        {
            _dbContext.Usuarios.Add(modelo);
            await _dbContext.SaveChangesAsync();
            return modelo;
        }
    }
}
