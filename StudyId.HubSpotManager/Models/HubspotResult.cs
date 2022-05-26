namespace StudyId.HubSpotManager.Models
{
    public class HubspotResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    public class HubspotResult<T> : HubspotResult
    {
        public T Data { get; set; }
    }
}
