namespace OtpGenerator.Logic.Interfaces
{
    public interface IEncryptionKeyGenerator
    {
        byte[] GenerateKey(byte[] userSecret, byte[] salt);
        byte[] GenerateSalt();
    }
}