using Ctrip.Common;
using Ctrip.Entities;
using Ctrip.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 

namespace Ctrip.Response
{
    [Serializable]
    public class CancelResponse:BaseResponse
    {
        public HeaderInfo Header { get; set; }
        public bool IsSuccess { get; set; }
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
                    XmlParse.SelectSingleNode(base.ResultXML, "/Response/HotelResponse/OTA_CancelRS/Errors/Error", (c) =>
                    {
                        suc = false;
                        base.Error = new Error();
                        base.Error.Type = XmlParse.GetAttribute(c, "Type");
                        base.Error.Code = XmlParse.GetAttribute(c, "Code");
                        base.Error.Message = c.InnerText;
                    });

                    if (suc)
                    {
                        XmlParse.SelectSingleNode(base.ResultXML, "/Response/HotelResponse/OTA_CancelRS/Success", node =>
                        {
                            this.IsSuccess = true;
                        });
                    }
                }
            }

        }
    }
}
