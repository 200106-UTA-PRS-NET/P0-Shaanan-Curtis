using PizzaBox.Client.Assets;
using PizzaBox.Domain.Init;
using PizzaBox.Domain.Abstract;
using PizzaBox.Storing.Entities;
using PizzaBox.Storing.Interface;
using System;
using System.Threading;

namespace PizzaBox.Client
{
    class Program
    {
        internal static User CurrentUser = new User();
        internal static sbyte alive;
        internal static bool sale = false;

        private static bool exit_proc;
        private static bool priority = false;
        
        static int Session()
        {
            Console.WriteLine("Signed in as " + CurrentUser.FullName);
            switch (CurrentUser.Username)
            {
                /*
                case "admin":
                    Employee PBAssociate = new Employee();
                    PBAssociate.CurrentUser = CurrentUser;
                    if (PBAssociate.Session() == 1)
                    {
                        alive = 0;
                        exit_proc = false;
                        return 1;
                    }
                    alive = PBAssociate.CurrentUser.SessionLive;
                    exit_proc = PBAssociate.Exits;
                    break;
                */
                default:
                    CustomerSession PBCustomer = new CustomerSession(sale);
                    PBCustomer.CurrentUser = CurrentUser;
                    if (priority)
                    {
                        //PBCustomer.Order();
                        Console.Write("[PRESS ENTER TO CONTINUE]");
                        Console.Write("\r");
                        Console.ReadLine();
                        PBCustomer.Exit();
                    }
                    else
                        PBCustomer.Session();
                    alive = PBCustomer.CurrentUser.SessionLive;
                    exit_proc = PBCustomer.Exits;
                    break;
            }

            return 0;
        }
        
        static void UI(IPizzaboxRepository pizzaboxRepository)
        {
            string header1 = "Welcome to Pizza Box";
            string header2 = "Please sign in to your account to get started.\nIf you don't have an account, sign up with us now and get 50% off your first purchase!";
            
            bool shown = false;
            while (!exit_proc)
            {
                do
                {
                    if (!priority)
                    {
                        Console.WriteLine(header1);
                        if (!shown)
                        {
                            Console.WriteLine(header2);
                            shown = true;
                        }

                        Run.InitRun();
                    }
                    else
                    {
                        CurrentUser = Run.Login("sa-admin", "Tommy");
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
            Utilities.Exit();
        }
        static void Main()
        {
            ///DbOptions options = new DbOptions();    //options needs to be initialized in constructor (without static)
            using IPizzaboxRepository pizzaboxRepository = DbOptions.CreatePizzaboxRepository();
            UI(pizzaboxRepository);
        }
    }
}
