namespace Project_Redmil_MVC.Models.RequestModel.Applyforfrenchise
{
    public class ApplyForFrenchiseRequest : BaseResponseModel
    {
       
        public string? Userid { get; set; }
        public string? CityName { get; set; }
        public string? AreaName { get; set; }
        public string? Token { get; set; }
        public string? Preference { get; set; }
        //public string? Preference2 { get; set; }

        public string? checksum { get; set; }
    }
}
