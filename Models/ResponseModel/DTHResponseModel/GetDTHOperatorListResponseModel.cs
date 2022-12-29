namespace Project_Redmil_MVC.Models.ResponseModel.DTHResponseModel
{
    public class GetDTHOperatorListResponseModel
    {
        public int Id { get; set; }
        public string? Operatorname { get; set; }
        public string? Opcode { get; set; }
        public string? Img { get; set; }
        public int ServiceId { get; set; }

        public string baseurl { get; set; }
        public List<GetDTHOperatorListResponseModel>? lstOperator { get; set; }
    }
}
