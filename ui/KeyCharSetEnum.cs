namespace ui
{
    public enum KeyCharSetEnum
    {
        // in checking order, least-flexible first
        Octal,
        Numeric, 
        HexadecimalUpper, 
        HexadecimalLower, 
        AlphabetUpper, 
        AlphabetLower,
        // ReSharper disable once InconsistentNaming
        AlphaNumericUpperSansIOU10, // 1Password Secret Key
        AlphaNumericUpperSans1890,  // BitWarden TOTP
        AlphaNumericUpper,
        AlphaNumericLower,
        AlphaNumeric,
        Base64,
        ReadableSansSpace,
        Readable
    }
}