using System;
using System.IO;
using System.Reflection;
using JsonFx.Serialization;

namespace Spectrum.API.Configuration
{
    public class Settings : Section
    {
        private string FileName { get; }
        private string RootDirectory { get; }
        private string FilePath => Path.Combine(Path.Combine(RootDirectory, Defaults.PrivateSettingsDirectory), FileName);

        public Settings(string fileName)
        {
            RootDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            FileName = $"{fileName}.json";

            if (File.Exists(FilePath))
            {
                var saveLater = false;
                using (var sr = new StreamReader(FilePath))
                {
                    var json = sr.ReadToEnd();
                    var reader = new JsonFx.Json.JsonReader();

                    Section sec = null;

                    try
                    {
                        sec = reader.Read<Section>(json);
                    }
                    catch
                    {
                        saveLater = true;
                    }

                    if (sec != null)
                    {
                        foreach (string k in sec.Keys)
                        {
                            Add(k, sec[k]);
                        }
                    }
                }

                if (saveLater)
                {
                    Save();
                }
            }
        }

        public void Save(bool formatJson = true)
        {
            DataWriterSettings st = new DataWriterSettings { PrettyPrint = formatJson };
            var writer = new JsonFx.Json.JsonWriter(st);

            using (var sw = new StreamWriter(FilePath, false))
            {
                sw.WriteLine(writer.Write(this));
            }
        }
    }
}
