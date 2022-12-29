﻿namespace Project_Redmil_MVC.Models.ResponseModel
{
    public class GetAllPlansResponseModel: BaseResponseModel
    {
        public int Amount { get; set; }
        public string Description { get; set; }
        public string LocationName { get; set; }
        public string PlanName { get; set; }
        public int ServiceId { get; set; }
        public int ServiceProviderId { get; set; }
        public string ServiceProviderName { get; set; }
        public double Talktime { get; set; }
        public string Validity { get; set; }

        public List<GetAllPlansResponseModel> lstgetAllPlansResponseModels { get; set; }
    }
}
