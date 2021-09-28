using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WindowsFormsApp1
{
    [Serializable]
    public class SettingsSaveLoad
    {
        public float waitTimeMin = 0;
        public string webhook = "";
        public bool discordWebhook = false;
        public bool pageCheck = true;

        public void Save()
        {
            waitTimeMin = (float)Program.waittime;
            webhook = Program.Webhook;
            discordWebhook = Program.discordWebhook;
            pageCheck = Program.pageCheck;

            //Saving part
            BinaryFormatter formatter = new BinaryFormatter();
            string paths = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ao3Listen\\config.cfg");
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ao3Listen"));
            var stream = new FileStream(paths, FileMode.Create);
            SettingsSaveLoad data = this;
            formatter.Serialize(stream, data);
            stream.Close();
        }

        public static SettingsSaveLoad Load()
        {
            string paths = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ao3Listen\\config.cfg");
            if (File.Exists(paths))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                var stream = new FileStream(paths, FileMode.Open);
                SettingsSaveLoad data = formatter.Deserialize(stream) as SettingsSaveLoad;
                stream.Close();
                return data;
            }
            else
            {
                return null;
            }
        }
    }
}
