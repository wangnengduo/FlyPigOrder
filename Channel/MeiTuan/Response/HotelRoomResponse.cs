using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Response
{

    public class HotelRoomResponse : BaseResponse
    {
        public HotelRoomResult result { get; set; }

    }


    public class HotelRoomResult
    {
        /// <summary>
        /// 物理房型数据
        /// </summary>
        public Dictionary<long, List<RealRoomInfo>> realRoomInfos { get; set; }
    }


    public class RealRoomInfo
    {
        public RealRoomBaseInfo realRoomBaseInfo { get; set; }

        public List<long> goodsIds { get; set; }
    }


    public class RealRoomBaseInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int realRoomId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int roomType { get; set; }
        /// <summary>
        /// 空调双床房
        /// </summary>
        public string roomName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string roomDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string useableArea { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string capacity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int window { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string windowView { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string windowBad { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? extraBed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string floor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? internetWay { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
    }
}
