namespace Project_Redmil_MVC.Models.ResponseModel
{
    public class GetMyTeamMemberCountNewResponseModel:BaseResponseModel
    {
        public string TotalMember { get; set; }
        public string DirectMember { get; set; }
        public string IndirectMember { get; set; }
        public string SuperIndirectMember { get; set; }
       public List<GetMyTeamMemberCountNewResponseModel> lst { get; set; }
    }
}
