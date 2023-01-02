﻿using Microsoft.AspNetCore.Mvc;
using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Models;
using Project_Redmil_MVC.Models.RequestModel;
using Project_Redmil_MVC.Models.RequestModel.ElectricityBillRequestModel;
using Project_Redmil_MVC.Models.RequestModel.HospitalBillRequestModel;
using Project_Redmil_MVC.Models.ResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.ElectricityBillResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.HospitalBillResponseModel;
using System.Net.NetworkInformation;

namespace Project_Redmil_MVC.Controllers.BillPayments.HospitalBillController
{
    public class HospitalBillController : Controller
    {
        private readonly string Baseurl;
        private readonly IConfiguration _config;
        public HospitalBillOperatorListResponseModel lstOperator;

        public HospitalBillController(IConfiguration config)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
        }
        public IActionResult HospitalBill()
        {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {

                return RedirectToAction("ErrorForLogin", "Error");

            }
            ViewBag.OpName = new SelectList(GetOperatorList(), "Id", "Operatorname");
            return View();
        }

        #region Fetch And Pay Bill
        public JsonResult FetchBill(string Number, string Input1, string Input2, string Operator, string ccf, string Amount, string Payment)
        {
            var baseUrl = "https://api.redmilbusinessmall.com";
            HospitalBillOperatorListRequestModel requestModel = new HospitalBillOperatorListRequestModel();
            try
            {
                requestModel.Userid = "NA";
                requestModel.State = "";
                requestModel.Category = "Hospital";
                #region Checksum (BbpsBillerByState|Unique Key|UserId|ServiceId)

                string input = Checksum.MakeChecksumString("BbpsBillerByState", Checksum.checksumKey, requestModel.Userid, requestModel.State, requestModel.Category);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);

                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/BbpsBillerByState");
                var client = new RestClient($"{Baseurl}{ApiName.BbpsBillerByState}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                var datadeserialize = deserialize.Data;
                lstOperator = JsonConvert.DeserializeObject<HospitalBillOperatorListResponseModel>(datadeserialize.ToString());
                if (!string.IsNullOrEmpty(Number))
                {
                    var dataBillerInfo = lstOperator.billerInfo.Where(x => x.Id == Operator);
                    var billerAdhoc = lstOperator.billerInfo.FirstOrDefault().inputParam.FirstOrDefault().Optional;

                    #region Fetching Input Param Value Statically

                    //Static Set Input Param 1 value and Input Param 2 value

                    var Name = dataBillerInfo.FirstOrDefault().inputParam.FirstOrDefault().Name;
                    #endregion
                    List<GetBBPSBillsTmpResponseModel> lstResponse = new List<GetBBPSBillsTmpResponseModel>();
                    GetBBPSBillsTmpRequestModel requestmodel = new GetBBPSBillsTmpRequestModel();
                    try
                    {
                        requestmodel.Userid = "2084";
                        requestmodel.Mobileno = Number;
                        requestmodel.BillerId = dataBillerInfo.FirstOrDefault().Bbps;
                        string inputParamKey = "";
                        int count1 = 1;
                        if (dataBillerInfo.FirstOrDefault().inputParam.Length > 1)
                        {
                            for (int i = 0; i < dataBillerInfo.FirstOrDefault().inputParam.Length; i++)
                            {

                                if (count1 <= dataBillerInfo.FirstOrDefault().inputParam.Length - 1)
                                {
                                    inputParamKey += dataBillerInfo.FirstOrDefault().inputParam[i].Name + "^";
                                }
                                else
                                {
                                    inputParamKey += dataBillerInfo.FirstOrDefault().inputParam[i].Name;
                                }
                                count1++;
                            }
                        }
                        else
                        {
                            inputParamKey = dataBillerInfo.FirstOrDefault().inputParam.FirstOrDefault().Name;
                        }
                        requestmodel.InputParam1 = inputParamKey;
                        if (!string.IsNullOrEmpty(Input1))
                        {
                            string inputpar = "";
                            inputpar = Input1;
                            if (!string.IsNullOrEmpty(Input2))
                            {
                                inputpar += "^" + Input2;
                            }
                            requestmodel.InputParam2 = inputpar;
                        }

                        requestmodel.Ip_address = "192.168.1.1";
                        requestmodel.ccf = ccf;
                        requestmodel.Token = "";
                        #region Checksum (GetBBPSBillsTmp|Unique Key|UserId)
                        string inputN = Checksum.MakeChecksumString("GetBBPSBillsTmp", Checksum.checksumKey, requestmodel.Userid, requestmodel.Mobileno.Trim(), requestmodel.BillerId.Trim(), requestmodel.InputParam1.Trim(),
                            requestmodel.InputParam2.Trim(), requestmodel.Ip_address.Trim(), requestmodel.ccf);
                        string CheckSumN = Checksum.ConvertStringToSCH512Hash(inputN);
                        #endregion
                        requestmodel.checksum = CheckSumN;
                        var clientN = new RestClient("https://proapitest4.redmilbusinessmall.com/api/GetBBPSBillsTmp");//
                                                                                                                       //var client = new RestClient($"{Baseurl}{ApiName.BbpsBillerByState}");
                        var requestN = new RestRequest(Method.POST);
                        requestN.AddHeader("Content-Type", "application/json");
                        var jsonN = JsonConvert.SerializeObject(requestmodel);
                        requestN.AddJsonBody(jsonN);
                        IRestResponse responseN = clientN.Execute(requestN);
                        var resultN = responseN.Content;
                        var deserializeN = JsonConvert.DeserializeObject<BaseBillResponseModel>(responseN.Content);
                        if (deserializeN.Statuscode == "TXN")
                        {
                            var data = deserializeN.Data;
                            var data22 = deserializeN.AdditionalInfo;

                            GetBBPSBillsTmpResponseModel.Data getBBPSBillsTmpResponseModel = JsonConvert.DeserializeObject<GetBBPSBillsTmpResponseModel.Data>(data.ToString());
                            var billNumber = Input1;
                            GetBBPSBillsTmpResponseModel.AdditionalInfo datalist1 = JsonConvert.DeserializeObject<GetBBPSBillsTmpResponseModel.AdditionalInfo>(data22.ToString());
                            //Final Bill Payment
                            if (!string.IsNullOrEmpty(Amount) && (!string.IsNullOrEmpty(ccf)) && (!string.IsNullOrEmpty(Payment)))
                            {
                                PayBBPSBillsTmpRequestModel requestPayModel = new PayBBPSBillsTmpRequestModel();
                                try
                                {
                                    requestPayModel.RequestId = getBBPSBillsTmpResponseModel.ReqestNo;
                                    requestPayModel.Mobileno = Number;
                                    requestPayModel.BillerId = lstOperator.billerInfo.Where(x => x.Id == Operator).FirstOrDefault().Bbps;
                                    requestPayModel.Biller = getBBPSBillsTmpResponseModel.billerResponseEnc;
                                    requestPayModel.AdditionalInfo = getBBPSBillsTmpResponseModel.additionalInfoEnc;
                                    requestPayModel.Input = getBBPSBillsTmpResponseModel.inputParamsEnc;
                                    requestPayModel.IpAddress = GetIp();
                                    requestPayModel.MacAddress = GetMacAddress(requestPayModel.IpAddress);
                                    requestPayModel.ccf = ccf;
                                    //requestPayModel.billerAdhoc = getElectricityOperatorListResponseModel.billerInfo.Where(x => x.Id == Operator).FirstOrDefault().BillerAdhoc;
                                    requestPayModel.billerAdhoc = billerAdhoc;

                                    string inputParamKey1 = "";
                                    int count1N = 1;
                                    if (dataBillerInfo.FirstOrDefault().inputParam.Length > 1)
                                    {
                                        for (int i = 0; i < dataBillerInfo.FirstOrDefault().inputParam.Length; i++)
                                        {

                                            if (count1 <= dataBillerInfo.FirstOrDefault().inputParam.Length - 1)
                                            {
                                                inputParamKey1 += dataBillerInfo.FirstOrDefault().inputParam[i].Name + "^";
                                            }
                                            else
                                            {
                                                inputParamKey1 += dataBillerInfo.FirstOrDefault().inputParam[i].Name;
                                            }
                                            count1N++;
                                        }
                                    }
                                    else
                                    {
                                        inputParamKey = dataBillerInfo.FirstOrDefault().inputParam.FirstOrDefault().Name;
                                    }
                                    requestPayModel.InputParam1 = inputParamKey;
                                    if (!string.IsNullOrEmpty(Input1))
                                    {
                                        string inputpar = "";
                                        inputpar = Input1;
                                        if (!string.IsNullOrEmpty(Input2))
                                        {
                                            inputpar += "^" + Input2;
                                        }
                                        requestPayModel.InputParam2 = inputpar;
                                    }
                                    requestPayModel.type = "Pay";
                                    //requestPayModel.billValidationStatus = getElectricityOperatorListResponseModel.billerInfo.FirstOrDefault().BillValidation;
                                    requestPayModel.billValidationStatus = lstOperator.billerInfo.Where(x => x.Id == Operator).FirstOrDefault().BillValidation;
                                    requestPayModel.Wallet = Payment;
                                    string amount = Amount;
                                    string FinalAmount = string.Empty;
                                    if (amount.Contains('₹'))
                                    {
                                        string[] arrAmount = amount.Split('₹');
                                        FinalAmount = arrAmount[1].Trim();
                                    }
                                    else
                                    {
                                        FinalAmount = amount;
                                    }
                                    requestPayModel.Amount = FinalAmount;
                                    requestPayModel.Mode = "App";
                                    requestPayModel.Userid = "2084";
                                    string UseridCheck = "2084";
                                    //requestPayModel.Token = "";

                                    #region Checksum (PayBBPSBillsTmp|Unique Key|UseridCheck|Mobileno|Mode|Amount|RequestID|BillerId|InputParam1|InputParam2)
                                    string inputN1 = Checksum.MakeChecksumString("PayBBPSBillsTmp", Checksum.checksumKey, requestPayModel.Userid,
                                        requestPayModel.Mobileno, requestPayModel.Mode, requestPayModel.Amount, requestPayModel.RequestId, requestPayModel.BillerId,
                                        requestPayModel.InputParam1, requestPayModel.InputParam2);

                                    string CheckSumN1 = Checksum.ConvertStringToSCH512Hash(inputN1);
                                    #endregion
                                    requestPayModel.checksum = CheckSumN1;

                                    var clientN1 = new RestClient("https://proapitest5.redmilbusinessmall.com/api/PayBBPSBillsTmp");
                                    // var clientN = new RestClient($"{Baseurl}{ApiName.PayBill}");
                                    var requestN1 = new RestRequest(Method.POST);
                                    requestN1.AddHeader("Content-Type", "application/json");
                                    var jsonN1 = JsonConvert.SerializeObject(requestPayModel);
                                    requestN1.AddJsonBody(jsonN1);
                                    IRestResponse responseN1 = clientN1.Execute(requestN1);
                                    var resultN1 = responseN1.Content;
                                    var deserializeN1 = JsonConvert.DeserializeObject<BaseBillResponseModel>(responseN1.Content);
                                    var dataFinal = deserializeN1.Data;

                                    {
                                        var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<GetElectricityFinalResponseModel>>>(responseN1.Content);

                                        return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message, Data = deserializ.Data.FirstOrDefault() });
                                    }

                                }
                                catch (Exception ex)
                                {
                                    ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                                    requestModelEx.ExceptionMessage = ex;
                                    requestModelEx.Data = requestPayModel;
                                    var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                                    var requestEx = new RestRequest(Method.POST);
                                    requestEx.AddHeader("Content-Type", "application/json");
                                    var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                                    requestEx.AddJsonBody(jsonEx);
                                    IRestResponse responseEx = clientEx.Execute(requestEx);
                                    var resultEx = responseEx.Content;
                                }
                                return Json(deserialize);
                            }
                            else
                            {
                                return Json(new
                                {
                                    data = getBBPSBillsTmpResponseModel,
                                    additionalInfo = datalist1
                                });
                            }
                        }
                        else if (deserializeN.Statuscode == "ERR")
                        {
                            return Json(deserializeN);
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                        requestModelEx.ExceptionMessage = ex;
                        requestModelEx.Data = requestmodel;
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
                return Json(lstOperator);
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

        #endregion


        #region HospitalBillOperatorList
        public List<Operatornames> GetOperatorList()
        {
            var baseUrl = "https://api.redmilbusinessmall.com";
            HospitalBillOperatorListRequestModel requestModel = new HospitalBillOperatorListRequestModel();
            List<Operatornames> OpList = new List<Operatornames>();
            try
            {
                requestModel.Userid = "NA";
                requestModel.State = "";
                requestModel.Category = "Hospital";
                #region Checksum (BbpsBillerByState|Unique Key|UserId|ServiceId)

                string input = Checksum.MakeChecksumString("BbpsBillerByState", Checksum.checksumKey, requestModel.Userid, requestModel.State, requestModel.Category);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);

                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/BbpsBillerByState");
                var client = new RestClient($"{Baseurl}{ApiName.BbpsBillerByState}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                var datadeserialize = deserialize.Data;
                var deserializeN = JsonConvert.DeserializeObject<HospitalBillOperatorListResponseModel>(datadeserialize.ToString());

                var arr = deserializeN.billerInfo;


                if (arr != null)
                {
                    foreach (var item in arr)
                    {
                        OpList.Add(new Operatornames
                        {
                            Id = item.Id,
                            Operatorname = item.Operatorname

                        });
                    }
                }
                return OpList;
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
            return OpList;

        }

        #endregion


        #region GetCCF When User Changes amount

        public JsonResult GetCCF(string ccf, string amount)
        {
            GetCCFRequestModel requestModel = new GetCCFRequestModel();
            try
            {
                string ccF = ccf;
                string Ccf = string.Empty;
                if (ccf.Contains('₹'))
                {
                    string[] cCF = ccF.Split('₹');
                    Ccf = cCF[1].Trim();
                }
                else
                {
                    Ccf = ccf;
                }


                requestModel.ccf = Ccf;
                requestModel.amount = amount;
                var client = new RestClient($"{Baseurl}{ApiName.CCF}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<GetCCFResponseModel>(response.Content);
                return Json(deserialize);
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

        #endregion


        #region GetBalance
        [HttpPost]
        public JsonResult GetBalance()
        {
            GetBalanceRequestModel getBalanceRequestModel = new GetBalanceRequestModel();
            try
            {
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
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                requestModelEx.ExceptionMessage = ex;
                requestModelEx.Data = getBalanceRequestModel;
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

        #endregion

        #region IP Address
        public string GetIp()
        {
            var hostName = System.Net.Dns.GetHostName();
            var ips = System.Net.Dns.GetHostAddressesAsync(hostName);
            string ipAddress = ips.Result[1].ToString();
            return ipAddress;

        }
        #endregion


        #region MAC Address

        public string GetMacAddress(string ipAddress)

        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            String sMacAddress = string.Empty;

            foreach (NetworkInterface adapter in nics)

            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();

                }

            }

            return sMacAddress;

        }

        #endregion

        #region Split Value
        public string Split(string value)
        {
            string param = value;
            string rvalue = string.Empty;
            if (param.Contains('₹'))
            {
                string[] Param = param.Split('₹');
                rvalue = Param[1].Trim();
            }
            else
            {
                rvalue = value;
            }
            return rvalue;

        }
        #endregion

        #region GetOperatorById
        public JsonResult GetOperatorById(string OpId)
        {
            var baseUrl = "https://api.redmilbusinessmall.com";
            HospitalBillOperatorListRequestModel requestModel = new HospitalBillOperatorListRequestModel();
            try
            {
                requestModel.Userid = "NA";
                requestModel.State = "";
                requestModel.Category = "Hospital";
                #region Checksum (BbpsBillerByState|Unique Key|UserId|ServiceId)

                string input = Checksum.MakeChecksumString("BbpsBillerByState", Checksum.checksumKey, requestModel.Userid, requestModel.State, requestModel.Category);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);

                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/BbpsBillerByState");
                var client = new RestClient($"{Baseurl}{ApiName.BbpsBillerByState}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                var datadeserialize = deserialize.Data;
                var deserializeN = JsonConvert.DeserializeObject<HospitalBillOperatorListResponseModel>(datadeserialize.ToString());

                var arr = deserializeN.billerInfo;
                var a = arr.Where(x => x.Id == OpId);
                return Json(a);
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
        #endregion
    }
}
