namespace Bol.Core
{
    public class Constants
    {
        public const char CODENAME_DIVIDER = '<';
        public const string PERSONAL_CODENAME_INITIAL = "P";
        public const string COMMERCIAL_CODENAME_INITIAL = "C";
        public const string ORGANIZATION_CODENAME_INITIAL = "O";
        
        public const byte B_ADDRESS_PREFIX = 0x19;
        public const string B_ADDRESS_PLAIN_PREFIX = "BBBB";
        public const string C_ADDRESS_PLAIN_PREFIX = "BCCC";

        public const uint B_ADDRESS_START = 0x1949C6B9;
        public const uint B_ADDRESS_END = 0x1949C78C;
        public const uint C_ADDRESS_START = 0x1954F01A;
        public const uint C_ADDRESS_END = 0x1954F0ED;
    }
}
