using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PizzaBox.Domain.Abstract;
using PizzaBox.Domain.Init;
using PizzaBox.Storing.Entities;
using PizzaBox.Storing.Interface;

namespace PizzaBox.Domain.Classes
{
    public class EmployeeSession : AbstractSession
    {
        public override void Info()
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
        public override void History()
        {
            using IPizzaboxRepository pizzaboxRepository = DbOptions.CreatePizzaboxRepository();

            Console.WriteLine("1. Recent Orders by Shop");
            Console.WriteLine("2. All Orders");
            Console.Write("Please make a selection (enter digit): ");
            string answer = Console.ReadLine();
            Console.Clear();
            bool all = false;
            switch (answer.ToLower())
            {
                case "1":
                case "recent orders by shop":
                case "by shop":
                case "shop":
                case "store":
                case "store orders":
                    all = false;
                    break;

                case "2":
                case "all orders":
                case "all":
                case "everything":
                case "all stores":
                case "all shops":
                case "all locations":
                    all = true;
                    break;

                default:
                    Console.WriteLine("Invalid selection.");
                    return;
            }

            IEnumerable<Orders> results;

            if (all)
            {
                Console.WriteLine("ORDER HISTORY AT ALL LOCATIONS");
                results = pizzaboxRepository.GetOrdersBy("", "all");
            }
            else
            {
                #region ADMIN INPUT STORE LOCATION
                string locationid = "";
                int count = 0;
                int id = 0;
                trials = 3;
                do
                {
                    count = Locations();
                    Console.Write("Pick a location (Enter ID Here): ");
                    locationid = Console.ReadLine();
                    Console.Clear();
                    if (int.TryParse(locationid, out id))
                    {
                        if (id < 1 || id > count)
                        {
                            if (trials > 1)
                                Console.WriteLine("Please enter a digit according to store IDs displayed below:");
                        }
                        else
                            break;
                    }
                    else
                    {
                        if (trials > 1)
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
                #endregion

                var store = pizzaboxRepository.GetStoreById(id);
                Console.WriteLine("ORDERS AT " + store.City.ToUpper() + " SHOP");

                results = pizzaboxRepository.GetOrdersBy(locationid, "store");
            }

            if (results.Count() < 1)
            {
                Console.WriteLine("No orders have been made.\n");
                return;
            }

            Orders[] O = new Orders[results.Count()];
            Ordertype[] OT = new Ordertype[results.Count()];
            int[] ids = new int[results.Count()];

            #region Initialization of O, OT, and ids
            int i;
            for (i = 0; i < results.Count(); i++)
            {
                O[i] = new Orders()
                {
                    OrderId = 0,
                    StoreId = 0,
                    Username = ""
                };
                OT[i] = new Ordertype()
                {
                    OrderId = 0,
                    Preset = "",
                    Custom = "",
                    Dt = "",
                    Tm = ""
                };
                ids[i] = new int();
            }
            #endregion

            #region Map Orders results
            i = 0;
            foreach (var val in results)
            {
                O[i] = val;
                ids[i] = val.OrderId;

                #region NOTE:
                ///NOTE: DataReader can not use result from Entity1 query while querying data in Entity2
                #endregion
                i++;
            }

            for (i = 0; i < results.Count(); i++)
            {
                OT[i] = pizzaboxRepository.GetOrdertypeById(ids[i]);

                #region OT[i] can be assumed != null as long as developers don't mess with it.  Make sure they can't
                /*
                if (OT[i] == null)
                {
                    //handle it
                }
                */
                #endregion
            }
            #endregion

            #region Display Full List of Recent Orders
            i = 0;
            while (i < results.Count())
            {
                int j = 0, n = 0;
                string nums, sequence;

                ///DISPLAY ORDER# AND DATE/TIME
                Console.WriteLine("Order #" + O[i].OrderId.ToString().PadLeft(12 - O[i].OrderId.ToString().Length, '0'));
                string D = OT[i].Dt.Replace('.', '/');
                string T = OT[i].Tm.Replace('.', ':');
                Console.WriteLine(D + " " + T);

                ///PRESET TRANSLATION TO HUMAN-READABLE FORMAT
                do
                {
                    ///RESET TRANSLATED SEQUENCE AFTER EACH PRINT
                    nums = "";
                    sequence = "";

                    if ((OT[i].Preset.Length > 0 && OT[i].Preset[0] == '-') || OT[i].Preset.Length == 0)
                        break;

                    ///GET FULL DIGIT FROM ORIGINAL SEQUENCE UNTIL ISLETTER - Worst O(3)
                    while (j < OT[i].Preset.Length && Char.IsDigit(OT[i].Preset[j]))
                    {
                        nums += OT[i].Preset[j];
                        j++;
                    }

                    //NUMS IS NOT A NUMBER? SKIP
                    if (!int.TryParse(nums, out n))
                        break;

                    ///GET FULL CHAR SEQUENCE FROM ORIGINAL UNTIL NEXT DIGIT/END - Worst O(2)
                    while (j < OT[i].Preset.Length && Char.IsLetter(OT[i].Preset[j]))
                    {
                        switch (OT[i].Preset[j])
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

                        j++;
                    }

                    ///PRINT AMOUNT AND ORDER IN READABLE FORMAT
                    Console.WriteLine(Convert.ToInt32(nums) + " " + sequence);

                } while (j < OT[i].Preset.Length);

                ///CUSTOM TRANSLATION TO HUMAN-READABLE FORMAT
                j = 0; n = 0;
                do
                {
                    ///RESET TRANSLATED SEQUENCE AFTER EACH PRINT
                    nums = "";
                    sequence = "";

                    if ((OT[i].Custom.Length > 0 && OT[i].Custom[0] == '-') || OT[i].Custom.Length == 0)
                        break;

                    ///GET FULL DIGIT FROM ORIGINAL SEQUENCE UNTIL ISLETTER - Worst O(3)
                    while (j < OT[i].Custom.Length && Char.IsDigit(OT[i].Custom[j]))
                    {
                        nums += OT[i].Custom[j];
                        j++;
                    }

                    if (!int.TryParse(nums, out n))
                        break;

                    ///GET FULL CHAR SEQUENCE FROM ORIGINAL UNTIL NEXT DIGIT/END - Worst O(2)
                    while (j < OT[i].Custom.Length && Char.IsLetter(OT[i].Custom[j]))
                    {
                        switch (OT[i].Custom[j])
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

                        j++;
                    }

                    ///PRINT AMOUNT AND ORDER IN READABLE FORMAT
                    Console.WriteLine(Convert.ToInt32(nums) + " " + sequence);

                } while (j < OT[i].Custom.Length);

                Console.WriteLine();
                i++;
            }
            #endregion
        }
        public override int Session()
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

                case "store orders":
                case "store history":
                case "shop orders":
                case "shop history":
                case "recent orders":
                case "recent history":
                case "history":
                case "orders":
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

        public void Inventory()
        {
            using IPizzaboxRepository pizzaboxRepository = DbOptions.CreatePizzaboxRepository();

            string locationid = "";
            int tried = trials = 3;
            int id = 0;
            do
            {
                Console.WriteLine("Please select your location:");
                int count = Locations();
                Console.Write("Store ID (enter digit): ");
                locationid = Console.ReadLine();
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

            var query = pizzaboxRepository.GetInventoryByStore(id);
            var store = pizzaboxRepository.GetStoreById(id);
            if (query == null)
            {
                if (store == null)
                {
                    Console.WriteLine("There is no such store.");
                    return;
                }

                Console.WriteLine("Inventory at location #" + store.StoreId + " is currently unavailable. Please contact your administrator.");
                return;
            }

            Console.WriteLine("Store #" + id.ToString().PadLeft(12 - id.ToString().Length, '0'));
            Console.WriteLine("Has enough ingredients for " + query.Preset + " Presets");
            Console.WriteLine("Has enough ingredients for " + query.Custom + " Customs");

            Console.WriteLine();
        }
        public void AddStore()
        {
            int trials = 3;
            bool flag = false;
            do
            {
                Console.WriteLine("STORE DETAILS");
                if (flag)
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
        bool Authenticator()
        {
            Console.WriteLine("As an employee, you agree to be held liable for any misuse of the following information:");
            Console.WriteLine("1. Agree");
            Console.WriteLine("2. Disagree");
            Console.WriteLine();
            Console.Write("Your answer: ");
            string answer = Console.ReadLine();
            Console.Clear();
            bool pass = false;
            switch (answer.ToLower())
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
