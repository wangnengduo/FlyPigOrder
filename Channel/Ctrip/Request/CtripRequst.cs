using Ctrip.Common;
using Ctrip.Config;
using Ctrip.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrip.Request
{

    [Serializable]
    public   class CtripRequest<TResponse>
        where TResponse : BaseResponse
    {

        public CtripRequest(IBaseRequest<TResponse> request,ApiConfig config)
        {
            this.RequestBody = request;
            this.Header = new HeaderRequest(config, RequestBody.Method);
        }
 
        public IBaseRequest<TResponse> RequestBody { get; set; }

        public HeaderRequest Header { get; set; }

        public string ToXMLString()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            xml += "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body><Request xmlns=\"http://ctrip.com/\"><requestXML>";
            xml += "<![CDATA[<Request>";
            string head = RequestXmlWrapper.ToXml(this.Header);
            string body = "<HotelRequest><RequestBody xmlns:ns=\"http://www.opentravel.org/OTA/2003/05\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">";
            string content = RequestXmlWrapper.ToXml(this.RequestBody);
            if (this.RequestBody.GetType() != typeof(HotelDescriptiveRequest) && this.RequestBody.GetType() != typeof(D_HotelOrderMiniInfoRequest) && this.RequestBody.GetType() != typeof(D_GetCtripOrderIDRequest))
            {
                content = content.Replace("<", "<ns:").Replace("<ns:/", "</ns:");
            }
            content = content.Replace("\"True\"", "\"true\"").Replace("\"False\"", "\"false\"");
            body += content;
            body += "</RequestBody></HotelRequest>";
            if (this.RequestBody.GetType() == typeof(D_HotelOrderMiniInfoRequest) || this.RequestBody.GetType() == typeof(D_GetCtripOrderIDRequest))
            {
                body = content;
            }
            xml += (head + body);
            xml += "</Request>]]></requestXML></Request></soap:Body></soap:Envelope>";
            return xml;
        }
    }
}
