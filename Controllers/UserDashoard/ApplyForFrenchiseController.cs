
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Collections;
using RestSharp;
using Project_Redmil_MVC.Helper;
using Project_Redmil_MVC.Models;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Logging;
//using Firebase.Auth;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Project_Redmil_MVC.Models.RequestModel.Applyforfrenchise;
using Project_Redmil_MVC.CommonHelper;

namespace Project_Redmil_MVC.Controllers.UserDashoard
{
    public class ApplyForFrenchiseController : Controller
    {
        private readonly string Baseurl;
        private readonly IConfiguration _config;
        public ApplyForFrenchiseController(IConfiguration config)
        {
            _config = config;
            Baseurl = "https://proapitest4.redmilbusinessmall.com/api/"; HelperMethod.GetBaseURl(_config);
        }

        [HttpGet]
        public IActionResult ApplyForFrenchise()
        {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {

                return RedirectToAction("ErrorForLogin", "Error");

            }
            return View();
        }

        [HttpPost]
       
       public JsonResult ApplyForFrenchise(string city,string area,string frenchise)
        {
            
            ApplyForFrenchiseRequest requestModel = new ApplyForFrenchiseRequest();
            requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
            requestModel.Token = "";
            requestModel.CityName = city;
            requestModel.AreaName = area;
            requestModel.Preference = frenchise;
            #region Checksum (addsender|Unique Key|UserId)
            string input = Checksum.MakeChecksumString(ApiName.ApplyForFranchise, Checksum.checksumKey,requestModel.Userid);
            string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
            #endregion
            //var client = new RestClient("https://api.redmilbusinessmall.com/api/User/ValidateUser");
            var client = new RestClient($"{Baseurl}{ApiName.ApplyForFranchise}");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            requestModel.checksum = CheckSum;
            var json = JsonConvert.SerializeObject(requestModel);
            request.AddJsonBody(json);
            IRestResponse response = client.Execute(request);
            var result = response.Content;
            var des = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
            return Json(des);
        }
    }
}
