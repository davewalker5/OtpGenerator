namespace OtpGenerator.Logic.Interfaces
{
    public interface IDataDecryptor
    {
        byte[] Decrypt(byte[] data, byte[] userSecret, byte[] salt);
    }
}