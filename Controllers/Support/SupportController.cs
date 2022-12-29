using Microsoft.AspNetCore.Mvc;
using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Models.RequestModel.FeedbackRequestmodel;
using Project_Redmil_MVC.Models.RequestModel.IVRDurationCheck;
using Project_Redmil_MVC.Models.RequestModel.ivrrequestmodel;
using Project_Redmil_MVC.Models.RequestModel.RmDetailRequestModel;
using Project_Redmil_MVC.Models.RequestModel.SupportRequestModel;
using Project_Redmil_MVC.Models.ResponseModel.RmDetailwiseResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.SupportResponseModel;

namespace Project_Redmil_MVC.Controllers.Support
{
    public class SupportController : Controller
    {
        private readonly string Baseurl;
        private readonly IConfiguration _config;
        public List<DepartmentOnCalllistResponseModel> lstTResponse;
        public SupportController(IConfiguration config)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
        }
        public IActionResult Index()
        {
            return View();

        }
        public IActionResult Support()
        {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {

                return RedirectToAction("ErrorForLogin", "Error");

            }
            var Name = HttpContext.Session.GetString("Name");
            var Email = HttpContext.Session.GetString("Email");
            ViewBag.RollType = HttpContext.Session.GetString("Rolltype");
            //ViewBag.RollType = null;
            ViewBag.Department = new SelectList(GetDepartmentCall(""), "Department_APIKey", "Department_head");
            var mobile = HttpContext.Session.GetString("Mobile").ToString();
            ViewBag.Mobile = mobile;
            ViewBag.Name = Name;    
            ViewBag.Email = Email;  
            return View();

        }
        public List<DepartmentOnCalllistResponseModel> GetDepartmentCall(string Heading_name)
        {
            GetcallRequestModel GetcallRequestModelsobj = new GetcallRequestModel();
            List<DepartmentOnCalllistResponseModel> lstdata = new List<DepartmentOnCalllistResponseModel>();
            var Data = new List<SupportController>();
            GetcallRequestModelsobj.UserId = "NA";
            GetcallRequestModelsobj.page_name = "DepartmentOnCalllist";
            #region Checksum (GetBalance|Unique Key|UserId)
            string input = Checksum.MakeChecksumString("GetDepartmentCall", Checksum.checksumKey, GetcallRequestModelsobj.UserId, GetcallRequestModelsobj.page_name);
            string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
            #endregion
            GetcallRequestModelsobj.checksum = CheckSum;
            //var client = new RestClient($"{Baseurl}{ApiName.Getbalance}");
            var client = new RestClient("https://proapitest5.redmilbusinessmall.com/api/GetDepartmentCall");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var json = JsonConvert.SerializeObject(GetcallRequestModelsobj);
            request.AddJsonBody(json);
            IRestResponse response = client.Execute(request);
            var result = response.Content;
            var deserialize = JsonConvert.DeserializeObject<DepartmentOnCalllistBaseResponseModel>(response.Content);
            var data = deserialize.Data;
            lstTResponse = JsonConvert.DeserializeObject<List<DepartmentOnCalllistResponseModel>>(JsonConvert.SerializeObject(data));
            lstdata = lstTResponse.ToList();
            foreach (var tom in lstdata.Where(w => w.Department_head == "NA"))
            {
                tom.Department_head = tom.Department_name;
            }
            return lstdata;

        }
        [HttpPost]
        public JsonResult ForHeading(string ForHeading)
        {
            GetcallRequestModel GetcallRequestModelsobj = new GetcallRequestModel();
            List<DepartmentOnCalllistResponseModel> lstdata = new List<DepartmentOnCalllistResponseModel>();
            var Data = new List<SupportController>();
            GetcallRequestModelsobj.UserId = "NA";
            GetcallRequestModelsobj.page_name = "DepartmentOnCalllist";
            #region Checksum (GetBalance|Unique Key|UserId)
            string input = Checksum.MakeChecksumString("GetDepartmentCall", Checksum.checksumKey, GetcallRequestModelsobj.UserId, GetcallRequestModelsobj.page_name);
            string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
            #endregion
            GetcallRequestModelsobj.checksum = CheckSum;
            //var client = new RestClient($"{Baseurl}{ApiName.Getbalance}");
            var client = new RestClient("https://proapitest5.redmilbusinessmall.com/api/GetDepartmentCall");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var json = JsonConvert.SerializeObject(GetcallRequestModelsobj);
            request.AddJsonBody(json);
            IRestResponse response = client.Execute(request);
            var result = response.Content;
            var deserialize = JsonConvert.DeserializeObject<DepartmentOnCalllistBaseResponseModel>(response.Content);
            var data = deserialize.Data;
            var datalist = JsonConvert.DeserializeObject<List<DepartmentOnCalllistResponseModel>>(JsonConvert.SerializeObject(data));
            var dataaa = datalist.Where(x => x.Department_head == ForHeading).ToList();
            return Json(dataaa);
        }
        [HttpPost]
        public JsonResult IVRDurationCheck(string MaindepartmentName, string mobileno, string ApiKey)
        {

            IVRDurationCheckRequestModel IVRDurationCheckRequestModelsobj = new IVRDurationCheckRequestModel();
            //List<DepartmentOnCalllistResponseModel> lstdata = new List<DepartmentOnCalllistResponseModel>();
            //var Data = new List<SupportController>();
            IVRDurationCheckRequestModelsobj.UserId = HttpContext.Session.GetString("Id").ToString();
            IVRDurationCheckRequestModelsobj.DepartmentName = MaindepartmentName;  
            #region Checksum (GetBalance|Unique Key|UserId));
            string input = Checksum.MakeChecksumString("IVRDurationCheck", Checksum.checksumKey, IVRDurationCheckRequestModelsobj.UserId);
            string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
            #endregion
            IVRDurationCheckRequestModelsobj.checksum = CheckSum;
            //var client = new RestClient($"{Baseurl}{ApiName.Getbalance}");
            var client = new RestClient("https://proapitest5.redmilbusinessmall.com/api/IVRDurationCheck");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var json = JsonConvert.SerializeObject(IVRDurationCheckRequestModelsobj);
            request.AddJsonBody(json);
            IRestResponse response = client.Execute(request);
            var result = response.Content;  
            var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
            return Json(deserialize);
            
        }
        [HttpPost]

        public JsonResult Calluser(string mobileno, string ApiKey)
        {
                IvrRequestModel obj = new IvrRequestModel();
                obj.apikey = ApiKey;
                obj.refid = DateTime.Now.ToString();
                obj.mobileno = mobileno;
                #region Checksum (GetBalance|Unique Key|UserId));
                string input1 = Checksum.MakeChecksumString("GetIVRResponse2", Checksum.checksumKey);
                string CheckSum1 = Checksum.ConvertStringToSCH512Hash(input1);
                #endregion
                obj.checksum = CheckSum1;
                //var client = new RestClient($"{Baseurl}{ApiName.Getbalance}");
                var client1 = new RestClient("https://proapitest5.redmilbusinessmall.com/api/GetIVRResponse2");
                var request1 = new RestRequest(Method.POST);
                request1.AddHeader("Content-Type", "application/json");
                var json1 = JsonConvert.SerializeObject(obj);
                request1.AddJsonBody(json1);
                IRestResponse response1 = client1.Execute(request1);
                var result1 = response1.Content;
                var deserialize1 = JsonConvert.DeserializeObject<BaseResponseModel>(response1.Content);
                return Json(deserialize1);
        
        }
        [HttpPost]
        public JsonResult Feedback(string Remark, string SatisfiedY_N,string Reason,string mobileno)
        {
            IVR_FEEDBACKRequestModel IVR_FEEDBACKRequestModelsobj = new IVR_FEEDBACKRequestModel();
            List<DepartmentOnCalllistResponseModel> lstdata = new List<DepartmentOnCalllistResponseModel>();
            var Data = new List<SupportController>();
            IVR_FEEDBACKRequestModelsobj.refid = DateTime.Now.ToString();
            
            IVR_FEEDBACKRequestModelsobj.Remarks = Remark;
            IVR_FEEDBACKRequestModelsobj.SatisfiedY_N = SatisfiedY_N;
            IVR_FEEDBACKRequestModelsobj.Reason = Reason;
            IVR_FEEDBACKRequestModelsobj.Mobileno = mobileno;
            IVR_FEEDBACKRequestModelsobj.userId = "NA";
            #region Checksum (GetBalance|Unique Key|UserId));
            string input = Checksum.MakeChecksumString("AddFeedback", Checksum.checksumKey, IVR_FEEDBACKRequestModelsobj.userId,IVR_FEEDBACKRequestModelsobj.Mobileno,IVR_FEEDBACKRequestModelsobj.SatisfiedY_N);
            string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
            #endregion
            IVR_FEEDBACKRequestModelsobj.checksum = CheckSum;
            var client = new RestClient($"{Baseurl}{ApiName.AddFeedback}");
            //var client = new RestClient("https://api.redmilbusinessmall.com/api/AddFeedback");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var json = JsonConvert.SerializeObject(IVR_FEEDBACKRequestModelsobj);
            request.AddJsonBody(json);
            IRestResponse response = client.Execute(request);
            var result = response.Content;
            var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
            return Json(deserialize.Message);
           

        }
        [HttpPost]
        public JsonResult RmDetailUserWise()
        {
            
            RmDetailRequestModel obj = new RmDetailRequestModel();
            obj.UserId = "2084";
            #region Checksum (GetBalance|Unique Key|UserId));
            string input = Checksum.MakeChecksumString("GetRMDetailsUserwise", Checksum.checksumKey, obj.UserId);
            string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
            #endregion
            obj.checksum = CheckSum;
            //var client = new RestClient($"{Baseurl}{ApiName.GetRMDetailsUserwise}");
            var client = new RestClient("https://proapitest5.redmilbusinessmall.com/api/GetRMDetailsUserwise");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var json = JsonConvert.SerializeObject(obj);
            request.AddJsonBody(json);
            IRestResponse response = client.Execute(request);
            var result = response.Content;
            var dataNew = JsonConvert.DeserializeObject<BaseResponseModelT<List<RmDetaiwiseResponseModel>>>(response.Content);
            return Json(dataNew);
        }

    }
}
