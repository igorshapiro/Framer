using System.IO;
using Newtonsoft.Json;

namespace Framer {
    public class ApplicationSettings {
        private const string SETTINGS_FILE_NAME = "framer.settings";

        public string FramesDirectory { get; set; }

        public string ImagesDirectory { get; set; }

        internal static ApplicationSettings Load() {
            if (File.Exists(SETTINGS_FILE_NAME)) {
                return JsonConvert.DeserializeObject<ApplicationSettings>(File.ReadAllText(SETTINGS_FILE_NAME));
            }
            return new ApplicationSettings { FramesDirectory = @"C:\Users\Public\Pictures\Frames" };
        }

        public void Save() {
            File.WriteAllText(SETTINGS_FILE_NAME, JsonConvert.SerializeObject(this));
        }
    }
}