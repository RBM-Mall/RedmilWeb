namespace Project_Redmil_MVC.Models
{
    public class BusinessTranscationRequestReport
    {
        internal string baseUrl;

        public string Status { get; set; }
        public string PageNumber { get; set; }
        public string Report { get; set; }
        public string UserId { get; set; }
        public string SortBy { get; set; }
        public string ServiceType { get; set; }
        public string Checksum { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
