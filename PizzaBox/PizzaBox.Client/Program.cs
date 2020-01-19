using System;
using PizzaBox.Domain;
using System.Threading;
using PizzaBox.Domain.Models;

namespace PizzaBox.Client
{
    class Program
    {
        static InitRunAssets assets = new InitRunAssets();
        static bool session;

        /// <summary>
        /// - Runs while user is signed out.
        /// </summary>
        static void InitRun()
        {
            //InitRunAssets assets = new InitRunAssets();  hide until DB ready
            string answer;

            Console.Write("Enter Command (say Info for help): ");
            answer = Console.ReadLine();
            switch (answer.ToLower())
            {
                case "info":
                case "help":
                    Console.WriteLine();
                    assets.Info();
                    break;

                case "logon":
                case "login":
                case "signin":
                case "log on":
                case "sign in":
                    Console.WriteLine();
                    assets.Login();
                    session = assets.testSignin.session;
                    break;

                case "sign up":
                case "signup":
                    Console.WriteLine();
                    assets.Signup();
                    session = assets.member.session;
                    break;

                case "clearscreen":
                case "clear":
                case "cls":
                    Console.Clear();
                    break;

                case "exit":
                    Console.WriteLine();
                    assets.Exit();
                    break; //redundant

                default:
                    assets.CLError(answer);
                    break;
            }
        }

        static void Session()
        {
            Console.WriteLine("Session is currently active");
        }

        static void Main()
        {
            string header1 = "Welcome to Pizza Box";
            string header2 = "Please sign in to your account to get started.\nIf you don't have an account, sign up with us now and get 50% off your first order!";
            bool shown = false;
            do
            {
                Console.WriteLine(header1);
                if (!shown)
                {
                    Console.WriteLine();
                    Console.WriteLine(header2);
                    shown = true;
                }

                InitRun();
            } while (!session);

            Session();
        }
    }
}
