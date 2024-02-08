namespace Bol.Core.Model
{
    public class NinSpecification
    {
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string InternationalName { get; set; }
        public string LocalName { get; set; }
        public string Format { get; set; }
        public int? Digits { get; set; }
        public int? SplitIndex { get; set; }
        public string Regex { get; set; }
        public NinStatus Status { get; set; }
    }

    public enum NinStatus
    {
        Inactive = 0,
        Active = 1
    }
}
