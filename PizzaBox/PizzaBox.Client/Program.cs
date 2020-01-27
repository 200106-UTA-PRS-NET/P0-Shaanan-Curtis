using PizzaBox.Domain.Init;
using PizzaBox.Storing.Entities;
using PizzaBox.Storing.Interface;
using System;
using System.Threading;

namespace PizzaBox.Client
{
    class Program
    {
        //static DbOptions Database = new DbOptions();
        //static pizzaboxContext DB = new pizzaboxContext(Database.options);
        static InitRunAssets Assets = new InitRunAssets();
        static User CurrentUser = new User();
        static sbyte alive;
        public static bool exit_proc;
        private static bool priority = false;
        public static bool sale = false;

        //none were static
        #region INIT
        private static int trials;
        public static void Info()
        {
            Console.WriteLine("INFO");
            Console.WriteLine("Info - Display a list of available commands. This command is always available.");
            Console.WriteLine("Login - Username and password entry.");
            Console.WriteLine("Sign Up - Create a new account.");
            Console.WriteLine("Clear - Clear screen.");
            Console.WriteLine("Exit - You'll be back.\n");
        }
        public static User Login()
        {
            using IPizzaboxRepository pizzaboxRepository = DbOptions.CreatePizzaboxRepository();
            User Me = new User();
            User AuthorizedUser = new User();
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
                AuthorizedUser = pizzaboxRepository.UserAuthentication(Me.Username, Me.Pass);
                if (AuthorizedUser == null)
                {
                    Console.Clear();
                    Error("authentication");
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

            /*
                if (DB.User.Where(u => u.Username.Contains(Me.Username)).Count() > 0 && DB.User.Where(p => p.Pass.Contains(Me.Pass) && p.Username.Contains(Me.Username)).Count() > 0)
                {
                    
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
                        
                        //Error("Session");
                        //break;
                        
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
            */
        }
        public static User Login(string access_level, string name)
        {
            User Me = new User();

            if (access_level == "sa-admin")
                Console.WriteLine($"Special Access - Authorized Login for {name}.\n");

            Me = Login();

            return Me;
        }
        public static User Signup()
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
                        Error("exists");
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
                    Error("Entry");

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
                            Error("exists");
                        /*
                        else if (DB.User.Any(u => u.Username == newUser.Username))
                            Error("exists");
                        */
                        else
                            pass = true;
                        break;
                }

                if (newUser.Username.Length == 0)
                    Error("user");

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
            Loading('.', 3, 3);
            Console.WriteLine();
            #endregion

            newUser.SessionLive = 1;
            pizzaboxRepository.AddUser(newUser);
            return newUser;
        }
        #endregion

        //loading was static
        #region INIT UTILITIES
        private static void Loading(char symbol, int len, int duration)
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
        public static void Exit()
        {
            Console.Write("Thanks for trying Pizza Box ");
            Loading('~', 3, 1);
            System.Environment.Exit(0);
        }
        public static void CLError(string entry)
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
        private static void Error(string typerr)

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
        #endregion


        /// <summary>
        /// - Runs while user is signed out.
        /// </summary>
        static void InitRun()
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
                    //Assets.Info();
                    break;

                case "logon":
                case "login":
                case "signin":
                case "log on":
                case "log in":
                case "sign on":
                case "sign in":
                    CurrentUser = Login();
                    alive = CurrentUser.SessionLive;
                    //CurrentUser = Assets.Login();
                    //alive = CurrentUser.SessionLive;
                    break;

                case "sign up":
                case "signup":
                    CurrentUser = Signup();
                    alive = CurrentUser.SessionLive;
                    //CurrentUser = Assets.Signup();
                    //alive = CurrentUser.SessionLive;
                    sale = true;
                    break;

                case "clear screen":
                case "clearscreen":
                case "clear":
                case "cls":
                    Console.Clear();
                    break;

                case "exit":
                    Exit();
                    //Assets.Exit();
                    break;

                default:
                    CLError(answer);
                    //Assets.CLError(answer);
                    break;
            }
        }
        /*
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
                        PBCustomer.Session(sale);
                    alive = PBCustomer.Me.SessionLive;
                    exit_proc = PBCustomer.Exits;
                    break;
            }

            return 0;
        }
        */
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

                        InitRun();
                    }
                    else
                    {
                        CurrentUser = Login("sa-admin", "Tommy");
                        alive = CurrentUser.SessionLive;
                        //CurrentUser = Assets.Login("sa-admin");
                        //alive = CurrentUser.SessionLive;
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
                    //if (Session() == 1)
                    //priority = true;
                }
                Console.Clear();
            }
            Exit();
            //Assets.Exit();
        }
        static void Main()
        {
            DbOptions options = new DbOptions();
            using IPizzaboxRepository pizzaboxRepository = DbOptions.CreatePizzaboxRepository();
            UI(pizzaboxRepository);
        }
    }
}
