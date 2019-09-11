using Ctrip.Common;
using Ctrip.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrip.Response
{
    [Serializable]
    public class HotelResSaveResponse : BaseResponse
    {
        public HeaderInfo Header { get; set; }
        public  HotelReservation Order { get; set; }
        public string Warning { get; set; }
        public string CtripOrderId { get; set; }
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
                    XmlParse.SelectSingleNode(base.ResultXML, "/Response/HotelResponse/OTA_HotelResSaveRS/Errors/Error", (c) =>
                    {
                        suc = false;
                        base.Error = new Error();
                        base.Error.Type = XmlParse.GetAttribute(c, "Type");
                        base.Error.Code = XmlParse.GetAttribute(c, "Code");
                        base.Error.Message = c.InnerText;
                    });
                    XmlParse.SelectSingleNode(base.ResultXML, "/Response/HotelResponse/OTA_HotelResSaveRS/Warnings/Warning", (c) =>
                    {

                        string type = XmlParse.GetAttribute(c, "Type");
                        string code = XmlParse.GetAttribute(c, "Code");
                        string warn = c.InnerText;
                        this.Warning = string.Format("{0} {1} {2}", type, code, warn);
                    });
                    if (suc)
                    {

                    }
                }
            }

        }

        internal void ParseXML2()
        {
            bool suc = true;
            XmlParse.SelectSingleNode(base.ResponseXml, "/Response/Header", (c) =>
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
            XmlParse.SelectSingleNode(base.ResponseXml, "/Response/HotelResponse/OTA_HotelResSaveRS/Errors/Error", (c) =>
            {
                suc = false;
                base.Error = new Error();
                base.Error.Type = XmlParse.GetAttribute(c, "Type");
                base.Error.Code = XmlParse.GetAttribute(c, "Code");
                base.Error.Message = c.InnerText;
            });
            XmlParse.SelectSingleNode(base.ResponseXml, "/Response/HotelResponse/OTA_HotelResSaveRS/Warnings/Warning", (c) =>
            {

                string type = XmlParse.GetAttribute(c, "Type");
                string code = XmlParse.GetAttribute(c, "Code");
                string warn = c.InnerText;
                this.Warning = string.Format("{0} {1} {2}", type, code, warn);
            });
            if (suc)
            {
                XmlParse.SelectSingleNode(base.ResponseXml, "/Response/HotelResponse/OTA_HotelResSaveRS/HotelReservations/HotelReservation/ResGlobalInfo/HotelReservationIDs", (c) =>
                {
                    this.Order = new Ctrip.Entities.HotelReservation() { HotelReservationIDs = new List<HotelReservationID>() };
                    XmlParse.SelectNodes(c, "HotelReservationID", ns =>
                    {
                        foreach (System.Xml.XmlNode node in ns)
                        {
                            HotelReservationID idInfo = new HotelReservationID();
                            idInfo.ResID_Type = XmlParse.GetAttribute(node, "ResID_Type");
                            idInfo.ResID_Value = XmlParse.GetAttribute(node, "ResID_Value");
                            if (idInfo.ResID_Type == "501")
                            {
                                this.CtripOrderId = idInfo.ResID_Value;
                            }
                            this.Order.HotelReservationIDs.Add(idInfo);
                        }
                    });
                });
            }

        }
    }
}
