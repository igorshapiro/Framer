using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Framer {
    public class ApplicationSettings {
        private const string SETTINGS_FILE_NAME = "framer.settings";

        public string FramesDirectory { get; set; }

        public string ImagesDirectory { get; set; }

        internal static ApplicationSettings Load() {
            string fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                           SETTINGS_FILE_NAME);
            if (File.Exists(fileName)) {
                return JsonConvert.DeserializeObject<ApplicationSettings>(File.ReadAllText(fileName));
            }
            string defaultFramesDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Frames");
            return new ApplicationSettings { FramesDirectory = defaultFramesDirectory };
        }

        public void Save() {
            File.WriteAllText(SETTINGS_FILE_NAME, JsonConvert.SerializeObject(this));
        }
    }
}