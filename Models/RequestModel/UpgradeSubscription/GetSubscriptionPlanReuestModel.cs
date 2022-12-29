namespace Project_Redmil_MVC.Models.RequestModel.UpgradeSubscription
{
    public class GetSubscriptionPlanReuestModel
    {
        public string? Userid { get; set; }
        
        public string? Token { get; set; }
        public string? checksum { get; set; }
    }
}
