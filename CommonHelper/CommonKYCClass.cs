using Project_Redmil_MVC.Models.RequestModel.AepsRequestModel;
using Project_Redmil_MVC.Models.RequestModel;
using Project_Redmil_MVC.Models.ResponseModel.AepsResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.DTHResponseModel;

namespace Project_Redmil_MVC.CommonHelper
{
    public class CommonKYCClass
    {
        
        public static List<AepsOnboardingResponseModel> GetOnboardingResponse(string uname)
        {
            AepsOnboardingRequestModel requestModel = new AepsOnboardingRequestModel();
            List<AepsOnboardingResponseModel> lstresponse = new List<AepsOnboardingResponseModel>();
            try
            {
                requestModel.UserId = uname;
                var client = new RestClient("https://proapitest5.redmilbusinessmall.com/api/IsFingpayRegister");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                if (string.IsNullOrEmpty(result))
                {
                    return null;
                }
                else
                {
                    var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                    if (deserialize.Statuscode == "TXN")
                    {
                        var data = deserialize.Data;
                        var dataList = JsonConvert.DeserializeObject<List<AepsOnboardingResponseModel>>(JsonConvert.SerializeObject(data));
                        if (dataList != null)
                        {
                            foreach (var item in dataList)
                            {
                                lstresponse.Add(new AepsOnboardingResponseModel
                                {
                                    MerchantKey = item.MerchantKey,
                                    IsOnboard = item.IsOnboard,
                                    IsKyc = item.IsKyc,
                                });
                            }
                        }
                        return lstresponse;
                    }
                    else if (deserialize.Statuscode == "ERR")
                    {
                        return lstresponse;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = requestModel;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                requestEx.AddJsonBody(json);
                IRestResponse response = client.Execute(requestEx);
                var result = response.Content;
            }
            return lstresponse;
        }
    }
}
