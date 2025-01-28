namespace OtpGenerator.Logic.Interfaces
{
    public interface IUserSecretProvider
    {
        byte[] GetSecret();
    }
}