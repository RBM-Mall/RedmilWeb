namespace Project_Redmil_MVC.Models
{
    public class ResponseModelGeneric<T>
    {
        public string? Statuscode { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}
