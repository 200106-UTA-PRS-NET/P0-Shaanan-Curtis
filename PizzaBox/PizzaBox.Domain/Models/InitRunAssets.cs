using System;
using System.Threading;
using System.Linq;
using PizzaBox.Storing;
using PizzaBox.Storing.Repositories;
using Microsoft.EntityFrameworkCore;

namespace PizzaBox.Domain.Models
{
    public class InitRunAssets
    {
        private static int trials = 3;
        private static DbOptions Database = new DbOptions();
        private static pizzaboxContext DB = new pizzaboxContext(Database.options);

        /// <summary>
        /// - Displays loading animation
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="len"></param>
        private static void Loading(char symbol, int len)
        {
            for (int i = 0; i < len; i++)
            {
                Thread.Sleep(696);
                Console.Write(symbol + " ");
            }
        }

        /// <summary>
        /// - Displays list of available commands
        /// </summary>
        public void Info()
        {
            Console.WriteLine("INFO");
            Console.WriteLine("Info - Display a list of available commands. This command is always available.");
            Console.WriteLine("Login - Username and password entry.");
            Console.WriteLine("Sign Up - Create a new account.");
            Console.WriteLine("Clear - Clear screen.");
            Console.WriteLine("Exit - You'll be back.\n");
        }

        /// <summary>
        /// - Logs into user account
        /// </summary>
        public User Login()
        {
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

                //case sensitive verification
                if (DB.User.Where(u => u.Username.Contains(Me.Username)).Count() > 0 && DB.User.Where(p => p.Pass.Contains(Me.Pass) && p.Username.Contains(Me.Username)).Count() > 0)
                {
                    #region unnecessary loading commented out
                    /*
                    for (int i = 0; i < 3; i++)
                    {
                        Loading('.', 3);
                        Console.Write("\r     \r");
                    }
                    */
                    #endregion

                    var result = from u in DB.User where u.Username == Me.Username select u;
                    if (result.Count() < 1)
                    {
                        Console.Clear();
                        Error("Authentication");
                        continue;
                    }
                    else
                    {
                        var result2 = from u in DB.User where u.Pass == Me.Pass select u;
                        if (result2.Count() < 1)
                        {
                            Console.Clear();
                            Error("Authentication");
                            continue;
                        }
                    }

                    Me.FullName = result.SingleOrDefault().FullName;
                    Me.SessionLive = result.SingleOrDefault().SessionLive;

                    bool success = false;
                    switch (Me.SessionLive)
                    {
                        case 1:
                        //Bug in reading Me.SessionLive (database updated appropriately, not being transferred)
                        /*
                        Error("Session");
                        break;
                        */
                        case 0:
                            Console.WriteLine("Login successful\n");
                            result.Single().SessionLive = 1;
                            Me.SessionLive = 1;
                            DB.SaveChanges();
                            success = true;
                            break;
                    }

                    if (success)
                        break;
                }
                else
                {
                    Console.Clear();
                    Error("Authentication");
                }

            } while (trials > 0);

            return Me;
        }

        /// <summary>
        /// - Special Access Login
        /// -> logs into user account from admin session and includes additional functionality
        /// </summary>
        /// <param name="access_level"></param>
        /// <returns></returns>
        public User Login(string access_level)
        {
            User Me = new User();
            
            if (access_level == "sa-admin")
                Console.WriteLine("Special Access Priority - Order For Customer\n");

            Me = Login();
 
            return Me;
        }

        /// <summary>
        /// - Creates a new user account
        /// </summary>
        public User Signup()
        {
            //Temporary USER object to be saved to Database
            User newUser = new User();
            newUser.Username = "";
            newUser.Pass = "";
            newUser.FullName = "";
            newUser.SessionLive = 0;

            //Title
            Console.WriteLine("SIGN UP");
            Console.WriteLine("You're one step away from being a Pizza Box member!");
            Console.WriteLine("___________________________________________________");

            //Account Type:
            string answer;
            trials = 3;
            do
            {
                Console.Write("Account Type (customer or employee): ");
                answer = Console.ReadLine();
           
                
                if (answer.ToLower() == "employee")
                {
                    if (DB.User.Any(u => u.Username == "admin"))
                        Error("exists");
                    else
                    {
                        newUser.Username = "admin";
                        break;
                    }
                }
                else if (answer.ToLower() == "customer")
                    break;
                else
                    Error("Entry");

            } while (trials > 0);

            //Username:
            bool pass = false;
            trials = 3;
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
                        else if (DB.User.Any(u => u.Username == newUser.Username))
                            Error("exists");
                        else
                            pass = true;
                        break;
                }

                if (newUser.Username.Length == 0)
                    Error("User");
                
            } while (!pass);

            //Password:
            Console.Write("Password: ");
            newUser.Pass = Console.ReadLine();

            //Name:
            Console.Write("Full Name: ");
            newUser.FullName = Console.ReadLine();

            //Creating account...
            Console.Write("\nGetting your account ready ");
            #region loading animation

            for (int i = 0; i < 3; i++)
            {
                Loading('.', 3);
                Console.SetCursorPosition(Console.CursorLeft - 6, Console.CursorTop);
                Console.Write("      ");
                Console.SetCursorPosition(Console.CursorLeft - 6, Console.CursorTop);
            }
            Console.WriteLine();

            #endregion

            newUser.SessionLive = 1;
            DB.Add(newUser);
            DB.SaveChanges();
            return newUser;
        }

        /// <summary>
        /// - Exits application with message
        /// </summary>
        public void Exit()
        {
            Console.Write("Thanks for trying Pizza Box ");
            Loading('~', 3);
            System.Environment.Exit(0);
        }

        /// <summary>
        /// Command Line Error
        /// - Displays error message per incorrect entry
        /// - Decrements trials counter
        /// </summary>
        /// <param name="entry"></param>
        public void CLError(string entry)
        {
            switch (trials)
            {
                case 1:
                    Console.WriteLine("Sorry, we tend to think inside the box.\nYou can always contact us directly at 7499274992 (PIZZAPIZZA).\n");
                    Exit();
                    break;

                default:
                    Console.WriteLine("There is no " + entry + " command. Try again.\n");
                    break;
            }

            trials--;
        }

        /// <summary>
        /// General Error
        /// - Displays type-specific error message
        /// - Decrements trials counter
        /// </summary>
        /// <param name="typerr"></param>
        private void Error(string typerr)
        {
            switch (trials)
            {
                case 1:
                    Console.WriteLine("Give us a call at 7499274992 and we'll be happy to assist you.\n");
                    Exit();
                    break;

                default:
                    switch (typerr.ToLower())
                    {
                        case "user":
                            Console.WriteLine("You do not have access to this username. Try again.\n");
                            break;

                        case "entry":
                            Console.WriteLine("Entry does not exist. Try again.\n");
                            break;

                        case "authentication":
                            Console.WriteLine("Username/Password Incorrect. Try again.\n");
                            break;

                        case "session":
                            Console.WriteLine("User is already signed in. Try again.\n");
                            break;

                        case "exists":
                            Console.WriteLine("User already exists. Try again.\n");
                            break;
                    }
                    break;
            }

            trials--;
        }
    }
}
