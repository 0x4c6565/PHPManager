using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Web.Management.PHP.Utility
{
    public static class InstallAction
    {
        public static void Install()
        {
            Console.WriteLine("Initialising installation..");

            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();
            var assemblyFullName = assemblyName.FullName;
            var clientAssemblyFullName = assemblyFullName.Replace(assemblyName.Name, "Web.Management.PHP");

            Console.WriteLine("Checking for previous versions of PHPManager..");
            UIModuleHelper.RemoveUIModuleProvider("PHP"); // This is necessary for the upgrade scenario

            Console.WriteLine("Installing PHPManager module..");
            UIModuleHelper.AddUIModuleProvider("PHP", "Web.Management.PHP.PHPProvider, " + clientAssemblyFullName);
        }

        public static void Uninstall()
        {
            Console.WriteLine("Uninstalling PHPManager module..");
            UIModuleHelper.RemoveUIModuleProvider("PHP");
        }
    }
}
