using AppLoginA.Models;
using System.Threading.Tasks;

namespace AppLoginA.Servicios.Contrato
{
    public interface ITokenService
    {
        string GenerateToken(Usuario user);
    }
}
