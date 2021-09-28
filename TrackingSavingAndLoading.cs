using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WindowsFormsApp1
{
    [Serializable]
    public class TrackingSavingAndLoading
    {
        public List<Tracker> Track = new List<Tracker>();
        public bool temp= false;
        public void Save()
        {
            Track = Program.Track;

            //Saving part
            BinaryFormatter formatter = new BinaryFormatter();
            string paths = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ao3Listen\\Track.sav");
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ao3Listen"));
            var stream = new FileStream(paths, FileMode.Create);
            TrackingSavingAndLoading data = this;
            formatter.Serialize(stream, data);
            stream.Close();
        }

        public static TrackingSavingAndLoading Load()
        {
            string paths = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ao3Listen\\Track.sav");
            if (File.Exists(paths))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                var stream = new FileStream(paths, FileMode.Open);
                TrackingSavingAndLoading data = formatter.Deserialize(stream) as TrackingSavingAndLoading;
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