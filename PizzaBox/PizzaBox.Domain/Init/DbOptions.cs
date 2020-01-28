using System;
using System.IO;
using PizzaBox.Storing.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PizzaBox.Storing.Interface;
using PizzaBox.Storing.Repositories;

namespace PizzaBox.Domain.Init
{
    public sealed class DbOptions
    {
        public static DbContextOptions<pizzaboxContext> options;

        static DbOptions()
        {
            var configBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = configBuilder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<pizzaboxContext>();
            optionsBuilder.UseMySql(configuration.GetConnectionString("PizzaBoxDataSource"));
            options = optionsBuilder.Options;
        }

        public static IPizzaboxRepository CreatePizzaboxRepository()
        {
            var dbContext = new pizzaboxContext(options);
            return new PizzaboxRepository(dbContext);
        }
    }
}
