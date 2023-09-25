using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firm.Infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FirmDBContext>
    {
        public FirmDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FirmDBContext>();
            //optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=FirmWebAppDB;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true;");
            optionsBuilder.UseSqlServer("Data Source=192.168.0.7\\sqlexpress;initial catalog=FirmWebAppDB;user id=sa;password=kg@123;Trusted_Connection=False;MultipleActiveResultSets=true;Encrypt=false;TrustServerCertificate=true;");
            //optionsBuilder.UseSqlServer("Data Source=192.168.0.7\\sqlexpress;initial catalog=FirmWebAppDB;user id=sa;password=kg@123;Trusted_Connection=False;MultipleActiveResultSets=true;Encrypt=false;TrustServerCertificate=true");
            return new FirmDBContext(optionsBuilder.Options);

        }
    }
}