using System;
using System.Collections.Generic;
using System.Text;
using PizzaBox.Client;
using PizzaBox.Domain.Init;
using PizzaBox.Storing.Entities;
using PizzaBox.Storing.Interface;

namespace PizzaBox.Client.Assets
{
    internal class Run
    {
        internal static int trials;
        internal static void Info()
        {
            Console.WriteLine("INFO");
            Console.WriteLine("Info - Display a list of available commands. This command is always available.");
            Console.WriteLine("Login - Username and password entry.");
            Console.WriteLine("Sign Up - Create a new account.");
            Console.WriteLine("Clear - Clear screen.");
            Console.WriteLine("Exit - You'll be back.\n");
        }
        internal static User Login()
        {
            using IPizzaboxRepository pizzaboxRepository = DbOptions.CreatePizzaboxRepository();
            User Me = new User();

            trials = 3;
            do
            {
                #region Username and Password Entry
                Console.WriteLine("USER LOGIN");
                Console.Write("Username: ");
                Me.Username = Console.ReadLine();
                Console.Write("Password: ");
                Me.Pass = Console.ReadLine();
                Console.WriteLine();
                #endregion

                #region Case-Sensitive Verification
                var AuthorizedUser = pizzaboxRepository.UserAuthentication(Me.Username, Me.Pass);
                if (AuthorizedUser == null)
                {
                    Console.Clear();
                    Utilities.Error("authentication");
                    continue;
                }
                #endregion

                #region Authentication
                Me = AuthorizedUser;
                bool success = false;
                switch (Me.SessionLive)
                {
                    //ALREADY LOGGED IN
                    case 1:
                    /*
                    Error("session");
                    break;
                    */

                    //NOT LOGGED IN
                    case 0:
                        Console.WriteLine("Login successful\n");
                        Me.SessionLive = 1;
                        pizzaboxRepository.UpdateUser(Me);
                        success = true;
                        break;
                }

                if (success)
                    break;
                #endregion

            } while (trials > 0);

            return Me;
        }
        internal static User Login(string access_level, string uname)
        {
            if (access_level == "sa-admin")
                Console.WriteLine($"Special Access - Authorized Login for {uname}.\n");

            var Me = Login();

            return Me;
        }
        internal static User Signup()
        {
            using IPizzaboxRepository pizzaboxRepository = DbOptions.CreatePizzaboxRepository();
            User newUser = new User
            {
                Username = "",
                Pass = "",
                FullName = "",
                SessionLive = 0
            };

            #region Header
            Console.WriteLine("SIGN UP");
            Console.WriteLine("You're one step away from being a Pizza Box member!");
            Console.WriteLine("___________________________________________________");
            #endregion

            #region Choose Account Type
            trials = 3;
            string answer;
            do
            {
                Console.Write("Account Type (customer or employee): ");
                answer = Console.ReadLine();

                if (answer.ToLower() == "employee")
                {
                    if (pizzaboxRepository.GetUserById("admin") != null)
                        Utilities.Error("exists");
                    /*
                    if (DB.User.Any(u => u.Username == "admin"))
                        Error("exists");
                    */
                    else
                    {
                        newUser.Username = "admin";
                        break;
                    }
                }
                else if (answer.ToLower() == "customer")
                    break;
                else
                    Utilities.Error("entry");

            } while (trials > 0);
            #endregion

            #region Choose Username 
            trials = 3;
            bool pass = false;
            do
            {
                Console.Write("Username: ");
                switch (newUser.Username)
                {
                    case "admin":
                        Console.WriteLine("admin");
                        pass = true;
                        break;

                    default:
                        newUser.Username = Console.ReadLine();
                        if (newUser.Username.ToLower() == "admin")
                            newUser.Username = "";
                        else if (pizzaboxRepository.GetUserById(newUser.Username) != null)
                            Utilities.Error("exists");
                        /*
                        else if (DB.User.Any(u => u.Username == newUser.Username))
                            Error("exists");
                        */
                        else
                            pass = true;
                        break;
                }

                if (newUser.Username.Length == 0)
                    Utilities.Error("user");

            } while (!pass);
            #endregion

            #region Choose Password
            Console.Write("Password: ");
            newUser.Pass = Console.ReadLine();
            #endregion

            #region Choose Name
            Console.Write("Full Name: ");
            newUser.FullName = Console.ReadLine();
            #endregion

            Console.Write("\nGetting your account ready ");

            #region loading animation
            Utilities.Loading('.', 3, 3);
            Console.WriteLine();
            #endregion

            newUser.SessionLive = 1;
            pizzaboxRepository.AddUser(newUser);
            return newUser;
        }

        internal static void InitRun()
        {
            string answer;
            Console.Write("Enter Command (say Info for help): ");
            answer = Console.ReadLine();
            Console.Clear();
            switch (answer.ToLower())
            {
                case "info":
                case "help":
                    Run.Info();
                    //Assets.Info();
                    break;

                case "logon":
                case "login":
                case "signin":
                case "log on":
                case "log in":
                case "sign on":
                case "sign in":
                    Program.CurrentUser = Login();
                    Program.alive = Program.CurrentUser.SessionLive;
                    //CurrentUser = Assets.Login();
                    //alive = CurrentUser.SessionLive;
                    break;

                case "sign up":
                case "signup":
                    Program.CurrentUser = Signup();
                    Program.alive = Program.CurrentUser.SessionLive;
                    //CurrentUser = Assets.Signup();
                    //alive = CurrentUser.SessionLive;
                    Program.sale = true;
                    break;

                case "clear screen":
                case "clearscreen":
                case "clear":
                case "cls":
                    Console.Clear();
                    break;

                case "exit":
                    Utilities.Exit();
                    //Assets.Exit();
                    break;

                default:
                    Utilities.CLError(answer);
                    //Assets.CLError(answer);
                    break;
            }
        }
    }
}
