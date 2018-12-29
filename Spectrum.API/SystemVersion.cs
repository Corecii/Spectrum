namespace Spectrum.API
{
    public class SystemVersion
    {
        public static int DistanceBuild
        {
            get { return SVNRevision.number_; }
            set { SVNRevision.number_ = value; }
        }

        public static string VersionString => $"DISTANCE {DistanceBuild} (SPECTRUM [00AA77]GAMMA[-])";
    }
}