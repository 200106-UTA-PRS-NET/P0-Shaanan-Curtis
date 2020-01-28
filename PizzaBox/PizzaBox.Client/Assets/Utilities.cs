using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using PizzaBox.Client;

namespace PizzaBox.Client.Assets
{
    internal class Utilities
    {
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
        internal static void CLError(string entry)
        {
            switch (PizzaBox.Client.Assets.Run.trials)
            {
                case 1:
                    Console.WriteLine("Sorry, we tend to think inside the box.\nYou can always contact us directly at 7499274992 (PIZZAPIZZA).\n");
                    Exit();
                    break;

                default:
                    Console.WriteLine("There is no " + entry + " command. Try again.\n");
                    break;
            }

            PizzaBox.Client.Assets.Run.trials--;
        }
        internal static void Error(string typerr)

        {
            switch (PizzaBox.Client.Assets.Run.trials)
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

            PizzaBox.Client.Assets.Run.trials--;
        }
        internal static void Exit()
        {
            Console.Write("Thanks for trying Pizza Box ");
            Loading('~', 3, 1);
            System.Environment.Exit(0);
        }
    }
}
