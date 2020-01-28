using System;
using System.Collections.Generic;
using PizzaBox.Client.Assets;
using PizzaBox.Domain;
using PizzaBox.Domain.Classes;
using PizzaBox.Domain.Init;
using PizzaBox.Storing;
using PizzaBox.Storing.Entities;
using PizzaBox.Storing.Interface;

using Xunit;

namespace PizzaBox.Testing
{
    public class UnitTests
    {
        [Fact]
        public void Fail_Authentication()
        {
            // ARRANGE
            using IPizzaboxRepository pizzaboxRepository = DbOptions.CreatePizzaboxRepository();
            User Expected = null;
            string[] username_auth = { "admin", "Uname23", "Admin", "uname23", "ad", "Uname","Password123","adminpass" };
            string[] password_auth = { "adminpas", "Password", "adminpass", "Password123", "admin", "password123", "Uname23", "admin" };

            for(int i=0; i<username_auth.Length; i++)
            {
                // ACT
                User Actual = pizzaboxRepository.UserAuthentication(username_auth[i], password_auth[i]);

                // ASSERT
                Assert.Equal(Expected, Actual);
            }
        }

        //Reflects positive results in database
        [Fact]
        public void Check_AddUser()
        {
            // ARRANGE
            using IPizzaboxRepository pizzaboxRepository = DbOptions.CreatePizzaboxRepository();
            User U1 = new User
            {
                Username = "whaa",
                Pass = "pa",
                FullName = "Yah Who",
                SessionLive = 0
            };
            User Expected = U1;

            // ACT
            pizzaboxRepository.AddUser(U1);
            User Actual = pizzaboxRepository.GetUserById("whaa");

            // ASSERT
            Assert.Equal(Expected, Actual);
        }

        //Reflects positive results in database
        [Fact]
        public void Check_UpdateUser()
        {
            // ARRANGE
            using IPizzaboxRepository pizzaboxRepository = DbOptions.CreatePizzaboxRepository();
            User U1 = new User()
            {
                Username = "Uname23",
                Pass = "Password123",
                FullName = "That Way",
                SessionLive = 1
            };
            User Expected = U1;

            // ACT
            pizzaboxRepository.UpdateUser(U1);
            User Actual = pizzaboxRepository.GetUserById("Uname23");

            // ASSERT
            Assert.Equal(Expected, Actual);
        }
    }
}
