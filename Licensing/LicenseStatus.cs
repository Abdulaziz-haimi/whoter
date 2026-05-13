namespace water3.Licensing
{
    public class LicenseStatus
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public LicenseInfo Info { get; set; }
    }
}
