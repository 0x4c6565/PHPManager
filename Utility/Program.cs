using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Web.Management.PHP.Utility
{
    //TODO utilise resource for languages
    class Program
    {
        static void Main(string[] args)
        {
            Arguments arguments = new Arguments(args);
            if (!string.IsNullOrEmpty(arguments["help"]))
            {
                Console.WriteLine(GetHelp());
                Environment.Exit(0);
            }
            if (string.IsNullOrEmpty(arguments["action"]))
            {
                Console.Error.WriteLine("Must specify action");
                Environment.Exit(1);
            }

            Action action = Action.Install;
            try
            {
                action = (Action)Enum.Parse(typeof(Action), arguments["action"], true);
            }
            catch (ArgumentException)
            {
                Console.Error.WriteLine("Invalid action specified [{0}]{1}{1}{2}", arguments["action"], Environment.NewLine, GetHelp());
                Environment.Exit(2);
            }

            try
            {
                ProcessAction(action);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Failed to process action{0}{1}{0}{0}", Environment.NewLine, ex, GetHelp());
                Environment.Exit(3);
            }

            Console.WriteLine("Finished processing");
        }

        enum Action
        {
            Install,
            Uninstall
        }

        private static void ProcessAction(Action action)
        {
            switch (action)
            {
                case Action.Install:
                    
                    InstallAction.Install();
                    break;
                    
                case Action.Uninstall:
                    
                    InstallAction.Uninstall();
                    break;

            }
        }

        private static string GetHelp()
        {
            return "Usage: utility /action <Install|Uninstall>";
        }
    }
}

