using System;
using System.Threading;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using PizzaBox.Storing.Repositories;
using PizzaBox.Domain.Models;
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

        public const int toppings_arraylen = 4;
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

        public int Locations()
        {
            //int count = 0;
            var results = from s in DB.Store select s;

            //***
            if(results.Count() == 0)
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
        private static bool promo;

        decimal sales_price = 0.00m;

        static void Info()
        {
            Console.WriteLine("Info - Display a list of available commands.");
            Console.WriteLine("Logout - Sign out of application");
            Console.WriteLine("Locations - Display a list of store locations");
            Console.WriteLine("History - Display a list of recent orders by date");
            Console.WriteLine("Menu - Display menu");
            Console.WriteLine("Order - Order from PizzaBox!");
            Console.WriteLine("Clear - Clear screen");
            Console.WriteLine("Exit - Log out and exit application\n");
        }

        public void Menu()
        {
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
            if(result.Count() == 0)
            {
                Console.WriteLine("You have not ordered anything yet.\n");
                return;
            }

            foreach(var val in result)
            {
                Console.WriteLine("Order #" + val.ID.ToString().PadLeft(12 - val.ID.ToString().Length, '0'));
                string D = val.DATE.Replace('.', '/');
                string T = val.TIME.Replace('.', ':');
                Console.WriteLine(D + " " + T);

                int i = 0, n = 0;
                string nums, sequence;
                //TRANSLATE PRESETS
                do
                {
                    //reset for next pizza under presets
                    nums = "";
                    sequence = "";

                    if (val.PRESET.Length > 0 && val.PRESET[0] == '-')
                        break;
                    else if (val.PRESET.Length == 0)
                        break;

                    // Worst O(3)
                    while (i < val.PRESET.Length && Char.IsDigit(val.PRESET[i]))
                    {
                        nums += val.PRESET[i];
                        i++;
                    }

                    if (!int.TryParse(nums, out n))
                        break;

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

                i = 0; n = 0;
                //TRANSLATE CUSTOMS
                do
                {
                    //reset for next pizza under presets
                    nums = "";
                    sequence = "";

                    if (val.CUSTOM.Length > 0 && val.CUSTOM[0] == '-')
                        break;
                    else if (val.CUSTOM.Length == 0)
                        break;

                    // Worst O(3)
                    while (i < val.CUSTOM.Length && Char.IsDigit(val.CUSTOM[i]))
                    {
                        nums += val.CUSTOM[i];
                        i++;
                    }

                    if (!int.TryParse(nums, out n))
                        break;

                    // Worst O(2)
                    while (i < val.CUSTOM.Length && Char.IsLetter(val.CUSTOM[i]))
                    {
                        switch (val.CUSTOM[i])
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

                } while (i < val.CUSTOM.Length);
                
                Console.WriteLine();
            }
        }

        public void Order()
        {
            Orders MyOrder = new Orders();
            Ordertype ODetails = new Ordertype();
            Order_tracker Tracker;
            List<Order_tracker>Q = new List<Order_tracker>();

            MyOrder.Username = Me.Username;

            decimal total_cost = 0.00m;
            int pizzas = 0;
            int preset_amt = 0;
            int custom_amt = 0;

            string p_sequence = "";           //save sequence (holds total order for preset)
            string c_sequence = "";           //save sequence (holds total order for custom)

            //LOCATION
            int count = 0;
            trials = 3;
            do
            {
                count = Locations();
                Console.Write("Pick a location (Enter ID Here): ");
                var locationid = Console.ReadLine();
                Console.Clear();
                if (int.TryParse(locationid, out int id))
                {
                    if (id < 1 || id > count)
                    {
                        if (trials > 1)
                            Console.WriteLine("Please enter a digit according to store IDs displayed below:");
                    }
                    else
                    {
                        MyOrder.StoreId = id;
                        break;
                    }
                }
                else
                {
                    if(trials>1)
                        Console.WriteLine("Please enter a digit according to store IDs displayed below:");
                }

                trials--;

            } while (trials > 0);
            if (trials < 1)
            {
                Console.WriteLine("Sorry, we tend to think inside the box.\nYou can always contact us directly at 7499274992 (PIZZAPIZZA).\n");
                Exit();
                return;
            }

            string answer;
            bool shown, ispreset, iscustom, error;
            var results = from s in DB.Store where s.StoreId == MyOrder.StoreId select s;
            
            bool another;
            do
            {
                answer = "";
                shown = false;
                error = false;
                ispreset = false;
                iscustom = false;
                another = false;

                Tracker.preset = false;
                Tracker.preset_order = "";
                Tracker.size = "";
                Tracker.sauce = "";
                Tracker.crust = "";
                Tracker.cheese = "";
                Tracker.amt = 0;
                Tracker.toppings = new string[toppings_arraylen];
                Tracker.cost = 0.00m;
                for (int i = 0; i < Tracker.toppings.Length; i++)
                    Tracker.toppings[i] = "";

                //PRESET OR CUSTOM
                Console.WriteLine("You are ordering from the " + results.SingleOrDefault().City + " store.\n");
                trials = 3;
                do
                {
                    Console.WriteLine("Would you like a Preset or Custom pizza?");
                    if (!shown)
                    {
                        Console.WriteLine();
                        Console.WriteLine("For menu, just enter \"menu\" at any time.");
                        Console.WriteLine("For additional information, just enter \"info\" or \"help\".");
                        shown = true;
                    }

                    Console.Write("Order (style): ");
                    answer = Console.ReadLine();
                    Console.Clear();
                    Console.WriteLine("You are ordering from the " + results.SingleOrDefault().City + " store.\n");
                    switch (answer.ToLower())
                    {
                        case "menu":
                        case "\"menu\"":
                        case "m":
                            Console.WriteLine("MENU");
                            Menu();
                            break;

                        case "info":
                        case "help":
                        case "\"info\"":
                        case "\"help\"":
                        case "i":
                        case "h":
                            Console.WriteLine("Sure thing...");
                            Console.WriteLine("Our pizzas come in two styles:");
                            Console.WriteLine("1. Preset - Pre-built specialty pizzas designed by our chef");
                            Console.WriteLine("2. Custom - Be a boss and choose your own toppings\n");
                            break;

                        case "preset":
                        case "1. preset":
                        case "1":
                        case "p":
                            ispreset = true;
                            Tracker.preset = true;
                            break;

                        case "custom":
                        case "2. custom":
                        case "2":
                        case "c":
                            iscustom = true;
                            break;

                        default:
                            CLError(answer);
                            break;
                    }

                    // now that Tracker.preset, these are unnecessary variables (deal with it later)!!!
                    if (ispreset || iscustom)
                        break;

                } while (trials > 0);
                if (trials < 1)
                    return;

                //AMOUNT PRESET/CUSTOM
                trials = 3;
                do
                {
                    Console.WriteLine("How many would you like?");
                    Console.Write("Order (enter digit): ");
                    answer = Console.ReadLine();
                    Console.Clear();
                    Console.WriteLine("You are ordering from the " + results.SingleOrDefault().City + " store.\n");
                    if (int.TryParse(answer, out int number))
                    {
                        if(number < 1)
                        {
                            //computecost(); make this a method and return out!!!
                            //or make a jump (take the leap)
                            ComputeOrder(Tracker, Q, total_cost);
                            //add order to db!!!
                            return;
                        }
                        pizzas += number;
                        if (pizzas > 100)
                        {
                            pizzas -= number;

                            if (trials > 1)
                            {
                                Console.Write("Orders limited to 100 pizzas at a time: ");
                                if (100 - pizzas > 0)
                                {
                                    Console.WriteLine(100 - pizzas + " remaining");
                                }

                                error = true;
                            }
                            else
                            {
                                Console.WriteLine("Sorry, we tend to think inside the box.\nYou can always contact us directly at 7499274992 (PIZZAPIZZA).\n");
                                Exit();
                                return;
                            }
                        }
                        else if (ispreset)
                        {
                            preset_amt += number;
                            p_sequence += answer;
                        }
                        else
                        {
                            custom_amt += number;
                            c_sequence += answer;
                        }
                            
                        if(!error)
                        {
                            Tracker.amt = number;
                            break;
                        }
                    }
                    else
                    {
                        if (trials > 1)
                            Console.WriteLine("Please enter a digit amount.");
                        else
                        {
                            CLError(answer);
                            return;
                        }
                    }

                    trials--;
                } while (trials > 0);

                //PRESET ORDER SPECIFIC
                if (ispreset)
                {
                    //PRESET NAME
                    trials = 3;
                    do
                    {
                        Console.WriteLine("Please choose from the menu below:");
                        Console.WriteLine("Preset");
                        Console.WriteLine("1. Vegan | Small $4 | Medium $8 | Large $12");
                        Console.WriteLine("2. Pepperoni | Small $5 | Medium $10 | Large $15");
                        Console.WriteLine("3. BBQ Chicken | Small $6 | Medium $12 | Large $18");
                        Console.WriteLine("4. Meatball | Small $7 | Medium $14 | Large $21");
                        Console.WriteLine("5. Supreme | Small $8 | Medium $16 | Large $24");
                        Console.WriteLine("6. Greek | Small $9 | Medium $18 | Large $27\n");

                        Console.Write("Order (enter digit): ");
                        answer = Console.ReadLine();
                        Console.Clear();
                        Console.WriteLine("You are ordering from the " + results.SingleOrDefault().City + " store.\n");
                        switch (answer.ToLower())
                        {
                            case "1":
                            case "vegan":
                                Tracker.preset_order = "VEGAN";
                                break;

                            case "2":
                            case "pepperoni":
                                Tracker.preset_order = "PEPPERONI";
                                break;

                            case "3":
                            case "bbq":
                            case "chicken":
                            case "bbq chicken":
                                Tracker.preset_order = "BBQ CHICKEN";
                                break;

                            case "4":
                            case "meatball":
                            case "meat":
                                Tracker.preset_order = "MEATBALL";
                                break;

                            case "5":
                            case "supreme":
                                Tracker.preset_order = "SUPREME";
                                break;

                            case "6":
                            case "greek":
                                Tracker.preset_order = "GREEK";
                                break;

                            default:
                                CLError(answer);
                                break;
                        }

                        if (Tracker.preset_order.Length > 0)
                            break;

                    } while (trials > 0);
                    if (trials < 1)
                        return;
                }

                //SIZE
                trials = 3;
                do
                {
                    Console.WriteLine("Would you like that in a Small, Medium, or Large?");
                    Console.Write("Order (size): ");
                    answer = Console.ReadLine();
                    Console.Clear();
                    Console.WriteLine("You are ordering from the " + results.SingleOrDefault().City + " store.\n");
                    switch (answer.ToLower())
                    {
                        case "s":
                        case "sm":
                        case "small":
                            if (ispreset)
                                p_sequence += "S";
                            else
                            {
                                c_sequence += "S";
                                Tracker.cost += Tracker.amt * 3.00m;
                            }

                            Tracker.size = "SMALL";
                            break;

                        case "m":
                        case "md":
                        case "medium":
                            if (ispreset)
                                p_sequence += "M";
                            else
                            {
                                c_sequence += "M";
                                Tracker.cost += Tracker.amt * 6.00m;
                            }

                            Tracker.size = "MEDIUM";
                            break;

                        case "l":
                        case "lg":
                        case "large":
                            if (ispreset)
                                p_sequence += "L";
                            else
                            {
                                c_sequence += "L";
                                Tracker.cost += Tracker.amt * 9.00m;
                            }

                            Tracker.size = "LARGE";
                            break;

                        default:
                            CLError(answer);
                            break;
                    }

                    if (Tracker.size.Length > 0)
                        break;

                } while (trials > 0);
                if (trials < 1)
                    return;

                //IF PRESET, GO AHEAD AND CALCULATE COST
                if (Tracker.preset)
                {
                    switch(Tracker.preset_order)
                    {
                        case "VEGAN":
                            //put the entire switch statement in a method and take in as parameter decimal starting val (V:4.00, P:5.00, etc) and Tracker.size
                            //then just call the method in each case with different arguments
                            switch (Tracker.size)
                            {
                                case "SMALL":
                                    Tracker.cost = Tracker.amt * 4.00m;
                                    break;

                                case "MEDIUM":
                                    Tracker.cost = Tracker.amt * 8.00m;
                                    break;

                                case "LARGE":
                                    Tracker.cost = Tracker.amt * 12.00m;
                                    break;
                            }
                            break;

                        case "PEPPERONI":
                            switch (Tracker.size)
                            {
                                case "SMALL":
                                    Tracker.cost = Tracker.amt * 5.00m;
                                    break;

                                case "MEDIUM":
                                    Tracker.cost = Tracker.amt * 10.00m;
                                    break;

                                case "LARGE":
                                    Tracker.cost = Tracker.amt * 15.00m;
                                    break;
                            }
                            break;

                        case "BBQ CHICKEN":
                            switch (Tracker.size)
                            {
                                case "SMALL":
                                    Tracker.cost = Tracker.amt * 6.00m;
                                    break;

                                case "MEDIUM":
                                    Tracker.cost = Tracker.amt * 12.00m;
                                    break;

                                case "LARGE":
                                    Tracker.cost = Tracker.amt * 18.00m;
                                    break;
                            }
                            break;

                        case "MEATBALL":
                            switch (Tracker.size)
                            {
                                case "SMALL":
                                    Tracker.cost = Tracker.amt * 7.00m;
                                    break;

                                case "MEDIUM":
                                    Tracker.cost = Tracker.amt * 14.00m;
                                    break;

                                case "LARGE":
                                    Tracker.cost = Tracker.amt * 21.00m;
                                    break;
                            }
                            break;

                        case "SUPREME":
                            switch (Tracker.size)
                            {
                                case "SMALL":
                                    Tracker.cost = Tracker.amt * 8.00m;
                                    break;

                                case "MEDIUM":
                                    Tracker.cost = Tracker.amt * 16.00m;
                                    break;

                                case "LARGE":
                                    Tracker.cost = Tracker.amt * 24.00m;
                                    break;
                            }
                            break;

                        case "GREEK":
                            switch (Tracker.size)
                            {
                                case "SMALL":
                                    Tracker.cost = Tracker.amt * 9.00m;
                                    break;

                                case "MEDIUM":
                                    Tracker.cost = Tracker.amt * 18.00m;
                                    break;

                                case "LARGE":
                                    Tracker.cost = Tracker.amt * 27.00m;
                                    break;
                            }
                            break;
                    }
                }

                //CRUST
                trials = 3;
                do
                {
                    Console.WriteLine("Crust Thick or Thin?");
                    Console.Write("Order (crust): ");
                    answer = Console.ReadLine();
                    Console.Clear();
                    Console.WriteLine("You are ordering from the " + results.SingleOrDefault().City + " store.\n");
                    switch (answer.ToLower())
                    {
                        case "1":
                        case "thick":
                        case "thick crust":
                            if (ispreset)
                                p_sequence += "k";
                            else
                                c_sequence += "k";

                            Tracker.crust = "THICK";
                            break;

                        case "2":
                        case "thin":
                        case "thin crust":
                            if (ispreset)
                                p_sequence += "n";
                            else
                                c_sequence += "n";

                            Tracker.crust = "THIN";
                            break;

                        default:
                            CLError(answer);
                            break;
                    }

                    if (Tracker.crust.Length > 0)
                        break;

                } while (trials > 0);
                if (trials < 1)
                    return;

                //CUSTOM ORDER SPECIFIC
                if (iscustom)
                {
                    //SAUCE
                    trials = 3;
                    do
                    {
                        Console.WriteLine("Choose a sauce:");
                        Console.WriteLine("1. Traditional $0.50");
                        Console.WriteLine("2. BBQ $1");
                        Console.WriteLine("3. Alfredo $1\n");

                        Console.Write("Order (enter digit): ");
                        answer = Console.ReadLine();
                        Console.Clear();
                        Console.WriteLine("You are ordering from the " + results.SingleOrDefault().City + " store.\n");
                        switch (answer.ToLower())
                        {
                            case "1":
                            case "traditional":
                            case "trad":
                                Tracker.sauce = "TRADITIONAL";
                                Tracker.cost += Tracker.amt * 0.50m;
                                break;

                            case "2":
                            case "bbq":
                            case "barbeque":
                                Tracker.sauce = "BBQ";
                                Tracker.cost += Tracker.amt * 1.00m;
                                break;

                            case "3":
                            case "alfredo":
                            case "fredo":
                                Tracker.sauce = "ALFREDO";
                                Tracker.cost += Tracker.amt * 1.00m;
                                break;

                            default:
                                CLError(answer);
                                break;
                        }

                        if (Tracker.sauce.Length > 0)
                            break;

                    } while (trials > 0);
                    if (trials < 1)
                        return;

                    //CHEESE
                    trials = 3;
                    do
                    {
                        Console.WriteLine("Pick a cheese:");
                        Console.WriteLine("0. None");
                        Console.WriteLine("1. Regular $0.50");
                        Console.WriteLine("2. Goat (Greek) $1");
                        Console.WriteLine("3. Queso $2\n");

                        Console.Write("Order (enter digit): ");
                        answer = Console.ReadLine();
                        Console.Clear();
                        Console.WriteLine("You are ordering from the " + results.SingleOrDefault().City + " store.\n");
                        switch (answer.ToLower())
                        {
                            case "":
                            case "0":
                            case "none":
                            case "no":
                            case "no cheese":
                            case "no thanks":
                            case "no thankyou":
                            case "no thank you":
                                Tracker.cheese = "NONE";
                                break;

                            case "1":
                            case "regular":
                            case "reg":
                                Tracker.cheese = "REGULAR";
                                Tracker.cost += Tracker.amt * 0.50m;
                                break;

                            case "2":
                            case "goat":
                            case "greek":
                            case "goat (greek)":
                            case "goat greek":
                            case "greek goat":
                                Tracker.cheese = "GOAT";
                                Tracker.cost += Tracker.amt * 1.00m;
                                break;

                            case "3":
                            case "queso":
                            case "queso fresco":
                            case "fresco":
                                Tracker.cheese = "QUESO";
                                Tracker.cost += Tracker.amt * 2.00m;
                                break;

                            default:
                                CLError(answer);
                                break;
                        }

                        if (Tracker.cheese.Length > 0)
                            break;

                    } while (trials > 0);
                    if (trials < 1)
                        return;

                    //TOPPINGS
                    trials = 3;
                    do
                    {
                        Console.WriteLine("Pick your toppings.");
                        Console.WriteLine("For multiple toppings, simply enter a stream of digits up to four.");
                        Console.WriteLine("For any less, you will need to press enter twice:");
                        Console.WriteLine("0. None");
                        Console.WriteLine("1. Veggies/Fruit $0.50");
                        Console.WriteLine("2. Pepperoni $1");
                        Console.WriteLine("3. Chicken $1");
                        Console.WriteLine("4. Meatballs $3\n");

                        Console.Write("Order (enter digit/s): ");
                        answer = Console.ReadLine();
                        Console.Clear();
                        Console.WriteLine("You are ordering from the " + results.SingleOrDefault().City + " store.\n");

                        if (answer.Length==0 || answer[0] == '0')
                            break;
                        
                        int i = 0;
                        while ((i < answer.Length) && (i < toppings_arraylen))
                        {
                            if (Char.IsDigit(answer[i]))
                            {
                                switch (answer[i])
                                {
                                    case '1':
                                        Tracker.toppings[i] = "VEGGIES/FRUIT";
                                        Tracker.cost += Tracker.amt * 0.50m;
                                        break;

                                    case '2':
                                        Tracker.toppings[i] = "PEPPERONI";
                                        Tracker.cost += Tracker.amt * 1.00m;
                                        break;

                                    case '3':
                                        Tracker.toppings[i] = "CHICKEN";
                                        Tracker.cost += Tracker.amt * 1.00m;
                                        break;

                                    case '4':
                                        Tracker.toppings[i] = "MEATBALLS";
                                        Tracker.cost += Tracker.amt * 3.00m;
                                        break;

                                    default:
                                        CLError(answer);
                                        break;
                                }

                                if (Tracker.toppings[i].Length == 0)
                                    break;

                            }
                            else
                            {
                                CLError(answer);
                                break;
                            }

                            i++;
                        }

                        if (Tracker.toppings[toppings_arraylen - 1].Length > 0)
                            break;

                    } while (trials > 0);
                    if (trials < 1)
                        return;
                }

                if(pizzas < 100)
                {
                    //CALCULATE CURRENT COST & SAVE STATE OF VARS for PRINTOUT
                    Console.WriteLine("Should I mark this order as complete?");
                    Console.Write("Enter Y/N: ");
                    answer = Console.ReadLine();
                    Console.Clear();
                    switch (answer.ToLower())
                    {
                        case "n":
                        case "no":
                        case "not yet":
                        case "nope":
                            Console.WriteLine("Wow, this must be some good pizza!");
                            another = true;
                            break;

                        case "y":
                        case "yes":
                        case "sure":
                        case "ok":
                        case "k":
                        case "okay":
                        case "alright":
                            break;

                        default:
                            Console.Write("I'm going to take that as a no ");
                            Loading('.', 3);
                            break;
                    }
                }

                total_cost += Tracker.cost;
                Q.Add(Tracker);

                if (total_cost > 250.00m)
                {
                    Console.Clear();
                    trials = 3;
                    do
                    {
                        Console.WriteLine("Orders can not exceed $250.");
                        Console.WriteLine("Choose which items you would like to remove:\n");

                        for (int i = 0; i < Q.Count; i++)
                        {
                            if (Q[i].preset)
                                Console.WriteLine(i + 1 + ": (" + Q[i].amt + ") " + Q[i].size + ", " + Q[i].crust + " " + Q[i].preset_order);
                            else
                            {
                                Console.WriteLine(i + 1 + ": (" + Q[i].amt + ") " + Q[i].size + ", " + Q[i].crust + " " + Q[i].cheese + " on " + Q[i].sauce);
                                Console.Write("Toppings: ");
                                for (int j = 0; j < Q[i].toppings.Length; j++)
                                {
                                    Console.Write(Q[i].toppings[j]);

                                    if (j < Q[i].toppings.Length - 1)
                                        Console.Write(", ");
                                }
                                Console.WriteLine();
                            }
                            Console.WriteLine("$" + Tracker.cost + "\n");
                        }

                        Console.WriteLine("Total: $" + total_cost);
                        Console.Write("Remove (Enter Digit): ");
                        answer = Console.ReadLine();
                        Console.Clear();
                        if (int.TryParse(answer, out int num) && num > 0 && num <= Q.Count)
                        {
                            total_cost -= Q[num - 1].cost;
                            Q.RemoveAt(num - 1);
                        }
                        else
                        {
                            if (trials > 1)
                                Console.WriteLine("Please enter a valid digit according to the list below:");
                            else
                            {
                                Console.WriteLine("Sorry, we tend to think inside the box.\nYou can always contact us directly at 7499274992 (PIZZAPIZZA).\n");
                                Exit();
                                return;
                            }

                            trials--;
                        }
                    } while (total_cost > 250.00m);

                    if (total_cost == 0.00m)
                        another = true;
                }
            } while (another);

            if (promo)
                sales_price = total_cost*0.50m;     //try not to hardcode this for next update (KeyValues<discounts,bools>)

            //PRINT EACH FROM THE QUEUE
            ComputeOrder(Tracker, Q, total_cost);

            if(p_sequence.Length < 1)
            {
                p_sequence = "-";
            }
            else if(c_sequence.Length < 1)
            {
                c_sequence = "-";
            }

            //STORE AND PRINT
            DB.Add(MyOrder);
            DB.SaveChanges();

            var inner = from ot in DB.Ordertype select ot.OrderId;
            var outer = from o in DB.Orders where !inner.Contains(o.OrderId) select o;
            ODetails.OrderId = outer.FirstOrDefault().OrderId;
            ODetails.Preset = p_sequence;
            ODetails.Custom = c_sequence;
            DateTime date = DateTime.Now;
            ODetails.Dt = date.ToString("MM/dd/yyyy");
            ODetails.Tm = date.ToString("HH:mm");
            DB.Add(ODetails);
            DB.SaveChanges();

            Inventory Remainder = new Inventory();
            var ret = from i in DB.Inventory where i.StoreId == MyOrder.StoreId select i;
            if(ret.Count() > 0)
            {
                ret.SingleOrDefault().Preset -= preset_amt;
                ret.SingleOrDefault().Custom -= custom_amt;
            }

            DB.SaveChanges();
        }

        public void ComputeOrder(Order_tracker Tracker, List<Order_tracker> Q, decimal total_cost)
        {
            Console.Clear();
            Console.WriteLine("Order Confirmed\n");

            for (int i = 0; i < Q.Count; i++)
            {
                Tracker = Q[i];

                if (Tracker.preset)
                    Console.WriteLine("(" + Tracker.amt + ") " + Tracker.size + ", " + Tracker.crust + " " + Tracker.preset_order);
                else
                {
                    Console.WriteLine("(" + Tracker.amt + ") " + Tracker.size + ", " + Tracker.crust + " " + Tracker.cheese + " on " + Tracker.sauce);
                    Console.Write("Toppings: ");
                    for (int j = 0; j < Tracker.toppings.Length; j++)
                    {
                        Console.Write(Tracker.toppings[j]);

                        if (j < Tracker.toppings.Length - 1)
                            Console.Write(", ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("$" + Tracker.cost + "\n");
            }

            if(promo)
            {
                Console.WriteLine("Pre-discount Subtotal: $" + total_cost);
                Console.WriteLine("Discount Total: $" + sales_price);
                promo = false;  //may be the wrong place, not much time to trace
            }
            else
                Console.WriteLine("Total: $" + total_cost);
        }
        public void Session(bool sale)
        {
            promo = sale;
            string answer;
            Console.Write("Enter Command (say Info for help): ");
            answer = Console.ReadLine();
            Console.Clear();
            switch (answer.ToLower())
            {
                case "info":
                case "help":
                    Console.WriteLine("INFO");
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
                    Console.WriteLine("STORE LOCATIONS");
                    Locations();
                    break;

                case "history":
                case "recent":
                case "my orders":
                case "orders":
                    Console.WriteLine("RECENT ORDERS");
                    History();
                    break;

                case "menu":
                    Console.WriteLine("MENU");
                    Menu();
                    break;

                case "order":
                    Console.WriteLine("MAKE AN ORDER");
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
            Console.WriteLine("Add Store - Add a new store to the list");
            Console.WriteLine("Clear - Clear screen");
            Console.WriteLine("Exit - Log out and exit application\n");
        }

        public void Inventory()
        {
            //Query ORDER table for inventory
            Console.WriteLine("INVENTORY");
            int tried = trials = 3;
            int id = 0;
            do
            {
                Console.WriteLine("Please select your location:");
                int count = Locations();
                Console.Write("Store ID (enter digit): ");
                string locationid = Console.ReadLine();
                Console.Clear();
                if (int.TryParse(locationid, out id))
                {
                    if (id < 1 || id > count)
                    {
                        if (trials > 1)
                        {
                            Console.WriteLine("Please enter a digit according to store IDs displayed below:");
                            trials--;
                        }
                        else
                        {
                            Console.WriteLine("You have failed to identify your shop " + tried + " times.");
                            Exit();
                            return;
                        }
                    }
                    else
                        break;
                }
            } while (trials > 0);

            var query = from i in DB.Inventory
                        where i.StoreId == id
                        select i;

            if(query.Count() > 0)
            {
                Console.WriteLine("Store #" + id.ToString().PadLeft(12 - id.ToString().Length, '0'));
                Console.WriteLine("Has enough ingredients for " + query.SingleOrDefault().Preset + " Presets");
                Console.WriteLine("Has enough ingredients for " + query.SingleOrDefault().Custom + " Customs");
            }
            else
                Console.WriteLine("Inventory is currently unavailable.");
  
            Console.WriteLine();
        }

        public void History()
        {
            Console.WriteLine("STORE ORDERS");
            IDictionary<string, int> Map = new Dictionary<string, int>();
            var result = from o in DB.Orders
                         join ot in DB.Ordertype
                         on o.OrderId equals ot.OrderId
                         select new
                         {
                             ID = o.OrderId,
                             USER = o.Username,
                             PRESET = ot.Preset,
                             CUSTOM = ot.Custom,
                             DATE = ot.Dt,
                             TIME = ot.Tm
                         };

            foreach (var val in result)
            {
                Console.WriteLine("Order #" + val.ID.ToString().PadLeft(12 - val.ID.ToString().Length, '0'));
                string D = val.DATE.Replace('.', '/');
                string T = val.TIME.Replace('.', ':');
                Console.WriteLine(D + " " + T);

                int i = 0, n = 0;
                string nums, sequence;
                //TRANSLATE PRESETS
                do
                {
                    //reset for next pizza under presets
                    nums = "";
                    sequence = "";

                    if (val.PRESET.Length > 0 && val.PRESET[0] == '-')
                        break;
                    else if (val.PRESET.Length == 0)
                        break;

                    // Worst O(3)
                    while (i < val.PRESET.Length && Char.IsDigit(val.PRESET[i]))
                    {
                        nums += val.PRESET[i];
                        i++;
                    }

                    if (!int.TryParse(nums, out n))
                        break;

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

                i = 0; n = 0;
                //TRANSLATE CUSTOMS
                do
                {
                    //reset for next pizza under presets
                    nums = "";
                    sequence = "";

                    if (val.CUSTOM.Length > 0 && val.CUSTOM[0] == '-')
                        break;
                    else if (val.CUSTOM.Length == 0)
                        break;

                    // Worst O(3)
                    while (i < val.CUSTOM.Length && Char.IsDigit(val.CUSTOM[i]))
                    {
                        nums += val.CUSTOM[i];
                        i++;
                    }

                    if (!int.TryParse(nums, out n))
                        break;

                    // Worst O(2)
                    while (i < val.CUSTOM.Length && Char.IsLetter(val.CUSTOM[i]))
                    {
                        switch (val.CUSTOM[i])
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

                } while (i < val.CUSTOM.Length);

                Console.WriteLine();
            }
        }

        public void AddStore()
        {
            int trials = 3;
            bool flag = false;
            do
            {
                Console.WriteLine("STORE DETAILS");
                if(flag)
                    Console.WriteLine("*255 character limit");
                Console.Write("Enter City: ");
                string city = Console.ReadLine();
                if (flag)
                    Console.WriteLine("*2 character abbreviations only");
                Console.Write("Enter State (e.g TX): ");
                string state = Console.ReadLine();
                if (flag)
                    Console.WriteLine("*5 to 10 digit zip code");
                Console.Write("Enter Zip: ");
                string zip = Console.ReadLine();

                if (city.Length > 255 || state.Length < 2 || state.Length > 2 || zip.Length < 5 || zip.Length > 10)
                {
                    if (trials == 1)
                        Exit();

                    Console.Clear();
                    flag = true;
                    trials--;
                }
                else
                    break;

            } while (trials > 0);

            Console.WriteLine();
            Console.WriteLine("Thank you for your request. Your DB Admin will get in touch with you shortly.\n");

        }

        public int Session()
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
                case "shops":
                    Locations();
                    break;

                case "history":
                    History();
                    break;

                case "inventory":
                    Inventory();
                    break;

                case "order":
                    if (Authenticator())
                        return 1;
                    break;

                case "add":
                case "create":
                case "add store":
                case "create store":
                    AddStore();
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
            return 0;
        }

        bool Authenticator()
        {
            Console.WriteLine("As an employee, you agree to be held liable for any misuse of the following information:");
            Console.WriteLine("1. Agree");
            Console.WriteLine("2. Disagree");
            Console.WriteLine();
            Console.Write("Your answer: ");
            string answer = Console.ReadLine();
            bool pass = false;
            switch(answer.ToLower())
            {
                case "1":
                case "1. agree":
                case "agree":
                case "agreed":
                case "yes":
                case "ok":
                case "okay":
                case "sure":
                case "of course":
                case "i agree":
                case "i do":
                case "yes i do":
                    pass = true;
                    break;

                default:
                    break;
            }

            return pass;
        }
    }
}
