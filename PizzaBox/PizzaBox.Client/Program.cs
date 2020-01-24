using System;
using System.IO;
using System.Threading;
using PizzaBox.Domain;
using PizzaBox.Domain.Abstracts;
using PizzaBox.Domain.Models;
using PizzaBox.Storing;
using PizzaBox.Storing.Repositories;

namespace PizzaBox.Client
{
    class Program
    {
        static DbOptions Database = new DbOptions();
        static pizzaboxContext DB = new pizzaboxContext(Database.options);
        static InitRunAssets Assets = new InitRunAssets();
        static User CurrentUser = new User();
        static sbyte alive;
        public static bool exit_proc;
        private static bool priority = false;

        /// <summary>
        /// - Runs while user is signed out.
        /// </summary>
        static void InitRun()
        {
            Console.Write("Enter Command (say Info for help): ");
            string answer;
            answer = Console.ReadLine();
            Console.Clear();
            switch (answer.ToLower())
            {
                case "info":
                case "help":
                    Assets.Info();
                    break;

                case "logon":
                case "login":
                case "signin":
                case "log on":
                case "log in":
                case "sign on":
                case "sign in":
                    CurrentUser = Assets.Login();
                    alive = CurrentUser.SessionLive;
                    break;

                case "sign up":
                case "signup":
                    CurrentUser = Assets.Signup();
                    alive = CurrentUser.SessionLive;
                    break;

                case "clear screen":
                case "clearscreen":
                case "clear":
                case "cls":
                    Console.Clear();
                    break;

                case "exit":
                    Assets.Exit();
                    break;

                default:
                    Assets.CLError(answer);
                    break;
            }
        }

        static int Session()
        {
            Console.WriteLine("Signed in as " + CurrentUser.FullName);
            switch (CurrentUser.Username)
            {
                case "admin":
                    Employee PBAssociate = new Employee();
                    PBAssociate.Me = CurrentUser;
                    if (PBAssociate.Session() == 1)
                    {
                        alive = 0;
                        exit_proc = false;
                        return 1;
                    }
                    alive = PBAssociate.Me.SessionLive;
                    exit_proc = PBAssociate.Exits;
                    break;

                default:
                    Customer PBCustomer = new Customer();
                    PBCustomer.Me = CurrentUser;
                    if (priority)
                    {
                        PBCustomer.Order();
                        Console.Write("[PRESS ENTER TO CONTINUE]");
                        Console.Write("\r");
                        Console.ReadLine();
                        PBCustomer.Exit();
                    }
                    else
                        PBCustomer.Session();
                    alive = PBCustomer.Me.SessionLive;
                    exit_proc = PBCustomer.Exits;
                    break;
            }

            return 0;
        }

        static void Main()
        {
            string header1 = "Welcome to Pizza Box";
            string header2 = "Please sign in to your account to get started.\nIf you don't have an account, sign up with us now and get 50% off your first order!";
            bool shown = false;
            while (!exit_proc)
            {
                do
                {
                    if(!priority)
                    {
                        Console.WriteLine(header1);
                        if (!shown)
                        {
                            Console.WriteLine(header2);
                            shown = true;
                        }

                        InitRun();
                    }
                    else
                    {
                        CurrentUser = Assets.Login("sa-admin");
                        alive = CurrentUser.SessionLive;
                    }
                    
                } while (alive == 0);
                shown = false;
                bool clearin = false;
                while (alive == 1)
                {
                    if (!clearin)
                    {
                        Thread.Sleep(600);
                        Console.Clear();
                        clearin = true;
                    }
                    if (Session() == 1)
                        priority = true;
                }
                Console.Clear();
            }
            Assets.Exit();
        }
    }
}
