using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Models;
using Project_Redmil_MVC.Models.RequestModel;
using Project_Redmil_MVC.Models.RequestModel.ElectricityBillRequestModel;
using Project_Redmil_MVC.Models.RequestModel.LoanRepaymentBillRequestModel;
using Project_Redmil_MVC.Models.ResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.ElectricityBillResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.LoanRepaymentBillResponseModel;
using System.Net.NetworkInformation;

namespace Project_Redmil_MVC.Controllers.BillPayments.LoanRepaymentBillController
{
    public class LoanRepaymentBillController : Controller
    {
        private readonly string Baseurl;
        private readonly IConfiguration _config;
        public LoanRepaymentOperatorListResponseModel lstOperator { get; set; }
        public LoanRepaymentBillController(IConfiguration config)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
        }

        public IActionResult LoanRepaymentBill()
        {

            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {

                return RedirectToAction("ErrorForLogin", "Error");

            }
            ViewBag.OpName = new SelectList(GetOperatorList(), "Id", "Operatorname");
            return View();
        }

        #region FetchBill and PayAlso

        [HttpPost]

        public JsonResult FetchBill(string Number, string Operator, string Input, string Input2, string ccf, string Amount, string Payment)
        {
            List<Operatornames> OpList = new List<Operatornames>();
            var baseUrl = "https://api.redmilbusinessmall.com";
            LoanRepaymentOperatorListRequestModel requestModel = new LoanRepaymentOperatorListRequestModel();
            try
            {
                requestModel.Userid = "NA";
                requestModel.State = "";
                requestModel.Category = "Loan Repayment";
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
                if (string.IsNullOrEmpty(result))
                {
                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                }
                else
                {
                    var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                    if (deserialize.Statuscode == "TXN" && deserialize != null)
                    {
                        var datadeserialize = deserialize.Data;
                        lstOperator = JsonConvert.DeserializeObject<LoanRepaymentOperatorListResponseModel>(datadeserialize.ToString());
                        if (!string.IsNullOrEmpty(Number))
                        {
                            var dataBillerInfo = lstOperator.billerInfo.Where(x => x.Id == Operator);
                            var billerAdhoc = lstOperator.billerInfo.FirstOrDefault().inputParam.FirstOrDefault().Optional;

                            #region Fetching Input Param Value Statically

                            //Static Set Input Param 1 value and Input Param 2 value

                            var Name = dataBillerInfo.FirstOrDefault().inputParam.FirstOrDefault().Name;
                            #endregion
                            //#region Fetching Input Param Value Dynamically
                            //Inputparam inputParam = new Inputparam()
                            //{
                            //    Name = dataBillerInfo.FirstOrDefault().inputParam.FirstOrDefault().Name,
                            //    Type = dataBillerInfo.FirstOrDefault().inputParam.FirstOrDefault().Type,
                            //    MaxLength = dataBillerInfo.FirstOrDefault().inputParam.FirstOrDefault().MaxLength,
                            //    Optional = dataBillerInfo.FirstOrDefault().inputParam.FirstOrDefault().Optional
                            //};
                            //PropertyInfo[] ClassProperties;
                            //string inputParamKey = "";
                            //string inputParamValue = "";
                            //ClassProperties = typeof(Inputparam).GetProperties();
                            //int count = 1;
                            //foreach (PropertyInfo propertyInfo in ClassProperties)
                            //{
                            //    if (count <= ClassProperties.Length - 1)
                            //    {
                            //        inputParamKey += propertyInfo.Name + "^";
                            //        inputParamValue += inputParam.GetType().GetProperty(propertyInfo.Name).GetValue(inputParam, null).ToString() + "^";
                            //    }
                            //    else
                            //    {
                            //        inputParamKey += propertyInfo.Name;
                            //        inputParamValue += inputParam.GetType().GetProperty(propertyInfo.Name).GetValue(inputParam, null).ToString();
                            //    }
                            //    count++;
                            //}

                            //var inputkey = inputParamKey;
                            //var inputparam = inputParamValue;
                            //#endregion


                            List<GetBBPSBillsTmpResponseModel> lstResponse = new List<GetBBPSBillsTmpResponseModel>();
                            GetBBPSBillsTmpRequestModel requestmodel = new GetBBPSBillsTmpRequestModel();
                            try
                            {
                                requestmodel.Userid = HttpContext.Session.GetString("Id").ToString();
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
                                if (!string.IsNullOrEmpty(Input2))
                                {
                                    requestmodel.InputParam2 = Input + "^" + Input2;
                                }
                                else
                                {
                                    requestmodel.InputParam2 = Input;
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
                                var clientN = new RestClient("https://proapitest4.redmilbusinessmall.com/api/GetBBPSBillsTmp");
                                var requestN = new RestRequest(Method.POST);
                                requestN.AddHeader("Content-Type", "application/json");
                                var jsonN = JsonConvert.SerializeObject(requestmodel);
                                requestN.AddJsonBody(jsonN);
                                IRestResponse responseN = clientN.Execute(requestN);
                                var resultN = responseN.Content;
                                if (string.IsNullOrEmpty(resultN))
                                {
                                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                                }
                                else
                                {
                                    var deserializeN = JsonConvert.DeserializeObject<BaseBillResponseModel>(responseN.Content);
                                    if (deserializeN.Statuscode == "TXN")
                                    {
                                        var data = deserializeN.Data;
                                        var data22 = deserializeN.AdditionalInfo;

                                        GetBBPSBillsTmpResponseModel.Data getBBPSBillsTmpResponseModel = JsonConvert.DeserializeObject<GetBBPSBillsTmpResponseModel.Data>(data.ToString());
                                        var billNumber = Input;
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
                                                requestPayModel.billerAdhoc = billerAdhoc;
                                                if (dataBillerInfo.FirstOrDefault().inputParam.Length > 1)
                                                {
                                                    for (int i = 0; i < dataBillerInfo.FirstOrDefault().inputParam.Length; i++)
                                                    {
                                                        var a = "^";
                                                        requestPayModel.InputParam1 += dataBillerInfo.FirstOrDefault().inputParam.FirstOrDefault().Name + a;
                                                    }
                                                }
                                                else
                                                {
                                                    requestPayModel.InputParam1 = dataBillerInfo.FirstOrDefault().inputParam.FirstOrDefault().Name;
                                                }

                                                if (!string.IsNullOrEmpty(Input2))
                                                {
                                                    requestPayModel.InputParam2 = Input + "^" + Input2;
                                                }
                                                else
                                                {
                                                    requestPayModel.InputParam2 = Input;
                                                }
                                                requestPayModel.type = "Pay";
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

                                                var amountData = CheckingBalanceClass.GetBalance(requestmodel.Userid);
                                                double newAmount = (amountData.FirstOrDefault().MainBal);
                                                if (newAmount > double.Parse(FinalAmount))
                                                {
                                                    requestPayModel.Amount = FinalAmount;
                                                }
                                                else
                                                {
                                                    return Json(new
                                                    {
                                                        status = "Insufficient Balance",
                                                    });
                                                }
                                                requestPayModel.Mode = "App";
                                                requestPayModel.Userid = HttpContext.Session.GetString("Id").ToString();
                                                string UseridCheck = HttpContext.Session.GetString("Id").ToString();

                                                #region Checksum (PayBBPSBillsTmp|Unique Key|UseridCheck|Mobileno|Mode|Amount|RequestID|BillerId|InputParam1|InputParam2)
                                                string inputN1 = Checksum.MakeChecksumString("PayBBPSBillsTmp", Checksum.checksumKey, requestPayModel.Userid,
                                                    requestPayModel.Mobileno, requestPayModel.Mode, requestPayModel.Amount, requestPayModel.RequestId, requestPayModel.BillerId,
                                                    requestPayModel.InputParam1, requestPayModel.InputParam2);

                                                string CheckSumN1 = Checksum.ConvertStringToSCH512Hash(inputN1);
                                                #endregion
                                                requestPayModel.checksum = CheckSumN1;

                                                var clientN1 = new RestClient("https://proapitest5.redmilbusinessmall.com/api/PayBBPSBillsTmp");
                                                var requestN1 = new RestRequest(Method.POST);
                                                requestN1.AddHeader("Content-Type", "application/json");
                                                var jsonN1 = JsonConvert.SerializeObject(requestPayModel);
                                                requestN1.AddJsonBody(jsonN1);
                                                IRestResponse responseN1 = clientN1.Execute(requestN1);
                                                var resultN1 = responseN1.Content;
                                                if (string.IsNullOrEmpty(resultN1))
                                                {
                                                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                                                }
                                                else
                                                {
                                                    var deserializeN1 = JsonConvert.DeserializeObject<BaseBillResponseModel>(responseN1.Content);
                                                    if(deserializeN1.Statuscode=="TXN" && deserialize != null)
                                                    {
                                                        var dataFinal = deserializeN1.Data;

                                                        {
                                                            var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<GetElectricityFinalResponseModel>>>(responseN1.Content);

                                                            return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message, Data = deserializ.Data.FirstOrDefault() });
                                                        }
                                                    }
                                                    else if (deserializeN1.Statuscode == "ERR")
                                                    {
                                                        var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<GetElectricityFinalResponseModel>>>(responseN1.Content);

                                                        return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message, Data = deserializ.Data.FirstOrDefault() });
                                                    }
                                                    else
                                                    {
                                                        return Json(new { Result = "UnExpectedStatusCode", url = Url.Action("ErrorForExceptionLog", "Error") });
                                                    }
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
                                                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
                                            }

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
                                    else
                                    {
                                        return Json(new { Result = "UnExpectedStatusCode", url = Url.Action("ErrorForExceptionLog", "Error") });
                                    }
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
                                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
                            }
                        }
                        return Json(lstOperator);
                    }
                    else if (deserialize.Statuscode == "ERR")
                    {
                        return Json(deserialize);
                    }
                    else
                    {
                        return Json(new { Result = "UnExpectedStatusCode", url = Url.Action("ErrorForExceptionLog", "Error") });
                    }
                    
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
                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
            }
        }

        #endregion

        #region LoanRepaymentBillOperatorList
        public List<Operatornames> GetOperatorList()
        {
            var baseUrl = "https://api.redmilbusinessmall.com";
            LoanRepaymentOperatorListRequestModel requestModel = new LoanRepaymentOperatorListRequestModel();
            List<Operatornames> OpList = new List<Operatornames>();
            try
            {
                requestModel.Userid = "NA";
                requestModel.State = "";
                requestModel.Category = "Loan Repayment";
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
                if (string.IsNullOrEmpty(result))
                {
                    return null;
                    
                }
                else
                {
                    var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                    if(deserialize.Statuscode=="TXN"&& deserialize != null)
                    {
                        var datadeserialize = deserialize.Data;
                        var deserializeN = JsonConvert.DeserializeObject<LoanRepaymentOperatorListResponseModel>(datadeserialize.ToString());

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
                    else if (deserialize.Statuscode == "ERR")
                    {
                        return OpList;  
                    }
                    else
                    {
                        return null;
                    }
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
                if (string.IsNullOrEmpty(result))
                {
                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                }
                else
                {
                    var deserialize = JsonConvert.DeserializeObject<GetCCFResponseModel>(response.Content);
                    if(deserialize.Statuscode=="TXT" && deserialize != null)
                    {
                        return Json(deserialize);
                    }
                    else if (deserialize.Statuscode == "ERR")
                    {
                        return Json(deserialize);
                    }
                    else
                    {
                        return Json(new { Result = "UnExpectedStatusCode", url = Url.Action("ErrorForExceptionLog", "Error") });
                    }
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
                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
            }
        }

        #endregion


        #region GetBalance
        [HttpPost]
        public JsonResult GetBalance()
        {
            GetBalanceRequestModel getBalanceRequestModel = new GetBalanceRequestModel();
            List<GetBalanceResponseModel> lstdata = new List<GetBalanceResponseModel>();
            try
            {
                getBalanceRequestModel.Userid = HttpContext.Session.GetString("Id").ToString();
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
                if (string.IsNullOrEmpty(result))
                {
                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                }
                else
                {
                    var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                    if (deserialize.Statuscode == "TXN" && deserialize != null)
                    {
                        var data = deserialize.Data;
                        var datalist = JsonConvert.DeserializeObject<List<GetBalanceResponseModel>>(JsonConvert.SerializeObject(data));

                        lstdata = datalist.ToList();
                        return Json(lstdata);
                    }
                    else if (deserialize.Statuscode == "ERR")
                    {
                        return Json(deserialize);
                    }
                    else
                    {
                        return Json(new { Result = "UnExpectedStatusCode", url = Url.Action("ErrorForExceptionLog", "Error") });
                    }
                }
                
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
                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
            }
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

        public JsonResult GetOperatorById(string OpId)
        {
            List<Operatornames> OpList = new List<Operatornames>();
            var baseUrl = "https://api.redmilbusinessmall.com";
            LoanRepaymentOperatorListRequestModel requestModel = new LoanRepaymentOperatorListRequestModel();
            try
            {
                requestModel.Userid = "NA";
                requestModel.State = "";
                requestModel.Category = "Loan Repayment";
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
                if (string.IsNullOrEmpty(result))
                {
                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                }
                else
                {
                    var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                    if (deserialize.Statuscode == "TXN" && deserialize != null)
                    {
                        var datadeserialize = deserialize.Data;
                        var deserializeN = JsonConvert.DeserializeObject<LoanRepaymentOperatorListResponseModel>(datadeserialize.ToString());

                        var arr = deserializeN.billerInfo;
                        var a = arr.Where(x => x.Id == OpId);
                        return Json(a);
                    }
                    else if (deserialize.Statuscode == "ERR")
                    {
                        return Json(deserialize);
                    }
                    else
                    {
                        return Json(new { Result = "UnExpectedStatusCode", url = Url.Action("ErrorForExceptionLog", "Error") });
                    }
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
                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
            }
        }
    }
}
