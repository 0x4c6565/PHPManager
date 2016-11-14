using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Management.PHP.Utility
{
    public static class UIModuleHelper
    {
        
        /// <summary> 
        /// Adds the specified UI Module by name 
        /// </summary> 
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
