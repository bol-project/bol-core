namespace Bol.Core.Model
{
    public class NinSpecification
    {
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string InternationalName { get; set; }
        public string LocalName { get; set; }
        public string Format { get; set; }
        public int Digits { get; set; }
        public int SplitIndex { get; set; }
        public string Regex { get; set; }
        public NinType Type { get; set; }
    }

    public enum NinType
    {
        N = 0,
        A = 1,
        AN = 2
    }
}
