namespace Project_Redmil_MVC.Models.ResponseModel
{
    public class GetMobileMNPDetailsResponseModel : BaseResponseModel
    {
        public string? MobileNo { get; set; }
        public string? SystemReferenceNo { get; set; }
        public string? CorpRefNo { get; set; }
        public string? CurrentOperator { get; set; }
        public string? CurrentLocation { get; set; }
        public string? PreviousOperator { get; set; }
        public string? PreviousLocation { get; set; }
        public bool Ported { get; set; }
        public string? Charged { get; set; }
        public string? Error { get; set; }
    }
}
