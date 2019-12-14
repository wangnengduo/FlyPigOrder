using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Channel.Ctrip
{
    [Serializable]
    public abstract class CtBaseResponse
    {
        public CtResponseStatus ResponseStatus { get; set; }
        public string body { get; set; }
        public string code { get; set; }
        public string message { get; set; }
    }

    [Serializable]
    public class CtResponseStatus
    {
        public string Timestamp { get; set; }
        public string Ack { get; set; }
        //public List<CzErrors> Errors { get; set; }
        //public List<CzExtension> Extension { get; set; }
        public string Version { get; set; }
    }

    [Serializable]
    internal class CtBaseResponse<T>
    {
        public T result { get; set; }
    }
}
