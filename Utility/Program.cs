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
                Console.Error.WriteLine("Invalid action specified [{0}]", arguments["action"]);
                Environment.Exit(2);
            }

            try
            {
                ProcessAction(action);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Failed to process action{0}{0}{1}", Environment.NewLine, ex);
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

                    Console.WriteLine("Initialising installation..");

                    var assembly = Assembly.GetExecutingAssembly();
                    var assemblyName = assembly.GetName();
                    var assemblyFullName = assemblyName.FullName;
                    var clientAssemblyFullName = assemblyFullName.Replace(assemblyName.Name, "Web.Management.PHP");

                    Console.WriteLine("Checking for previous versions of PHPManager..");
                    RemoveUIModuleProvider("PHP"); // This is necessary for the upgrade scenario

                    Console.WriteLine("Installing PHPManager module..");
                    AddUIModuleProvider("PHP", "Web.Management.PHP.PHPProvider, " + clientAssemblyFullName);
                    break;
                    
                case Action.Uninstall:

                    RemoveUIModuleProvider("PHP");
                    break;

            }
        }

        private static string GetHelp()
        {
            return "Usage: utility /action <Install|Uninstall>";
        }

        public static void AddUIModuleProvider(string name, string type)
        {
            using (var mgr = new ServerManager())
            {

                // First register the Module Provider  
                var adminConfig = mgr.GetAdministrationConfiguration();

                var moduleProvidersSection = adminConfig.GetSection("moduleProviders");
                var moduleProviders = moduleProvidersSection.GetCollection();
                if (FindByAttribute(moduleProviders, "name", name) == null)
                {
                    var moduleProvider = moduleProviders.CreateElement();
                    moduleProvider.SetAttributeValue("name", name);
                    moduleProvider.SetAttributeValue("type", type);

                    Console.WriteLine("Adding module provider..");
                    moduleProviders.Add(moduleProvider);
                }

                // Now register it so that all Sites have access to this module 
                var modulesSection = adminConfig.GetSection("modules");
                var modules = modulesSection.GetCollection();
                if (FindByAttribute(modules, "name", name) == null)
                {
                    var module = modules.CreateElement();
                    module.SetAttributeValue("name", name);

                    Console.WriteLine("Adding module..");
                    modules.Add(module);
                }

                mgr.CommitChanges();
            }
        }

        /// <summary> 
        /// Helper method to find an element based on an attribute 
        /// </summary> 
        private static ConfigurationElement FindByAttribute(IEnumerable<ConfigurationElement> collection, string attributeName, string value)
        {
            return collection.FirstOrDefault(element => String.Equals((string)element.GetAttribute(attributeName).Value, value, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary> 
        /// Removes the specified UI Module by name 
        /// </summary> 
        public static void RemoveUIModuleProvider(string name)
        {
            using (var mgr = new ServerManager())
            {
                // First remove it from the sites 
                var adminConfig = mgr.GetAdministrationConfiguration();
                var modulesSection = adminConfig.GetSection("modules");
                var modules = modulesSection.GetCollection();
                var module = FindByAttribute(modules, "name", name);
                if (module != null)
                {
                    Console.WriteLine("Removing module..");
                    modules.Remove(module);
                }

                // now remove the ModuleProvider 
                var moduleProvidersSection = adminConfig.GetSection("moduleProviders");
                var moduleProviders = moduleProvidersSection.GetCollection();
                var moduleProvider = FindByAttribute(moduleProviders, "name", name);
                if (moduleProvider != null)
                {
                    Console.WriteLine("Removing module provider..");
                    moduleProviders.Remove(moduleProvider);
                }

                mgr.CommitChanges();
            }
        }
    }
}

