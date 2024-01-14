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

        public const string INVALID_CODENAME = "Invalid Person CodeName format. Person CodeName format should be: " + "P<GRC<PAPADOPOULOS<G<<<1963M<ca8FXTowBuE<1B941";
        public const string CODENAME_BIRTHDATE_FORMAT = "yyyydd";
        public const int CODENAME_CHECKSUM_LENGTH = 4;
        public const int CODENAME_CHECKSUM_BYTES = 2;
        public const int CODENAME_PARTS = 9;
        public const int CODENAME_BIRTHYEAR_GENDER_LENGTH = 5;
        public const int CODENAME_BDATE_NAME_NIN_BASE58_LENGTH = 11;
        public const int CODENAME_COMBINATION_CHECKSUM_LENGTH = 5;
        
        public const string HASH_ZEROS = "0000000000000000000000000000000000000000000000000000000000000000";
    }
}
