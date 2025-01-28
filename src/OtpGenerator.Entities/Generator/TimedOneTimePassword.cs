namespace OtpGenerator.Entities.Generator
{
    public class TimedOneTimePassword
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string Account { get; set; }
        public string Code { get; set; }
        public int RemainingSeconds { get; set; }
    }
}