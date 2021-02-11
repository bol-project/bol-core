namespace Bol.Core
{
    public class Constants
    {
        public const char CODENAME_DIVIDER = '<';
        public const string PERSONAL_CODENAME_INITIAL = "P";
        public const string COMMERCIAL_CODENAME_INITIAL = "C";
        public const string ORGANIZATION_CODENAME_INITIAL = "O";
        
        public const byte BOL_ADDRESS_PREFIX = 0x19;
        public const string B_ADDRESS_PLAIN_PREFIX = "BBB";
        public const string C_ADDRESS_PLAIN_PREFIX = "BCC";

        public const uint B_ADDRESS_START = 0x1949BF00;
        public const uint B_ADDRESS_END = 0x1949ED00;
        public const uint C_ADDRESS_START = 0x1954E700;
        public const uint C_ADDRESS_END = 0x19551600;
    }
}
