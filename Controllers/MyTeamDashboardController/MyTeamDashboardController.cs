using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project_Redmil_MVC.Helper;
using Project_Redmil_MVC.Models;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project_Redmil_MVC.Models.ResponseModel;
using Project_Redmil_MVC.Models.RequestModel;
using System.Data;
using Project_Redmil_MVC.CommonHelper;

namespace Project_Redmil_MVC.Controllers.MyTeamDashboardController
{
    public class MyTeamDashboardController : Controller
    {
        private readonly string Baseurl;
        private readonly IConfiguration _config;
        public MyTeamDashboardController(IConfiguration config)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
        }

        [HttpGet]
        public IActionResult MyTeamDashboard()
        {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {
                return RedirectToAction("ErrorForLogin", "Error");

            }
            GetMyTeamMemberCountNewRequestModel req = new GetMyTeamMemberCountNewRequestModel();
            try
            {
                //req.UserId = "2084";
                #region Checksum (Recharge|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("GetMyTeamMemberCountNew", Checksum.checksumKey, "2084");
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                req.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/GetMyTeamMemberCountNew");
                var client = new RestClient($"{Baseurl}{ApiName.GetMyTeamMemberCountNew}");

                ////Create request with GET
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(req);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                var datadeserialize = deserialize.Data;
                var data = JsonConvert.DeserializeObject<GetMyTeamMemberCountNewResponseModel>(JsonConvert.SerializeObject(datadeserialize));
                if (data.Statuscode != null&& data.Statuscode=="TXN")
                {
                    return View(data);
                }
                else if (data.Statuscode == "ERR")
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("ErrorHandle", "Error");
                }
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = req;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
            }
            return View();

        }
        #region MyTeamDashboardData
        [HttpPost]
        public IActionResult MyTeamDashboardData()
        {
            GetMyTeamMemberCountNewRequestModel req = new GetMyTeamMemberCountNewRequestModel();
            try
            {
                req.UserId = "2084";
                #region Checksum (Recharge|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("GetMyTeamMemberCountNew", Checksum.checksumKey, "2084");
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                req.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/GetMyTeamMemberCountNew");
                var client = new RestClient($"{Baseurl}{ApiName.GetMyTeamMemberCountNew}");
                ////Create request with GET
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(req);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                var datadeserialize = deserialize.Data;
                var data = JsonConvert.DeserializeObject<GetMyTeamMemberCountNewResponseModel>(JsonConvert.SerializeObject(datadeserialize));
                if (data.Statuscode == "TXN")
                {
                    return Json(data);
                }
                else if (data.Statuscode == "ERR")
                {
                    return Json(data);
                }
                else
                {
                    return RedirectToAction("ErrorHandle", "Error");
                }
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = req;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
            }
            return Json("");

        }
        #endregion

    }
}
