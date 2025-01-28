namespace OtpGenerator.Logic.Interfaces
{
    public interface IDataEncryptor
    {
        (byte[] encrypted, byte[] salt) Encrypt(byte[] data, byte[] userSecret);
    }
}