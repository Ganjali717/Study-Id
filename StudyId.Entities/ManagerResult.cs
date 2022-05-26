namespace StudyId.Entities
{
    public class ManagerResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    public class ManagerResult<T> : ManagerResult
    {
        public T Data { get; set; }
    }

    public class PagedManagerResult<T> : ManagerResult<T>
    {
        public int Page { get; set; }
        public int Take { get; set; }
        public int Total { get; set; }
        public string? OrderBy { get; set; } 
        public bool? OrderAsc { get; set; } 
    }
}
