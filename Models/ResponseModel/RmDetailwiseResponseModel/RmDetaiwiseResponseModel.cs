namespace Project_Redmil_MVC.Models.ResponseModel.RmDetailwiseResponseModel
{
    public class RmDetaiwiseResponseModel
    {
        public string Name { get; set; }
        public string Mobileno { get; set; }

        [JsonProperty("BH Name")]
        public string BHName { get; set; }

        [JsonProperty("BH ID")]
        public int BHID { get; set; }

        [JsonProperty("BH Mobile")]
        public string BHMobile { get; set; }
    }
}
