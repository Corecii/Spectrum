using Newtonsoft.Json;
using Spectrum.API;
using Spectrum.API.Exceptions;
using System;
using System.IO;
using System.Text;

namespace Spectrum.Manager.Runtime.Metadata
{
    public class PluginManifest
    {
        [JsonProperty("FriendlyName", Required = Required.Always)]
        public string FriendlyName;

        [JsonProperty("Author")]
        public string Author;

        [JsonProperty("AuthorContact")]
        public string AuthorContact;

        [JsonProperty("ModuleFileName", Required = Required.Always)]
        public string ModuleFileName;

        [JsonProperty("EntryClassName", Required = Required.Always)]
        public string EntryClassName;

        [JsonProperty("IPCIdentifier")]
        public string IPCIdentifier;

        [JsonProperty("Dependencies")]
        public string[] Dependencies;

        [JsonProperty("Priority")]
        public int? Priority;

        [JsonProperty("SkipLoad")]
        public bool SkipLoad;

        public static PluginManifest FromFile(string filePath)
        {
            string json;

            try
            {
                using (var sr = new StreamReader(filePath))
                    json = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new MetadataReadException("Failed to open the manifest file.", true, string.Empty, ex);
            }

            try
            {
                var manifest = JsonConvert.DeserializeObject<PluginManifest>(json);

                if (manifest.Priority == null)
                    manifest.Priority = 10;

                if (manifest == null)
                    throw new MetadataReadException("JSON deserializer returned null.", false, json);

                return manifest;
            }
            catch (JsonException je)
            {
                throw new MetadataReadException("Failed to deserialize JSON data.", false, json, je);
            }
            catch (Exception e)
            {
                throw new MetadataReadException("Unexpected metadata read exception occured.", false, json, e);
            }
        }

        public bool IsValid()
        {
            return (!string.IsNullOrEmpty(FriendlyName)) &&
                   (!string.IsNullOrEmpty(ModuleFileName)) &&
                   (!string.IsNullOrEmpty(EntryClassName));
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Name: {FriendlyName}");
            sb.AppendLine($"Module file name: {ModuleFileName}");
            sb.AppendLine($"Entry class name: {EntryClassName}");

            if(!string.IsNullOrEmpty(IPCIdentifier))
                sb.AppendLine($"Declared IPC identifier: {IPCIdentifier}");

            if (!string.IsNullOrEmpty(Author))
                sb.AppendLine($"By: {Author}");

            if (!string.IsNullOrEmpty(AuthorContact))
                sb.AppendLine($"Contact: {AuthorContact}");

            if (Dependencies != null && Dependencies.Length > 0)
            {
                sb.AppendLine($"Dependencies: ");
                foreach (var str in Dependencies)
                {
                    sb.AppendLine($"  {str}");
                }
            }

            return sb.ToString();
        }
    }
}
