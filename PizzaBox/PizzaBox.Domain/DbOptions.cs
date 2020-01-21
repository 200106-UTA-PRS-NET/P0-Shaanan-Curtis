using System;
using System.IO;
using PizzaBox.Storing.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PizzaBox.Domain
{
    public class DbOptions
    {
        public readonly DbContextOptions<pizzaboxContext> options;

        public DbOptions()
        {
            var configBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = configBuilder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<pizzaboxContext>();
            optionsBuilder.UseMySql(configuration.GetConnectionString("PizzaBoxDataSource"));
            options = optionsBuilder.Options;
        }
    }
}
