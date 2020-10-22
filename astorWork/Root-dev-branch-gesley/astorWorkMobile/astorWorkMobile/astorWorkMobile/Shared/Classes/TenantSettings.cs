namespace astorWorkMobile.Shared.Classes
{
    public class TenantSettings
    {
        public string TenantName { get; set; }
        public string BackgroundImageURL { get; set; }
        public string LogoImageURL { get; set; }
        public string PowerBIWorkSpaceGUID { get; set; }
        public int TimeZone { get; set; }
        public bool Enabled { get; set; }
        public string Hostname { get; set; }
        public string EnabledModules { get; set; }
    }
}
