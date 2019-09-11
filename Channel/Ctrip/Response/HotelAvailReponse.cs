using Ctrip.Common;
using Ctrip.Entities;
using CtripOld.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrip.Response
{
    [Serializable]
    public class HotelAvailReponse : BaseResponse
    {

        public HeaderInfo Header { get; set; }
        public string AvailabilityStatus { get; set; }
        public CancelPenalty CancelRule { get; set; }
        public decimal AmountBeforeTax { get; set; }
        public string CurrencyCode { get; set; }
        public VxRoomStay RoomStay { get; set; }


        internal override void ParseXml()
        {
            if (base.ResponseXml != null)
            {
                bool suc = true;
                string result = string.Empty;
                XmlParse.SelectSingleNode(base.ResponseXml, "/", (c) =>
                {
                    result = c.InnerText;
                });
                if (string.IsNullOrEmpty(result))
                {
                    suc = false;
                }
                if (suc)
                {
                    base.ResultXML = new System.Xml.XmlDataDocument();
                    result = result.Replace("xmlns=\"http://www.opentravel.org/OTA/2003/05\"", "");
                    base.ResultXML.LoadXml(result);
                    //System.Xml.XmlNamespaceManager mgr = new System.Xml.XmlNamespaceManager(base.ResultXML.NameTable);
                    //mgr.AddNamespace("ns", "http://www.opentravel.org/OTA/2003/05");

                    XmlParse.SelectSingleNode(base.ResultXML, "/Response/Header", (c) =>
                    {
                        this.Header = new HeaderInfo();
                        this.Header.ShouldRecordPerformanceTime = XmlParse.GetAttribute(c, "ShouldRecordPerformanceTime").ToBoolean();
                        string t = XmlParse.GetAttribute(c, "Timestamp");
                        if (t.Count((ch) => ch == ':') == 3)
                        {
                            int i = t.LastIndexOf(':');
                            t = t.Remove(i) + "." + t.Substring(i + 1);
                        }
                        this.Header.Timestamp = t.ToDateTime();
                        this.Header.ReferenceID = XmlParse.GetAttribute(c, "ReferenceID");
                        this.Header.RecentlyTime = XmlParse.GetAttribute(c, "RecentlyTime").ToDateTime();
                        this.Header.AccessCount = XmlParse.GetAttribute(c, "AccessCount");
                        this.Header.CurrentCount = XmlParse.GetAttribute(c, "CurrentCount");
                        this.Header.ResetTime = XmlParse.GetAttribute(c, "ResetTime").ToDateTime();
                        this.Header.ResultCode = XmlParse.GetAttribute(c, "ResultCode");
                        if (this.Header.ResultCode == "Fail")
                        {
                            this.Error = new Error();
                            this.Error.Code = this.Header.ResultCode;
                            this.Error.Type = XmlParse.GetAttribute(c, "ResultNo");
                            this.Error.Message = XmlParse.GetAttribute(c, "ResultMsg");
                        }
                    });
                    XmlParse.SelectSingleNode(base.ResultXML, "/Response/HotelResponse/OTA_HotelAvailRS/Errors/Error", (c) =>
                    {
                        suc = false;
                        base.Error = new Error();
                        base.Error.Type = XmlParse.GetAttribute(c, "Type");
                        base.Error.Code = XmlParse.GetAttribute(c, "Code");
                        base.Error.Message = c.InnerText;
                    });

                    //if (suc)
                    {
                        XmlParse.SelectSingleNode(base.ResultXML, "/Response/HotelResponse/OTA_HotelAvailRS/RoomStays/RoomStay", node =>
                        {
                            this.AvailabilityStatus = XmlParse.GetAttribute(node, "AvailabilityStatus");
                            RoomStay = new VxRoomStay();

                            XmlParse.SelectNodes(node, "RoomTypes/RoomType", (r_x) =>
                            {
                                if (RoomStay.RoomTypes == null) RoomStay.RoomTypes = new List<VxRoomType>();
                                foreach (System.Xml.XmlNode tpx in r_x)
                                {
                                    VxRoomType ve = new VxRoomType();
                                    ve.RoomType = XmlParse.GetAttribute(tpx, "RoomType"); ;
                                    ve.RoomTypeCode = XmlParse.GetAttribute(tpx, "RoomTypeCode");
                                    RoomStay.RoomTypes.Add(ve);
                                }
                            });

                            XmlParse.SelectNodes(node, "RatePlans/RatePlan", (r_x) =>
                            {
                                if (RoomStay.RatePlans == null) RoomStay.RatePlans = new List<VxRatePlan>();
                                foreach (System.Xml.XmlNode tpx in r_x)
                                {
                                    VxRatePlan ve = new VxRatePlan();
                                    ve.RatePlanCode = XmlParse.GetAttribute(tpx, "RatePlanCode");
                                    ve.RatePlanName = XmlParse.GetAttribute(tpx, "RatePlanName");
                                    ve.AvailableQuantity = Convert.ToInt32(XmlParse.GetAttribute(tpx, "AvailableQuantity"));
                                    ve.PrepaidIndicator = Convert.ToBoolean(XmlParse.GetAttribute(tpx, "PrepaidIndicator"));
                                    ve.IsInstantConfirm = Convert.ToBoolean(XmlParse.GetAttribute(tpx, "IsInstantConfirm"));
                                    ve.InvoiceTargetType = Convert.ToInt32(XmlParse.GetAttribute(tpx, "InvoiceTargetType"));
                                    ve.SupplierID = Convert.ToInt32(XmlParse.GetAttribute(tpx, "SupplierID"));
                                    XmlParse.SelectSingleNode(tpx, "MealsIncluded ", (bk) =>
                                    {
                                        ve.Breakfast = Convert.ToBoolean(XmlParse.GetAttribute(bk, "Breakfast"));
                                    });
                                    RoomStay.RatePlans.Add(ve);
                                }
                            });

                            XmlParse.SelectNodes(node, "RoomRates/RoomRate", (r_x) =>
                            {
                                if (RoomStay.RoomRates == null) RoomStay.RoomRates = new List<VxRoomRate>();
                                foreach (System.Xml.XmlNode tpx in r_x)
                                {
                                    VxRoomRate ve = new VxRoomRate();
                                    ve.RoomTypeCode = XmlParse.GetAttribute(tpx, "RoomTypeCode");
                                    ve.RatePlanCode = XmlParse.GetAttribute(tpx, "RatePlanCode");
                                    XmlParse.SelectNodes(tpx, "Rates/Rate", (_prate) =>
                                    {
                                        if (ve.Rates == null) ve.Rates = new List<VxRate>();
                                        foreach (System.Xml.XmlNode xra in _prate)
                                        {
                                            VxRate _rm = new VxRate();
                                            _rm.EffectiveDate = XmlParse.GetAttribute(xra, "EffectiveDate");
                                            _rm.ExpireDate = XmlParse.GetAttribute(xra, "ExpireDate");
                                            _rm.MaxGuestApplicable = Convert.ToInt32(XmlParse.GetAttribute(xra, "MaxGuestApplicable"));
                                            XmlParse.SelectSingleNode(xra, "MealsIncluded", (prx) =>
                                            {
                                                _rm.NumberOfBreakfast = Convert.ToInt32(XmlParse.GetAttribute(prx, "NumberOfBreakfast"));
                                            });
                                            XmlParse.SelectSingleNode(xra, "Base", (prx) =>
                                            {
                                                _rm.AmountBeforeTax = Convert.ToDecimal(XmlParse.GetAttribute(prx, "AmountBeforeTax"));
                                                _rm.CurrencyCode = XmlParse.GetAttribute(prx, "CurrencyCode");
                                                if (_rm.CurrencyCode != "CNY")
                                                {
                                                    XmlParse.SelectSingleNode(prx, "TPA_Extensions/OtherCurrency/AmountPercentType", other =>
                                                    {
                                                        decimal price = XmlParse.GetAttribute(other, "Amount").ToDecimal();
                                                        string code = XmlParse.GetAttribute(other, "CurrencyCode").ToString();
                                                        if (code == "CNY")
                                                        {
                                                            _rm.AmountBeforeTax = price;
                                                            _rm.CurrencyCode = code;
                                                        }
                                                    });
                                                }
                                            });

                                            ///结算价节点
                                            XmlParse.SelectSingleNode(xra, "TPA_Extensions/Cost", (prx) =>
                                            {
                                                _rm.Cost = Convert.ToDecimal(XmlParse.GetAttribute(prx, "Amount"));
                                                _rm.CostCurrencyCode = XmlParse.GetAttribute(prx, "CurrencyCode");
                                                if (_rm.CostCurrencyCode != "CNY")
                                                {
                                                    XmlParse.SelectSingleNode(prx, "OtherCurrency/AmountPercentType", other =>
                                                    {
                                                        decimal price = XmlParse.GetAttribute(other, "Amount").ToDecimal();
                                                        string code = XmlParse.GetAttribute(other, "CurrencyCode").ToString();
                                                        if (code == "CNY")
                                                        {
                                                            _rm.Cost = price;
                                                            _rm.CostCurrencyCode = code;
                                                        }
                                                    });
                                                }
                                            });


                                            XmlParse.SelectNodes(xra, "Fees/Fee", (prx) =>
                                            {
                                                if (_rm.Fees == null) _rm.Fees = new List<VxFee>();
                                                foreach (System.Xml.XmlNode ftx in prx)
                                                {
                                                    VxFee fg = new VxFee();
                                                    fg.Amount = Convert.ToDecimal(XmlParse.GetAttribute(ftx, "Amount"));
                                                    fg.ChargeUnit = Convert.ToDecimal(XmlParse.GetAttribute(ftx, "ChargeUnit"));
                                                    fg.Code = XmlParse.GetAttribute(ftx, "Code");
                                                    fg.CurrencyCode = XmlParse.GetAttribute(ftx, "CurrencyCode");
                                                    XmlParse.SelectSingleNode(ftx, "Description/Text", (ttx) =>
                                                    {
                                                        fg.Description = ttx.Value;
                                                    });
                                                    _rm.Fees.Add(fg);
                                                }
                                            });

                                            ve.Rates.Add(_rm);
                                        }
                                    });
                                    RoomStay.RoomRates.Add(ve);
                                }
                            });

                            XmlParse.SelectNodes(node, "BasicPropertyInfo/VendorMessages/VendorMessage", (r_x) =>
                            {
                                try
                                {
                                    if (RoomStay.VendorMessages == null) RoomStay.VendorMessages = new List<string>();
                                    foreach (System.Xml.XmlNode tpx in r_x)
                                    {
                                        string tlt = XmlParse.GetAttribute(tpx, "Title");
                                        if (tlt == "酒店提示")
                                        {
                                            XmlParse.SelectNodes(tpx, "SubSection", (txs) =>
                                            {
                                                foreach (System.Xml.XmlNode tl in txs)
                                                {
                                                    var tst = tl.SelectSingleNode("Paragraph/Text");
                                                    if (tst != null && !string.IsNullOrEmpty(tst.InnerText))
                                                    {
                                                        RoomStay.VendorMessages.Add(tst.InnerText);
                                                    }
                                                }
                                            });
                                        }
                                    }
                                }
                                catch { }
                            });


                            XmlParse.SelectSingleNode(node, "CancelPenalties/CancelPenalty", can =>
                            {
                                this.CancelRule = new CancelPenalty();
                                this.CancelRule.Start = XmlParse.GetAttribute(can, "Start").ToDateTime();
                                this.CancelRule.End = XmlParse.GetAttribute(can, "End").ToDateTime();
                                XmlParse.SelectSingleNode(can, "AmountPercent", am =>
                                {
                                    this.CancelRule.Amount = XmlParse.GetAttribute(am, "Amount").ToDecimal();
                                    this.CancelRule.CurrencyCode = XmlParse.GetAttribute(am, "CurrencyCode");
                                });
                            });
                            XmlParse.SelectSingleNode(node, "Total", tn =>
                            {
                                this.AmountBeforeTax = XmlParse.GetAttribute(tn, "AmountBeforeTax").ToDecimal();
                                this.CurrencyCode = XmlParse.GetAttribute(tn, "CurrencyCode").ToString();
                                if (this.CurrencyCode != "CNY")
                                {
                                    XmlParse.SelectSingleNode(tn, "TPA_Extensions/OtherCurrency/AmountPercentType", other =>
                                    {
                                        decimal price = XmlParse.GetAttribute(other, "Amount").ToDecimal();
                                        string code = XmlParse.GetAttribute(other, "CurrencyCode").ToString();
                                        if (code == "CNY")
                                        {
                                            this.AmountBeforeTax = price;
                                            this.CurrencyCode = code;
                                        }
                                    });
                                }
                            });
                        });
                    }
                }
            }

        }
    }


    [Serializable]
    public class VxRoomStay
    {
        public string AvailabilityStatus { get; set; }
        public List<VxRoomType> RoomTypes { get; set; }
        public List<VxRatePlan> RatePlans { get; set; }
        public List<VxRoomRate> RoomRates { get; set; }
        public List<string> VendorMessages { get; set; }
    }

    [Serializable]
    public class VxRoomType
    {
        public string RoomType { get; set; }
        public string RoomTypeCode { get; set; }
    }
    [Serializable]
    public class VxRatePlan
    {
        //        <!--RatePlanCode属性：价格计划代码；RatePlanName属性：价格计划名称；PrepaidIndicator属性：是否要求预付；
        //            AvailableQuantity属性：剩余房间数量大于8的时候显示为8
        //IsInstantConfirm属性：是否立即确认，true代表是，false为否，20150716新增
        //ReceiveTextRemark属性：房型是否允许自由备注，true表示允许，false表示不允许-->
        public string RatePlanCode { get; set; }
        public string RatePlanName { get; set; }
        public int AvailableQuantity { get; set; }
        public bool PrepaidIndicator { get; set; }
        public bool IsInstantConfirm { get; set; }
        public string ReceiveTextRemark { get; set; }
        public bool Breakfast { get; set; }

        public int InvoiceTargetType { get; set; }

        public int SupplierID { get; set; }

        /// <summary>
        /// 早餐份数
        /// </summary>
        public int Bkc { get; set; }
    }
    [Serializable]
    public class VxRoomRate
    {
        public string RoomTypeCode { get; set; }
        public string RatePlanCode { get; set; }
        public List<VxRate> Rates { get; set; }
    }

    [Serializable]
    public class VxRate
    {
        public string EffectiveDate { get; set; }
        public string ExpireDate { get; set; }
        public int MaxGuestApplicable { get; set; }
        public decimal AmountBeforeTax { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Cost { get; set; }
        public string CostCurrencyCode { get; set; }
        public int NumberOfBreakfast { get; set; }
        public List<VxFee> Fees { get; set; }
    }

    [Serializable]
    public class VxFee
    {
        public string Code { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        /// <summary>
        /// 扣款单位:如每房每晚/每人
        /// </summary>
        public decimal ChargeUnit { get; set; }
        public string Description { get; set; }
    }
}
