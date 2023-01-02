﻿using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Controllers.UserDashoard;
using Project_Redmil_MVC.Helper;
using Project_Redmil_MVC.Models;
using Project_Redmil_MVC.Models.PayoutCommision;
using Project_Redmil_MVC.Models.RequestModel;
using Project_Redmil_MVC.Models.RequestModel.AccountverificationwithchargeRequestmodel;
using Project_Redmil_MVC.Models.RequestModel.DMT2RequestModel;
using Project_Redmil_MVC.Models.RequestModel.ForimageRequestModel;
using Project_Redmil_MVC.Models.RequestModel.ivrrequestmodel;
using Project_Redmil_MVC.Models.RequestModel.LiveFaceLiveNess;
using Project_Redmil_MVC.Models.RequestModel.MakeCashOutNewRequestModel;
using Project_Redmil_MVC.Models.RequestModel.PassbookTranferAmountImpsRequest;
using Project_Redmil_MVC.Models.RequestModel.SupportGetTransactionUsingBalanaceIdRequestModel;
using Project_Redmil_MVC.Models.ResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.AccountverificationForsignupwithchargeResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.DMT2ResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.GetBank;
using Project_Redmil_MVC.Models.ResponseModel.PassbookBReward;
using Project_Redmil_MVC.Models.ResponseModel.PassbookGetCashOutSurchargeNewResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.PassBookGetMultiAccountDetailResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.PassbookGetTransactionUsingBalanaceIdResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.PassbookTransactionAmountResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.PayoutCommisonResponseModel;
using RestSharp;
using RestSharp.Deserializers;
using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
using System.Reflection;
//using System.Web.Helpers;
using Microsoft.AspNetCore.Hosting;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Project_Redmil_MVC.Models.RequestModel.InsertMultiAccountDetailForUserRequestModel;
using Project_Redmil_MVC.Models.RequestModel.ValidateOTPForMultiAccountRequestOtp;
using Project_Redmil_MVC.Models.RequestModel.ValidateOTPForMultiAccountRequestModel;
using static System.Net.WebRequestMethods;
using Project_Redmil_MVC.Models.ResponseModel.InsertMultiAccountDetailNewRequestModel;
using Project_Redmil_MVC.Models.RequestModel.InsertMultiAccoutn_NewRequestModel;
using Project_Redmil_MVC.Models.ResponseModel.PssbookSuccesResponseModal;
using Project_Redmil_MVC.Models.RequestModel.PassboookDownloadWalletRequestModel;
using Project_Redmil_MVC.Models.ResponseModel.GetCashDepositeResponseModel;
using Project_Redmil_MVC.Models.RequestModel.MakeCashOutDeposite;

namespace Project_Redmil_MVC.Controllers
{
    public class PassbookController : Controller
    {
        private readonly string Baseurl;
        private readonly IConfiguration _config;
        private IWebHostEnvironment _webHostEnvironment;
        //private readonly ApplicationContext context;
        //private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment environment;
        InserMutltiAccount objMulti = new InserMutltiAccount();
        MakeCashOutNewRequestModel objfinal = new MakeCashOutNewRequestModel();
        public PassbookController(IConfiguration config, IWebHostEnvironment webHostEnvironment)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
            _webHostEnvironment = webHostEnvironment;
        }

        List<CashWalletRecentResponseDetailsPassbookModel> gdata = new List<CashWalletRecentResponseDetailsPassbookModel>();
        #region Cash Wallet Passbook 

        [HttpGet]
        public IActionResult Passbook()
        {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {
                return RedirectToAction("ErrorForLogin", "Error");
            }
            //var baseUrl = _config.GetSection("ApiUrl").GetSection("BaseUrl").Value;
            var baseUrl = "https://api.redmilbusinessmall.com";
            var checkSum = _config.GetSection("ApiUrl").GetSection("CheckSumUrl").Value;
            var Client = _config.GetSection("ApiUrl").GetSection("ClientUrl").Value;
            List<AdvanceWalletResponsePassbookDetailsModel> adv = AdvanceWallet();
            List<BRewardDetailResponseModel> BrRewads = BRP();
            List<RERewardResponseModel> RERewrads = REP();
            //var Data1 = new List<CashWalletRecentResponseDetailsPassbookModel>();
            AdvanceWalletRequestPassbookDetailsModel requestModel = new AdvanceWalletRequestPassbookDetailsModel();
            requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
            requestModel.FilterBy = "All";
            requestModel.WalletType = "Cash Wallet";
            requestModel.PageNumber = "1";
            requestModel.Token = "ezPSD8JHSi-20841401";
            requestModel.FromDate = "2022-11-01";
            requestModel.ToDate = DateTime.Now.ToString("yyyy-MM-dd");
            #region Checksum (GetUserBalanceSummaryWithPaging|Unique Key|UserId)
            string input = Checksum.MakeChecksumString("GetUserBalanceSummaryWithPaging", Checksum.checksumKey,
                requestModel.Userid, requestModel.FilterBy, requestModel.WalletType, requestModel.PageNumber,
                requestModel.Token, requestModel.FromDate, requestModel.ToDate);
            //string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
            string CheckSum = "f42e229f1389fa25f4cffa1f976bc0941e8e5b1710645eaf85ce528afa463718c7f582fe376f30989b4b9f7dfc47001fb4f148b1a85c45e76d8c8b8961e400fa";
            #endregion
            CashWalletRecentResponseDetailsPassbookModel cashWalletRecent = new CashWalletRecentResponseDetailsPassbookModel();
            cashWalletRecent.baseUrl = Baseurl;
            requestModel.checksum = CheckSum;
            //var client = new RestClient(Client);
            var client = new RestClient($"{Baseurl}{ApiName.GetUserBalanceSummaryWithPaging}");
            //Create request with GET
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var json = JsonConvert.SerializeObject(requestModel);
            request.AddJsonBody(json);
            IRestResponse response = client.Execute(request);
            var result = response.Content;
            var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
            var datadeserialize = deserialize.Data;
            var data = JsonConvert.DeserializeObject<List<CashWalletRecentResponseDetailsPassbookModel>>(JsonConvert.SerializeObject(datadeserialize));
            if (deserialize.Statuscode == "TXN")
            {
                if (data.FirstOrDefault().New_bal.ToString() != null && data.FirstOrDefault().New_bal.ToString() != "")
                {
                    HttpContext.Session.SetString("Balance", data.FirstOrDefault().New_bal.ToString());
                    HttpContext.Session.GetString("Balance").ToString();
                }
                else
                {
                    HttpContext.Session.SetString("Balance", "0");
                }
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        gdata.Add(new CashWalletRecentResponseDetailsPassbookModel
                        {
                            Id = item.Id,
                            Title = item.Title,
                            Amount = item.Amount,
                            Detail = item.Detail,
                            CreditDebit = item.CreditDebit,
                            Client = item.Client,
                            GstAmount = item.GstAmount,
                            TdsAmount = item.TdsAmount,
                            New_bal = item.New_bal,
                            Img = baseUrl + item.Img,
                            Amount1 = string.Format("{0:0.00}", item.Amount - Convert.ToDouble(item.TdsAmount)).ToString(),
                            AdvanceWalletResponsePassbookDetailsModel = adv,
                            BRewardDetailResponseModel = BrRewads,
                            RERewardResponseModel = RERewrads
                        });
                    }
                }
            }
            //var deserialize = JsonConvert.DeserializeObject<BaseResponseModelT<List<CashWalletRecentResponseDetailsPassbookModel>>>(response.Content);
            else
            {

            }

            return View(gdata);
        }

        #endregion

        #region Advance Wallet Passbook

        [HttpGet]
        public List<AdvanceWalletResponsePassbookDetailsModel> AdvanceWallet()
        {
            var baseUrl = "https://api.redmilbusinessmall.com";
            //var baseUrl = "api.redmilbusinessmall.com/";
            var Client = _config.GetSection("ApiUrl").GetSection("ClientUrl").Value;
            var advData1 = new List<AdvanceWalletResponsePassbookDetailsModel>();
            AdvanceWalletRequestPassbookDetailsModel requestModel = new AdvanceWalletRequestPassbookDetailsModel();
            try
            {
                requestModel.Userid = "2084";
                requestModel.FilterBy = "All";
                requestModel.WalletType = "Advance Wallet";
                requestModel.PageNumber = "1";
                requestModel.Token = "ezPSD8JHSi-20841401";
                requestModel.FromDate = "2022-11-01";
                requestModel.ToDate = DateTime.Now.ToString("yyyy-MM-dd");
                #region Checksum (GetUserBalanceSummaryWithPaging|Unique Key|UserId)
                //GetUserBalanceSummaryWithPaging|Unique Key|Userid|WalletType|FilterBy|PageNumber
                string input = Checksum.MakeChecksumString("GetUserBalanceSummaryWithPaging", Checksum.checksumKey,
                    requestModel.Userid, requestModel.WalletType, requestModel.FilterBy, requestModel.PageNumber);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                AdvanceWalletResponsePassbookDetailsModel advanceWalletRecent = new AdvanceWalletResponsePassbookDetailsModel();
                advanceWalletRecent.baseUrl = baseUrl;
                requestModel.checksum = "ac91b8846d245fc904ed71194f96b992575fc3f72b329f480c329833f7c7934f26caadc40b6981f303f9f06ab980c43f5d8168ed276e87d94008d91849d30b54";
                //var client = new RestClient(Client);
                var client = new RestClient($"{Baseurl}{ApiName.GetUserBalanceSummaryWithPaging}");
                //Create request with GET
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
                if(deserialize.Statuscode=="TXN" && deserialize != null)
                {
                    var datadeserialize = deserialize.Data;
                    var advData = JsonConvert.DeserializeObject<List<AdvanceWalletResponsePassbookDetailsModel>>(JsonConvert.SerializeObject(datadeserialize));

                    if (advData != null)
                    {
                        foreach (var item in advData)
                        {
                            advData1.Add(new AdvanceWalletResponsePassbookDetailsModel
                            {
                                Id = item.Id,
                                Title = item.Title,
                                Amount = item.Amount,
                                Client = item.Client,
                                New_bal = item.New_bal,
                                Img = baseUrl + item.Img,
                                Detail = item.Detail,
                                Amount1 = string.Format("{0:0.00}", item.New_bal - item.Amount).ToString(),

                            });
                        }
                    }
                    return advData1;
                }
                else if (deserialize.Statuscode == "ERR")
                {
                    return advData1;
                }
                else
                {
                    
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
            return advData1;

        }

        #endregion

        #region Get Balance Method
        [HttpPost]
        public List<GetBalanceResponseModel> GetBalance()
        {
            GetBalanceRequestModel getBalanceRequestModel = new GetBalanceRequestModel();
            List<GetBalanceResponseModel> lstdata = new List<GetBalanceResponseModel>();
            try
            {
                getBalanceRequestModel.Userid = "2084";
                #region Checksum (GetBalance|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("Getbalance", Checksum.checksumKey, getBalanceRequestModel.Userid);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                getBalanceRequestModel.checksum = CheckSum;
                //API URL Has been changed by Siddhartha Sir
                var client = new RestClient("https://api.redmilbusinessmall.com/api/Getbalance");
                //var client = new RestClient($"{Baseurl}{ApiName.Getbalance}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(getBalanceRequestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                if(deserialize.Statuscode=="TXN" && deserialize != null)
                {
                    var data = deserialize.Data;

                    lstdata = JsonConvert.DeserializeObject<List<GetBalanceResponseModel>>(JsonConvert.SerializeObject(data)).ToList();
                    return lstdata;
                }
                else if (deserialize.Statuscode == "ERR")
                {
                    return lstdata;
                }
                else
                {

                }
                
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = getBalanceRequestModel;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                requestEx.AddJsonBody(json);
                IRestResponse response = client.Execute(requestEx);
                var result = response.Content;
            }
            return lstdata;

        }
        #endregion

        #region Passbook Detail

        public JsonResult GetBalancedetail(string BalanceId)
        {

            GetTransactionDetail obj = new GetTransactionDetail();
            obj.Userid = "2084";
            obj.BalanceId = BalanceId;
            obj.Type = "1";
            obj.Wallet = "Cash Wallet";
            var client = new RestClient($"{Baseurl}{ApiName.GetTransactionDetailsUseingBalanceId}");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var json = JsonConvert.SerializeObject(obj);
            request.AddJsonBody(json);
            IRestResponse response = client.Execute(request);
            var result = response.Content;
            var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
            if (deserialize.Statuscode == "ERR")
            {
                return Json(deserialize);

            }
            return Json(result);

        }
        #endregion
        #region GetAdvanceBalancedetail
        public JsonResult GetAdvanceBalancedetail(string BalanceId)
        {
            GetTransactionDetail obj = new GetTransactionDetail();
            try
            {
                obj.Userid = "2084";
                obj.BalanceId = BalanceId;
                obj.Type = "1";
                obj.Wallet = "Advance Wallet";
                var client = new RestClient($"{Baseurl}{ApiName.GetTransactionDetailsUseingBalanceId}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(obj);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                if (result != null)
                {
                    return Json(result);
                }
                else
                {
                    return Json("");
                }
                
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = obj;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                requestEx.AddJsonBody(json);
                IRestResponse response = client.Execute(requestEx);
                var result = response.Content;
            }
            return Json("");

        }
        #endregion
        #region DownloadReportForWallet
        public JsonResult DownloadReportForWallet(string fromDate, string currentDate)
        {

            DownloadWalletRequestModel requestModel = new DownloadWalletRequestModel();
            requestModel.UserId = "2180";
            requestModel.PageNumber = "1";
            requestModel.ToDate = currentDate;
            requestModel.FromDate = fromDate;
            requestModel.ServiceType = "RewardWallet";
            requestModel.Report = "true";
            requestModel.SortBy = "desc";
            requestModel.Status = "";
            #region Checksum (GetUserBalanceSummaryWithPaging|Unique Key|UserId)
            //GetUserBalanceSummaryWithPaging|Unique Key|Userid|WalletType|FilterBy|PageNumber
            string input = Checksum.MakeChecksumString("transactionsreport", Checksum.checksumKey,
                requestModel.UserId);
            string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
            requestModel.Checksum = CheckSum;
            #endregion
            var client = new RestClient($"{Baseurl}{ApiName.transactionsreport}");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var json = JsonConvert.SerializeObject(requestModel);
            request.AddJsonBody(json);
            IRestResponse response = client.Execute(request);
            var result = response.Content;
            var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
            if (deserialize.Statuscode == "TXN")
            {
                var myBaseUrl = "https://api.redmilbusinessmall.com/";
                return Json(myBaseUrl + deserialize.Message);
            }
            return Json(result);


        }
        #endregion
        #region RERewardsPassbook
        public JsonResult RERewardsPassbook(string fromDate, string currentDate)
        {
            try
            {


            DownloadWalletRequestModel requestModel = new DownloadWalletRequestModel();
            requestModel.UserId = "2180";
            requestModel.PageNumber = "1";
            requestModel.ToDate = currentDate;
            requestModel.FromDate = fromDate;
            requestModel.ServiceType = "ReferEarnWallet";
            requestModel.Report = "true";
            requestModel.SortBy = "desc";
            requestModel.Status = "";
            #region Checksum (GetUserBalanceSummaryWithPaging|Unique Key|UserId)
            //GetUserBalanceSummaryWithPaging|Unique Key|Userid|WalletType|FilterBy|PageNumber
            string input = Checksum.MakeChecksumString("transactionsreport", Checksum.checksumKey,
                requestModel.UserId);
            string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
            requestModel.Checksum = CheckSum;
            #endregion
            var client = new RestClient($"{Baseurl}{ApiName.transactionsreport}");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var json = JsonConvert.SerializeObject(requestModel);
            request.AddJsonBody(json);
            IRestResponse response = client.Execute(request);
            var result = response.Content;
            var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
            if (deserialize.Statuscode == "TXN")
            {
                var myBaseUrl = "https://api.redmilbusinessmall.com/";
                return Json(myBaseUrl + deserialize.Message);
            }
            return Json(result);

        }
        #endregion

        public JsonResult CashWallet(string fromDate, string currentDate)
        {

            DownloadWalletRequestModel requestModel = new DownloadWalletRequestModel();
            requestModel.UserId = "2180";
            requestModel.PageNumber = "1";
            requestModel.ToDate = currentDate;
            requestModel.FromDate = fromDate;
            requestModel.ServiceType = "CashWallet";
            requestModel.Report = "true";
            requestModel.SortBy = "desc";
            requestModel.Status = "";
            #region Checksum (GetUserBalanceSummaryWithPaging|Unique Key|UserId)
            //GetUserBalanceSummaryWithPaging|Unique Key|Userid|WalletType|FilterBy|PageNumber
            string input = Checksum.MakeChecksumString("transactionsreport", Checksum.checksumKey,
                requestModel.UserId);
            string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
            requestModel.Checksum = CheckSum;
            #endregion
            var client = new RestClient($"{Baseurl}{ApiName.transactionsreport}");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var json = JsonConvert.SerializeObject(requestModel);
            request.AddJsonBody(json);
            IRestResponse response = client.Execute(request);
            var result = response.Content;
            var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
            if (deserialize.Statuscode == "TXN")
            {
                var myBaseUrl = "https://api.redmilbusinessmall.com/";
                return Json(myBaseUrl + deserialize.Message);
            }
            return Json(result);

        }
        public JsonResult AdvancedDownload(string fromDate, string currentDate)
        {

            DownloadWalletRequestModel requestModel = new DownloadWalletRequestModel();
            requestModel.UserId = "2180";
            requestModel.PageNumber = "1";
            requestModel.ToDate = currentDate;
            requestModel.FromDate = fromDate;
            requestModel.ServiceType = "AdvanceWallet";
            requestModel.Report = "true";
            requestModel.SortBy = "desc";
            requestModel.Status = "";
            #region Checksum (GetUserBalanceSummaryWithPaging|Unique Key|UserId)
            //GetUserBalanceSummaryWithPaging|Unique Key|Userid|WalletType|FilterBy|PageNumber
            string input = Checksum.MakeChecksumString("transactionsreport", Checksum.checksumKey,
                requestModel.UserId);
            string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
            requestModel.Checksum = CheckSum;
            #endregion
            var client = new RestClient($"{Baseurl}{ApiName.transactionsreport}");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var json = JsonConvert.SerializeObject(requestModel);
            request.AddJsonBody(json);
            IRestResponse response = client.Execute(request);
            var result = response.Content;
            var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
            if (deserialize.Statuscode == "TXN")
            {
                var myBaseUrl = "https://api.redmilbusinessmall.com/";
                return Json(myBaseUrl + deserialize.Message);
            }
            return Json(deserialize);

        }
        #region BRewardDetailResponseModel
        public List<BRewardDetailResponseModel> BRP()
        {

            var baseUrl = "https://api.redmilbusinessmall.com";
            var checkSum = _config.GetSection("ApiUrl").GetSection("CheckSumUrl").Value;
            var Client = _config.GetSection("ApiUrl").GetSection("ClientUrl").Value;
            List<BRewardDetailResponseModel> BrRewads = new List<BRewardDetailResponseModel>();
            AdvanceWalletRequestPassbookDetailsModel requestModel = new AdvanceWalletRequestPassbookDetailsModel();
            try
            {
                requestModel.Userid = "2084";
                requestModel.FilterBy = "All";
                requestModel.WalletType = "BRewards";
                requestModel.PageNumber = "1";
                requestModel.Token = "ezPSD8JHSi-20841401";
                requestModel.FromDate = "2022-07-10";
                requestModel.ToDate = "2022-11-23";
                #region Checksum (GetUserBalanceSummaryWithPaging|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("GetUserBalanceSummaryWithPaging", Checksum.checksumKey,
                    requestModel.Userid, requestModel.WalletType, requestModel.FilterBy, requestModel.PageNumber);
                //string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                string CheckSum = "77311af5b2eb0057763a5a5650028f18743e3cb7b4b83fb6610650a168fedf5fe62efd90bca24ca1d95d4aa1e0c5fdc3f25ed233c9aef5ea3b979f7961da10ce";
                #endregion
                BRewardDetailResponseModel cashWalletRecent = new BRewardDetailResponseModel();
                cashWalletRecent.baseUrl = Baseurl;
                requestModel.checksum = CheckSum;
                //var client = new RestClient(Client);
                var client = new RestClient($"{Baseurl}{ApiName.GetUserBalanceSummaryWithPaging}");
                //Create request with GET
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
                if(deserialize.Statuscode=="TXN" && deserialize != null)
                {
                    var datadeserialize = deserialize.Data;
                    var data = JsonConvert.DeserializeObject<List<BRewardDetailResponseModel>>(JsonConvert.SerializeObject(datadeserialize));
                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            BrRewads.Add(new BRewardDetailResponseModel
                            {
                                Id = item.Id,
                                Title = item.Title,
                                Point = item.Point,
                                Detail = item.Detail,
                                CreditDebit = item.CreditDebit,
                                Client = item.Client,
                                Old_bal = item.Old_bal,
                                New_bal = item.New_bal,
                                Img = baseUrl + item.Img,
                                Transaction_date = item.Transaction_date
                                //Amount1 = string.Format("{0:0.00}", item.Amount - item.TdsAmount).ToString()
                            });
                        }
                    }

                    return BrRewads;
                }
                else if (deserialize.Statuscode == "ERR")
                {
                    
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = "";
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                requestEx.AddJsonBody(json);
                IRestResponse response = client.Execute(requestEx);
                var result = response.Content;
            }
            return BrRewads;
        }

        #endregion
        #region RERewardResponseModel
        public List<RERewardResponseModel> REP()
        {

            var baseUrl = "https://api.redmilbusinessmall.com";
            var checkSum = _config.GetSection("ApiUrl").GetSection("CheckSumUrl").Value;
            var Client = _config.GetSection("ApiUrl").GetSection("ClientUrl").Value;
            List<RERewardResponseModel> RERewards = new List<RERewardResponseModel>();
            AdvanceWalletRequestPassbookDetailsModel requestModel = new AdvanceWalletRequestPassbookDetailsModel();
            try
            {
                requestModel.Userid = "2084";
                requestModel.FilterBy = "All";
                requestModel.WalletType = "RERewards";
                requestModel.PageNumber = "1";
                requestModel.Token = "ezPSD8JHSi-20841401";
                requestModel.FromDate = "2022-07-10";
                requestModel.ToDate = DateTime.Now.ToString("yyyy-MM-dd");
                #region Checksum (GetUserBalanceSummaryWithPaging|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("GetUserBalanceSummaryWithPaging", Checksum.checksumKey,
                    requestModel.Userid, requestModel.WalletType, requestModel.FilterBy, requestModel.PageNumber);
                // string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                string CheckSum = "541fc499a9c9cbc125d0ddb34a209d3e7d2f46a167d6982500fa143dec5fc82aa6144c5588c4243a476ffbbb0751a98dd5b08b364596418a93f9193ca4785b65";
                #endregion
                RERewardResponseModel cashWalletRecent = new RERewardResponseModel();
                cashWalletRecent.baseUrl = Baseurl;
                requestModel.checksum = CheckSum;
                //var client = new RestClient(Client);
                var client = new RestClient($"{Baseurl}{ApiName.GetUserBalanceSummaryWithPaging}");
                //Create request with GET
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
                if(deserialize.Statuscode=="TXN" && deserialize != null)
                {
                    var datadeserialize = deserialize.Data;
                    var data = JsonConvert.DeserializeObject<List<RERewardResponseModel>>(JsonConvert.SerializeObject(datadeserialize));
                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            RERewards.Add(new RERewardResponseModel
                            {
                                Id = item.Id,
                                Title = item.Title,
                                Point = item.Point,
                                Detail = item.Detail,
                                CreditDebit = item.CreditDebit,
                                Client = item.Client,
                                Old_bal = item.Old_bal,
                                New_bal = item.New_bal,
                                Img = baseUrl + item.Img,
                                Transaction_date = item.Transaction_date
                                //Amount1 = string.Format("{0:0.00}", item.Amount - item.TdsAmount).ToString()
                            });
                        }
                    }

                    return RERewards;
                }
                else if (deserialize.Statuscode == "ERR")
                {

                }
                else
                {

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
            return RERewards;




        }
        #endregion

        #region RERewardDetail
        public JsonResult RERewardDetail(string BalanceId)
        {
            GetTransactionDetail obj = new GetTransactionDetail();
            try
            {
                obj.Userid = "2084";
                obj.BalanceId = BalanceId;
                obj.Type = "1";
                obj.Wallet = "RERewards";
                var client = new RestClient($"{Baseurl}{ApiName.GetTransactionDetailsUseingBalanceId}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(obj);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                if (result != null)
                {
                    return Json(result);
                }
                else
                {
                    return Json("");
                }
                
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = obj;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                requestEx.AddJsonBody(json);
                IRestResponse response = client.Execute(requestEx);
                var result = response.Content;
            }
            return Json("");

        }
        #endregion
        #region BRReward
        public JsonResult BRReward(string BalanceId)
        {
            GetTransactionDetail obj = new GetTransactionDetail();
            try
            {
                obj.Userid = "2084";
                obj.BalanceId = BalanceId;
                obj.Type = "1";
                obj.Wallet = "BRewards";
                var client = new RestClient($"{Baseurl}{ApiName.GetTransactionDetailsUseingBalanceId}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(obj);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                if (result != null)
                {
                    return Json(result);
                }
                else
                {
                    return Json("");
                }
                
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = obj;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                requestEx.AddJsonBody(json);
                IRestResponse response = client.Execute(requestEx);
                var result = response.Content;
            }
            return Json("");


        }
        #endregion
        public IActionResult RechargeyourCash()
        {
            List<GetBalanceResponseModel> lstdata = new List<GetBalanceResponseModel>();
            lstdata = GetBalance();
            return View(lstdata);
        }
        public IActionResult TranferToBank()
        {

            ViewBag.BankAccount = new SelectList(BankDetail(), "Id", "BankName");
            var baseUrl = "https://api.redmilbusinessmall.com";
            //var baseUrl = "api.redmilbusinessmall.com/";
            var Client = _config.GetSection("ApiUrl").GetSection("ClientUrl").Value;
            List<PassbookGetCashsurchargeRespnseModel> TransferCash = new List<PassbookGetCashsurchargeRespnseModel>();
            AdvanceWalletRequestPassbookDetailsModel requestModel = new AdvanceWalletRequestPassbookDetailsModel();
            try
            {
                requestModel.Userid = "2084";
                #region Checksum (GetUserBalanceSummaryWithPaging|Unique Key|UserId)
                //GetUserBalanceSummaryWithPaging|Unique Key|Userid|WalletType|FilterBy|PageNumber
                string input = Checksum.MakeChecksumString("GetCashoutSurchargeNew", Checksum.checksumKey,
                    requestModel.Userid);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                var client = new RestClient($"{Baseurl}{ApiName.GetCashoutSurchargeNew}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
                if(deserialize.Statuscode=="TXN" && deserialize != null)
                {
                    var datadeserialize = deserialize.Data;
                    var AddCashData = JsonConvert.DeserializeObject<PassbookGetCashsurchargeRespnseModel>(JsonConvert.SerializeObject(datadeserialize));

                    return View(AddCashData);
                }
                else if (deserialize.Statuscode == "ERR")
                {
                    return View(deserialize);
                }
                else
                {
                    //return View()
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
            return View();

        }
        string ServiceId;
        string OpId;
        private object webHostEnvironment;
        public JsonResult ImpstransferAmount(string Amount, string ServiceId, string OpId)
        {
            var baseUrl = "https://api.redmilbusinessmall.com";
            GetCashoutAcutalAmountCreditRequestModel requestModel = new GetCashoutAcutalAmountCreditRequestModel();
            try
            {
                requestModel.UserId = "2180";
                requestModel.ModeId = "1";
                requestModel.Amount = Amount;
                ServiceId = ServiceId;
                OpId = OpId;
                #region Checksum (GetUserBalanceSummaryWithPaging|Unique Key|UserId)
                //GetUserBalanceSummaryWithPaging|Unique Key|Userid|WalletType|FilterBy|PageNumber
                string input = Checksum.MakeChecksumString("GetCashoutAcutalAmountCredit", Checksum.checksumKey,
                    requestModel.UserId, requestModel.ModeId, requestModel.Amount);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                var client = new RestClient($"{Baseurl}{ApiName.GetCashoutAcutalAmountCredit}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
                if (!string.IsNullOrEmpty(deserialize.Statuscode) && deserialize.Statuscode == "ERR")
                {
                    return Json(deserialize);
                }
                else if(deserialize.Statuscode=="TXN")
                {
                    var datadeserialize = deserialize.Data;
                    var TranferData = JsonConvert.DeserializeObject<TransferAmountResponseModel>(JsonConvert.SerializeObject(datadeserialize));

                    return Json(new { actualAmount = TranferData.ActualAmount, charge = TranferData.Charge, Mode = "IMPS" });
                }
                else
                {
                    return Json("");
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
            return Json("");


        }
        public JsonResult NefttransferAmount(string Amount, string ServiceId, string OpId)
        {
            var baseUrl = "https://api.redmilbusinessmall.com";
            GetCashoutAcutalAmountCreditRequestModel requestModel = new GetCashoutAcutalAmountCreditRequestModel();
            try
            {
                requestModel.UserId = "2180";
                requestModel.ModeId = "2";
                requestModel.Amount = Amount;
                #region Checksum (GetUserBalanceSummaryWithPaging|Unique Key|UserId)
                //GetUserBalanceSummaryWithPaging|Unique Key|Userid|WalletType|FilterBy|PageNumber
                string input = Checksum.MakeChecksumString("GetCashoutAcutalAmountCredit", Checksum.checksumKey,
                    requestModel.UserId, requestModel.ModeId, requestModel.Amount);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                var client = new RestClient($"{Baseurl}{ApiName.GetCashoutAcutalAmountCredit}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
                if (!string.IsNullOrEmpty(deserialize.Statuscode) && deserialize.Statuscode == "ERR")
                {
                    return Json(deserialize);
                }
                else if(deserialize.Statuscode=="TXN")
                {
                    var datadeserialize = deserialize.Data;
                    var TranferData = JsonConvert.DeserializeObject<TransferAmountResponseModel>(JsonConvert.SerializeObject(datadeserialize));
                    return Json(new { actualAmount = TranferData.ActualAmount, charge = TranferData.Charge, Mode = "NEFT" });
                }
                else
                {
                    return Json("");
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
            return Json("");


        }
        public JsonResult RtgstransferAmount(string Amount, string ServiceId, string OpId)
        {
            var baseUrl = "https://api.redmilbusinessmall.com";
            GetCashoutAcutalAmountCreditRequestModel requestModel = new GetCashoutAcutalAmountCreditRequestModel();
            try
            {
                requestModel.UserId = "2180";
                requestModel.ModeId = "3";
                requestModel.Amount = Amount;
                ServiceId = ServiceId;
                OpId = OpId;
                #region Checksum (GetUserBalanceSummaryWithPaging|Unique Key|UserId)
                //GetUserBalanceSummaryWithPaging|Unique Key|Userid|WalletType|FilterBy|PageNumber
                string input = Checksum.MakeChecksumString("GetCashoutAcutalAmountCredit", Checksum.checksumKey,
                    requestModel.UserId, requestModel.ModeId, requestModel.Amount);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                var client = new RestClient($"{Baseurl}{ApiName.GetCashoutAcutalAmountCredit}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
                if (!string.IsNullOrEmpty(deserialize.Statuscode) && deserialize.Statuscode == "ERR")
                {
                    return Json(deserialize);
                }
                else if(deserialize.Statuscode=="TXN")
                {
                    var datadeserialize = deserialize.Data;
                    var TranferData = JsonConvert.DeserializeObject<TransferAmountResponseModel>(JsonConvert.SerializeObject(datadeserialize));
                    return Json(new { actualAmount = TranferData.ActualAmount, charge = TranferData.Charge, Mode = "RTGS" });
                }
                else
                {
                    return Json("");
                }
            }
            catch(Exception ex)
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
            return Json("");
           

        }
        public JsonResult MultiAcountDetail()
        {
            var baseUrl = "https://api.redmilbusinessmall.com";
            GetCashoutAcutalAmountCreditRequestModel requestModel = new GetCashoutAcutalAmountCreditRequestModel();
            List<GetMultiAccountDetailResponse> Bankdetail = new List<GetMultiAccountDetailResponse>();
            try
            {
                requestModel.Userid = "2180";
                #region Checksum (GetUserBalanceSummaryWithPaging|Unique Key|UserId)
                //GetUserBalanceSummaryWithPaging|Unique Key|Userid|WalletType|FilterBy|PageNumber
                string input = Checksum.MakeChecksumString("GetMultiAccountDetailsForUsers", Checksum.checksumKey,
                    requestModel.Userid);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                requestModel.checksum = CheckSum;
                #endregion
                var client = new RestClient($"{Baseurl}{ApiName.GetMultiAccountDetailsForUsers}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
                if(deserialize.Statuscode=="TXN" && deserialize != null)
                {
                    var datadeserialize = deserialize.Data;
                    var data = JsonConvert.DeserializeObject<List<GetMultiAccountDetailResponse>>(JsonConvert.SerializeObject(datadeserialize));
                    //if (data != null)
                    //{
                    //    //foreach (var item in data)
                    //    //{
                    //    //    Bankdetail.Add(new GetMultiAccountDetailResponse
                    //    //    {
                    //    //        Bankdetail.BankId=item.BankId,
                    //    //        Bankdetail.BankName=item.BankName,
                    //    //        Bankdetail.AccountNo=item.AccountNo,
                    //    //        Bankdetail.Ifsc=item.Ifsc,
                    //    //        Bankdetail.BeniName

                    //    //    })
                    //    //}
                    //}

                    return Json(data);
                }
                else if (deserialize.Statuscode == "ERR")
                {
                    return Json(deserialize);
                }
                else
                {
                    return Json("");
                }
                

            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                requestModelEx.ExceptionMessage = ex;
                requestModelEx.Data = requestModel;
                var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                requestEx.AddJsonBody(jsonEx);
                IRestResponse responseEx = clientEx.Execute(requestEx);
                var resultEx = responseEx.Content;
            }
            return Json("");

        }
        String Amount;
        string ModeId;
        string BankId;
        string BankName;
        string BeniName;
        string Account;
        string IFSC;
        //string obj;

        public JsonResult Confirm(string Amount, string ServiceId, string OpId, string ModeId, string BankId, string BankName, string BeniName, string Account, string IFSC)
        {
            var baseUrl = "https://api.redmilbusinessmall.com";
            GetCashoutAcutalAmountCreditRequestModel requestModel = new GetCashoutAcutalAmountCreditRequestModel();
            try
            {
                //List<GetMultiAccountDetailResponse> Bankdetail = new List<GetMultiAccountDetailResponse>();
                requestModel.Userid = "2180";
                #region Checksum (GetUserBalanceSummaryWithPaging|Unique Key|UserId)
                //GetUserBalanceSummaryWithPaging|Unique Key|Userid|WalletType|FilterBy|PageNumber
                string input = Checksum.MakeChecksumString("AaadharPicVerification", Checksum.checksumKey,
                    requestModel.Userid);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                requestModel.checksum = CheckSum;
                #endregion
                var client = new RestClient($"{Baseurl}{ApiName.AaadharPicVerification}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
                if (deserialize.Statuscode == "TXN" || deserialize.Data == "Non Verified")
                {
                    return Json(deserialize);
                }
                else if (deserialize.Statuscode == "SKP" || deserialize.Data == "Exempted")
                {
                    try
                    {

                        //MakeCashOutNewRequestModel obj = new MakeCashOutNewRequestModel();
                        if (ModeId == "IMPS")
                        {
                            objfinal.ModeId = "1";
                        }
                        else if (ModeId == "NEFT")
                        {
                            objfinal.ModeId = "2";
                        }
                        else if (ModeId == "RTGS")
                        {
                            objfinal.ModeId = "3";
                        }
                        objfinal.UserId = "2180";
                        objfinal.ServiceId = ServiceId;
                        objfinal.OpId = OpId;
                        objfinal.BankId = BankId;
                        objfinal.BeniName = BeniName;
                        objfinal.Account = Account;
                        objfinal.IFSC = IFSC;
                        objfinal.Amount = Amount;
                        //var client1 = new RestClient("https://api.redmilbusinessmall.com/api/MakeCashOutNew");
                        var client1 = new RestClient($"{Baseurl}{ApiName.MakeCashOutNew}");
                        var request1 = new RestRequest(Method.POST);
                        request.AddHeader("Content-Type", "application/json");
                        var json1 = JsonConvert.SerializeObject(objfinal);
                        request1.AddJsonBody(json1);
                        IRestResponse response1 = client1.Execute(request1);
                        var result1 = response1.Content;
                        var datadesi = JsonConvert.DeserializeObject<BaseResponseModelT<List<SuccessResponseModel>>>(response1.Content);
                        var deserialize1 = datadesi.Data;
                        //var deserialize1 = JsonConvert.DeserializeObject<ResponseModel1>(response1.Content);
                        //SuccessResponseModel
                        return Json(datadesi);
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                        requestModelEx.ExceptionMessage = ex;
                        requestModelEx.Data = objfinal;
                        var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                        var requestEx = new RestRequest(Method.POST);
                        requestEx.AddHeader("Content-Type", "application/json");
                        var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                        requestEx.AddJsonBody(jsonEx);
                        IRestResponse responseEx = clientEx.Execute(requestEx);
                        var resultEx = responseEx.Content;
                    }
                }

                return Json(deserialize);

            }
            catch(Exception ex)
            {
                ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                requestModelEx.ExceptionMessage = ex;
                requestModelEx.Data = requestModel;
                var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                requestEx.AddJsonBody(jsonEx);
                IRestResponse responseEx = clientEx.Execute(requestEx);
                var resultEx = responseEx.Content;
            }
            return Json("");

        }
        public JsonResult FaceLiveLiness(string showimage)
        {

            FaceLivelinessRequestModel obj = new FaceLivelinessRequestModel();
            try
            {
                //List<GetMultiAccountDetailResponse> Bankdetail = new List<GetMultiAccountDetailResponse>();
                obj.Userid = "2180";
                obj.FileName = showimage;
                var client = new RestClient($"{Baseurl}{ApiName.FaceLiveliNess}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(obj);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
                if (deserialize.Statuscode == "TXN")
                {
                    try
                    {

                        //MakeCashOutNewRequestModel obj = new MakeCashOutNewRequestModel();
                        if (ModeId == "IMPS")
                        {
                            objfinal.ModeId = "1";
                        }
                        else if (ModeId == "NEFT")
                        {
                            objfinal.ModeId = "2";
                        }
                        else if (ModeId == "RTGS")
                        {
                            objfinal.ModeId = "3";
                        }
                        objfinal.UserId = "2180";
                        objfinal.ServiceId = ServiceId;
                        objfinal.OpId = OpId;
                        objfinal.BankId = BankId;
                        objfinal.BeniName = BeniName;
                        objfinal.Account = Account;
                        objfinal.IFSC = IFSC;
                        objfinal.Amount = Amount;
                        //var client1 = new RestClient("https://api.redmilbusinessmall.com/api/MakeCashOutNew");
                        var client1 = new RestClient($"{Baseurl}{ApiName.MakeCashOutNew}");
                        var request1 = new RestRequest(Method.POST);
                        request.AddHeader("Content-Type", "application/json");
                        var json1 = JsonConvert.SerializeObject(objfinal);
                        request1.AddJsonBody(json1);
                        IRestResponse response1 = client1.Execute(request1);
                        var result1 = response1.Content;
                        var datadesi = JsonConvert.DeserializeObject<BaseResponseModelT<List<SuccessResponseModel>>>(response1.Content);
                        var deserialize1 = datadesi.Data;
                        //var deserialize1 = JsonConvert.DeserializeObject<ResponseModel1>(response1.Content);
                        //SuccessResponseModel
                        return Json(deserialize1);
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                        requestModelEx.ExceptionMessage = ex;
                        requestModelEx.Data = objfinal;
                        var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                        var requestEx = new RestRequest(Method.POST);
                        requestEx.AddHeader("Content-Type", "application/json");
                        var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                        requestEx.AddJsonBody(jsonEx);
                        IRestResponse responseEx = clientEx.Execute(requestEx);
                        var resultEx = responseEx.Content;
                    }
                }

                else if(deserialize.Statuscode=="ERR")
                {
                    return Json(deserialize);

                }
                else
                {
                    return Json("");
                }

                return Json(deserialize);
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                requestModelEx.ExceptionMessage = ex;
                requestModelEx.Data = obj;
                var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                requestEx.AddJsonBody(jsonEx);
                IRestResponse responseEx = clientEx.Execute(requestEx);
                var resultEx = responseEx.Content;
            }
            return Json("");

        }
        // public JsonResult AccountDetail()
        // {
        //     //FaceLivelinessRequestModel requestModel = new FaceLivelinessRequestModel();
        //     //requestModel.Userid = "Na";
        //     //requestModel.Mobile = "8802470198";

        //     //#region Checksum (GetUserBalanceSummaryWithPaging|Unique Key|UserId)
        //     ////GetUserBalanceSummaryWithPaging|Unique Key|Userid|WalletType|FilterBy|PageNumber
        //     //string input = Checksum.MakeChecksumString("Getbank", Checksum.checksumKey,
        //     //    requestModel.Userid, requestModel.Mobile);
        //     //string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
        //     //requestModel.checksum = CheckSum;
        //     //#endregion
        //     //var client = new RestClient($"{Baseurl}{ApiName.Getbank}");
        //     //var request = new RestRequest(Method.POST);
        //     //request.AddHeader("Content-Type", "application/json");
        //     //var json = JsonConvert.SerializeObject(requestModel);
        //     //request.AddJsonBody(json);
        //     //IRestResponse response = client.Execute(request);
        //     //var result = response.Content;
        //     ViewBag.BankAccount = new SelectList(BankDetail(), "Id", "BankName");
        //     return Json();

        //}
        public List<GetBankResponseModel> BankDetail()
        {


            List<GetBankResponseModel> lstresponse = new List<GetBankResponseModel>();
            //var Data = new List<TranferToBankroller>();
            FaceLivelinessRequestModel requestModel = new FaceLivelinessRequestModel();
            try
            {
                requestModel.Userid = "Na";
                requestModel.Mobile = "8802470198";
                #region Checksum (Getbank|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("Getbank", Checksum.checksumKey,
                    requestModel.Userid, requestModel.Mobile);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                requestModel.checksum = CheckSum;
                #endregion
                var client = new RestClient($"{Baseurl}{ApiName.Getbank}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var Bankdetail = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                if(Bankdetail.Statuscode=="TXN" && Bankdetail != null)
                {
                    var data = Bankdetail.Data;
                    var datalist = JsonConvert.DeserializeObject<List<GetBankResponseModel>>(JsonConvert.SerializeObject(data));
                    lstresponse = datalist.ToList();
                    return lstresponse;
                }
                else if (Bankdetail.Statuscode == "ERR")
                {

                }
                else{

                }
                
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                requestModelEx.ExceptionMessage = ex;
                requestModelEx.Data = requestModel;
                var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                requestEx.AddJsonBody(jsonEx);
                IRestResponse responseEx = clientEx.Execute(requestEx);
                var resultEx = responseEx.Content;
            }
            return lstresponse;


        }


        public JsonResult VerifyAccountDetail(string Account, string ConfirmAccount, string Ifsc, string BeneficaryName)
        {
            VerificationwithchargeRequestModelcs requestModel = new VerificationwithchargeRequestModelcs();
            try
            {
                requestModel.Userid = "2180";
                requestModel.Mobileno = HttpContext.Session.GetString("Mobile").ToString();
                requestModel.Account = Account;
                requestModel.Ifsc = Ifsc;
                requestModel.BeniName = BeneficaryName;
                requestModel.Mode = "Web";
                #region Checksum (GetUserBalanceSummaryWithPaging|Unique Key|UserId)
                //GetUserBalanceSummaryWithPaging|Unique Key|Userid|WalletType|FilterBy|PageNumber
                string input = Checksum.MakeChecksumString("AccountVerificationForSignupwithCharge", Checksum.checksumKey, requestModel.Userid, requestModel.Mobileno, requestModel.Account, requestModel.Ifsc, requestModel.Mode, requestModel.BeniName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                requestModel.checksum = CheckSum;
                #endregion
                var client = new RestClient("https://proapitest5.redmilbusinessmall.com/api/AccountVerificationForSignupwithCharge");
                //var client = new RestClient($"{Baseurl}{ApiName.AccountVerificationForSignupwithCharge}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<BaseResponseModelT<List<AccountSignWithChargeResponseModel>>>(response.Content);
                if(deserialize.Statuscode=="TXN" && deserialize != null)
                {
                    var datadeserialize = deserialize.Data;
                    //var data = JsonConvert.DeserializeObject<List<AccountSignWithChargeResponseModel>>(JsonConvert.SerializeObject(datadeserialize));
                    if (deserialize.Statuscode == "ERR" || deserialize.Statuscode == "TXN")
                    {
                        return Json(deserialize);
                    }
                    //else if (deserialize.Statuscode == "TXN")
                    //{
                    //    return Json(deserialize);
                    //}
                    //else if (deserialize.Statuscode == "TXN" && requestModel.BeniName == HttpContext.Session.GetString("Name"))
                    //{
                    //    ValidateOTPForMultiAccountRequestModel obj = new ValidateOTPForMultiAccountRequestModel();
                    //    obj.Userid = "2180";
                    //    obj.Mobile = HttpContext.Session.GetString("Mobile").ToString();
                    //    string input1 = Checksum.MakeChecksumString("SendOTPForMultiAccount", Checksum.checksumKey, obj.Userid, obj.Mobile);
                    //    string CheckSum1 = Checksum.ConvertStringToSCH512Hash(input1);
                    //    obj.checksum = CheckSum1;
                    //    var client1 = new RestClient("https://proapitest2.redmilbusinessmall.com/api/SendOTPForMultiAccount");
                    //    //var client = new RestClient($"{Baseurl}{ApiName.AccountVerificationForSignupwithCharge}");
                    //    var request1 = new RestRequest(Method.POST);
                    //    request1.AddHeader("Content-Type", "application/json");
                    //    var json1 = JsonConvert.SerializeObject(obj);
                    //    request1.AddJsonBody(json1);
                    //    IRestResponse response1 = client1.Execute(request1);
                    //    var result1 = response1.Content;
                    //    var deserialize1 = JsonConvert.DeserializeObject<ResponseModel1>(response1.Content);
                    //    //if (deserialize1.Statuscode == "OSS" && deserialize1.Message=="Otp Sent Successfully")
                    //    //{

                    //    return Json(deserialize1);

                    //    // }

            //}
            else
            {
                return Json(deserialize);
                //return Json(new { data.FirstOrDefault().Surcharge, ERRS = "ERRS" });
            }


        }
        public JsonResult SendOtp()
        {
            ValidateOTPForMultiAccountRequestModel obj = new ValidateOTPForMultiAccountRequestModel();
            try
            {
                obj.Userid = "2180";
                obj.Mobile = HttpContext.Session.GetString("Mobile").ToString();
                string input1 = Checksum.MakeChecksumString("SendOTPForMultiAccount", Checksum.checksumKey, obj.Userid, obj.Mobile);
                string CheckSum1 = Checksum.ConvertStringToSCH512Hash(input1);
                obj.checksum = CheckSum1;
                var client1 = new RestClient("https://proapitest5.redmilbusinessmall.com/api/SendOTPForMultiAccount");
                //var client = new RestClient($"{Baseurl}{ApiName.AccountVerificationForSignupwithCharge}");
                var request1 = new RestRequest(Method.POST);
                request1.AddHeader("Content-Type", "application/json");
                var json1 = JsonConvert.SerializeObject(obj);
                request1.AddJsonBody(json1);
                IRestResponse response1 = client1.Execute(request1);
                var result1 = response1.Content;
                var deserialize1 = JsonConvert.DeserializeObject<ResponseModel1>(response1.Content);
                if (deserialize1.Statuscode == "TXN" && deserialize1.Data == "Otp Validated Successfully")
                {

                    return Json(deserialize1);
                }
                else if(deserialize1.Statuscode=="ERR")
                {

                return Json(deserialize1);
            }

        }
        public JsonResult ConfirmValidateOtpForUserAccount(string Otp)
        {

            ValidateOtpForMultiAccountConfirmRequestModel objvalidate = new ValidateOtpForMultiAccountConfirmRequestModel();
            try
            {
                objvalidate.Mobile = HttpContext.Session.GetString("Mobile").ToString();
                objvalidate.Otp = Otp;
                objvalidate.Userid = "2180";
                string inputvalidate = Checksum.MakeChecksumString("ValidateOTPForMultiAccount", Checksum.checksumKey, objvalidate.Userid, objvalidate.Mobile, objvalidate.Otp);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(inputvalidate);
                objvalidate.checksum = CheckSum;
                var client = new RestClient("https://proapitest5.redmilbusinessmall.com/api/ValidateOTPForMultiAccount");
                //var client = new RestClient($"{Baseurl}{ApiName.AccountVerificationForSignupwithCharge}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(objvalidate);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
                if(deserialize.Statuscode=="TXN" && deserialize != null)
                {
                    return Json(deserialize);
                }
                else if (deserialize.Statuscode == "ERR")
                {
                    return Json(deserialize);
                }
                else
                {
                    return Json("");
                }
                

            }

            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                requestModelEx.ExceptionMessage = ex;
                requestModelEx.Data = objvalidate;
                var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                requestEx.AddJsonBody(jsonEx);
                IRestResponse responseEx = clientEx.Execute(requestEx);
                var resultEx = responseEx.Content;
            }
            return Json("");

        }

        [HttpPost]

        public IActionResult uploadimages(InserMutltiAccount fileData, string Account, string BeneficaryName, string IFSc, string BankList, string Relation)
        {


            string serverFolder = "";
            IFormFile iform = fileData.file;
            if (fileData.file != null)
            {
                string folder = "Uploadimages/Doc/";
                //folder += Guid.NewGuid().ToString() + "_" + files.file.FileName;
                folder += Path.GetFileName(iform.FileName);
                //files.file.FileName
                serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                // await files.file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                using (FileStream stream = new FileStream(serverFolder, FileMode.Create))
                {
                    iform.CopyTo(stream);
                }
            }
            //InserMutltiAccount obj = new InserMutltiAccount();
            var client = new RestClient($"{Baseurl}{ApiName.UploadKhataImages}");
            var requestN = new RestRequest(Method.POST);
            requestN.AddHeader("cache-control", "no-cache");
            requestN.AddFile("doc", serverFolder);
            requestN.AlwaysMultipartFormData = true;
            //var json = JsonConvert.SerializeObject(obj);
            //request.AddJsonBody(json);
            IRestResponse response = client.Execute(requestN);
            var result = response.Content;
            var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
            if(deserialize.Statuscode=="TXN" && deserialize != null)
            {
                try
                {
                    objMulti.BankId = BankList;
                    objMulti.RelationName = Relation;
                    objMulti.Userid = "2180";
                    objMulti.AccountNo = Account;
                    objMulti.IFSC = IFSc;
                    objMulti.BeniName = BeneficaryName;
                    objMulti.PanStatus = "Not Verified";
                    objMulti.UserNameInBank = BeneficaryName;
                    //objMulti.PanStatus = "Not Verified";
                    objMulti.RelationPanImagePath = deserialize.Data.ToString();
                    #region Checksum (InsertMultiAccountDetailsForUsers|Unique Key|UserId)
                    //GetUserBalanceSummaryWithPaging|Unique Key|Userid|WalletType|FilterBy|PageNumber
                    string input = Checksum.MakeChecksumString("InsertMultiAccountDetailsForUsers", Checksum.checksumKey, objMulti.Userid, objMulti.BankId, objMulti.AccountNo);
                    string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                    objMulti.checksum = CheckSum;
                    #endregion
                    var client1 = new RestClient("https://proapitest5.redmilbusinessmall.com/api/InsertMultiAccountDetailsForUsers");
                    //var client = new RestClient($"{Baseurl}{ApiName.AccountVerificationForSignupwithCharge}");
                    var request1 = new RestRequest(Method.POST);
                    request1.AddHeader("Content-Type", "application/json");
                    var json1 = JsonConvert.SerializeObject(objMulti);
                    request1.AddJsonBody(json1);
                    IRestResponse response1 = client1.Execute(request1);
                    var result1 = response1.Content;
                    var deserialize1 = JsonConvert.DeserializeObject<ResponseModel1>(response1.Content);
                    if (deserialize1.Statuscode == "TXN" && deserialize1 != null)
                    {
                        //ViewBag.Messageforadd = deserialize1.Message;
                        //ViewBag.SuccessMessage = deserialize1.Message;
                        //Response.Write("<script>alert('Data inserted successfully')</script>");
                        return Json(deserialize1);
                    }
                    else if (deserialize1.Statuscode == "ERR")
                    {
                        return Json(deserialize1);
                    }
                    else
                    {
                        return Json("");
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                    requestModelEx.ExceptionMessage = ex;
                    requestModelEx.Data = objMulti;
                    var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                    var requestEx = new RestRequest(Method.POST);
                    requestEx.AddHeader("Content-Type", "application/json");
                    var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                    requestEx.AddJsonBody(jsonEx);
                    IRestResponse responseEx = clientEx.Execute(requestEx);
                    var resultEx = responseEx.Content;
                }
            }
            else if(deserialize.Statuscode=="ERR")
            {
                return Json(deserialize);
            }
            else
            {
                return Json("");
            }
            
            return Json("");


        }




        public IActionResult ForSuccesPop()
        {
            string msg = ViewBag.SuccessMessage;
            ViewBag.Sudsdsd = msg;
            return View();
        }



        [HttpPost]
        public JsonResult AddAccount(string BankList, string Account, string IFSC, string BeneficaryName)
        {
            //InserMutliAccountNew obj = new InserMutliAccountNew();
            //obj.Userid = "2180";
            //obj.BankId = BankList;
            //obj.AccountNo = Account;
            //obj.IFSC = IFSC;
            //obj.BeniName = BeneficaryName;

            objMulti.BankId = BankList;
            objMulti.RelationName = "";
            objMulti.Userid = "2180";
            objMulti.AccountNo = Account;
            objMulti.IFSC = IFSC;
            objMulti.BeniName = BeneficaryName;
            objMulti.PanStatus = "";
            objMulti.UserNameInBank = BeneficaryName;
            objMulti.RelationPanImagePath = "";
            #region Checksum (InsertMultiAccountDetailsForUsers|Unique Key|UserId)
            //GetUserBalanceSummaryWithPaging|Unique Key|Userid|WalletType|FilterBy|PageNumber
            string input = Checksum.MakeChecksumString("InsertMultiAccountDetailsForUsers", Checksum.checksumKey, objMulti.Userid, objMulti.BankId, objMulti.AccountNo);
            string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
            objMulti.checksum = CheckSum;
            #endregion
            var client = new RestClient("https://proapitest5.redmilbusinessmall.com/api/InsertMultiAccountDetailsForUsers");
            //var client = new RestClient($"{Baseurl}{ApiName.AccountVerificationForSignupwithCharge}");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var json = JsonConvert.SerializeObject(objMulti);
            request.AddJsonBody(json);
            IRestResponse response = client.Execute(request);
            var result = response.Content;
            var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
            return Json(deserialize);
        }

        public JsonResult DetailDeposite()
        {
            var client = new RestClient($"{Baseurl}{ApiName.GetCashdepositeMode}");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            //var json = JsonConvert.SerializeObject(obj);
            //request.AddJsonBody(json);
            IRestResponse response = client.Execute(request);
            var result = response.Content;
            var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
            var datadeserialize = deserialize.Data;
            var data = JsonConvert.DeserializeObject<List<GetcashdepositeResponseModel>>(JsonConvert.SerializeObject(datadeserialize));
            //var datadeserialize = deserialize.Data;
            //var TranferData = JsonConvert.DeserializeObject<GetcashdepositeResponseModel>(JsonConvert.SerializeObject(datadeserialize));
            return Json(data);


        }
        public JsonResult FinalMakeCashDeposite(MakeCashoutDespoisteRequestModel File, string Tid, string Dataing,String Timing, string Remarks,string AmountT, string Mode)
        {

            string serverFolder = "";
            IFormFile iform = File.Docss;
            if (File.Docss != null)
            {
                string folder = "Uploadimages/Doc/";
                //folder += Guid.NewGuid().ToString() + "_" + files.file.FileName;
                folder += Path.GetFileName(iform.FileName);
                //files.file.FileName
                serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                // await files.file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                using (FileStream stream = new FileStream(serverFolder, FileMode.Create))
                {
                    iform.CopyTo(stream);
                }
            }
            //InserMutltiAccount obj = new InserMutltiAccount();
            var client = new RestClient($"{Baseurl}{ApiName.File}");
            var requestN = new RestRequest(Method.POST);
            requestN.AddHeader("cache-control", "no-cache");
            requestN.AddFile("doc", serverFolder);
            requestN.AlwaysMultipartFormData = true;
            //var json = JsonConvert.SerializeObject(obj);
            //request.AddJsonBody(json);
            IRestResponse response = client.Execute(requestN);
            var result = response.Content;
            var deserialize1 = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
            MakeCashoutDespoisteRequestModel obj = new MakeCashoutDespoisteRequestModel();
            obj.TransactionId = Tid;
            obj.TransactionDate = Dataing;
            obj.Docs= deserialize1.Data.ToString();
            obj.Remarks = Remarks;
            obj.ChargedAmount = "0.00";
            obj.ChargeType = "true";
            obj.Charge = "0.00";
            obj.Amount = AmountT;
            obj.ModeId = Mode;
            obj.Userid = "2084";
            #region Checksum (MakeCashDeposite|Unique Key|UserId)
            //GetUserBalanceSummaryWithPaging|Unique Key|Userid|WalletType|FilterBy|PageNumber
            string input = Checksum.MakeChecksumString("MakeCashDeposite", Checksum.checksumKey, obj.Userid, obj.ModeId, obj.Amount, obj.Charge, obj.ChargeType,obj.ChargedAmount, obj.Remarks,obj.Docs,obj.TransactionDate,obj.TransactionId);
            string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
            obj.checksum = CheckSum;
            #endregion
            //var client = new RestClient($"{Baseurl}{ApiName.GetCashdepositeMode}");
            var client1 = new RestClient("https://proapitest1.redmilbusinessmall.com/api/MakeCashDeposite");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var json = JsonConvert.SerializeObject(obj);
            request.AddJsonBody(json);
            IRestResponse response1 = client1.Execute(request);
            var result1 = response1.Content;
            var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response1.Content);
            
            
            return Json(deserialize);


        }

    }
}
