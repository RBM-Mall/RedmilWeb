using Microsoft.AspNetCore.Mvc;
using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Models.RequestModel.SelfHelp;
using Project_Redmil_MVC.Models.RequestModel;
using Project_Redmil_MVC.Models.ResponseModel.SelfHelpResponseModel;

namespace Project_Redmil_MVc 
{
    public class self_HelpController : Controller
    {
        private readonly string Baseurl;
        private readonly IConfiguration _config;

        public self_HelpController(IConfiguration config)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
        }

        public IActionResult Index()
        {
            return View();
        }
        #region SelfHelp
        public JsonResult SelfHelp(string ServiceId)
        {

            try
            {
                SelfHelpRequestModel SelfRequest = new SelfHelpRequestModel();
                SelfRequest.Userid = "NA";
                SelfRequest.ServiceId = ServiceId;
                string input = Checksum.MakeChecksumString("ViewFQL", Checksum.checksumKey,
                    SelfRequest.Userid);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                SelfRequest.Checksum = CheckSum;
                var client = new RestClient($"{Baseurl}{ApiName.ViewFQL}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(SelfRequest);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                if (string.IsNullOrEmpty(result))
                {
                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                }
                else
                {
                    var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
                    if (deserialize.Statuscode == "ERR")
                    {
                        return Json(deserialize);
                    }
                    else
                    {
                        var datadeserialize = deserialize.Data;
                        var data = JsonConvert.DeserializeObject<List<SelfHelpResponseModel>>(JsonConvert.SerializeObject(datadeserialize));
                        //var datadeserialize = deserialize.Data;
                        //var TranferData = JsonConvert.DeserializeObject<GetcashdepositeResponseModel>(JsonConvert.SerializeObject(datadeserialize));
                        return Json(data);
                    }
                    
                }

            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                requestModelEx.ExceptionMessage = ex;
                requestModelEx.Data = "";
                var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                requestEx.AddJsonBody(jsonEx);
                IRestResponse responseEx = clientEx.Execute(requestEx);
                var resultEx = responseEx.Content;
                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
            }
        }
        #endregion
    }
}
