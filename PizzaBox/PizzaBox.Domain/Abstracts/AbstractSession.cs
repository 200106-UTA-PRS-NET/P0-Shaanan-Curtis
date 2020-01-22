using System;
using System.Threading;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using PizzaBox.Storing.Repositories;
using Microsoft.EntityFrameworkCore;

namespace PizzaBox.Domain.Abstracts
{
    public abstract class AbstractSession
    {
        public static DbOptions Database = new DbOptions();
        public static pizzaboxContext DB = new pizzaboxContext(Database.options);
        public User Me = new User();
        internal int trials = 3;
        internal bool exits = false;
        public bool Exits
        { get { return exits; } }

        public void Locations()
        {
            Console.WriteLine("STORE LOCATIONS");
            var results = from s in DB.Store select s;

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
            }
            Console.WriteLine();
        }

        public void Logout()
        {
            Console.Write("Signing you out ");

            #region loading animation

            for (int i = 0; i < 2; i++)
            {
                Loading('.', 3);
                Console.SetCursorPosition(Console.CursorLeft - 6, Console.CursorTop);
                Console.Write("      ");
                Console.SetCursorPosition(Console.CursorLeft - 6, Console.CursorTop);
            }

            #endregion

            //Update CurrentUser's session state
            var users = DB.User.First(u => u.Username == Me.Username);
            users.SessionLive = Me.SessionLive = 0;
            DB.SaveChanges();
        }

        /// <summary>
        /// - Displays loading animation
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="len"></param>
        static internal void Loading(char symbol, int len)
        {
            for (int i = 0; i < len; i++)
            {
                Thread.Sleep(696);
                Console.Write(symbol + " ");
            }
        }

        /// <summary>
        /// - Exits application with message
        /// </summary>
        public void Exit()
        {
            Logout();
            exits = true;
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
    }

    public class Customer : AbstractSession
    {
        static void Info()
        {
            Console.WriteLine("INFO");
            Console.WriteLine("Info - Display a list of available commands.");
            Console.WriteLine("Logout - Sign out of application");
            Console.WriteLine("Locations - Display a list of store locations");
            Console.WriteLine("History - Display a list of recent orders by date");
            Console.WriteLine("Menu - Display menu");
            Console.WriteLine("Order - Order from PizzaBox!");
            Console.WriteLine("Clear - Clear screen");
            Console.WriteLine("Exit - Log out and exit application\n");
        }

        /*
        Crust - Thin, Thick
        Size - small, medium, large
        Toppings - None, Pepperoni, Chicken, Beef, Steak, Mushrooms, Banana Pepper, Jalapeno
        Preset - Pepperoni, Cheese, Supreme, BBQ Chicken, Veggie, Meatball
         */
        public void Menu()
        {
            Console.WriteLine("MENU");
            Console.WriteLine("__________________________________________________________________________");
            Console.WriteLine("Preset");
            Console.WriteLine("1. Vegan | Small $4 | Medium $8 | Large $12");
            Console.WriteLine("2. Pepperoni | Small $5 | Medium $10 | Large $15");
            Console.WriteLine("3. BBQ Chicken | Small $6 | Medium $12 | Large $18");
            Console.WriteLine("4. Meatball | Small $7 | Medium $14 | Large $21");
            Console.WriteLine("5. Supreme | Small $8 | Medium $16 | Large $24");
            Console.WriteLine("6. Greek | Small $9 | Medium $18 | Large $27");
            Console.WriteLine();
            Console.WriteLine("Custom");
            Console.WriteLine("Sizes | Small $3 | Medium $6 | Large $9");
            Console.WriteLine("Sauces | Traditional $0.50 | BBQ $1 | Alfredo $1");
            Console.WriteLine("Cheese | Regular $0.50 | Goat (Greek) $1 | Queso $2");
            Console.WriteLine("Toppings | Veggies/Fruit $0.50 | Pepperoni $1 | Chicken $1 | Meatballs $3");
            Console.WriteLine("__________________________________________________________________________");
            Console.WriteLine();
        }

        public void History()
        {
            Console.WriteLine("RECENT ORDERS");
            IDictionary<string, int> Map = new Dictionary<string, int>();
            var result = from o in DB.Orders
                         join ot in DB.Ordertype
                         on o.OrderId equals ot.OrderId
                         where o.Username == Me.Username
                         select new
                         {
                             ID = ot.OrderId,
                             PRESET = ot.Preset,
                             CUSTOM = ot.Custom,
                             DATE = ot.Dt,
                             TIME = ot.Tm
                         };

            foreach(var val in result)
            {
                Console.WriteLine("Order #" + val.ID.ToString().PadLeft(12 - val.ID.ToString().Length, '0'));
                int i = 0, n = 0;
                string nums, sequence;
                //TRANSLATE PRESETS
                do
                {
                    //reset for next pizza under presets
                    nums = "";
                    sequence = "";

                    // Worst O(3)
                    while (i < val.PRESET.Length && Char.IsDigit(val.PRESET[i]))
                    {
                        nums += val.PRESET[i];
                        i++;
                    }

                    n = Convert.ToInt32(nums);
                    // Worst O(2)
                    while (i < val.PRESET.Length && Char.IsLetter(val.PRESET[i]))
                    {
                        switch (val.PRESET[i])
                        {
                            case 'S':
                                sequence += "Small Preset, ";
                                break;

                            case 'M':
                                sequence += "Medium Preset, ";
                                break;

                            case 'L':
                                sequence += "Large Preset, ";
                                break;

                            case 'k':
                                sequence += "thick crust pizza";
                                if (n > 1)
                                    sequence += "s";
                                break;

                            case 'n':
                                sequence += "thin crust pizza";
                                if (n > 1)
                                    sequence += "s";
                                break;
                        }

                        i++;
                    }

                    Console.WriteLine(Convert.ToInt32(nums) + " " + sequence);

                } while (i < val.PRESET.Length);
               
                //Console.WriteLine(val.PRESET + "\t" + val.CUSTOM + "\t" + val.DATE + "\t" + val.TIME);
            }
       
            /*
            string nums, sequence;
            int n;
            int i;
            */
            /*
            foreach(Orders o in results)
            {
                Console.WriteLine();
                
                if(o.Preset[0] != '-')
                {
                    i = 0;
                    do
                    {
                        nums = "";
                        n = 0;
                        sequence = "";

                        while (Char.IsDigit(o.Preset[i]) && i < o.Preset.Length)
                        {
                            nums.Append(o.Preset[i]);
                            i++;
                        }
                        n = Convert.ToInt32(nums);
                        while (Char.IsLetter(o.Preset[i]) && i < o.Preset.Length)
                        {
                            switch (o.Preset[i])
                            {
                                case 'S':
                                    sequence += "Small Preset, ";
                                    break;

                                case 'M':
                                    sequence += "Medium Preset, ";
                                    break;

                                case 'L':
                                    sequence += "Large Preset, ";
                                    break;

                                default:
                                    break;
                            }

                            switch (o.Preset[i])
                            {
                                case 'k':
                                    sequence += "thick crust pizza";
                                    if (n > 1)
                                        sequence += "s";
                                    break;

                                case 'n':
                                    sequence += "thin crust pizza";
                                    if (n > 1)
                                        sequence += "s";
                                    break;

                                default:
                                    break;
                            }
                            i++;
                        }
                        Map.Add(sequence, n);
                    } while (i < o.Preset.Length);
                }
                if(o.Custom[0] != '-')
                {
                    i = 0;
                    do
                    {
                        nums = "";
                        n = 0;
                        sequence = "";
                        //decrypt

                        while (Char.IsDigit(o.Custom[i]) && i < o.Custom.Length)
                        {
                            nums.Append(o.Custom[i]);
                            i++;
                        }
                        n = Convert.ToInt32(nums);
                        while (Char.IsLetter(o.Custom[i]) && i < o.Custom.Length)
                        {
                            switch (o.Custom[i])
                            {
                                case 'S':
                                    sequence += "Small Custom, ";
                                    break;

                                case 'M':
                                    sequence += "Medium Custom, ";
                                    break;

                                case 'L':
                                    sequence += "Large Custom, ";
                                    break;
                            }

                            switch (o.Custom[i])
                            {
                                case 'k':
                                    sequence += "thick crust pizza";
                                    if (n > 1)
                                        sequence += "s";
                                    break;

                                case 'n':
                                    sequence += "thin crust pizza";
                                    if (n > 1)
                                        sequence += "s";
                                    break;
                            }
              
                            i++;
                        }
                        Map.Add(sequence, n);
                    } while (i < o.Custom.Length);
                }
                Console.WriteLine(o.Odate + ": Order #" + o.OrderId);
                foreach(KeyValuePair<string,int> item in Map)
                    Console.WriteLine(item.Value + " " + item.Key);
                
                Console.WriteLine();
                
            }
        */
        }

        public void Order()
        {
            Console.WriteLine("MAKE AN ORDER");
            Console.WriteLine();
        }

        public void Session()
        {
            string answer;
            Console.Write("Enter Command (say Info for help): ");
            answer = Console.ReadLine();
            Console.Clear();
            switch (answer.ToLower())
            {
                case "info":
                case "help":
                    Info();
                    break;

                case "logout":
                case "signout":
                case "log out":
                case "sign out":
                    Logout();
                    break;

                case "locations":
                case "location":
                case "stores":
                case "store":
                    Locations();
                    break;

                case "history":
                    History();
                    break;

                case "menu":
                    Menu();
                    break;

                case "order":
                    Order();
                    break;

                case "cls":
                case "clear":
                case "clearscreen":
                case "clear screen":
                    Console.Clear();
                    break;

                case "exit":
                    Exit();
                    break;

                default:
                    CLError(answer);
                    break;
            }
        }
    }

    public class Employee : AbstractSession
    {
        public void Info()
        {
            Console.WriteLine("INFO");
            Console.WriteLine("Info - Display a list of available commands.");
            Console.WriteLine("Logout - Sign out of application");
            Console.WriteLine("Locations - Display a list of store locations");
            Console.WriteLine("History - Display a list of recent orders by date");
            Console.WriteLine("Inventory - Display inventory");
            Console.WriteLine("Order - For orders by phone");
            Console.WriteLine("Clear - Clear screen");
            Console.WriteLine("Exit - Log out and exit application\n");
        }

        public void Inventory()
        {
            //Query ORDER table for inventory
            Console.WriteLine("INVENTORY");
            Console.WriteLine("Your store has enough ingredients for Preset pizzas");
            Console.WriteLine("Your store has enough ingredints for Custom pizzas");
            Console.WriteLine();
        }

        public void History()
        {
            Console.WriteLine("RECENT STORE ORDERS");
            Console.WriteLine();
        }

        public void Order()
        {
            Console.WriteLine("MAKE AN ORDER FOR CUSTOMER");
            Console.WriteLine();
        }

        public void Session()
        {
            string answer;

            Console.Write("Enter Command (say Info for help): ");
            answer = Console.ReadLine();
            Console.Clear();
            switch (answer.ToLower())
            {
                case "info":
                case "help":
                    Info();
                    break;

                case "logout":
                case "signout":
                case "log out":
                case "sign out":
                    Logout();
                    break;

                case "locations":
                case "location":
                case "stores":
                case "store":
                    Locations();
                    break;

                case "history":
                    History();
                    break;

                case "inventory":
                    Inventory();
                    break;

                case "order":
                    Order();
                    break;

                case "cls":
                case "clear":
                case "clearscreen":
                case "clear screen":
                    Console.Clear();
                    break;

                case "exit":
                    Exit();
                    break;

                default:
                    CLError(answer);
                    break;
            }
        }
    }
}
