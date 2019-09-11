using Ctrip.Common;
using Ctrip.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrip.Response
{
    public class HotelOrderMiniInfoResponse : BaseResponse
    {

        public HeaderInfo Header { get; set; }
        public HotelOrderMiniInfo MiniInfo { get; set; }

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
                    XmlParse.SelectSingleNode(base.ResultXML, "/Response/HotelResponse/OTA_PingRS/Errors/Error", (c) =>
                    {
                        suc = false;
                        base.Error = new Error();
                        base.Error.Type = XmlParse.GetAttribute(c, "Type");
                        base.Error.Code = XmlParse.GetAttribute(c, "Code");
                        base.Error.Message = c.InnerText;
                    });

                    if (suc)
                    {
                        XmlParse.SelectSingleNode(base.ResultXML, "/Response/HotelOrderMiniInfoResponse", node =>
                        {
                            this.MiniInfo = new HotelOrderMiniInfo();
                            XmlParse.SelectSingleNode(node, "AllianceId", n => { this.MiniInfo.AllianceId = n.InnerText; });
                            XmlParse.SelectSingleNode(node, "Sid", n => { this.MiniInfo.Sid = n.InnerText; });
                            XmlParse.SelectSingleNode(node, "Ouid", n => { this.MiniInfo.Ouid = n.InnerText; });
                            XmlParse.SelectSingleNode(node, "OrderId", n => { this.MiniInfo.OrderId = n.InnerText; });
                            XmlParse.SelectSingleNode(node, "HotelId", n => { this.MiniInfo.HotelId = n.InnerText; });
                            XmlParse.SelectSingleNode(node, "HotelName", n => { this.MiniInfo.HotelName = n.InnerText; });
                            XmlParse.SelectSingleNode(node, "FromOrderId", n => { this.MiniInfo.FromOrderId = n.InnerText; });
                            XmlParse.SelectSingleNode(node, "OrderStatus", n => { this.MiniInfo.OrderStatus = n.InnerText; });
                            XmlParse.SelectSingleNode(node, "Price", n => { this.MiniInfo.Price = n.InnerText.ToDecimal(); });
                            XmlParse.SelectSingleNode(node, "Quantity", n => { this.MiniInfo.Quantity = n.InnerText.ToInt32(); });
                            XmlParse.SelectSingleNode(node, "OrderDate", n => { this.MiniInfo.OrderDate = n.InnerText.ToDateTime(); });
                            XmlParse.SelectSingleNode(node, "CheckInDate", n => { this.MiniInfo.CheckInDate = n.InnerText.ToDateTime(); });
                            XmlParse.SelectSingleNode(node, "CheckOutDate", n => { this.MiniInfo.CheckOutDate = n.InnerText.ToDateTime(); });
                        });
                    }
                }
            }

        }


    }
}
