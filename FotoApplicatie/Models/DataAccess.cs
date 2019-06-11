using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FotoApplicatie.Models
{
    public class DataAccess : DbContext
    {
        public DataAccess(DbContextOptions<DataAccess> options)
            : base(options)
        { }

        public DbSet<User> User { get; set; }
    }
}
