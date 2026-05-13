namespace water3.Licensing
{
    public class LicenseInfo
    {
        public string CustomerName { get; set; }
        public string MachineId { get; set; }
        public string Plan { get; set; }
        public int MaxUsers { get; set; }
        public string IssuedAtUtc { get; set; }
        public string ExpiryDateUtc { get; set; }
    }
}
