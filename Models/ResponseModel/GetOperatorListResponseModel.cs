namespace Project_Redmil_MVC.Models.ResponseModel
{
    public class GetOperatorListResponseModel : BaseResponseModel
    {
        public int Id { get; set; }
        public string? Operatorname { get; set; }
        public string? Opcode { get; set; }
        public string? Img { get; set; }
        public int ServiceId { get; set; }

        public List<ResponseOperator>? lstOperator { get; set; }

    }
}
