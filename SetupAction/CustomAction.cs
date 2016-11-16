using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.Reflection;

namespace SetupAction
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult InstallUIModule(Session session)
        {
            try
            {
                session.Log("Begin installation of UI module");

                var assembly = Assembly.GetExecutingAssembly();
                var assemblyName = assembly.GetName();
                var assemblyFullName = assemblyName.FullName;
                var clientAssemblyFullName = assemblyFullName.Replace(assemblyName.Name, "Web.Management.PHP");

                session.Log("Checking for previous versions of PHPManager..");
                UIModuleHelper.RemoveUIModuleProvider(session, "PHP"); // This is necessary for the upgrade scenario

                session.Log("Installing PHPManager module..");
                UIModuleHelper.AddUIModuleProvider(session, "PHP", "Web.Management.PHP.PHPProvider, " + clientAssemblyFullName);
            }
            catch (Exception ex)
            {
                session.Log(string.Format("Failed to install UI module - {0}", ex.Message));
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }


        [CustomAction]
        public static ActionResult UninstallUIModule(Session session)
        {
            try
            {
                session.Log("Uninstalling PHPManager module..");
                UIModuleHelper.RemoveUIModuleProvider(session, "PHP");
            }
            catch (Exception ex)
            {
                session.Log(string.Format("Failed to uninstall UI module - {0}", ex.Message));
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }
    }
}
