﻿using Microsoft.AspNetCore.Mvc;
using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Models.RequestModel;
using Project_Redmil_MVC.Models.RequestModel.LICIndiaBillRequestModel;
using Project_Redmil_MVC.Models.ResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.LICIndiaBillResponseModel;
using static Project_Redmil_MVC.Models.ResponseModel.LICIndiaBillResponseModel.GetLICIndiaBillResponseModel;

namespace Project_Redmil_MVC.Controllers.BillPayments.LICIndiaBillController
{
    public class LICIndiaBillController : Controller
    {
        public GetLICIndiaBillResponseModel getLICIndiaBillResponseModel;
        public GetLICIndiaBillResponseModel.BillFetch getLICIndiaBillFetchResponseModel;
        private readonly string Baseurl;
        private readonly IConfiguration _config;
        public LICIndiaBillController(IConfiguration config)
        {
            _config = config;
            Baseurl = "https://proapitest4.redmilbusinessmall.com/api/";//HelperMethod.GetBaseURl(_config);
        }


        public IActionResult LICIndiaBill()
        {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {

                return RedirectToAction("ErrorForLogin", "Error");

            }
            return View();
        }

        #region FetchLICBill

        [HttpPost]

        public JsonResult FetchBill(string caNumber, string email, string Payment)
        {
            GetLICIndiaBillRequestModel requestModel = new GetLICIndiaBillRequestModel();
            requestModel.canumber = caNumber;
            requestModel.ad1 = email;
            requestModel.UserId = "2084";
            #region Checksum (fetchlicbill|Unique Key|UserId)
            string input = Checksum.MakeChecksumString("LIC", Checksum.checksumKey, requestModel.UserId, requestModel.canumber, requestModel.ad1);
            string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
            #endregion
            requestModel.checksum = CheckSum;
            var client = new RestClient($"{Baseurl}{ApiName.GetLICBill}");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var json = JsonConvert.SerializeObject(requestModel);
            request.AddJsonBody(json);
            IRestResponse response = client.Execute(request);
            var result = response.Content;
            var deserialize = JsonConvert.DeserializeObject<BaseLICResponseModelT<GetLICIndiaBillResponseModel>>(response.Content);
            getLICIndiaBillResponseModel = deserialize.Data;
            getLICIndiaBillFetchResponseModel = deserialize.Data.bill_fetch;

            #region Pay LIC Bill
            if (deserialize.Statuscode == "TXN")
            {
                if (!string.IsNullOrEmpty(Payment))
                {
                    PayLICIndiaBillRequestModel requestmodel = new PayLICIndiaBillRequestModel();
                    requestmodel.canumber = caNumber;
                    requestmodel.Userid = "2084";
                    requestmodel.amount = getLICIndiaBillResponseModel.amount;
                    requestmodel.Wallet = Payment;
                    requestmodel.ad1 = email;
                    requestmodel.ad2 = "";
                    requestmodel.ad3 = "";
                    requestmodel.latitude = "28.582121";
                    requestmodel.longitude = "77.326698";
                    requestmodel.bill_fetch = getLICIndiaBillResponseModel.bill_fetch;
                    requestmodel.Token = "";
                    //requestmodel.mode = "";
                    #region Checksum (PayLICBill|Unique Key|Userid|canumber|ad1|amount)
                    string inputN = Checksum.MakeChecksumString("PayLICBill", Checksum.checksumKey, requestmodel.Userid, requestmodel.canumber, requestmodel.ad1, requestmodel.amount);
                    string CheckSumN = Checksum.ConvertStringToSCH512Hash(inputN);
                    #endregion

                    requestmodel.checksum = CheckSumN;
                    var clientN = new RestClient($"{Baseurl}{ApiName.PayLICBill}");
                    var requestN = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    var jsonN = JsonConvert.SerializeObject(requestmodel);
                    requestN.AddJsonBody(jsonN);
                    IRestResponse responseN = client.Execute(requestN);
                    var resultN = responseN.Content;
                    var deserializePayLICBill = JsonConvert.DeserializeObject<BaseResponseModel>(responseN.Content);
                    if (deserializePayLICBill.Statuscode == "TXN")
                    {
                        var dataPayLICBill = deserializePayLICBill.Data;

                        var dataListPayLICBill = JsonConvert.DeserializeObject<List<PayLICIndiaBillResponseModel>>(JsonConvert.SerializeObject(dataPayLICBill));
                        return Json(dataListPayLICBill);
                    }
                    else
                    {
                        var dataPayLICBill = deserializePayLICBill.Message;
                        return Json(dataPayLICBill);

                    }
                }

                return Json(getLICIndiaBillFetchResponseModel);
            }
            else if (deserialize.Statuscode == "ERR")
            {
                return Json(deserialize.Message);
            }

            #endregion
            return Json("");

        }

        #endregion

        #region GetBalance
        [HttpPost]
        public JsonResult GetBalance()
        {
            GetBalanceRequestModel getBalanceRequestModel = new GetBalanceRequestModel();
            getBalanceRequestModel.Userid = "2084";
            #region Checksum (GetBalance|Unique Key|UserId)
            string input = Checksum.MakeChecksumString("Getbalance", Checksum.checksumKey, getBalanceRequestModel.Userid);
            string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
            #endregion
            getBalanceRequestModel.checksum = CheckSum;
            //API URL Has been changed by Siddhartha Sir
            var client = new RestClient($"{Baseurl}{ApiName.Getbalance}");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var json = JsonConvert.SerializeObject(getBalanceRequestModel);
            request.AddJsonBody(json);
            IRestResponse response = client.Execute(request);
            var result = response.Content;
            var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
            var data = deserialize.Data;
            var datalist = JsonConvert.DeserializeObject<List<GetBalanceResponseModel>>(JsonConvert.SerializeObject(data));
            List<GetBalanceResponseModel> lstdata = new List<GetBalanceResponseModel>();
            lstdata = datalist.ToList();
            return Json(lstdata);
        }

        #endregion
    }
}
