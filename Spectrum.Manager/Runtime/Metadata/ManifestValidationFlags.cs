using System;

namespace Spectrum.Manager.Runtime.Metadata
{
    [Flags]
    public enum ManifestValidationFlags
    {
        MissingFriendlyName = 1,
        MissingModuleFileName = 2,
        MissingEntryClassName = 4
    }
}
