using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace OtpGenerator.Logic.Base32
{
    /*
    * This class is taken from the following article:
    *
    * https://binary-encoding-decoding.mojoauth.com/base32-with-c/
    */
    [ExcludeFromCodeCoverage]
    public static class Base32Encoder
    {
        private const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        public static string Encode(byte[] data)
        {
            StringBuilder result = new StringBuilder();
            int buffer = 0, bitsInBuffer = 0;
            foreach (byte b in data)
            {
                buffer <<= 8;
                buffer |= b;
                bitsInBuffer += 8;
                while (bitsInBuffer >= 5)
                {
                    int index = (buffer >> (bitsInBuffer - 5)) & 0x1F; // Get the last 5 bits
                    result.Append(Base32Alphabet[index]);
                    bitsInBuffer -= 5;
                }
            }
            // Handle remaining bits
            if (bitsInBuffer > 0)
            {
                buffer <<= (5 - bitsInBuffer);
                result.Append(Base32Alphabet[buffer & 0x1F]);
            }
            return result.ToString();
        }
    }
}