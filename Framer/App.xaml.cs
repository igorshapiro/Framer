using System;
using System.Reflection;
using Microsoft.Win32;

namespace Framer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App {
        private static readonly ApplicationSettings s_settings = ApplicationSettings.Load();

        public static ApplicationSettings Settings {
            get { return s_settings; }
        }

        public App() {
            try {
                string command = string.Format("\"{0}\" \"{1}\"", Assembly.GetExecutingAssembly().Location, "%L");
                Register("Folder", "Frame", "Frame", command);
            }
            catch (UnauthorizedAccessException) {}
        }

        public static void Register(string fileType, string shellKeyName, string menuText, string menuCommand)
        {
            // create path to registry location

            string regPath = string.Format(@"{0}\shell\{1}",
                                           fileType, shellKeyName);

            // add context menu to the registry

            using (RegistryKey key =
                   Registry.ClassesRoot.CreateSubKey(regPath))
            {
                key.SetValue(null, menuText);
            }

            // add command that is invoked to the registry

            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(
                string.Format(@"{0}\command", regPath)))
            {
                key.SetValue(null, menuCommand);
            }
        }
    }
}
