namespace StudyId.SmtpManager
{
    public class SmtpResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
    public class SmtpResult<T> : SmtpResult
    {
        public T Data { get; set; }
    }
}
