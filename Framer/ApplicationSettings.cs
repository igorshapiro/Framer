using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Framer {
    public class ApplicationSettings {
        private const string SETTINGS_FILE_NAME = "framer.settings";

        public string FramesDirectory { get; set; }

        public string ImagesDirectory { get; set; }

        private static string ConfigFile {
            get {
                string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return Path.Combine(dir ?? Directory.GetCurrentDirectory(), SETTINGS_FILE_NAME);
            }
        }

        internal static ApplicationSettings Load() {
            if (File.Exists(ConfigFile)) {
                return JsonConvert.DeserializeObject<ApplicationSettings>(File.ReadAllText(ConfigFile));
            }
            string defaultFramesDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Frames");
            return new ApplicationSettings { FramesDirectory = defaultFramesDirectory };
        }

        public void Save() {
            File.WriteAllText(ConfigFile, JsonConvert.SerializeObject(this));
        }
    }
}