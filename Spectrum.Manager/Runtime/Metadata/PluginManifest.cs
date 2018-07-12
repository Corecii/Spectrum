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

        [JsonName(Name = "Priority")]
        public int Priority;

        [JsonName(Name = "SkipLoad")]
        public bool SkipLoad;

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
                var manifest = new JsonReader().Read<PluginManifest>(json);

                if (manifest == null)
                    throw new MetadataReadException("JSON deserializer returned null.", false, json);

                return manifest;
            }
            catch(Exception ex)
            {
                throw new MetadataReadException("Failed to deserialize JSON data.", false, json, ex);
            }
        }

        public bool IsValid()
        {
            return (!string.IsNullOrEmpty(FriendlyName)) &&
                   (!string.IsNullOrEmpty(Author)) &&
                   (!string.IsNullOrEmpty(AuthorContact)) &&
                   (!string.IsNullOrEmpty(ModuleFileName)) &&
                   (!string.IsNullOrEmpty(EntryClassName)) &&
                   (!string.IsNullOrEmpty(IPCIdentifier));
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Name: {FriendlyName}");
            sb.AppendLine($"By: {Author} | {AuthorContact}");
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
