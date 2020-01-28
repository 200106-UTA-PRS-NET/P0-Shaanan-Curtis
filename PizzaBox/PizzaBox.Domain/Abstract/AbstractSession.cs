using System;
using System.Threading;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using PizzaBox.Domain.Init;
using PizzaBox.Storing.Entities;
using PizzaBox.Storing.Interface;
//using Microsoft.EntityFrameworkCore;

//!!! = whenever you see this mark, code is complete but needs to be refined

///decorator design dividing sessions between customers and employees
///additional features throughout program
namespace PizzaBox.Domain.Abstract
{
    public abstract class AbstractSession
    {
        public User CurrentUser = new User();
        internal int trials;
        internal int toppings_arraylen = 4;

        #region Backing Fields
        internal bool exits = false;
        #endregion

        #region Properties
        public bool Exits
        { get { return exits; } }
        #endregion

        #region Structs
        public struct Order_tracker
        {
            public bool preset;
            public string preset_order;
            public string size;
            public string sauce;
            public string crust;
            public string cheese;
            public int amt;
            public string[] toppings;
            public decimal cost;
        };
        #endregion

        public int Locations()
        {
            using IPizzaboxRepository pizzaboxRepository = DbOptions.CreatePizzaboxRepository();
            var results = pizzaboxRepository.GetAllStores();

            //***
            if (results.Count() == 0)
            {
                bool flag = false;
                Console.WriteLine("We are still in construction. We should be finished soon.");
                Console.WriteLine("Would you like to sign up for updates?");
                Console.Write("Enter (Y/N): ");
                string answer = Console.ReadLine();
                bool updates = false;
                Console.Clear();
                switch(answer.ToLower())
                {
                    case "y":
                    case "yes":
                    case "sure":
                    case "alright":
                    case "ok":
                    case "okay":
                        updates = true;
                        break;

                    case "n":
                    case "no":
                    case "nah":
                    case "nope":
                    case "no thank you":
                    case "no thanks":
                    case "im good":
                    case "i'm good":
                    case "im ok":
                    case "i'm ok":
                    case "im okay":
                    case "i'm okay":
                    default:
                        break;
                }

                if(updates)
                {
                    do
                    {
                        if (flag)
                        {
                            Console.WriteLine("*Make sure your email contains '@' and ends with an extension (e.g .com)");
                            Console.WriteLine("If you would like to navigate back, just leave it blank or enter anything that could be interpreted as 'go back'");
                            flag = false;
                        }
                        Console.Write("Enter Email: ");
                        answer = Console.ReadLine();
                        switch (answer.ToLower())
                        {
                            case "back":
                            case "go back":
                            case "that way":
                            case "<<<-":
                            case "<<-":
                            case "<-":
                            case "<<<":
                            case "<<":
                            case "<":
                            case "-":
                            case "..":
                            case "/..":
                            case "previous":
                            case "":
                                return 0;
                        }

                        if (!answer.Contains("@") || answer[answer.Length - 4] != '.')
                        {
                            Console.Clear();
                            Console.WriteLine("Email Invalid Format");
                            flag = true;
                        }
                        else
                        {
                            Console.WriteLine("You have successfully signed up for updates.");
                            Console.WriteLine();
                            return 0;
                        }
                    } while (answer.Length > 0);
                }
                return 0;
            }

            foreach (Store s in results)
            {
                if (s.City.Length < 9)
                {
                    Console.Write(s.StoreId + "\t" + s.City);
                    for (int i = 0; i < 9 - s.City.Length; i++)
                        Console.Write(" ");
                    Console.WriteLine("\t" + s.State + "\t" + s.Zip);
                }
                else
                    Console.WriteLine(s.StoreId + "\t" + s.City + "\t" + s.State + "\t" + s.Zip);

                //count++;
            }
            Console.WriteLine();
            return results.Count();
        }   //!!! Marked for refinement
        public void Logout()
        {
            using IPizzaboxRepository pizzaboxRepository = DbOptions.CreatePizzaboxRepository();
            Console.Write("Signing you out ");

            #region Loading Animation
            Loading('.', 3, 2);
            #endregion

            CurrentUser.SessionLive = 0;
            pizzaboxRepository.UpdateUser(CurrentUser);
        }

        #region Utilities
        internal static void Loading(char symbol, int len, int duration)
        {
            for (int y = 0; y < duration; y++)
            {
                for (int x = 0; x < len; x++)
                {
                    Thread.Sleep(700);
                    Console.Write(symbol + " ");
                }
                Console.SetCursorPosition(Console.CursorLeft - 6, Console.CursorTop);
                Console.Write("      ");
                Console.SetCursorPosition(Console.CursorLeft - 6, Console.CursorTop);
            }
        }
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
        public void Exit()
        {
            Logout();
            exits = true;
        }
        #endregion

        public abstract void Info();
        public abstract void History();
        public abstract int Session();
    }
}
