

namespace Ctrip.Request
{

    using System.Diagnostics;
    using System;
    using System.Xml.Serialization;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System.Web.Services;


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "OTA_HotelResSaveSoap", Namespace = "http://ctrip.com/")]
    public partial class OTA_HotelResSave : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        private System.Threading.SendOrPostCallback RequestOperationCompleted;

        /// <remarks/>
        public OTA_HotelResSave()
        {
            this.Url = "http://openapi.ctrip.com/Hotel/OTA_HotelResSave.asmx";
        }

        /// <remarks/>
        public event RequestCompletedEventHandler RequestCompleted;

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://ctrip.com/Request", RequestNamespace = "http://ctrip.com/", ResponseNamespace = "http://ctrip.com/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string Request(string requestXML)
        {
            object[] results = this.Invoke("Request", new object[] {
                        requestXML});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginRequest(string requestXML, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("Request", new object[] {
                        requestXML}, callback, asyncState);
        }

        /// <remarks/>
        public string EndRequest(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void RequestAsync(string requestXML)
        {
            this.RequestAsync(requestXML, null);
        }

        /// <remarks/>
        public void RequestAsync(string requestXML, object userState)
        {
            if ((this.RequestOperationCompleted == null))
            {
                this.RequestOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRequestOperationCompleted);
            }
            this.InvokeAsync("Request", new object[] {
                        requestXML}, this.RequestOperationCompleted, userState);
        }

        private void OnRequestOperationCompleted(object arg)
        {
            if ((this.RequestCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RequestCompleted(this, new RequestCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void RequestCompletedEventHandler(object sender, RequestCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RequestCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal RequestCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }


}
