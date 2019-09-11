
using Ctrip.Config;
using Ctrip.Request;
using Ctrip.Response;
using Ctrip.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlyPig.Order.Framework.HttpWeb;
using FlyPig.Order.Framework.Logging;
using Ctrip.Common;

namespace Ctrip
{
    public class CtripApiClient
    {
        public CtripApiClient(ApiConfig config)
        {
            Config = config;
        }

        public ApiConfig Config { get; set; }

        /// <summary>
        /// 订单验证
        /// </summary>
        /// <param name="hotelCode"></param>
        /// <param name="ratePlanCategory"></param>
        /// <param name="ratePlanCode"></param>
        /// <param name="roomNum"></param>
        /// <param name="guestCount"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="lastArriveTime">HH:mm</param>
        public HotelAvailReponse CheckOrderAvail(string hotelCode, string ratePlanCategory, string ratePlanCode, int roomNum, int guestCount, DateTime start, DateTime end, string lastArriveTime)
        {
            HotelAvailRequest req = new HotelAvailRequest();
            AvailRequestSegment s = new AvailRequestSegment();
            Criterion_Avail c = new Criterion_Avail();
            c.HotelCode = hotelCode;
            c.RatePlanCandidates.RatePlanCandidateList.Add(new RatePlanCandidate_Avail() { RatePlanCategory = ratePlanCategory, RatePlanCode = ratePlanCode.Replace("_c","") });
            RoomStayCandidate r = new RoomStayCandidate();
            r.Quantity = roomNum;
            r.GuestCounts.GuestCountList.Add(new GuestCount() { Count = guestCount });
            c.RoomStayCandidates.RoomStayCandidateList.Add(r);
            c.StartDate = start.ToString("yyyy-MM-dd");
            c.EndDate = end.ToString("yyyy-MM-dd");

            //入住人最晚到店时间，有可能最晚到店日期为第二天凌晨，格式为 yyyy-MM-dd hh:mm:ss
            DateTime arrTime = new DateTime();
            string temp = start.Date.ToString("yyyy-MM-dd") + " " + lastArriveTime;

            if (DateTime.TryParse(temp, out arrTime))
            {
                if (arrTime > DateTime.Now)
                {
                    c.TPA_Extensions.LateArrivalTime = arrTime.Hour < 6 ? (start.AddDays(1).ToString("yyyy-MM-dd") + "T" + lastArriveTime + ":00.000+08:00") : (start.ToString("yyyy-MM-dd") + "T" + lastArriveTime + ":00.000+08:00");
                }
                else
                {
                    if (DateTime.Now.Hour > 23 && DateTime.Now.Minute > 30)
                    {
                        c.TPA_Extensions.LateArrivalTime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + "T03:00:00.000+08:00";
                    }
                    else if (DateTime.Now.Date > start.Date)
                    {
                        c.TPA_Extensions.LateArrivalTime = DateTime.Now.ToString("yyyy-MM-dd") + "T03:30:00.000+08:00";
                    }
                    else
                    {
                        c.TPA_Extensions.LateArrivalTime = DateTime.Now.ToString("yyyy-MM-dd") + "T23:50:00.000+08:00";
                    }
                }
            }
            else if (start.Date == DateTime.Now.Date)
            {
                c.TPA_Extensions.LateArrivalTime = DateTime.Now.Hour < 6 ? (DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + "T" + "03:00:00.000+08:00") : (DateTime.Now.ToString("yyyy-MM-dd") + "T" + "23:50:00.000+08:00");
            }
            else
            {
                c.TPA_Extensions.LateArrivalTime = start.ToString("yyyy-MM-ddT23:50:00.000+08:00");
            }
            s.HotelSearchCriteria.CriterionList.Add(c);
            req.OTA_HotelAvailRQ.AvailRequestSegments.AvailRequestSegmentList.Add(s);


            var res = ExecutePost(req);

            return res;
        }



        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public HotelOrderMiniInfo GetMiniOrderInfo(string orderId)
        {
            var request = new D_HotelOrderMiniInfoRequest(Config);
            request.HotelOrderMiniInfoRequest.OrderId = orderId;
            var response = ExecutePost(request);
            return response.MiniInfo;
        }


        /// <summary>
        /// 获取携程订单ID
        /// </summary>
        /// <param name="distributorOrderID"></param>
        /// <returns></returns>
        public DomestictCtripOrderIDInfo GetCtripOrderId(string distributorOrderID)
        {
            D_GetCtripOrderIDRequest request = new D_GetCtripOrderIDRequest();
            request.DomesticGetCtripOrderIDRequest = new DomesticGetCtripOrderIDRequest
            {
                DistributorOrderIDList = new DistributorOrderIDList
                {
                    DistributorOrderIDDetails = new List<DistributorOrderIDDetail> {
                         new DistributorOrderIDDetail{  DistributorOrderID=distributorOrderID}
        }
                }
            };

            var response = ExecutePost(request);
            return response.OrderMapInfos.FirstOrDefault();
        }



        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="ctripOrderId"></param>
        /// <returns></returns>
        public Ctrip.Entities.HotelReservation GetOrderDetail(string ctripOrderId)
        {

            OrderRequest req = new OrderRequest(Config);
            //req.OTA_ReadRQ.UniqueIDList.Add(new UniqueID() { Type = "504", ID = ctripOrderId });
            req.OTA_ReadRQ.UniqueIDList.Add(new UniqueID() { Type = "501", ID = ctripOrderId });
            var res = ExecutePost(req);

            return res.OrderList.Count > 0 ? res.OrderList[0] : null;
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="ctripOrderId"></param>
        /// <returns></returns>
        public Ctrip.Entities.HotelReservation GetOrderDetailByDistributor(string distributorOrderId)
        {
            OrderRequest req = new OrderRequest(Config);
            req.OTA_ReadRQ.UniqueIDList.Add(new UniqueID() { Type = "504", ID = distributorOrderId });
            var res = ExecutePost(req);
            return res.OrderList.Count > 0 ? res.OrderList[0] : null;
        }



        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="corder"></param>
        /// <returns></returns>
        public ZXcResponse SubmitOrderV(XZorder corder)
        {
            ZXcResponse resp = new ZXcResponse();
            LogWriter logWriter = new LogWriter("Ctrip/CreateOrder");
            try
            {
                Guid gid = Guid.NewGuid();
                if (string.IsNullOrEmpty(corder.ratePlanCategory))
                {
                    corder.ratePlanCategory = "501";
                }
                string ratePlanCategory = corder.ratePlanCategory;

                HotelResSaveRequest req = new HotelResSaveRequest(Config);
                //默认是国际的账号，
                req.OTA_HotelResSaveRQ.UniqueIDList.Find(c => c.Type == "28").ID = Config.AllianceID;
                req.OTA_HotelResSaveRQ.UniqueIDList.Find(c => c.Type == "503").ID = Config.SID;
                req.OTA_HotelResSaveRQ.UniqueIDList.Find(c => c.Type == "1").ID = Config.UID;
                req.OTA_HotelResSaveRQ.UniqueIDList.Find(c => c.Type == "504").ID = corder.orderNo;

                Ctrip.Request.HotelReservation info = new Ctrip.Request.HotelReservation();
                RoomStay room = new RoomStay();
                room.HotelCode = corder.hotelCode;
                room.RoomTypes.NumberOfUnits = corder.roomNum;

                room.RatePlans.RatePlanCode = corder.ratePlanCode;
                room.RatePlans.RatePlanCategory = ratePlanCategory;
                info.RoomStays.RoomStayList.Add(room);
                ResGuest guest = new ResGuest();
                foreach (var item in corder.guestNames)
                {
                    PersonName name = new PersonName() { Surname = item };
                    guest.Profiles.ProfileInfo.Profile.Customer.PersonNameList.Add(name);
                }
                ContactPerson per = new ContactPerson();
                per.PersonName = new PersonName() { Surname = corder.contactName };
                per.Email = "yf@xiwantrip.com";
                per.PhoneNumber = "13250771589";
                per.ContactType = "email";
                guest.Profiles.ProfileInfo.Profile.Customer.ContactPerson = per;
                if (!string.IsNullOrEmpty(corder.lateArriveTime))
                {
                    string[] arr = corder.lateArriveTime.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length == 2)
                    {
                        int hour = 0;
                        int.TryParse(arr[0], out hour);
                        if (hour < 6)
                        {
                            guest.TPA_Extensions.LateArrivalTime = corder.comeDate.Date.AddDays(1).ToString("yyyy-MM-dd") + "T" + corder.lateArriveTime + ":00+08:00";
                        }
                        else
                        {
                            guest.TPA_Extensions.LateArrivalTime = corder.comeDate.Date.ToString("yyyy-MM-dd") + "T" + corder.lateArriveTime + ":00+08:00";
                        }
                    }
                }
                if (string.IsNullOrEmpty(guest.TPA_Extensions.LateArrivalTime))
                {
                    if (corder.comeDate.Date.Date < DateTime.Now.Date && DateTime.Now.Hour < 6)
                    {
                        guest.TPA_Extensions.LateArrivalTime = DateTime.Now.AddMinutes(30).ToString("yyyy-MM-ddTHH:mm:00+08:00");
                    }
                    else
                    {
                        guest.TPA_Extensions.LateArrivalTime = corder.comeDate.Date.ToString("yyyy-MM-dd") + "T23:30:00+08:00";
                    }
                }
                if (Convert.ToDateTime(guest.TPA_Extensions.LateArrivalTime) < DateTime.Now)
                {
                    if (DateTime.Now.Hour < 6)
                    {
                        guest.TPA_Extensions.LateArrivalTime = DateTime.Now.ToString("yyyy-MM-ddT03:00:00+08:00");
                    }
                    else
                    {
                        guest.TPA_Extensions.LateArrivalTime = DateTime.Now.ToString("yyyy-MM-ddT23:30:00+08:00");
                    }
                }
                info.ResGuests.ResGuestList.Add(guest);
                info.ResGlobalInfo.GuestCounts.GuestCountList.Add(new Ctrip.Request.GuestCount() { Count = corder.guestNames.Count });
                info.ResGlobalInfo.StartDate = corder.comeDate.Date.ToString("yyyy-MM-ddTHH:mm:ss+08:00");
                info.ResGlobalInfo.EndDate = corder.leaveDate.Date.ToString("yyyy-MM-ddTHH:mm:ss+08:00");
                if (!string.IsNullOrEmpty(corder.specialRequest))
                {
                    info.ResGlobalInfo.SpecialRequests.SpecialRequest.Text = corder.specialRequest;
                }

                info.ResGlobalInfo.Total.AmountBeforeTax = corder.salePrice;
                //TODO   结算价模式
                info.ResGlobalInfo.TPA_Extensions.AmountBeforeTax = corder.totalPrice;
                //info.ResGlobalInfo.Total.AmountBeforeTax = corder.price;//指导价
                //info.ResGlobalInfo.Total.AmountBeforeTax = corder.price;//卖价
                //info.ResGlobalInfo.TPA_Extensions.TotalCost.AmountBeforeTax = corder.basePrice;//底价
                req.OTA_HotelResSaveRQ.HotelReservations.HotelReservationList.Add(info);


                var ctripRequest = new CtripRequest<HotelResSaveResponse>(req, Config);

                string xml = string.Empty;
                xml += "<Request>";
                string head = RequestXmlWrapper.ToXml(ctripRequest.Header);
                string body = "<HotelRequest><RequestBody xmlns:ns=\"http://www.opentravel.org/OTA/2003/05\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">";
                string content = RequestXmlWrapper.ToXml(ctripRequest.RequestBody);
                if (ctripRequest.RequestBody.GetType() != typeof(HotelDescriptiveRequest))
                {
                    content = content.Replace("<", "<ns:").Replace("<ns:/", "</ns:");
                }
                content = content.Replace("\"True\"", "\"true\"").Replace("\"False\"", "\"false\"");
                body += content;
                body += "</RequestBody></HotelRequest>";
                xml += (head + body);
                xml += "</Request>";

                logWriter.WriteOrder(corder.orderNo, $"请求参数：{xml}");

                //CtripOrderSubmit.OTA_HotelRes request = new CtripOrderSubmit.OTA_HotelRes();
                OTA_HotelResSave request = new OTA_HotelResSave();

                string response = request.Request(xml);
                logWriter.WriteOrder(corder.orderNo, $"请求参数：{response}");
                //if (WriteLog)
                //{
                //    Log.TFileLogManage.GetLog("xc_order_req").Debug(gid.ToString() + "::req::" + xml);
                //    Log.TFileLogManage.GetLog("xc_order_req").Debug(gid.ToString() + "::resp::" + response);
                //}

                response = response.Replace("xmlns=\"http://www.opentravel.org/OTA/2003/05\"", "");
                HotelResSaveResponse res = new HotelResSaveResponse();
                res.ResponseContent = response;
                res.ResponseXml = new System.Xml.XmlDocument();
                res.ResponseXml.LoadXml(response);
                res.ParseXML2();
                if (res.Error != null)
                {
                    resp.code = "-1";
                    resp.errMsg = res.Error.Type + ":" + res.Error.Code + ":" + res.Error.Message;
                }
                if (res != null && !string.IsNullOrEmpty(res.CtripOrderId))
                {
                    resp.code = "0";
                    resp.result = new ZXcResponseResult();
                    resp.result.orderId = res.CtripOrderId;
                }
            }
            catch (Exception ex)
            {
                resp.code = "-9";
                resp.errMsg = "ExecutePost_Exception" + ":" + ex.Message;
            }
            return resp;
        }

        /// <summary>
        /// 支付订单
        /// </summary>
        /// <param name="ctripOrderId"></param>
        /// <param name="localOrderNo"></param>
        /// <param name="payment"></param>
        /// <param name="msg"></param>
        /// <param name="currencyCode"></param>
        /// <returns></returns>
        public bool SubmitOrderPayment(string ctripOrderId, string localOrderNo, decimal payment, decimal salePrice, out string msg, string currencyCode = "CNY")
        {
            msg = string.Empty;
            bool suc = false;
            HotelResSubmitRequest req = new HotelResSubmitRequest();
            req.OTA_HotelResSubmitRQ.TransactionIdentifier = localOrderNo;
            req.OTA_HotelResSubmitRQ.ReservationPayment.ReservationID_ID = ctripOrderId;
            req.OTA_HotelResSubmitRQ.ReservationPayment.PaymentDetail.Amount = payment.ToString("f2");
            req.OTA_HotelResSubmitRQ.ReservationPayment.Invoice.Amount = salePrice;

            var res = ExecutePost(req);
            if (res.CtripOrderId == ctripOrderId)
            {
                suc = true;
            }
            if (res.Error != null)
            {
                msg = string.Format("{0}:{1}:{2}", res.Error.Type, res.Error.Code, res.Error.Message);
            }
            if (!string.IsNullOrEmpty(res.Warning))
            {
                msg += (" 提示:" + res.Warning);
            }
            return suc;
        }


        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="ctripOrderId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CancelOrder(string ctripOrderId, ref string msg)
        {
            CancelRequest req = new CancelRequest(Config);
            req.OTA_Cancel.UniqueIDList.Add(new UniqueID() { Type = "501", ID = ctripOrderId });
            CancelResponse res = ExecutePost(req);
            if (res.Error != null)
            {
                msg = res.Error.Type + ":" + res.Error.Code + ":" + res.Error.Message;
            }
            return res.IsSuccess;
        }


        private TResponse ExecutePost<TResponse>(IBaseRequest<TResponse> req)
            where TResponse : BaseResponse, new()
        {
            string path = string.Format("Ctrip/{0}/", req.Method);
            LogWriter logWriter = new LogWriter(path);
            string content = string.Empty;
            TResponse res = new TResponse();
            Guid gid = Guid.NewGuid();

            var request = new RequestModel(HttpVerbs.POST);
            request.ContentType = "text/xml";
            request.Url = ApiConfigManager.BaseUrl + req.Url;
            try
            {
                if (req != null)
                {
                    var ctripRequest = new CtripRequest<TResponse>(req, Config);


                    string xml = ToXMLString(ctripRequest);
                    string method = req.Method;

                    if (method == "OTA_Cancel" || method == "OTA_HotelResSubmit" || method == "OTA_HotelRes")
                    {
                        logWriter.Write("请求XML:\n" + xml);
                    }
                    if (!string.IsNullOrEmpty(xml))
                    {
                        request.Data = xml;
                        content = HttpUtility.PostHtml(request);
                        res.ResponseContent = content;
                        res.ResponseXml = new System.Xml.XmlDocument();
                        res.ResponseXml.LoadXml(content);
                        res.ParseXml();

                        //if (new List<string> { "SubmitOrder", "SubmitOrderPayment", "CheckOrderAvail" }.Contains(req.Method))
                        //{
                        //    logWriter.Write("请求XML:\n" + content);
                        //}

                        if (res.Error != null)
                        {
                            string msg = string.Format("hk_{0}", req.Url) + "responseXML:\n" + content + "\n--->>> " + res.Error.Code + ":" + res.Error.Message;
                            if (!string.IsNullOrEmpty(res.Error.Message) && res.Error.Message.Contains("RateLimit"))
                            {


                                if (method == "OTA_Cancel" || method == "OTA_HotelResSubmit" || method == "OTA_HotelRes")
                                {
                                    logWriter.Write("响应XML:\n" + content);
                                }

                                //if (ExeRetry)
                                //{
                                //    if (zcount <= 3)
                                //    {
                                //        zcount++;
                                //        System.Threading.Thread.Sleep(zcount * 500);
                                //        Log.TFileLogManage.GetLog("xc_response_Retry").Error(zcount.ToString() + ":::" + gid.ToString());
                                //        goto zRetry;
                                //    }
                                //}
                            }
                        }
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                res.Error = new Error();
                res.Error.Code = "ExecutePost_Exception";
                res.Error.Message = ex.Message;

                //log.WriteLog(ex, method);
                if (!string.IsNullOrEmpty(content))
                {
                    //log.WriteLog(content, method);
                }
                return res;
            }
        }

        private TResponse ExecutePostCheck<TResponse>(IBaseRequest<TResponse> req)
            where TResponse : BaseResponse, new()
        {
            string path = string.Format("Ctrip/{0}/", req.Method);
            LogWriter logWriter = new LogWriter(path);
            string content = string.Empty;
            TResponse res = new TResponse();
            Guid gid = Guid.NewGuid();

            var request = new RequestModel(HttpVerbs.POST);
            request.ContentType = "text/xml";
            request.Url = ApiConfigManager.BaseUrl + req.Url;
            try
            {
                if (req != null)
                {
                    var ctripRequest = new CtripRequest<TResponse>(req, Config);


                    string xml = ToXMLString(ctripRequest);
                    string method = req.Method;

                    if (method == "OTA_Cancel" || method == "OTA_HotelResSubmit" || method == "OTA_HotelRes")
                    {
                        logWriter.Write("请求XML:\n" + xml);
                    }
                    if (!string.IsNullOrEmpty(xml))
                    {
                        request.Data = xml;
                        content = HttpUtility.PostHtmlCheck(request);
                        res.ResponseContent = content;
                        res.ResponseXml = new System.Xml.XmlDocument();
                        res.ResponseXml.LoadXml(content);
                        res.ParseXml();

                        //if (new List<string> { "SubmitOrder", "SubmitOrderPayment", "CheckOrderAvail" }.Contains(req.Method))
                        //{
                        //    logWriter.Write("请求XML:\n" + content);
                        //}

                        if (res.Error != null)
                        {
                            string msg = string.Format("hk_{0}", req.Url) + "responseXML:\n" + content + "\n--->>> " + res.Error.Code + ":" + res.Error.Message;
                            if (!string.IsNullOrEmpty(res.Error.Message) && res.Error.Message.Contains("RateLimit"))
                            {


                                if (method == "OTA_Cancel" || method == "OTA_HotelResSubmit" || method == "OTA_HotelRes")
                                {
                                    logWriter.Write("响应XML:\n" + content);
                                }

                                //if (ExeRetry)
                                //{
                                //    if (zcount <= 3)
                                //    {
                                //        zcount++;
                                //        System.Threading.Thread.Sleep(zcount * 500);
                                //        Log.TFileLogManage.GetLog("xc_response_Retry").Error(zcount.ToString() + ":::" + gid.ToString());
                                //        goto zRetry;
                                //    }
                                //}
                            }
                        }
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                res.Error = new Error();
                res.Error.Code = "ExecutePost_Exception";
                res.Error.Message = ex.Message;

                //log.WriteLog(ex, method);
                if (!string.IsNullOrEmpty(content))
                {
                    //log.WriteLog(content, method);
                }
                return res;
            }
        }

        private string ToXMLString<TResponse>(CtripRequest<TResponse> request)
        where TResponse : BaseResponse
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            xml += "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body><Request xmlns=\"http://ctrip.com/\"><requestXML>";
            xml += "<![CDATA[<Request>";
            string head = RequestXmlWrapper.ToXml(request.Header);
            string body = "<HotelRequest><RequestBody xmlns:ns=\"http://www.opentravel.org/OTA/2003/05\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">";
            string content = RequestXmlWrapper.ToXml(request.RequestBody);
            if (request.RequestBody.GetType() != typeof(HotelDescriptiveRequest) && request.RequestBody.GetType() != typeof(D_HotelOrderMiniInfoRequest) && request.RequestBody.GetType() != typeof(D_GetCtripOrderIDRequest))
            {
                content = content.Replace("<", "<ns:").Replace("<ns:/", "</ns:");
            }
            content = content.Replace("\"True\"", "\"true\"").Replace("\"False\"", "\"false\"");
            body += content;
            body += "</RequestBody></HotelRequest>";
            if (request.RequestBody.GetType() == typeof(D_HotelOrderMiniInfoRequest) || request.RequestBody.GetType() == typeof(D_GetCtripOrderIDRequest))
            {
                body = content;
            }
            xml += (head + body);
            xml += "</Request>]]></requestXML></Request></soap:Body></soap:Envelope>";
            return xml;
        }

    }
}
