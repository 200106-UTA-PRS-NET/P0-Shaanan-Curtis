using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using PizzaBox.Storing;

namespace PizzaBox.Domain.Models
{
    public class InitRunAssets
    {

        static int trials = 3;
        public User member = new User();    //delete when DB ready

        //delete when DB ready (pretend this is the physical DB's USER table)
        public User testSignin = new User();   
        public InitRunAssets()
        {
            testSignin.username = "larry123";
            testSignin.password = "1 L0ve";
            testSignin.name = "Ma Jang";
            testSignin.session = false;
        }

        /// <summary>
        /// - Displays loading animation
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="len"></param>
        static void Loading(char symbol, int len)
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
            Console.WriteLine("Info - Displays a list of available commands. This command is always available.");
            Console.WriteLine("Login - Username and password entry.");
            Console.WriteLine("Sign Up - Create a new account.");
            Console.WriteLine("Clear - Clears screen.");
            Console.WriteLine("Exit - You'll be back.\n");
        }

        /// <summary>
        /// - Logs into user account
        /// </summary>
        public void Login()
        {
            string user = "";
            string pass = "";

            trials = 3;
            do
            {
                Console.WriteLine("USER LOGIN");
                Console.Write("Username: ");
                user = Console.ReadLine();
                Console.Write("Password: ");
                pass = Console.ReadLine();
                Console.WriteLine();

                //Query user from USER Table in DB
                if (user == testSignin.username && pass == testSignin.password)
                {
                    Console.WriteLine("Login successful");
                    testSignin.session = true;
                    break;
                }
                else
                    Error("Authentication");

            } while (!testSignin.session);
        }

        /// <summary>
        /// - Creates a new user account
        /// </summary>
        public void Signup()
        {
            //Temporary USER table
            //set USER class fields
            member.username = "";
            member.password = "";
            member.name = "";
            member.session = false;

            string answer;

            //Title
            Console.WriteLine("___________________________________________________");
            Console.WriteLine("SIGN UP");
            Console.WriteLine("You're one step away from being a Pizza Box member!");
            Console.WriteLine("___________________________________________________");
           
            //Account Type:
            trials = 3;
            do
            {
                Console.Write("Account Type (customer or employee): ");
                answer = Console.ReadLine();

                if (answer.ToLower() == "employee")
                {
                    member.username = "admin";
                    break;
                }
                else if (answer.ToLower() == "customer")
                    break;
               
                Error("Entry");

            } while (answer.ToLower() != "employee" && answer.ToLower() != "customer");

            //Username:
            trials = 3;
            do
            {
                Console.Write("Username: ");
                switch (member.username)
                {
                    case "admin":
                        Console.WriteLine("admin");
                        break;

                    default:
                        member.username = Console.ReadLine();
                        if (member.username.ToLower() == "admin")
                            member.username = "";
                        break; 
                }

                if (member.username.Length == 0)
                    Error("User");
                else
                    break;
                             
            } while (member.username.Length == 0);
            
            //Password:
            Console.Write("Password: ");
            member.password = Console.ReadLine();

            //Name:
            Console.Write("Full Name: ");
            member.name = Console.ReadLine();

            //Loading account...
            Console.WriteLine("\nGetting your account ready");
            for (int i = 0; i < 3; i++)
            {
                Loading('.', 3);
                Console.Write("\r     \r");
            }
            Console.WriteLine();

            //Start Session with user info
            member.session = true;
            //Save User to USER Table in DB
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
                    Console.WriteLine("Sorry, having trouble understanding you.\nYou can always contact us directly at 7499274992 (PIZZAPIZZA).\n");
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
        void Error(string typerr)
        {
            switch(trials)
            {
                case 1:
                    Console.WriteLine("Give us a call at 7499274992 and we'll be happy to assist you.\n");
                    Exit();
                    break;

                default:
                    switch(typerr.ToLower())
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
                    }
                    break;
            }

            trials--;
        }
    }
}
