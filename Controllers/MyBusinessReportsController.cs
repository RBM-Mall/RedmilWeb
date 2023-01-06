using System.Dynamic;
using Project_Redmil_MVC.Models.RequestModel;
using System.Configuration;
using System.Data;
using Project_Redmil_MVC.CommonHelper;
using System.Collections.Generic;
using Project_Redmil_MVC.Models.ResponseModel.DMT2ResponseModel;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Project_Redmil_MVC.Models.RequestModel.BusinessReportRequestModel;
using Project_Redmil_MVC.Models.ResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.BusinessReportResponseModel;
using Project_Redmil_MVC.Models;
using Project_Redmil_MVC.Helper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Composition;

namespace Project_Redmil_MVC.Controllers
{

    public class MyBusinessReportsController : Controller
    {
        private readonly string Baseurl;
        private readonly IConfiguration _config;

        public MyBusinessReportsController(IConfiguration config)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
        }

        [HttpGet]
        public IActionResult MyBusinessReports(string subCa, string from, string to, string sort, string download)
        {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {

                return RedirectToAction("ErrorForLogin", "Error");

            }
            ViewBag.Caterory = new SelectList(MyBusinessReports1(), "Id", "Name");
            GetTransactionReports(subCa, from, to, sort, download);
            return View();

        }

        #region MyBusinessReportsCategoryList
        public IEnumerable<BusinessReportModel> MyBusinessReports1()
        {
            var baseUrl = _config.GetSection("ApiUrl").GetSection("BaseUrl").Value;
            var Data1 = new List<BusinessReportModel>();
            BusinessReportModel report = new BusinessReportModel();
            try
            {
                report.baseUrl = baseUrl;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/transactions/service/category");
                var client = new RestClient($"{Baseurl}{ApiName.TransactionCategory}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(report);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
                if (deserialize.Statuscode == "TXN" && deserialize != null)
                {
                    var datadeserialize = deserialize.Data;
                    var data = JsonConvert.DeserializeObject<List<BusinessReportModel>>(JsonConvert.SerializeObject(datadeserialize));
                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            Data1.Add(new BusinessReportModel
                            {
                                Id = item.Id,
                                Name = item.Name,
                            });
                        }

                    }

                    return Data1;
                }
                else if (deserialize.Statuscode == "ERR")
                {
                    return Data1;
                }
                else
                {

                }

            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = report;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                //return (RedirectToAction("Index", new { message = "hi there!" }));
            }
            return Data1;


        }
        #endregion

        #region SubcategoryReports
        public JsonResult SubCategoryReports(int Id)
        {
            var baseUrl = _config.GetSection("ApiUrl").GetSection("BaseUrl").Value;

            var subCatData1 = new List<SubCategoryReportsModel>();

            SubCategoryReportsModel report = new SubCategoryReportsModel();
            try
            {
                report.baseUrl = baseUrl;

                //var client = new RestClient("https://api.redmilbusinessmall.com/api/transactions/service/subcategory");
                var client = new RestClient($"{Baseurl}{ApiName.TransactionSubCategory}");

                //Create request with GET
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(report);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
                if (deserialize.Statuscode == "TXN" && deserialize != null)
                {
                    var datadeserialize = deserialize.Data;
                    var data = JsonConvert.DeserializeObject<List<SubCategoryReportsModel>>(JsonConvert.SerializeObject(datadeserialize));
                    if (data != null)
                    {

                        var SubCategoryData = data.Where(x => x.CategoryId == Id);
                        foreach (var item in SubCategoryData)
                        {
                            subCatData1.Add(new SubCategoryReportsModel
                            {
                                Title = item.Title,
                                CategoryId = item.CategoryId,
                                Id = item.Id,

                            });
                        }
                    }

                    subCatData1.Insert(0, new SubCategoryReportsModel { Id = 0, Title = "-- Select SubCategories --" });
                    //DataTable dt = ToDataTable(Data1);
                    return Json(new SelectList(subCatData1, "Id", "Title"));
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
                requestModel1.Data = report;
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

        public JsonResult DDLStatus(string subCat, string fromdate, string todate, string sort)
        {
            BusinessTranscationRequestReport requestModel = new BusinessTranscationRequestReport();
            try
            {
                requestModel.UserId = "2084";


                requestModel.ServiceType = ReplaceServices(subCat);
                requestModel.FromDate = "";
                requestModel.ToDate = "";

                #region Checksum (status|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("report", Checksum.checksumKey,
            requestModel.UserId, requestModel.ServiceType, requestModel.FromDate, requestModel.ToDate);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion

                //requestModel.Checksum = "a2b9232f1290857d8aa9ebbef999e40e7fe48e5fda00666b0d0e0344fa5f459621af8ff3f873e90b3223c996c852490a523d2894975d93f07eb96bd2e62a1b4c";
                requestModel.Checksum = CheckSum;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/transactions/service/status");
                //var client = new RestClient($"{Baseurl}{ApiName.TransactionReport}");

                //JavaScriptSerializer js = new JavaScriptSerializer();

                //Create request with GET
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);

                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                var datadeserialize1 = deserialize.Statuscode;
                var datadeserialize = deserialize.Data;

                if (datadeserialize1 == "TXN")
                {
                    var Data5 = JsonConvert.DeserializeObject<List<StatusResponseModel>>(JsonConvert.SerializeObject(datadeserialize));
                    return Json(Data5);
                }
                else if (datadeserialize1 == "ERR")
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
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = requestModel;
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

        #region GetTransactionReports
        public JsonResult GetTransactionReports(string subCat, string? fromdate, string? todate, string sort, string download)
        {
            var reportDwldURL = "https://api.redmilbusinessmall.com";
            //var baseUrl = _config.GetSection("ApiUrl").GetSection("BaseUrl").Value;
            var Data1 = new List<BusinessTranscationRequestReport>();
            SubCategoryReportsModel Data2 = new SubCategoryReportsModel();

            var VirtualPaymentResponseData = new List<VirtualPaymentReportsModel>();
            var CashOutResponseData = new List<CashOutModel>();
            var UPIWalletRechargeResponseData = new List<UPIWalletRechargeModel>();

            var AePSCashWithdrawalResponseData = new List<AePSCashWithdrawalModel>();

            var AePSCashWithdrawalResponse = new AePSCashWithdrawalModel();

            var AePSMiniStatementResponseData = new List<AePSMiniStatementModel>();
            var AePSBalanceEnquiryResponseData = new List<AePSBalanceEnquiryModel>();
            var MicroATMCashWithdrawalResponseData = new List<MicroATMCashWithdrawalModel>();
            var MicroATMBalanceEnquiryResponseData = new List<MicroATMBalanceEnquiryModel>();
            var YBLDomesticMoneyTransferResponseData = new List<YBLDomesticMoneyTransferModel>();
            var CreditScoreResponseData = new List<CreditScoreModel>();
            var UPICashWithdrawalResponseData = new List<UPICashWithdrawalModel>();
            var MicroATMCashWithdrawalICICIResponseData = new List<MicroATMCashWithdrawalICICIModel>();
            var MicroATMBalanceEnquiryICICIResponseData = new List<MicroATMBalanceEnquiryICICIModel>();
            var FinoDMTModelResponseData = new List<FinoDMTModel>();

            var POSPaymentsResponseData = new List<POSPaymentsModel>();
            var SMSPaymentsResponseData = new List<SMSPaymentsModel>();

            var GoldInvestmentBuyResponseData = new List<GoldInvestmentBuyModel>();
            var GoldInvestmentSellResponseData = new List<GoldInvestmentSellModel>();

            var BillPaymentsResponseData = new List<BillPaymentsModel>();
            var DTHRechargeResponseData = new List<DTHRechargeModel>();
            var MobilePrepaidResponseData = new List<MobilePrepaidModel>();
            var MobilePostpaidResponseData = new List<MobilePostpaidModel>();
            var LICPremiumPaymentsResponseData = new List<LICPremiumPaymentsModel>();


            BusinessTranscationRequestReport requestModel = new BusinessTranscationRequestReport();
            try
            {
                requestModel.Status = "";
                requestModel.PageNumber = "1";

                requestModel.UserId = "2084";

                requestModel.SortBy = "desc";
                requestModel.ServiceType = ReplaceServices(subCat);


                requestModel.Report = "false";
                DateTime _fromdate, _todate;
                string? fromDate = fromdate;
                string? toDate = todate;

                if (fromdate != null && todate != null)
                {
                    _fromdate = DateTime.Parse(fromdate);
                    fromDate = _fromdate.ToString("dd-MMM-yyyy");

                    _todate = DateTime.Parse(toDate);
                    toDate = _todate.ToString("dd-MMM-yyyy");
                    //requestModel.Report = "true";
                    if (!string.IsNullOrEmpty(download))
                    {
                        requestModel.Report = "true";
                    }
                }

                else
                {
                    fromDate = "";
                    toDate = "";
                }

                requestModel.FromDate = fromDate;
                requestModel.ToDate = toDate;

                #region Checksum (report|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("report", Checksum.checksumKey,
            requestModel.UserId, requestModel.PageNumber, requestModel.SortBy, requestModel.ServiceType,
            requestModel.FromDate, requestModel.ToDate, requestModel.Report, requestModel.Status);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion

                CashOutModel transcationRequestReport = new CashOutModel();
                transcationRequestReport.baseUrl = Baseurl;
                requestModel.Checksum = "a2b9232f1290857d8aa9ebbef999e40e7fe48e5fda00666b0d0e0344fa5f459621af8ff3f873e90b3223c996c852490a523d2894975d93f07eb96bd2e62a1b4c";
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/transactions/report");
                var client = new RestClient($"{Baseurl}{ApiName.TransactionReport}");

                //Create request with GET
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);

                var result = response.Content;
                if (string.IsNullOrEmpty(result))
                {
                    return Json("");
                }
                else
                {
                    var deserialize = JsonConvert.DeserializeObject<BaseResponseModel2>(response.Content);
                    if (deserialize.Statuscode == "TXN" && deserialize != null)
                    {
                        var datadeserialize = deserialize.Data;
                        var datadeserialize1 = deserialize.Statuscode;
                        if (datadeserialize1 == null && subCat != "14")
                        {
                            return Json(datadeserialize1);
                        }
                        var msgdeserialize = deserialize.Message;
                        //if(subCatResponseData. == )
                        //if (!string.IsNullOrEmpty(download))
                        //{
                        //requestModel.Report = "true";
                        if (!string.IsNullOrEmpty(msgdeserialize) && !msgdeserialize.Equals("First Page"))
                        {
                            var docUrl = reportDwldURL + msgdeserialize;
                            return Json(docUrl);
                        }
                        //}

                        if (subCat == "5")
                        {
                            var Data5 = JsonConvert.DeserializeObject<List<LICPremiumPaymentsModel>>(JsonConvert.SerializeObject(datadeserialize));
                            //if (!string.IsNullOrEmpty(sort) && sort != "1")
                            //{
                            //    var a = ReplaceStatus(sort);
                            //    var Datanew = Data20.Where(x => x.Status == a);
                            //    return Json(Datanew);
                            //}
                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data5.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data5);
                                }
                            }
                            else if ((!string.IsNullOrEmpty(sort)) && sort == "1")
                            {
                                foreach (var item in Data5)
                                {
                                    LICPremiumPaymentsResponseData.Add(new LICPremiumPaymentsModel
                                    {
                                        Status = item.Status,
                                        BillReferenceId = item.BillReferenceId,
                                        BillPayAmount = item.BillPayAmount,
                                        CaNumber = item.CaNumber,
                                        WalletType = item.WalletType,

                                        RequestDate = item.RequestDate,
                                        CommissionAmount = item.CommissionAmount,
                                        RedmilTransactionId = item.RedmilTransactionId
                                    });
                                }
                                return Json(Data5);
                            }
                            return Json(Data5);

                        }

                        else if (subCat == "20")
                        {
                            var Data20 = JsonConvert.DeserializeObject<List<VirtualPaymentReportsModel>>(JsonConvert.SerializeObject(datadeserialize));
                            //if (!string.IsNullOrEmpty(sort) && sort != "1")
                            //{
                            //    var a = ReplaceStatus(sort);
                            //    var Datanew = Data20.Where(x => x.Status == a);
                            //    return Json(Datanew);
                            //}
                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data20.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data20);
                                }
                            }
                            else if ((!string.IsNullOrEmpty(sort)) && sort == "1")
                            {
                                foreach (var item in Data20)
                                {
                                    VirtualPaymentResponseData.Add(new VirtualPaymentReportsModel
                                    {
                                        Charges = item.Charges,
                                        RedmilReferenceId = item.RedmilReferenceId,
                                        VirtualAccountNumber = item.VirtualAccountNumber,
                                        RemitterAccountNumber = item.RemitterAccountNumber,
                                        NetAmount = item.NetAmount,

                                        RequestDate = item.RequestDate,
                                        IFSCCode = item.IFSCCode,
                                        Status = item.Status
                                    });
                                }
                                return Json(Data20);
                            }
                            return Json(Data20);

                        }

                        else if (subCat == "22")
                        {

                            var Data22 = JsonConvert.DeserializeObject<List<CashOutModel>>(JsonConvert.SerializeObject(datadeserialize));

                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data22.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data22);
                                }
                            }
                            else if ((!string.IsNullOrEmpty(sort)) && sort == "1")
                            {
                                foreach (var item in Data22)
                                {
                                    CashOutResponseData.Add(new CashOutModel
                                    {
                                        BeneficiaryName = item.BeneficiaryName,
                                        AccountNo = item.AccountNo,
                                        BankName = item.BankName,
                                        IFSCCode = item.IFSCCode,
                                        TransactionAmount = item.TransactionAmount,
                                        RedmilReferenceNumber = item.RedmilReferenceNumber,
                                        TransferType = item.TransferType,
                                        ActualBankCredit = item.ActualBankCredit,
                                        CurrentStatus = item.CurrentStatus,
                                        BankRRNNumber = item.BankRRNNumber,
                                        Status = item.Status

                                    });
                                }
                                return Json(Data22);
                            }
                            DataTable dt = ToDataTable(Data22);
                            return Json(Data22);

                        }

                        else if (subCat == "24")
                        {
                            var Data24 = JsonConvert.DeserializeObject<List<UPIWalletRechargeModel>>(JsonConvert.SerializeObject(datadeserialize));

                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data24.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data24);
                                }
                            }
                            else if ((!string.IsNullOrEmpty(sort)) && sort == "1")
                            {
                                foreach (var item in Data24)
                                {
                                    UPIWalletRechargeResponseData.Add(new UPIWalletRechargeModel
                                    {
                                        ActualWalletCredit = item.ActualWalletCredit,
                                        BankRRNNumber = item.BankRRNNumber,
                                        RedmilReferenceNumber = item.RedmilReferenceNumber,
                                        Charges = item.Charges,
                                        DEName = item.DEName,
                                        RequestDate = item.RequestDate,
                                        Status = item.Status
                                    });
                                }
                                return Json(Data24);
                            }
                            return Json(Data24);
                        }

                        else if (subCat == "25")
                        {
                            var Data25 = JsonConvert.DeserializeObject<List<UPICashWithdrawalModel>>(JsonConvert.SerializeObject(datadeserialize));

                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data25.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data25);
                                }
                            }
                            else if ((!string.IsNullOrEmpty(sort)) && sort == "1")
                            {
                                foreach (var item in Data25)
                                {
                                    UPICashWithdrawalResponseData.Add(new UPICashWithdrawalModel
                                    {
                                        PayerVirtualID = item.PayerVirtualID,
                                        BankRRNNumber = item.BankRRNNumber,
                                        PayerName = item.PayerName,
                                        Commission = item.Commission,
                                        CashWithdrawalAmount = item.CashWithdrawalAmount,
                                        RequestDate = item.RequestDate,
                                        Status = item.Status
                                    });
                                }
                                return Json(Data25);
                            }
                            return Json(Data25);
                        }

                        else if (subCat == "29")
                        {
                            var Data29 = JsonConvert.DeserializeObject<List<MicroATMCashWithdrawalICICIModel>>(JsonConvert.SerializeObject(datadeserialize));
                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data29.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data29);
                                }
                            }
                            else if ((!string.IsNullOrEmpty(sort)) && sort == "1")
                            {
                                foreach (var item in Data29)
                                {
                                    //MicroATMCashWithdrawalICICIResponseData.Add(new MicroATMCashWithdrawalICICIModel
                                    //{
                                    //    PayerVirtualID = item.PayerVirtualID,
                                    //    BankRRNNumber = item.BankRRNNumber,
                                    //    PayerName = item.PayerName,
                                    //    Commission = item.Commission,
                                    //    CashWithdrawalAmount = item.CashWithdrawalAmount,
                                    //    RequestDate = item.RequestDate,
                                    //    Status = item.Status
                                    //});
                                }
                                return Json(Data29);
                            }
                            return Json(Data29);
                        }

                        else if (subCat == "30")
                        {
                            var Data30 = JsonConvert.DeserializeObject<List<MicroATMBalanceEnquiryICICIModel>>(JsonConvert.SerializeObject(datadeserialize));
                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data30.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data30);
                                }
                            }
                            else if ((!string.IsNullOrEmpty(sort)) && sort == "1")
                            {
                                foreach (var item in Data30)
                                {
                                    MicroATMBalanceEnquiryICICIResponseData.Add(new MicroATMBalanceEnquiryICICIModel
                                    {
                                        //PayerVirtualID = item.PayerVirtualID,
                                        //BankRRNNumber = item.BankRRNNumber,
                                        //PayerName = item.PayerName,
                                        //Commission = item.Commission,
                                        //CashWithdrawalAmount = item.CashWithdrawalAmount,
                                        //RequestDate = item.RequestDate,
                                        //Status = item.Status
                                    });
                                }
                                return Json(Data30);
                            }
                            return Json(Data30);
                        }

                        else if (subCat == "31")
                        {
                            var Data31 = JsonConvert.DeserializeObject<List<FinoDMTModel>>(JsonConvert.SerializeObject(datadeserialize));
                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data31.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data31);
                                }
                            }
                            else if ((!string.IsNullOrEmpty(sort)) && sort == "1")
                            {
                                foreach (var item in Data31)
                                {
                                    //FinoDMTModelResponseData.Add(new FinoDMTModel
                                    //{
                                    //    PayerVirtualID = item.PayerVirtualID,
                                    //    BankRRNNumber = item.BankRRNNumber,
                                    //    PayerName = item.PayerName,
                                    //    Commission = item.Commission,
                                    //    CashWithdrawalAmount = item.CashWithdrawalAmount,
                                    //    RequestDate = item.RequestDate,
                                    //    Status = item.Status
                                    //});
                                }
                                return Json(Data31);
                            }
                            return Json(Data31);
                        }

                        else if (subCat == "15")
                        {
                            var Data15 = JsonConvert.DeserializeObject<List<AePSCashWithdrawalModel>>(JsonConvert.SerializeObject(datadeserialize));

                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data15.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data15);
                                }
                            }
                            else if ((!string.IsNullOrEmpty(sort)) && sort == "1")
                            {
                                foreach (var item in Data15)
                                {
                                    AePSCashWithdrawalResponseData.Add(new AePSCashWithdrawalModel
                                    {
                                        //CustomerName = item.CustomerName,
                                        CustomerMobileNumber = item.CustomerMobileNumber,
                                        CustomerAadhaarNo = item.CustomerAadhaarNo,
                                        TransactionAmount = item.TransactionAmount,
                                        BankName = item.BankName,
                                        IIN = item.IIN,
                                        NPCITransactionId = item.NPCITransactionId,
                                        BankRRNNumber = item.BankRRNNumber,
                                        RedmilTransactionId = item.RedmilTransactionId,
                                        BalanceAmount = item.BalanceAmount,
                                        CommissionAmount = item.CommissionAmount,
                                        FailureReason = item.FailureReason,
                                        Status = item.Status,

                                    });
                                }
                                return Json(Data15);
                            }

                            return Json(Data15);
                        }

                        else if (subCat == "16")
                        {
                            var Data16 = JsonConvert.DeserializeObject<List<AePSMiniStatementModel>>(JsonConvert.SerializeObject(datadeserialize));
                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data16.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data16);
                                }
                            }
                            else if ((!string.IsNullOrEmpty(sort)) && sort == "1")
                            {
                                foreach (var item in Data16)
                                {
                                    AePSMiniStatementResponseData.Add(new AePSMiniStatementModel
                                    {
                                        CustomerName = item.CustomerName,
                                        CustomerMobileNumber = item.CustomerMobileNumber,
                                        CustomerAadhaarNo = item.CustomerAadhaarNo,
                                        TransactionAmount = item.TransactionAmount,
                                        BankName = item.BankName,
                                        IIN = item.IIN,
                                        NPCITransactionId = item.NPCITransactionId,
                                        BankRRNNumber = item.BankRRNNumber,
                                        RedmilTransactionId = item.RedmilTransactionId,
                                        BalanceAmount = item.BalanceAmount,
                                        CommissionAmount = item.CommissionAmount,
                                        FailureReason = item.FailureReason,
                                        Status = item.Status

                                    });
                                }
                                return Json(Data16);
                            }
                            return Json(Data16);
                        }

                        else if (subCat == "17")
                        {
                            var Data17 = JsonConvert.DeserializeObject<List<AePSBalanceEnquiryModel>>(JsonConvert.SerializeObject(datadeserialize));
                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data17.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data17);
                                }
                            }
                            else if ((!string.IsNullOrEmpty(sort)) && sort == "1")
                            {
                                foreach (var item in Data17)
                                {
                                    AePSBalanceEnquiryResponseData.Add(new AePSBalanceEnquiryModel
                                    {
                                        CustomerName = item.CustomerName,
                                        CustomerMobileNumber = item.CustomerMobileNumber,
                                        CustomerAadhaarNo = item.CustomerAadhaarNo,
                                        TransactionAmount = item.TransactionAmount,
                                        BankName = item.BankName,
                                        IIN = item.IIN,
                                        NPCITransactionId = item.NPCITransactionId,
                                        BankRRNNumber = item.BankRRNNumber,
                                        RedmilTransactionId = item.RedmilTransactionId,
                                        BalanceAmount = item.BalanceAmount,
                                        CommissionAmount = item.CommissionAmount,
                                        FailureReason = item.FailureReason,
                                        Status = item.Status

                                    });
                                }
                                return Json(Data17);
                            }
                            return Json(Data17);
                        }

                        else if (subCat == "18")
                        {
                            var Data18 = JsonConvert.DeserializeObject<List<MicroATMCashWithdrawalModel>>(JsonConvert.SerializeObject(datadeserialize));
                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data18.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data18);
                                }
                            }
                            else if ((!string.IsNullOrEmpty(sort)) && sort == "1")
                            {
                                foreach (var item in Data18)
                                {
                                    MicroATMCashWithdrawalResponseData.Add(new MicroATMCashWithdrawalModel
                                    {
                                        CardHolderName = item.CardHolderName,
                                        CardHolderMobileNumber = item.CardHolderMobileNumber,
                                        CardType = item.CardType,
                                        CardNumber = item.CardNumber,
                                        TransactionAmount = item.TransactionAmount,
                                        NPCITransactionId = item.NPCITransactionId,
                                        BankRRNNumber = item.BankRRNNumber,
                                        RedmilTransactionId = item.RedmilTransactionId,
                                        BalanceAmount = item.BalanceAmount,
                                        FailureReason = item.FailureReason,
                                        Status = item.Status

                                    });
                                }
                                return Json(Data18);
                            }
                            return Json(Data18);
                        }

                        else if (subCat == "19")
                        {
                            var Data19 = JsonConvert.DeserializeObject<List<MicroATMBalanceEnquiryModel>>(JsonConvert.SerializeObject(datadeserialize));
                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data19.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data19);
                                }
                            }
                            else if ((!string.IsNullOrEmpty(sort)) && sort == "1")
                            {
                                foreach (var item in Data19)
                                {
                                    MicroATMBalanceEnquiryResponseData.Add(new MicroATMBalanceEnquiryModel
                                    {
                                        CardHolderName = item.CardHolderName,
                                        CardHolderMobileNumber = item.CardHolderMobileNumber,
                                        CardType = item.CardType,
                                        CardNumber = item.CardNumber,
                                        TransactionAmount = item.TransactionAmount,
                                        NPCITransactionId = item.NPCITransactionId,
                                        BankRRNNumber = item.BankRRNNumber,
                                        RedmilTransactionId = item.RedmilTransactionId,
                                        BalanceAmount = item.BalanceAmount,
                                        FailureReason = item.FailureReason,
                                        Status = item.Status

                                    });
                                }
                                return Json(Data19);
                            }
                            return Json(Data19);
                        }

                        else if (subCat == "21")
                        {
                            var Data21 = JsonConvert.DeserializeObject<List<YBLDomesticMoneyTransferModel>>(JsonConvert.SerializeObject(datadeserialize));
                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data21.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data21);
                                }
                            }
                            else if ((!string.IsNullOrEmpty(sort)) && sort == "1")
                            {
                                foreach (var item in Data21)
                                {
                                    YBLDomesticMoneyTransferResponseData.Add(new YBLDomesticMoneyTransferModel
                                    {
                                        RequestDate = item.RequestDate,
                                        SenderName = item.SenderName,
                                        SenderMobileNumber = item.SenderMobileNumber,
                                        TransactionAmount = item.TransactionAmount,
                                        Status = item.Status,
                                        ReceiverName = item.ReceiverName,
                                        ReceiverAccountNo = item.ReceiverAccountNo,
                                        BankName = item.BankName,
                                        IFSCCode = item.IFSCCode,
                                        TransactionType = item.TransactionType,
                                        Surcharge = item.Surcharge,
                                        CommissionAmount = item.CommissionAmount,
                                        RedmilReferenceId = item.RedmilReferenceId,
                                        WalletType = item.WalletType,
                                        Reason = item.Reason


                                    });
                                }
                                return Json(Data21);
                            }
                            return Json(Data21);
                        }

                        else if (subCat == "23")
                        {
                            var Data23 = JsonConvert.DeserializeObject<List<CreditScoreModel>>(JsonConvert.SerializeObject(datadeserialize));
                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data23.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data23);
                                }
                            }
                            else if ((!string.IsNullOrEmpty(sort)) && sort == "1")
                            {
                                foreach (var item in Data23)
                                {
                                    CreditScoreResponseData.Add(new CreditScoreModel
                                    {
                                        RequestDate = item.RequestDate,
                                        CustomerName = item.CustomerName,
                                        CustomerMobileNo = item.CustomerMobileNo,
                                        AmountPaid = item.AmountPaid,
                                        Status = item.Status,
                                        CommissionAmount = item.CommissionAmount,
                                        RedmilTransactionId = item.RedmilTransactionId,
                                        WalletType = item.WalletType


                                    });
                                }
                                return Json(Data23);
                            }
                            return Json(Data23);
                        }
                        else if (subCat == "26")
                        {
                            var Data26 = JsonConvert.DeserializeObject<List<GoldInvestmentBuyModel>>(JsonConvert.SerializeObject(datadeserialize));
                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data26.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data26);
                                }
                            }
                            else if ((!string.IsNullOrEmpty(sort)) && sort == "1")
                            {
                                foreach (var item in Data26)
                                {
                                    GoldInvestmentBuyResponseData.Add(new GoldInvestmentBuyModel
                                    {
                                        RequestDate = item.RequestDate,
                                        CustomerName = item.CustomerName,
                                        CustomerMobileNumber = item.CustomerMobileNumber,
                                        PurchaseAmount = item.PurchaseAmount,
                                        TransactionId = item.TransactionId,
                                        InvoiceID = item.InvoiceID,
                                        GoldRatePerGm = item.GoldRatePerGm,
                                        PurchaseQuantity = item.PurchaseQuantity,
                                        Commission = item.Commission,
                                        CustomerID = item.CustomerID,
                                        OldGoldBalance = item.OldGoldBalance,
                                        NewGoldBalance = item.NewGoldBalance,
                                        WalletType = item.WalletType,
                                        PANNo = item.PANNo,
                                        AccountNo = item.AccountNo,
                                        IFSCCode = item.IFSCCode,
                                        BankName = item.BankName,
                                        Status = item.Status

                                    });
                                }
                                return Json(Data26);
                            }
                            return Json(Data26);
                        }

                        else if (subCat == "27")
                        {
                            var Data27 = JsonConvert.DeserializeObject<List<GoldInvestmentSellModel>>(JsonConvert.SerializeObject(datadeserialize));
                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data27.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data27);
                                }
                            }
                            else if ((!string.IsNullOrEmpty(sort)) && sort == "1")
                            {
                                foreach (var item in Data27)
                                {
                                    GoldInvestmentSellResponseData.Add(new GoldInvestmentSellModel
                                    {
                                        RequestDate = item.RequestDate,
                                        CustomerName = item.CustomerName,
                                        CustomerMobileNumber = item.CustomerMobileNumber,
                                        SellAmount = item.SellAmount,
                                        TransactionId = item.TransactionId,
                                        InvoiceID = item.InvoiceID,
                                        GoldRatePerGm = item.GoldRatePerGm,
                                        SellQuantity = item.SellQuantity,
                                        NetAmountCredited = item.NetAmountCredited,
                                        CustomerID = item.CustomerID,
                                        OldGoldBalance = item.OldGoldBalance,
                                        NewGoldBalance = item.NewGoldBalance,
                                        BankCharges = item.BankCharges,
                                        PANNo = item.PANNo,
                                        AccountNo = item.AccountNo,
                                        IFSCCode = item.IFSCCode,
                                        BankName = item.BankName,
                                        Status = item.Status

                                    });
                                }
                                return Json(Data27);
                            }
                            return Json(Data27);
                        }



                        //Recharge And Bill Payments 

                        else if (subCat == "1")
                        {
                            var Data01 = JsonConvert.DeserializeObject<List<BillPaymentsModel>>(JsonConvert.SerializeObject(datadeserialize));
                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data01.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data01);
                                }
                            }
                            //else if ((!string.IsNullOrEmpty(sort)) && sort == "1")
                            //{
                            //    foreach (var item in Data1)
                            //    {
                            //        Billl.Add(new GoldInvestmentSellModel
                            //        {
                            //            RequestDate = item.RequestDate,
                            //            CustomerName = item.CustomerName,
                            //            CustomerMobileNumber = item.CustomerMobileNumber,
                            //            SellAmount = item.SellAmount,
                            //            TransactionId = item.TransactionId,
                            //            InvoiceID = item.InvoiceID,
                            //            GoldRatePerGm = item.GoldRatePerGm,
                            //            SellQuantity = item.SellQuantity,
                            //            NetAmountCredited = item.NetAmountCredited,
                            //            CustomerID = item.CustomerID,
                            //            OldGoldBalance = item.OldGoldBalance,
                            //            NewGoldBalance = item.NewGoldBalance,
                            //            BankCharges = item.BankCharges,
                            //            PANNo = item.PANNo,
                            //            AccountNo = item.AccountNo,
                            //            IFSCCode = item.IFSCCode,
                            //            BankName = item.BankName,
                            //            Status = item.Status

                            //        });
                            //    }
                            //    return Json(Data27);
                            //}
                            return Json(Data01);
                        }

                        else if (subCat == "2")
                        {
                            var Data02 = JsonConvert.DeserializeObject<List<DTHRechargeModel>>(JsonConvert.SerializeObject(datadeserialize));
                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data02.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data02);
                                }
                            }
                            return Json(Data02);
                        }

                        else if (subCat == "3")
                        {
                            var Data03 = JsonConvert.DeserializeObject<List<MobilePrepaidModel>>(JsonConvert.SerializeObject(datadeserialize));
                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data03.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data03);
                                }
                            }
                            return Json(Data03);
                        }

                        else if (subCat == "4")
                        {
                            var Data04 = JsonConvert.DeserializeObject<List<MobilePostpaidModel>>(JsonConvert.SerializeObject(datadeserialize));
                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data04.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data04);
                                }
                            }
                            return Json(Data04);
                        }

                        else if (subCat == "5")
                        {
                            var Data05 = JsonConvert.DeserializeObject<List<LICPremiumPaymentsModel>>(JsonConvert.SerializeObject(datadeserialize));
                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data05.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data05);
                                }
                            }
                            return Json(Data05);
                        }



                        //Travel Services

                        else if (subCat == "6")
                        {
                            var Data06 = JsonConvert.DeserializeObject<List<BusBookingModel>>(JsonConvert.SerializeObject(datadeserialize));
                            //if (!string.IsNullOrEmpty(sort) && sort != "1")
                            //{
                            //    var a = ReplaceStatus(sort);
                            //    if (a != "All")
                            //    {
                            //        var Datanew = Data06.Where(x => x.Status == a);
                            //        return Json(Datanew);
                            //    }
                            //    else
                            //    {
                            //        return Json(Data06);
                            //    }
                            //}
                            return Json(Data06);
                        }



                        else if (subCat == "7")
                        {

                            return Json(new { Result = "RedirectToBooking", url = Url.Action("TravelServices", "MyBusinessReport") });
                        }
                        //var Data07 = JsonConvert.DeserializeObject<List<BusBookingModel>>(JsonConvert.SerializeObject(datadeserialize));
                        ////if (!string.IsNullOrEmpty(sort) && sort != "1")
                        ////{
                        ////    var a = ReplaceStatus(sort);
                        ////    if (a != "All")
                        ////    {
                        ////        var Datanew = Data07.Where(x => x.Status == a);
                        ////        return Json(Datanew);
                        ////    }
                        ////    else
                        ////    {
                        ////        return Json(Data07);
                        ////    }
                        ////}
                        //return Json(Data07);


                        //Payment Services
                        else if (subCat == "8")
                        {
                            var Data06 = JsonConvert.DeserializeObject<List<POSPaymentsModel>>(JsonConvert.SerializeObject(datadeserialize));
                            //if (!string.IsNullOrEmpty(sort) && sort != "1")
                            //{
                            //    var a = ReplaceStatus(sort);
                            //    if (a != "All")
                            //    {
                            //        var Datanew = Data06.Where(x => x.Status == a);
                            //        return Json(Datanew);
                            //    }
                            //    else
                            //    {
                            //        return Json(Data06);
                            //    }
                            //}
                            return Json(Data06);
                        }



                        else if (subCat == "9")
                        {
                            var Data06 = JsonConvert.DeserializeObject<List<POSPaymentsModel>>(JsonConvert.SerializeObject(datadeserialize));
                            //if (!string.IsNullOrEmpty(sort) && sort != "1")
                            //{
                            //    var a = ReplaceStatus(sort);
                            //    if (a != "All")
                            //    {
                            //        var Datanew = Data06.Where(x => x.Status == a);
                            //        return Json(Datanew);
                            //    }
                            //    else
                            //    {
                            //        return Json(Data06);
                            //    }
                            //}
                            return Json(Data06);
                        }


                        else if (subCat == "10")
                        {
                            var Data06 = JsonConvert.DeserializeObject<List<SMSPaymentsModel>>(JsonConvert.SerializeObject(datadeserialize));
                            //if (!string.IsNullOrEmpty(sort) && sort != "1")
                            //{
                            //    var a = ReplaceStatus(sort);
                            //    if (a != "All")
                            //    {
                            //        var Datanew = Data06.Where(x => x.Status == a);
                            //        return Json(Datanew);
                            //    }
                            //    else
                            //    {
                            //        return Json(Data06);
                            //    }
                            //}
                            return Json(Data06);
                        }
                        else if (subCat == "11")
                        {
                            var Data06 = JsonConvert.DeserializeObject<List<UPIDynamicQR>>(JsonConvert.SerializeObject(datadeserialize));
                            //if (!string.IsNullOrEmpty(sort) && sort != "1")
                            //{
                            //    var a = ReplaceStatus(sort);
                            //    if (a != "All")
                            //    {
                            //        var Datanew = Data06.Where(x => x.Status == a);
                            //        return Json(Datanew);
                            //    }
                            //    else
                            //    {
                            //        return Json(Data06);
                            //    }
                            //}
                            return Json(Data06);
                        }


                        else if (subCat == "12")
                        {
                            var Data06 = JsonConvert.DeserializeObject<List<UPIStaticQR>>(JsonConvert.SerializeObject(datadeserialize));
                            //if (!string.IsNullOrEmpty(sort) && sort != "1")
                            //{
                            //    var a = ReplaceStatus(sort);
                            //    if (a != "All")
                            //    {
                            //        var Datanew = Data06.Where(x => x.Status == a);
                            //        return Json(Datanew);
                            //    }
                            //    else
                            //    {
                            //        return Json(Data06);
                            //    }
                            //}
                            return Json(Data06);
                        }

                        else if (subCat == "13")
                        {
                            // var Data06 = JsonConvert.DeserializeObject<List<>>(JsonConvert.SerializeObject(datadeserialize));
                            //if (!string.IsNullOrEmpty(sort) && sort != "1")
                            //{
                            //    var a = ReplaceStatus(sort);
                            //    if (a != "All")
                            //    {
                            //        var Datanew = Data06.Where(x => x.Status == a);
                            //        return Json(Datanew);
                            //    }
                            //    else
                            //    {
                            //        return Json(Data06);
                            //    }
                            //}
                            // return Json(Data06);
                        }

                        //Digital Distribution Services

                        else if (subCat == "32")
                        {
                            var Data06 = JsonConvert.DeserializeObject<List<AmazonPayGiftCardModel>>(JsonConvert.SerializeObject(datadeserialize));
                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data06.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data06);
                                }
                            }
                            return Json(Data06);
                        }



                        //Banking Solution Services

                        else if (subCat == "14")
                        {
                            HttpContext.Session.SetString("UserId", requestModel.UserId);
                            return Json(new { Result = "Redirect", url = Url.Action("PanCardRegistration", "MyBusinessReport") });
                        }


                        else if (subCat == "10")
                        {
                            var Data10 = JsonConvert.DeserializeObject<List<SMSPaymentsModel>>(JsonConvert.SerializeObject(datadeserialize));
                            if (!string.IsNullOrEmpty(sort) && sort != "1")
                            {
                                var a = ReplaceStatus(sort);
                                if (a != "All")
                                {
                                    var Datanew = Data10.Where(x => x.Status == a);
                                    return Json(Datanew);
                                }
                                else
                                {
                                    return Json(Data10);
                                }
                            }
                            else if ((!string.IsNullOrEmpty(sort)) && sort == "1")
                            {
                                foreach (var item in Data10)
                                {
                                    SMSPaymentsResponseData.Add(new SMSPaymentsModel
                                    {
                                        RequestDate = item.RequestDate,
                                        RequestData = item.RequestData,
                                        CustomerMobile = item.CustomerMobile,
                                        CreditAmount = item.CreditAmount,
                                        Status = item.Status,
                                        TransactionID = item.TransactionID,
                                        OrderID = item.OrderID,
                                        Mode = item.Mode,
                                        Surcharge = item.Surcharge


                                    });
                                }
                                return Json(Data10);
                            }
                            return Json(Data10);
                        }

                        return Json("");
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
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = requestModel;
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

        #region ViewReceipt
        public JsonResult ViewReceipt(string subCat, BusinessReportModel result) // ad1,ad2,ad3
        {
            AePSCashWithdrawalModel aePSCashWithdrawalModel = new AePSCashWithdrawalModel();
            AePSMiniStatementModel aePSMiniStatementModel = new AePSMiniStatementModel();
            AePSBalanceEnquiryModel aePSBalanceEnquiryModel = new AePSBalanceEnquiryModel();
            MicroATMCashWithdrawalModel microATMCashWithdrawalModel = new MicroATMCashWithdrawalModel();
            MicroATMBalanceEnquiryModel microATMBalanceEnquiryModel = new MicroATMBalanceEnquiryModel();
            MicroATMCashWithdrawalICICIModel microATMCashWithdrawalICICIModel = new MicroATMCashWithdrawalICICIModel();
            MicroATMBalanceEnquiryICICIModel microATMBalanceEnquiryICICIModel = new MicroATMBalanceEnquiryICICIModel();
            UPICashWithdrawalModel upiCashWithdrawalModel = new UPICashWithdrawalModel();
            YBLDomesticMoneyTransferModel yBLDomesticMoneyTransferModel = new YBLDomesticMoneyTransferModel();
            CreditScoreModel creditScoreModel = new CreditScoreModel();
            FinoDMTModel finoDMTModel = new FinoDMTModel();

            GoldInvestmentBuyModel1 goldInvestmentBuyModel1 = new GoldInvestmentBuyModel1();
            GoldInvestmentSellModel1 goldInvestmentSellModel1 = new GoldInvestmentSellModel1();

            POSPaymentsModel pOSPaymentsModel = new POSPaymentsModel();

            BillPaymentsModel billPaymentsModel = new BillPaymentsModel();
            DTHRechargeModel dTHRechargeModel = new DTHRechargeModel();
            MobilePrepaidModel mobilePrepaidModel = new MobilePrepaidModel();
            MobilePostpaidModel mobilePostpaidModel = new MobilePostpaidModel();
            LICPremiumPaymentsModel licPremiumPaymentsModel = new LICPremiumPaymentsModel();
            AmazonPayGiftCardModel amazonPayGiftCardModel = new AmazonPayGiftCardModel();
            switch (subCat)
            {

                //Banking Services
                case "15":
                    if (result.aePSCashWithdrawalModel != null)
                    {
                        aePSCashWithdrawalModel = JsonConvert.DeserializeObject<AePSCashWithdrawalModel>(JsonConvert.SerializeObject(result.aePSCashWithdrawalModel));
                    }
                    else
                    {
                        aePSCashWithdrawalModel = JsonConvert.DeserializeObject<AePSCashWithdrawalModel>(JsonConvert.SerializeObject(result));
                    }

                    break;
                case "16":
                    if (result.aePSCashWithdrawalModel != null)
                    {
                        aePSMiniStatementModel = JsonConvert.DeserializeObject<AePSMiniStatementModel>(JsonConvert.SerializeObject(result.aePSCashWithdrawalModel));
                    }
                    else
                    {
                        aePSMiniStatementModel = JsonConvert.DeserializeObject<AePSMiniStatementModel>(JsonConvert.SerializeObject(result));
                    }

                    break;
                case "17":
                    if (result.aePSCashWithdrawalModel != null)
                    {
                        aePSBalanceEnquiryModel = JsonConvert.DeserializeObject<AePSBalanceEnquiryModel>(JsonConvert.SerializeObject(result.aePSCashWithdrawalModel));
                    }
                    else
                    {
                        aePSBalanceEnquiryModel = JsonConvert.DeserializeObject<AePSBalanceEnquiryModel>(JsonConvert.SerializeObject(result));
                    }

                    break;
                case "18":
                    if (result.microATMCashWithdrawalModel != null)
                    {
                        microATMCashWithdrawalModel = JsonConvert.DeserializeObject<MicroATMCashWithdrawalModel>(JsonConvert.SerializeObject(result.microATMCashWithdrawalModel));
                    }
                    else
                    {
                        microATMCashWithdrawalModel = JsonConvert.DeserializeObject<MicroATMCashWithdrawalModel>(JsonConvert.SerializeObject(result));
                    }

                    break;
                case "19":
                    if (result.microATMBalanceEnquiryModel != null)
                    {
                        microATMBalanceEnquiryModel = JsonConvert.DeserializeObject<MicroATMBalanceEnquiryModel>(JsonConvert.SerializeObject(result.microATMBalanceEnquiryModel));
                    }
                    else
                    {
                        microATMBalanceEnquiryModel = JsonConvert.DeserializeObject<MicroATMBalanceEnquiryModel>(JsonConvert.SerializeObject(result));
                    }

                    break;

                case "21":
                    if (result.yBLDomesticMoneyTransferModel != null)
                    {
                        yBLDomesticMoneyTransferModel = JsonConvert.DeserializeObject<YBLDomesticMoneyTransferModel>(JsonConvert.SerializeObject(result.yBLDomesticMoneyTransferModel));
                    }
                    else
                    {
                        yBLDomesticMoneyTransferModel = JsonConvert.DeserializeObject<YBLDomesticMoneyTransferModel>(JsonConvert.SerializeObject(result));
                    }

                    break;

                case "23":
                    if (result.creditScoreModel != null)
                    {
                        creditScoreModel = JsonConvert.DeserializeObject<CreditScoreModel>(JsonConvert.SerializeObject(result.creditScoreModel));
                    }
                    else
                    {
                        creditScoreModel = JsonConvert.DeserializeObject<CreditScoreModel>(JsonConvert.SerializeObject(result));
                    }

                    break;

                case "25":
                    upiCashWithdrawalModel = JsonConvert.DeserializeObject<UPICashWithdrawalModel>(JsonConvert.SerializeObject(result));
                    break;


                case "29":
                    if (result.microATMCashWithdrawalModel != null)
                    {
                        microATMCashWithdrawalICICIModel = JsonConvert.DeserializeObject<MicroATMCashWithdrawalICICIModel>(JsonConvert.SerializeObject(result.microATMCashWithdrawalModel));
                    }
                    else
                    {
                        microATMCashWithdrawalICICIModel = JsonConvert.DeserializeObject<MicroATMCashWithdrawalICICIModel>(JsonConvert.SerializeObject(result));
                    }

                    break;
                case "30":
                    if (result.microATMCashWithdrawalModel != null)
                    {
                        microATMBalanceEnquiryICICIModel = JsonConvert.DeserializeObject<MicroATMBalanceEnquiryICICIModel>(JsonConvert.SerializeObject(result.microATMCashWithdrawalModel));
                    }
                    else
                    {
                        microATMBalanceEnquiryICICIModel = JsonConvert.DeserializeObject<MicroATMBalanceEnquiryICICIModel>(JsonConvert.SerializeObject(result));
                    }
                    break;

                case "31":
                    if (result.finoDMTModel != null)
                    {
                        finoDMTModel = JsonConvert.DeserializeObject<FinoDMTModel>(JsonConvert.SerializeObject(result.finoDMTModel));
                    }
                    else
                    {
                        finoDMTModel = JsonConvert.DeserializeObject<FinoDMTModel>(JsonConvert.SerializeObject(result));
                    }

                    break;



                //microATMBalanceEnquiryICICIModel = JsonConvert.DeserializeObject<MicroATMBalanceEnquiryICICIModel>(JsonConvert.SerializeObject(result));

                //case "9":
                //    pOSPaymentsModel = JsonConvert.DeserializeObject<POSPaymentsModel>(JsonConvert.SerializeObject(result));
                //    break;



                //Investment Services
                case "26":
                    if (result.goldInvestmentBuyModel1 != null)
                    {
                        goldInvestmentBuyModel1 = JsonConvert.DeserializeObject<GoldInvestmentBuyModel1>(JsonConvert.SerializeObject(result.goldInvestmentBuyModel1));
                    }
                    else
                    {
                        goldInvestmentBuyModel1 = JsonConvert.DeserializeObject<GoldInvestmentBuyModel1>(JsonConvert.SerializeObject(result));
                    }

                    break;
                case "27":
                    if (result.goldInvestmentSellModel1 != null)
                    {
                        goldInvestmentSellModel1 = JsonConvert.DeserializeObject<GoldInvestmentSellModel1>(JsonConvert.SerializeObject(result.goldInvestmentSellModel1));
                    }
                    else
                    {
                        goldInvestmentSellModel1 = JsonConvert.DeserializeObject<GoldInvestmentSellModel1>(JsonConvert.SerializeObject(result));
                    }

                    break;

                //Recharges and Bill Payments
                case "1":
                    if (result.billPaymentsModel != null)
                    {
                        billPaymentsModel = JsonConvert.DeserializeObject<BillPaymentsModel>(JsonConvert.SerializeObject(result.billPaymentsModel));
                    }
                    else
                    {
                        billPaymentsModel = JsonConvert.DeserializeObject<BillPaymentsModel>(JsonConvert.SerializeObject(result));
                    }
                    break;
                case "2":
                    if (result.mobilePrepaidPostpaid != null)
                    {
                        dTHRechargeModel = JsonConvert.DeserializeObject<DTHRechargeModel>(JsonConvert.SerializeObject(result.mobilePrepaidPostpaid));
                    }
                    else
                    {
                        dTHRechargeModel = JsonConvert.DeserializeObject<DTHRechargeModel>(JsonConvert.SerializeObject(result));
                    }
                    break;
                case "3":
                    if (result.mobilePrepaidPostpaid != null)
                    {
                        mobilePrepaidModel = JsonConvert.DeserializeObject<MobilePrepaidModel>(JsonConvert.SerializeObject(result.mobilePrepaidPostpaid));
                    }
                    else
                    {
                        mobilePrepaidModel = JsonConvert.DeserializeObject<MobilePrepaidModel>(JsonConvert.SerializeObject(result));
                    }
                    break;
                case "4":
                    if (result.mobilePrepaidPostpaid != null)
                    {
                        mobilePostpaidModel = JsonConvert.DeserializeObject<MobilePostpaidModel>(JsonConvert.SerializeObject(result.mobilePrepaidPostpaid));
                    }
                    else
                    {
                        mobilePostpaidModel = JsonConvert.DeserializeObject<MobilePostpaidModel>(JsonConvert.SerializeObject(result));
                    }
                    break;

                case "5":
                    if (result.licPremiumPaymentsModel != null)
                    {
                        licPremiumPaymentsModel = JsonConvert.DeserializeObject<LICPremiumPaymentsModel>(JsonConvert.SerializeObject(result.licPremiumPaymentsModel));
                    }
                    else
                    {
                        licPremiumPaymentsModel = JsonConvert.DeserializeObject<LICPremiumPaymentsModel>(JsonConvert.SerializeObject(result));
                    }

                    break;


                //Amazon Pay Gift Cards
                case "32":
                    if (result.amazonPayGiftCardModel != null)
                    {
                        amazonPayGiftCardModel = JsonConvert.DeserializeObject<AmazonPayGiftCardModel>(JsonConvert.SerializeObject(result.amazonPayGiftCardModel));
                    }
                    else
                    {
                        amazonPayGiftCardModel = JsonConvert.DeserializeObject<AmazonPayGiftCardModel>(JsonConvert.SerializeObject(result));
                    }

                    break;
            }

            //var client = new RestClient("https://api.redmilbusinessmall.com/api/transactions/receipt");
            //var client = new RestClient($"{Baseurl}{ApiName.TransactionSubCategory}");

            dynamic dynamicvalue;

            AePSCashWithdrawalModel1 requestModel = new AePSCashWithdrawalModel1();

            var json = "";

            if (subCat == "15")
            {
                requestModel.Status = aePSCashWithdrawalModel.Status;
                requestModel.RequestDate = aePSCashWithdrawalModel.RequestDate;
                requestModel.RequestTime = aePSCashWithdrawalModel.RequestTime;
                requestModel.BalAmount = Convert.ToInt64(aePSCashWithdrawalModel.BalanceAmount);
                requestModel.CustomerName = aePSCashWithdrawalModel.CustomerName;
                requestModel.AadhaarNumber = aePSCashWithdrawalModel.CustomerAadhaarNo;
                requestModel.CustomerMobile = aePSCashWithdrawalModel.CustomerMobileNumber;
                requestModel.BankName = aePSCashWithdrawalModel.BankName;
                requestModel.RRN = aePSCashWithdrawalModel.BankRRNNumber;
                requestModel.Reason = aePSCashWithdrawalModel.FailureReason;
                requestModel.IIN = Convert.ToInt32(aePSCashWithdrawalModel.IIN);
                requestModel.NpciTransId = aePSCashWithdrawalModel.NPCITransactionId;
                requestModel.RedmilTransactionId = Convert.ToInt32(aePSCashWithdrawalModel.RedmilTransactionId);
                requestModel.Amount = Convert.ToInt64(aePSCashWithdrawalModel.TransactionAmount);
                requestModel.DEName = aePSCashWithdrawalModel.DEName;
                requestModel.DEMobile = aePSCashWithdrawalModel.DEMobile;
                requestModel.ServiceType = ReplaceServices(subCat);
                json = JsonConvert.SerializeObject(requestModel);

            }

            else if (subCat == "16")
            {
                requestModel.Status = aePSMiniStatementModel.Status;
                requestModel.RequestDate = aePSMiniStatementModel.RequestDate;
                requestModel.RequestTime = aePSMiniStatementModel.RequestTime;
                requestModel.BalAmount = Convert.ToInt64(aePSMiniStatementModel.BalanceAmount);
                requestModel.CustomerName = aePSMiniStatementModel.CustomerName;
                requestModel.AadhaarNumber = aePSMiniStatementModel.CustomerAadhaarNo;
                requestModel.CustomerMobile = aePSMiniStatementModel.CustomerMobileNumber;
                requestModel.BankName = aePSMiniStatementModel.BankName;
                requestModel.RRN = aePSMiniStatementModel.BankRRNNumber;
                requestModel.Reason = aePSMiniStatementModel.FailureReason;
                requestModel.IIN = Convert.ToInt32(aePSMiniStatementModel.IIN);
                requestModel.NpciTransId = aePSMiniStatementModel.NPCITransactionId;
                requestModel.RedmilTransactionId = Convert.ToInt32(aePSMiniStatementModel.RedmilTransactionId);
                requestModel.Amount = aePSMiniStatementModel.TransactionAmount;
                requestModel.DEName = aePSMiniStatementModel.DEName;
                requestModel.DEMobile = aePSMiniStatementModel.DEMobile;
                requestModel.ServiceType = ReplaceServices(subCat);
                json = JsonConvert.SerializeObject(requestModel);

            }

            else if (subCat == "17")
            {
                requestModel.Status = aePSBalanceEnquiryModel.Status;
                requestModel.RequestDate = aePSBalanceEnquiryModel.RequestDate;
                requestModel.RequestTime = aePSBalanceEnquiryModel.RequestTime;
                requestModel.BalAmount = Convert.ToInt64(aePSBalanceEnquiryModel.BalanceAmount);
                requestModel.CustomerName = aePSBalanceEnquiryModel.CustomerName;
                requestModel.AadhaarNumber = aePSBalanceEnquiryModel.CustomerAadhaarNo;
                requestModel.CustomerMobile = aePSBalanceEnquiryModel.CustomerMobileNumber;
                requestModel.BankName = aePSBalanceEnquiryModel.BankName;
                requestModel.RRN = aePSBalanceEnquiryModel.BankRRNNumber;
                requestModel.Reason = aePSBalanceEnquiryModel.FailureReason;
                requestModel.IIN = Convert.ToInt32(aePSBalanceEnquiryModel.IIN);
                requestModel.NpciTransId = aePSBalanceEnquiryModel.NPCITransactionId;
                requestModel.RedmilTransactionId = Convert.ToInt32(aePSBalanceEnquiryModel.RedmilTransactionId);
                requestModel.Amount = Convert.ToInt64(aePSBalanceEnquiryModel.TransactionAmount);
                requestModel.DEName = aePSBalanceEnquiryModel.DEName;
                requestModel.DEMobile = aePSBalanceEnquiryModel.DEMobile;
                requestModel.ServiceType = ReplaceServices(subCat);
                json = JsonConvert.SerializeObject(requestModel);
            }

            YBLDomesticMoneyTransferModel1 yBLDomesticMoneyTransferModel1 = new YBLDomesticMoneyTransferModel1();

            if (subCat == "21")
            {
                yBLDomesticMoneyTransferModel1.Status = yBLDomesticMoneyTransferModel.Status;
                yBLDomesticMoneyTransferModel1.RequestDate = yBLDomesticMoneyTransferModel.RequestDate;
                yBLDomesticMoneyTransferModel1.RequestTime = yBLDomesticMoneyTransferModel.RequestTime;
                yBLDomesticMoneyTransferModel1.SenderName = yBLDomesticMoneyTransferModel.SenderName;
                yBLDomesticMoneyTransferModel1.SenderMobileNumber = yBLDomesticMoneyTransferModel.SenderMobileNumber.ToString();
                yBLDomesticMoneyTransferModel1.RedmilTransId = yBLDomesticMoneyTransferModel.RedmilReferenceId.ToString();
                yBLDomesticMoneyTransferModel1.Amount = yBLDomesticMoneyTransferModel.TransactionAmount.ToString();
                yBLDomesticMoneyTransferModel1.recMobile = yBLDomesticMoneyTransferModel.ReceiverMobileNumber;
                yBLDomesticMoneyTransferModel1.recBankName = yBLDomesticMoneyTransferModel.BankName;
                yBLDomesticMoneyTransferModel1.recAcNum = yBLDomesticMoneyTransferModel.ReceiverAccountNo.ToString();
                yBLDomesticMoneyTransferModel1.Reason = yBLDomesticMoneyTransferModel.Reason;
                yBLDomesticMoneyTransferModel1.rrn = yBLDomesticMoneyTransferModel.BankRRNNumber;
                yBLDomesticMoneyTransferModel1.TransType = yBLDomesticMoneyTransferModel.TransactionType;
                yBLDomesticMoneyTransferModel1.recName = yBLDomesticMoneyTransferModel.ReceiverName;
                yBLDomesticMoneyTransferModel1.Surcharge = yBLDomesticMoneyTransferModel.Surcharge.ToString();
                yBLDomesticMoneyTransferModel1.recIfsc = yBLDomesticMoneyTransferModel.IFSCCode.ToString();
                yBLDomesticMoneyTransferModel1.DEName = yBLDomesticMoneyTransferModel.DEName;
                yBLDomesticMoneyTransferModel1.DEMobile = yBLDomesticMoneyTransferModel.DEMobile;

                yBLDomesticMoneyTransferModel1.ServiceType = ReplaceServices(subCat);

                json = JsonConvert.SerializeObject(yBLDomesticMoneyTransferModel1);

            }

            CreditScoreModel1 creditScoreModel1 = new CreditScoreModel1();

            if (subCat == "23")
            {


                creditScoreModel1.Status = creditScoreModel.Status;
                creditScoreModel1.RequestDate = creditScoreModel.RequestDate;
                creditScoreModel1.RequestTime = creditScoreModel.RequestTime;
                creditScoreModel1.CustomerName = creditScoreModel.CustomerName;
                creditScoreModel1.CustomerMobileNo = creditScoreModel.CustomerMobileNo;
                creditScoreModel1.TransactionAmount = creditScoreModel.AmountPaid.ToString();
                creditScoreModel1.RedmilTransactionID = creditScoreModel.RedmilTransactionId.ToString();
                creditScoreModel1.DEName = creditScoreModel.DEName;
                creditScoreModel1.DEMobile = creditScoreModel.DEMobile;
                if (creditScoreModel.CIBILScorePDFLink.Equals("null"))
                {
                    creditScoreModel1.CIBILScorePDFLink = "Rajneesh";

                }
                else
                {
                    creditScoreModel1.CIBILScorePDFLink = creditScoreModel.CIBILScorePDFLink;
                }


                creditScoreModel1.ServiceType = ReplaceServices(subCat);

                json = JsonConvert.SerializeObject(creditScoreModel1);

            }

            FinoDMTModel1 finoDMTModel1 = new FinoDMTModel1();

            if (subCat == "31")
            {
                finoDMTModel1.Status = finoDMTModel.Status;
                finoDMTModel1.RequestDate = finoDMTModel.RequestDate;
                finoDMTModel1.RequestTime = finoDMTModel.RequestTime;
                if (!string.IsNullOrEmpty(finoDMTModel.TransactionID))
                {
                    finoDMTModel1.TransId = finoDMTModel.TransactionID;
                }
                else
                {
                    finoDMTModel1.TransId = "null";
                }
                finoDMTModel1.reqId = finoDMTModel.RequestId;
                finoDMTModel1.TransType = finoDMTModel.TransactionType;
                finoDMTModel1.RedmilTransId = finoDMTModel.RedmilReferenceId.ToString();
                finoDMTModel1.Unique = finoDMTModel.ClientUniqueId;
                finoDMTModel1.sendName = finoDMTModel.SenderName;
                finoDMTModel1.sendMobile = finoDMTModel.SenderMobileNumber.ToString();
                finoDMTModel1.recName = finoDMTModel.ReceiverName;
                finoDMTModel1.recAcNum = finoDMTModel.ReceiverAccountNo;
                finoDMTModel1.recBankName = finoDMTModel.BankName;
                finoDMTModel1.recIfsc = finoDMTModel.IFSCCode;
                finoDMTModel1.Surcharge = finoDMTModel.Surcharge;
                finoDMTModel1.amount = finoDMTModel.TransactionAmount.ToString();
                finoDMTModel1.bankCharges = finoDMTModel.BankCharges.ToString();
                finoDMTModel1.ServiceType = ReplaceServices(subCat);
                finoDMTModel1.DEName = finoDMTModel.DEName;
                finoDMTModel1.DEMobile = finoDMTModel.DEMobile;
                json = JsonConvert.SerializeObject(finoDMTModel1);

            }

            GoldInvestmentBuySellModel goldInvestmentBuySellModel = new GoldInvestmentBuySellModel();

            if (subCat == "26")
            {
                //goldInvestmentBuySellModel.BankName = goldInvestmentBuyModel.BankName;
                //goldInvestmentBuySellModel.RequestDate = goldInvestmentBuyModel.RequestDate;
                //goldInvestmentBuySellModel.RequestTime = goldInvestmentBuyModel.RequestTime;
                //goldInvestmentBuySellModel.CustomerName = goldInvestmentBuyModel.CustomerName;
                //goldInvestmentBuySellModel.CustomerMobileNumber = goldInvestmentBuyModel.CustomerMobileNumber;
                //goldInvestmentBuySellModel.GoldRatePerGram = goldInvestmentBuyModel.GoldRatePerGm;
                //goldInvestmentBuySellModel.CustomerID = goldInvestmentBuyModel.CustomerID.ToString();
                //goldInvestmentBuySellModel.OldGoldBalance = goldInvestmentBuyModel.OldGoldBalance;
                //goldInvestmentBuySellModel.NewGoldBalance = goldInvestmentBuyModel.NewGoldBalance;
                //goldInvestmentBuySellModel.cusInvoice = goldInvestmentBuyModel.InvoiceID;


                //goldInvestmentBuySellModel.TransactionAmount = goldInvestmentBuyModel.PurchaseAmount;
                //goldInvestmentBuySellModel.Quantity = goldInvestmentBuyModel.PurchaseQuantity;

                ////else
                ////{
                ////    goldInvestmentBuySellModel.TransactionAmount = goldInvestmentSellModel.SellAmount.ToString();
                ////    goldInvestmentBuySellModel.Quantity = goldInvestmentSellModel.SellQuantity;
                ////    goldInvestmentBuySellModel.BankCharge = goldInvestmentSellModel.BankCharges;
                ////    goldInvestmentBuySellModel.NetAmount = goldInvestmentSellModel.NetAmountCredited;
                ////}

                //goldInvestmentBuySellModel.Quantity = goldInvestmentSellModel.SellQuantity;
                //goldInvestmentBuySellModel.InvoiceId = goldInvestmentBuyModel.InvoiceID;
                //goldInvestmentBuySellModel.RedmilTransactionID = goldInvestmentBuyModel.TransactionId.ToString();
                //goldInvestmentBuySellModel.pan = goldInvestmentBuyModel.PANNo;
                //goldInvestmentBuySellModel.BankAccountNumber = goldInvestmentBuyModel.AccountNo;
                //goldInvestmentBuySellModel.IFSC = goldInvestmentBuyModel.BankName;
                //goldInvestmentBuySellModel.ServiceType = ReplaceServices(subCat);

                json = JsonConvert.SerializeObject(result.goldInvestmentBuyModel1);
            }

            GoldInvestmentSellModel goldInvestmentSellRequestModel = new GoldInvestmentSellModel();
            if (subCat == "27")
            {

                //goldInvestmentSellModel1.IFSCCode = goldInvestmentSellRequestModel.IFSCCode;
                //goldInvestmentSellModel1.BankAccountNumber = goldInvestmentSellRequestModel.AccountNo;
                //goldInvestmentSellModel1.DEName = goldInvestmentSellRequestModel.DEName;
                //goldInvestmentSellModel1.DEMobile = goldInvestmentSellRequestModel.DEMobile;
                //goldInvestmentSellModel1.BankCharges = goldInvestmentSellRequestModel.BankCharges;
                //goldInvestmentSellModel1.RequestDate = goldInvestmentSellRequestModel.RequestDate;
                //goldInvestmentSellModel1.RequestTime = goldInvestmentSellRequestModel.RequestTime;
                //goldInvestmentSellModel1.CustomerName = goldInvestmentSellRequestModel.CustomerName;
                //goldInvestmentSellModel1.CustomerMobileNumber = goldInvestmentSellRequestModel.CustomerMobileNumber;
                //goldInvestmentSellModel1.TransactionAmount = goldInvestmentSellRequestModel.SellAmount;
                //goldInvestmentSellModel1.TransactionID = goldInvestmentSellRequestModel.TransactionId;
                //goldInvestmentSellModel1.InvoiceId = goldInvestmentSellRequestModel.InvoiceID;
                //goldInvestmentSellModel1.GoldRatePerGram = goldInvestmentSellRequestModel.GoldRatePerGm;
                //goldInvestmentSellModel1.SellQuantity = goldInvestmentSellRequestModel.SellQuantity;
                ////if (subCat == "26")
                ////{
                ////    goldInvestmentBuySellModel.TransactionAmount = goldInvestmentBuyModel.PurchaseAmount;
                ////    goldInvestmentBuySellModel.Quantity = goldInvestmentBuyModel.PurchaseQuantity;
                ////}

                //goldInvestmentSellModel1.CustomerID = goldInvestmentSellRequestModel.CustomerID;
                //goldInvestmentSellModel1.OldGoldBalance = goldInvestmentSellRequestModel.OldGoldBalance;
                //goldInvestmentSellModel1.NewGoldBalance = goldInvestmentSellRequestModel.NewGoldBalance;
                //goldInvestmentSellModel1.CustomerPanNumber = goldInvestmentSellRequestModel.PANNo;

                //goldInvestmentSellModel1.BankName = goldInvestmentSellRequestModel.BankName;
                //goldInvestmentSellModel1.TransactionAmount = goldInvestmentSellRequestModel.NetAmountCredited;

                //goldInvestmentSellModel1.ServiceType = ReplaceServices(subCat);

                json = JsonConvert.SerializeObject(result.goldInvestmentSellModel1);
            }





            MicroATMCashWithdrawalModel1 requestModel1 = new MicroATMCashWithdrawalModel1();

            if (subCat == "30")
            {


                requestModel1.Status = microATMBalanceEnquiryICICIModel.Status;
                requestModel1.RequestDate = microATMBalanceEnquiryICICIModel.RequestDate;
                requestModel1.RequestTime = microATMBalanceEnquiryICICIModel.RequestTime;
                requestModel1.Balamount = Convert.ToInt64(microATMBalanceEnquiryICICIModel.BalanceAmount);
                requestModel1.CardHolderName = microATMBalanceEnquiryICICIModel.CardHolderName;
                requestModel1.CardHolderContactNumber = microATMBalanceEnquiryICICIModel.CardHolderMobileNumber;
                requestModel1.CardType = microATMBalanceEnquiryICICIModel.CardType;
                requestModel1.CardNumber = microATMBalanceEnquiryICICIModel.CardNumber;
                requestModel1.RRN = microATMBalanceEnquiryICICIModel.BankRRNNumber;
                requestModel1.Reason = microATMBalanceEnquiryICICIModel.FailureReason;
                requestModel1.NPCITransactionID = microATMBalanceEnquiryICICIModel.NPCITransactionId;
                requestModel1.RedmilTransactionID = microATMBalanceEnquiryICICIModel.RedmilTransactionId;
                requestModel1.TransactionAmount = Convert.ToInt64(microATMBalanceEnquiryICICIModel.TransactionAmount);
                requestModel.DEName = microATMBalanceEnquiryICICIModel.DEName;
                requestModel.DEMobile = microATMBalanceEnquiryICICIModel.DEMobile;

                requestModel1.ServiceType = ReplaceServices(subCat);

                json = JsonConvert.SerializeObject(requestModel1);


            }

            else if (subCat == "29")
            {
                requestModel1.Status = microATMCashWithdrawalICICIModel.Status;
                requestModel1.RequestDate = microATMCashWithdrawalICICIModel.RequestDate;
                requestModel1.RequestTime = microATMCashWithdrawalICICIModel.RequestTime;
                requestModel1.Balamount = Convert.ToInt64(microATMCashWithdrawalICICIModel.BalanceAmount);
                requestModel1.CardHolderName = microATMCashWithdrawalICICIModel.CardHolderName;
                requestModel1.CardHolderContactNumber = microATMCashWithdrawalICICIModel.CardHolderMobileNumber;
                requestModel1.CardType = microATMCashWithdrawalICICIModel.CardType;
                requestModel1.CardNumber = microATMCashWithdrawalICICIModel.CardNumber;
                requestModel1.RRN = microATMCashWithdrawalICICIModel.BankRRNNumber;
                requestModel1.Reason = microATMCashWithdrawalICICIModel.FailureReason;
                requestModel1.NPCITransactionID = microATMCashWithdrawalICICIModel.NPCITransactionId;
                requestModel1.RedmilTransactionID = microATMCashWithdrawalICICIModel.RedmilTransactionId;
                requestModel1.TransactionAmount = Convert.ToInt64(microATMCashWithdrawalICICIModel.TransactionAmount);
                requestModel1.DEName = microATMCashWithdrawalICICIModel.DEName;
                requestModel1.DEMobile = microATMCashWithdrawalICICIModel.DEMobile;

                requestModel1.ServiceType = ReplaceServices(subCat);

                json = JsonConvert.SerializeObject(requestModel1);
            }

            else if (subCat == "18")
            {
                requestModel1.Status = microATMCashWithdrawalModel.Status;
                requestModel1.RequestDate = microATMCashWithdrawalModel.RequestDate;
                requestModel1.RequestTime = microATMCashWithdrawalModel.RequestTime;
                requestModel1.Balamount = Convert.ToInt64(microATMCashWithdrawalModel.BalanceAmount);
                requestModel1.CardHolderName = microATMCashWithdrawalModel.CardHolderName;
                requestModel1.CardHolderContactNumber = microATMCashWithdrawalModel.CardHolderMobileNumber;
                requestModel1.CardType = microATMCashWithdrawalModel.CardType;
                requestModel1.CardNumber = microATMCashWithdrawalModel.CardNumber;
                requestModel1.RRN = microATMCashWithdrawalModel.BankRRNNumber;
                requestModel1.Reason = microATMCashWithdrawalModel.FailureReason;
                requestModel1.NPCITransactionID = microATMCashWithdrawalModel.NPCITransactionId;
                requestModel1.RedmilTransactionID = microATMCashWithdrawalModel.RedmilTransactionId;
                requestModel1.TransactionAmount = Convert.ToInt64(microATMCashWithdrawalModel.TransactionAmount);
                requestModel1.DEName = microATMCashWithdrawalModel.DEName;
                requestModel1.DEMobile = microATMCashWithdrawalModel.DEMobile;
                requestModel1.ServiceType = ReplaceServices(subCat);

                json = JsonConvert.SerializeObject(requestModel1);
            }

            else if (subCat == "19")
            {
                requestModel1.Status = microATMBalanceEnquiryModel.Status;
                requestModel1.RequestDate = microATMBalanceEnquiryModel.RequestDate;
                requestModel1.RequestTime = microATMBalanceEnquiryModel.RequestTime;
                requestModel1.Balamount = Convert.ToInt64(microATMBalanceEnquiryModel.BalanceAmount);
                requestModel1.CardHolderName = microATMBalanceEnquiryModel.CardHolderName;
                requestModel1.CardHolderContactNumber = microATMBalanceEnquiryModel.CardHolderMobileNumber;
                requestModel1.CardType = microATMBalanceEnquiryModel.CardType;
                requestModel1.CardNumber = microATMBalanceEnquiryModel.CardNumber;
                requestModel1.RRN = microATMBalanceEnquiryModel.BankRRNNumber;
                requestModel1.Reason = microATMBalanceEnquiryModel.FailureReason;
                requestModel1.NPCITransactionID = microATMBalanceEnquiryModel.NPCITransactionId;
                requestModel1.RedmilTransactionID = microATMBalanceEnquiryModel.RedmilTransactionId;
                requestModel1.TransactionAmount = Convert.ToInt64(microATMBalanceEnquiryModel.TransactionAmount);
                requestModel1.DEName = microATMBalanceEnquiryModel.DEName;
                requestModel1.DEMobile = microATMBalanceEnquiryModel.DEMobile;

                requestModel1.ServiceType = ReplaceServices(subCat);

                json = JsonConvert.SerializeObject(requestModel1);
            }

            //else if (subCat == "9")
            //{
            //    requestModel1.Status = microATMCashWithdrawalModel.Status;
            //    requestModel1.RequestDate = microATMCashWithdrawalModel.RequestDate;
            //    requestModel1.RequestTime = microATMCashWithdrawalModel.RequestTime;
            //    requestModel1.Balamount = Convert.ToInt64(microATMCashWithdrawalModel.BalanceAmount);
            //    requestModel1.CardHolderName = microATMCashWithdrawalModel.CardHolderName;
            //    requestModel1.CardHolderContactNumber = microATMCashWithdrawalModel.CardHolderMobileNumber;
            //    requestModel1.CardType = microATMCashWithdrawalModel.CardType;
            //    requestModel1.CardNumber = microATMCashWithdrawalModel.CardNumber;
            //    requestModel1.RRN = microATMCashWithdrawalModel.BankRRNNumber;
            //    requestModel1.Reason = microATMCashWithdrawalModel.FailureReason;
            //    requestModel1.NPCITransactionID = microATMCashWithdrawalModel.NPCITransactionId;
            //    requestModel1.RedmilTransactionID = microATMCashWithdrawalModel.RedmilTransactionId;
            //    requestModel1.TransactionAmount = Convert.ToInt64(microATMCashWithdrawalModel.TransactionAmount);
            //    requestModel.DEName = microATMCashWithdrawalModel.DEName;
            //    requestModel.DEMobile = microATMCashWithdrawalModel.DEMobile;

            //    requestModel1.ServiceType = ReplaceServices(subCat);

            //    json = JsonConvert.SerializeObject(requestModel1);
            //}



            BillPaymentsModel1 billPaymentsModel1 = new BillPaymentsModel1();

            if (subCat == "1")
            {
                billPaymentsModel1.Status = billPaymentsModel.Status;
                billPaymentsModel1.RequestDate = billPaymentsModel.RequestDate;
                billPaymentsModel1.RequestTime = billPaymentsModel.RequestTime;
                billPaymentsModel1.CustomerMobile = billPaymentsModel.CustomerMobileNumber;
                billPaymentsModel1.BillCategory = billPaymentsModel.BillerCategory;
                billPaymentsModel1.BillerName = billPaymentsModel.BillerName;
                billPaymentsModel1.BillerAmount = billPaymentsModel.BillAmount;
                billPaymentsModel1.CustomerConvenienceFees = billPaymentsModel.CCF;
                billPaymentsModel1.CustomerInputField = billPaymentsModel.CustomerInputField;
                billPaymentsModel1.CustomerInputFieldValue = billPaymentsModel.CustomerInputFieldValue;
                billPaymentsModel1.BillerTransactionID = billPaymentsModel.BillerReferenceId;
                billPaymentsModel1.RedmilTransactionID = billPaymentsModel.RedmilTransactionId.ToString();
                billPaymentsModel1.DEMobile = billPaymentsModel.DEMobile;
                billPaymentsModel1.DEName = billPaymentsModel.DEName;
                billPaymentsModel1.ServiceType = ReplaceServices(subCat);
                json = JsonConvert.SerializeObject(billPaymentsModel1);
            }

            MobilePrepaidPostpaidDTHRechargeModel mobilePrepaidPostpaidModel = new MobilePrepaidPostpaidDTHRechargeModel();

            if (subCat == "3" || subCat == "4" || subCat == "2")
            {
                mobilePrepaidPostpaidModel.Status = result.mobilePrepaidPostpaid.Status;
                mobilePrepaidPostpaidModel.RequestDate = result.mobilePrepaidPostpaid.RequestDate;
                mobilePrepaidPostpaidModel.RequestTime = result.mobilePrepaidPostpaid.RequestTime;
                mobilePrepaidPostpaidModel.CustomerMobile = result.mobilePrepaidPostpaid.CustomerMobile;
                mobilePrepaidPostpaidModel.OperatorName = result.mobilePrepaidPostpaid.OperatorName;
                mobilePrepaidPostpaidModel.Amount = result.mobilePrepaidPostpaid.Amount;
                mobilePrepaidPostpaidModel.TransactionId = result.mobilePrepaidPostpaid.OperatorTransactionId;
                mobilePrepaidPostpaidModel.RedmilTransactionId = result.mobilePrepaidPostpaid.RedmilTransactionId.ToString();
                mobilePrepaidPostpaidModel.DEName = result.mobilePrepaidPostpaid.DEName;
                mobilePrepaidPostpaidModel.DEMobile = result.mobilePrepaidPostpaid.DEMobile;
                mobilePrepaidPostpaidModel.ServiceType = ReplaceServices(subCat);

                json = JsonConvert.SerializeObject(mobilePrepaidPostpaidModel);
            }

            LICPremiumPaymentsModel1 licPremiumPaymentsModel1 = new LICPremiumPaymentsModel1();

            if (subCat == "5")
            {
                licPremiumPaymentsModel1.Status = licPremiumPaymentsModel.Status;
                licPremiumPaymentsModel1.RequestDate = licPremiumPaymentsModel.RequestDate;
                licPremiumPaymentsModel1.RequestTime = licPremiumPaymentsModel.RequestTime;
                licPremiumPaymentsModel1.caNumber = licPremiumPaymentsModel.CaNumber;
                licPremiumPaymentsModel1.BillerAmount = licPremiumPaymentsModel.BillPayAmount.ToString();
                licPremiumPaymentsModel1.Status = licPremiumPaymentsModel.Status;
                licPremiumPaymentsModel1.BillerTransactionID = licPremiumPaymentsModel.BillReferenceId;
                licPremiumPaymentsModel1.RedmilTransactionID = licPremiumPaymentsModel.RedmilTransactionId;
                licPremiumPaymentsModel1.DEName = licPremiumPaymentsModel.DEName;
                licPremiumPaymentsModel1.DEMobile = licPremiumPaymentsModel.DEMobile;
                licPremiumPaymentsModel1.ServiceType = ReplaceServices(subCat);
                json = JsonConvert.SerializeObject(licPremiumPaymentsModel1);

            }

            //Amazon Pay Gift Cards
            if (subCat == "32")
            {
                ViewRecieptAmazonGiftCard requestViewRecieptAmazonGiftCardModel = new ViewRecieptAmazonGiftCard();
                requestViewRecieptAmazonGiftCardModel.RequestDate = result.amazonPayGiftCardModel.RequestDate;
                requestViewRecieptAmazonGiftCardModel.RequestTime = result.amazonPayGiftCardModel.RequestTime;
                requestViewRecieptAmazonGiftCardModel.SenderName = result.amazonPayGiftCardModel.SenderName;
                requestViewRecieptAmazonGiftCardModel.SenderMobile = result.amazonPayGiftCardModel.SenderMobile;
                requestViewRecieptAmazonGiftCardModel.DEMobile = result.amazonPayGiftCardModel.DEMobile;
                requestViewRecieptAmazonGiftCardModel.DEName = result.amazonPayGiftCardModel.DEName;
                requestViewRecieptAmazonGiftCardModel.CustomerName = result.amazonPayGiftCardModel.ReceiverName;
                requestViewRecieptAmazonGiftCardModel.customerEmail = result.amazonPayGiftCardModel.ReceiverEmailID;
                requestViewRecieptAmazonGiftCardModel.CustomerMobileNumber = result.amazonPayGiftCardModel.ReceiverMobileNumber;
                requestViewRecieptAmazonGiftCardModel.qty = result.amazonPayGiftCardModel.Quantity;
                requestViewRecieptAmazonGiftCardModel.RedmilTransId = result.amazonPayGiftCardModel.RedmilTransactionId;
                requestViewRecieptAmazonGiftCardModel.Amount = result.amazonPayGiftCardModel.TransactionAmount;
                requestViewRecieptAmazonGiftCardModel.tAmount = result.amazonPayGiftCardModel.TotalAmount;
                requestViewRecieptAmazonGiftCardModel.card = result.amazonPayGiftCardModel.CardNumber;
                requestViewRecieptAmazonGiftCardModel.cardpin = result.amazonPayGiftCardModel.CardPin;
                requestViewRecieptAmazonGiftCardModel.catname = result.amazonPayGiftCardModel.CardDesigncategoryName;
                requestViewRecieptAmazonGiftCardModel.Status = result.amazonPayGiftCardModel.Status;
                requestViewRecieptAmazonGiftCardModel.carddesign = result.amazonPayGiftCardModel.CardDesignname;
                requestViewRecieptAmazonGiftCardModel.refno = result.amazonPayGiftCardModel.AmazonReferenceNumber;
                requestViewRecieptAmazonGiftCardModel.GiftMessage = result.amazonPayGiftCardModel.GiftMessage;
                requestViewRecieptAmazonGiftCardModel.orderid = result.amazonPayGiftCardModel.OrderId;
                requestViewRecieptAmazonGiftCardModel.ResponseCode = result.amazonPayGiftCardModel.ResponseCode;
                requestViewRecieptAmazonGiftCardModel.ServiceType = ReplaceServices(subCat);
                json = JsonConvert.SerializeObject(requestViewRecieptAmazonGiftCardModel);
            }
            try
            {
                var client = new RestClient("https://api.redmilbusinessmall.com/api/transactions/receipt");

                //Create request with GET
                var requestResp = new RestRequest(Method.POST);
                requestResp.AddHeader("Content-Type", "application/json");

                //var json = JsonConvert.SerializeObject(reqModel);
                requestResp.AddJsonBody(json);

                IRestResponse response = client.Execute(requestResp);

                var result1 = response.Content;
                if (string.IsNullOrEmpty(result1))
                {
                    return Json("");
                }
                else
                {
                    var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
                    if (deserialize.Statuscode == "TXN" && deserialize != null)
                    {
                        //var datadeserialize = deserialize.Data;
                        var msgdeserialize = deserialize.Message;
                        //var data = JsonConvert.DeserializeObject<AePSCashWithdrawalModel1>(JsonConvert.SerializeObject(msgdeserialize));
                        //requestModel = data.ToList();
                        //return Json(requestModel);
                        return new(msgdeserialize);
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
               

            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                requestModelEx.ExceptionMessage = ex;
                requestModelEx.Data = json;
                var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                requestEx.AddJsonBody(json);
                IRestResponse responseEx = clientEx.Execute(requestEx);
                var resultEx = responseEx.Content;
            }
            return Json("");

        }
        #endregion

        #region Replace Status Cases
        private string ReplaceStatus(string statusCode)
        {
            switch (statusCode)
            {
                case "1": return "All";
                case "2": return "Failure";
                case "3": return "NA";
                case "4": return "Pending";
                case "5": return "Success";

                default: return "All";
            }
        }
        #endregion

        #region Replace Services Cases
        private string ReplaceServices(string service)
        {
            switch (service)
            {
                //Redmil Business Mall
                case "20": return "VirtualPaymentReports";
                case "22": return "CashOut";
                case "24": return "UPIWalletRecharge";

                // Banking Services
                case "15": return "AepsCashWithdrawal";
                case "16": return "AepsMiniStatement";
                case "17": return "AepsBalanceEnquiry";

                case "18": return "MicroATMCashWithdrawal";
                case "19": return "MicroATMBalanceEnquiry";
                case "21": return "MoneyTransfer";
                case "23": return "CreditScore";

                case "25": return "UPICashWithdrawal";
                case "29": return "MicroATMCashWithdrawalICICI";
                case "30": return "MicroATMBalanceEnquiryICICI";

                case "31": return "FinoDMT";
                case "34": return "CashCollection2.0";

                // Payment Services
                case "8": return "AadharPay";
                case "9": return "POS";
                case "10": return "SMSPayments";
                case "11": return "DynamicQR";
                case "12": return "StaticQR";
                case "13": return "Hisab Kitab";

                // Investment Services
                case "26": return "BuyGold";
                case "27": return "SellGold";

                // Banking Solution Services
                case "14": return "PAN";
                case "33": return "NSDL PAN";


                // Recharge and Bill Payments
                case "1": return "BBPS";
                case "2": return "DTHRecharge";
                case "3": return "MobilePrepaid";
                case "4": return "MobilePostpaid";
                case "5": return "LIC";

                // Travel Services
                case "6": return "Hotel";
                case "7": return "BusBooking";

                // Digital Distribution Services
                case "32": return "AmazonGiftCard";

                default: return "All";
            }
        }
        #endregion

        #region Get Only DMT 2.0 Data List
        [HttpGet]
        public IActionResult GetDMTDataList()
        {
            BusinessTranscationRequestReport requestModel = new BusinessTranscationRequestReport();
            try
            {
                requestModel.Status = "";
                requestModel.PageNumber = "1";

                requestModel.UserId = "2084";

                requestModel.SortBy = "desc";
                requestModel.ServiceType = "FinoDMT";


                requestModel.Report = "false";
                requestModel.FromDate = string.Empty;
                requestModel.ToDate = string.Empty;

                #region Checksum (report|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("report", Checksum.checksumKey,
            requestModel.UserId, requestModel.PageNumber, requestModel.SortBy, requestModel.ServiceType,
            requestModel.FromDate, requestModel.ToDate, requestModel.Report, requestModel.Status);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion

                CashOutModel transcationRequestReport = new CashOutModel();
                transcationRequestReport.baseUrl = Baseurl;
                requestModel.Checksum = "a2b9232f1290857d8aa9ebbef999e40e7fe48e5fda00666b0d0e0344fa5f459621af8ff3f873e90b3223c996c852490a523d2894975d93f07eb96bd2e62a1b4c";
                //requestModel.Checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/transactions/report");
                var client = new RestClient($"{Baseurl}{ApiName.TransactionReport}");

                //Create request with GET
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<BaseResponseModel2>(response.Content);
                if (deserialize.Statuscode == "TXN" && deserialize != null)
                {
                    var datadeserialize = deserialize.Data;
                    var msgdeserialize = deserialize.Message;

                    if (!string.IsNullOrEmpty(msgdeserialize) && !msgdeserialize.Equals("First Page"))
                    {
                        var docUrl = Baseurl + msgdeserialize;
                        return Json(docUrl);
                    }
                    var Data31 = JsonConvert.DeserializeObject<List<FinoDMTResponseModel.FinoDMTModel>>(JsonConvert.SerializeObject(datadeserialize));
                    //List<FinoDMTResponseModel> lst = new List<FinoDMTResponseModel>();
                    //lst = Data31.ToList();
                    return View(Data31);
                }
                else if (deserialize.Statuscode == "ERR")
                {
                    return View(deserialize);
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
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
            }
            return View();


        }
        #endregion

        #region Convert List To DataTable
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        #endregion

        #region PanCardRegistration

        public IActionResult PanCardRegistration()
        {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {
                return RedirectToAction("ErrorForLogin", "Error");
            }
            GetBalanceRequestModel getBalanceRequestModel = new GetBalanceRequestModel();
            PanCardRegistrationResponseModel responseModel = new PanCardRegistrationResponseModel();
            try
            {
                //getBalanceRequestModel.Userid = "636854";//Faisal Siddiqui ID
                getBalanceRequestModel.Userid = "2084";//Sagar Sir ID
                #region Checksum (GetBalance|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("Getbalance", Checksum.checksumKey, getBalanceRequestModel.Userid);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                getBalanceRequestModel.checksum = CheckSum;
                //API URL Has been changed by Siddhartha Sir
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/Getbalance");
                var client = new RestClient($"{Baseurl}{ApiName.Getbalance}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(getBalanceRequestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                if (deserialize.Statuscode == "TXN" && deserialize != null)
                {
                    var data = deserialize.Data;
                    List<GetBalanceResponseModel> lstdata = new List<GetBalanceResponseModel>();
                    lstdata = JsonConvert.DeserializeObject<List<GetBalanceResponseModel>>(JsonConvert.SerializeObject(data)).ToList();
                    ViewBag.Balance = lstdata.FirstOrDefault().MainBal;

                    PanCardRegistrationRequestModel requestModel = new PanCardRegistrationRequestModel();
                    try
                    {
                        requestModel.UserId = "2084";
                        #region Checksum (CheckUTIAgentStatus|Unique Key|UserId|ServiceId)

                        string inputN = Checksum.MakeChecksumString("CheckUTIAgentStatus", Checksum.checksumKey, requestModel.UserId);
                        string CheckSumN = Checksum.ConvertStringToSCH512Hash(inputN);

                        #endregion
                        requestModel.checksum = CheckSumN;
                        var clientN = new RestClient($"{Baseurl}{ApiName.CheckUTIAgentStatus}");
                        var requestN = new RestRequest(Method.POST);
                        requestN.AddHeader("Content-Type", "application/json");
                        var jsonN = JsonConvert.SerializeObject(requestModel);
                        requestN.AddJsonBody(jsonN);
                        IRestResponse responseN = clientN.Execute(requestN);
                        var resultN = responseN.Content;
                        var deserializeN = JsonConvert.DeserializeObject<BaseResponseModel>(responseN.Content);
                        if (deserializeN.Statuscode == "UNR")
                        {
                            return Json(deserializeN);
                        }
                        else
                        {
                            var dataN = deserializeN.Data;
                            var deserialize1 = JsonConvert.DeserializeObject<PanCardRegistrationResponseModel>(dataN.ToString());
                            return View(deserialize1);
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                        requestModel1.ExceptionMessage = ex;
                        requestModel1.Data = requestModel;
                        var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                        var requestEx = new RestRequest(Method.POST);
                        requestEx.AddHeader("Content-Type", "application/json");
                        var jsonEx = JsonConvert.SerializeObject(requestModel1);
                        request.AddJsonBody(jsonEx);
                        IRestResponse responseEx = client.Execute(requestEx);
                        var resultEx = responseEx.Content;
                    }
                }
                else if (deserialize.Statuscode == "ERR")
                {
                    return View(deserialize);
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

        #region PSARegistration

        public JsonResult PSARegistration()
        {
            PanCardRegistrationRequestModel requestModel = new PanCardRegistrationRequestModel();
            try
            {
                requestModel.UserId = "2084";
                #region Checksum (CheckUTIAgentStatus|Unique Key|UserId|ServiceId)
                string inputN = Checksum.MakeChecksumString("CheckUTIAgentStatus", Checksum.checksumKey, requestModel.UserId);
                string CheckSumN = Checksum.ConvertStringToSCH512Hash(inputN);
                #endregion
                requestModel.checksum = CheckSumN;
                var clientN = new RestClient($"{Baseurl}{ApiName.CheckUTIAgentStatus}");
                var requestN = new RestRequest(Method.POST);
                requestN.AddHeader("Content-Type", "application/json");
                var jsonN = JsonConvert.SerializeObject(requestModel);
                requestN.AddJsonBody(jsonN);
                IRestResponse responseN = clientN.Execute(requestN);
                var resultN = responseN.Content;
                var deserializeN = JsonConvert.DeserializeObject<BaseResponseModel>(responseN.Content);
                if (deserializeN.Statuscode == "TXN")
                {
                    var dataN = deserializeN.Data;
                    var deserialize1 = JsonConvert.DeserializeObject<PanCardRegistrationResponseModel>(dataN.ToString());
                    return Json(deserialize1);
                }
                else if (deserializeN.Statuscode == "ERR")
                {
                    return Json(deserializeN);
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

        #region TravelServices
        public IActionResult TravelServices(string subCat, string bookingID, string status)
        {
            BusinessTranscationRequestReport requestModel = new BusinessTranscationRequestReport();
            try
            {
                requestModel.Status = "";
                requestModel.PageNumber = "1";
                requestModel.UserId = "2084";
                requestModel.SortBy = "desc";
                requestModel.ServiceType = "BusBooking";
                requestModel.Report = "false";
                requestModel.FromDate = string.Empty;
                requestModel.ToDate = string.Empty;

                #region Checksum (report|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("report", Checksum.checksumKey,
            requestModel.UserId, requestModel.PageNumber, requestModel.SortBy, requestModel.ServiceType,
            requestModel.FromDate, requestModel.ToDate, requestModel.Report, requestModel.Status);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion

                CashOutModel transcationRequestReport = new CashOutModel();
                transcationRequestReport.baseUrl = Baseurl;
                requestModel.Checksum = "a2b9232f1290857d8aa9ebbef999e40e7fe48e5fda00666b0d0e0344fa5f459621af8ff3f873e90b3223c996c852490a523d2894975d93f07eb96bd2e62a1b4c";
                //requestModel.Checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/transactions/report");
                var client = new RestClient($"{Baseurl}{ApiName.TransactionReport}");

                //Create request with GET
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<BaseResponseModel2>(response.Content);
                if (deserialize != null && deserialize.Statuscode == "TXN")
                {
                    var datadeserialize = deserialize.Data;
                    var msgdeserialize = deserialize.Message;

                    if (!string.IsNullOrEmpty(msgdeserialize) && !msgdeserialize.Equals("First Page"))
                    {
                        var docUrl = Baseurl + msgdeserialize;
                        return Json(docUrl);
                    }
                    var Data06 = JsonConvert.DeserializeObject<List<BusBookingModel>>(JsonConvert.SerializeObject(datadeserialize));
                    if (!string.IsNullOrEmpty(bookingID))
                    {
                        var DataNew = Data06.Where(x => x.BookingId == bookingID).ToList();
                        return Json(DataNew);
                    }

                    return View(Data06);
                }
                else if (deserialize.Statuscode == "ERR")
                {
                    return Json("");
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

        #region Cancel Bus Tickets
        public JsonResult CancelBusTickets(string tin, string seat)
        {
            CancelBusTicketsRequestModel request = new CancelBusTicketsRequestModel();
            try
            {
                request.Userid = "2084";
                request.tin = tin;
                request.seatsToCancel = seat;
                #region Checksum (SeatSellerCancelBooking|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("SeatSellerCancelBooking", Checksum.checksumKey,
           request.Userid, request.tin);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                request.checksum = CheckSum;
                //var clientN = new RestClient($"{Baseurl}{ApiName.SeatSellerCancelBooking}");
                var clientN = new RestClient("https://proapitest5.redmilbusinessmall.com/api/SeatSellerCancelBooking");
                var requestN = new RestRequest(Method.POST);
                requestN.AddHeader("Content-Type", "application/json");
                var jsonN = JsonConvert.SerializeObject(request);
                requestN.AddJsonBody(jsonN);
                IRestResponse responseN = clientN.Execute(requestN);
                var resultN = responseN.Content;
                var deserializeN = JsonConvert.DeserializeObject<BaseResponseModel>(responseN.Content);
                if (deserializeN.Statuscode == "TXN")
                {
                    var data = deserializeN.Data;
                    var deserializeData = JsonConvert.DeserializeObject<CancelBusTicketsResponseModel>(data.ToString());
                    return Json(deserializeData);
                }
                else if (deserializeN.Statuscode == "ERR")
                {
                    return Json(deserializeN);
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
                requestModel1.Data = request;
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
    }
}