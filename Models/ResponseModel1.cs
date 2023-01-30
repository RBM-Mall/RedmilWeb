
namespace Project_Redmil_MVC.Models
{
    public class ResponseModel1
    {
        public string? Statuscode { get; set; }
        public object? Message { get; set; }
        public object? Data { get; set; }
    }
    public class ResponseModelAAdharVerification
    {
        public string? Statuscode { get; set; }
        public object? Message { get; set; }
        public object? Surcharge { get; set; }
        public string? code  {get; set;}
        public string? Aadhar  {get; set;}
        public string? Name  {get; set;}
        
    }
    public class ResponseOperator
    {
        public int Id { get; set; }
        public string? Operatorname { get; set; }
        public string? Opcode { get; set; }
        public string? Img { get; set; }
        public string? Img1 { get; set; }
        public int ServiceId { get; set; }

        public string baseurl { get; set; }
        public List<ResponseOperator>? lstOperator { get; set; }

    }

    public class ResponseOperatorDetails
    {
        public string? MobileNo { get; set; }
        public string? SystemReferenceNo { get; set; }
        public string? CorpRefNo { get; set; }
        public string? CurrentOperator { get; set; }
        public string? CurrentLocation { get; set; }
        public string? PreviousOperator { get; set; }
        public string? PreviousLocation { get; set; }
        public bool? Ported { get; set; }
        public string? Charged { get; set; }
        public string? Error { get; set; }
    }


}
