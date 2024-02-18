using AppLoginA.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLoginA.DBContext
{
    public partial class BaseEFContext : DbContext
    {
        
        public BaseEFContext() { }

        public BaseEFContext(DbContextOptions<BaseEFContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Rol> Roles { get; set; }
        public virtual DbSet<RolOperacion> RolOperaciones { get; set; }
        public virtual DbSet<Operacion> Operaciones { get; set; }

    }
}
