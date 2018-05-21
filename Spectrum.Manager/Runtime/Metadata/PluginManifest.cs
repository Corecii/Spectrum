using JsonFx.Json;
using Spectrum.API;
using Spectrum.API.Exceptions;
using System;
using System.IO;
using System.Text;

namespace Spectrum.Manager.Runtime.Metadata
{
    public class PluginManifest
    {
        [JsonName(Name = "FriendlyName")]
        public string FriendlyName;

        [JsonName(Name = "Author")]
        public string Author;

        [JsonName(Name = "AuthorContact")]
        public string AuthorContact;

        [JsonName(Name = "ModuleFileName")]
        public string ModuleFileName;

        [JsonName(Name = "EntryClassName")]
        public string EntryClassName;

        [JsonName(Name = "IPCIdentifier")]
        public string IPCIdentifier;

        [JsonName(Name = "CompatibleAPILevel")]
        public APILevel CompatibleAPILevel;

        [JsonName(Name = "Dependencies")]
        public string[] Dependencies;

        public static PluginManifest FromFile(string filePath)
        {
            string json;

            try
            {
                using(var sr = new StreamReader(filePath))
                {
                    json = sr.ReadToEnd();
                }
            }
            catch(Exception ex)
            {
                throw new MetadataReadException("Failed to open the file.", true, string.Empty, ex);
            }

            try
            {
                return new JsonReader().Read<PluginManifest>(json);
            }
            catch(Exception ex)
            {
                throw new MetadataReadException("Failed to deserialize JSON data.", false, json, ex);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Name: {FriendlyName}");
            sb.AppendLine($"By: {AuthorContact}");
            sb.AppendLine($"Module file name: {ModuleFileName}");
            sb.AppendLine($"Entry class name: {EntryClassName}");
            sb.AppendLine($"Compatible API: {CompatibleAPILevel}");

            if(Dependencies != null && Dependencies.Length > 0)
            {
                sb.AppendLine($"Dependencies: ");
                foreach(var str in Dependencies)
                {
                    sb.AppendLine($"  {str}");
                }
            }

            return sb.ToString();
        }
    }
}
