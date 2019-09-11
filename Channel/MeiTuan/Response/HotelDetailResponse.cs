using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Response
{
    public class HotelDetailResponse : BaseResponse
    {
        public HotelDetailResult result { get; set; }

    }


    public class HotelDetailResult
    {
        public List<HotelDetail> hotelDetails { get; set; }
    }


    public class HotelDetail
    {
        /// <summary>
        /// 酒店基本信息
        /// </summary>
        public HotelBaseInfo baseInfo { get; set; }

        /// <summary>
        /// 酒店扩展信息
        /// </summary>
        public HotelExtendInfo extendInfo { get; set; }

        /// <summary>
        /// 酒店房型信息
        /// </summary>
        public List<RoomInfo> roomInfos { get; set; }


        /// <summary>
        /// 酒店图片信息
        /// </summary>
        public List<PoiImage> poiImages { get; set; }

        /// <summary>
        /// 酒店ID
        /// </summary>
        public long? hotelId { get; set; }
    }



    public class HotelBaseInfo
    {
        /// <summary>
        /// 酒店ID
        /// </summary>
        public long? hotelId { get; set; }

        /// <summary>
        /// 酒店名称
        /// </summary>
        public string pointName { get; set; }

        /// <summary>
        /// 酒店描述信息
        /// </summary>
        public string info { get; set; }

        /// <summary>
        /// 高德经度，取值为实际经度值*10的6次方取整
        /// </summary>
        public int? longitude { get; set; }

        /// <summary>
        /// 高德纬度，取值为实际纬度值*10的6次方取整
        /// </summary>
        public int? latitude { get; set; }

        /// <summary>
        /// 酒店地址
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// 酒店所在城市名，如“上海市”
        /// </summary>
        public string cityName { get; set; }

        /// <summary>
        /// 城市ID与城市名、城市行政区ID与行政区名的映射关系列表请联系分销平台获取
        /// </summary>
        public int? cityLocationId { get; set; }

        /// <summary>
        /// 酒店所在城市行政区名，如“松江区”
        /// </summary>
        public string locationName { get; set; }

        /// <summary>
        /// 城市行政区ID
        /// </summary>
        public int? locationId { get; set; }

        /// <summary>
        /// 酒店所在商圈名。
        /// </summary>
        public string bareaName { get; set; }

        /// <summary>
        /// 用户评分，10分制，取值为实际值*10。如用户评分为4.2分，则avgScore=42
        /// </summary>
        public int? avgScore { get; set; }

        /// <summary>
        /// 酒店联系电话。
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// 酒店填写的营业时间，无固定格式，如取值可以为“24小时”
        /// </summary>
        public string openInfo { get; set; }

        /// <summary>
        ///    酒店营业状态：
        ///0 营业中
        ///1 已关门
        ///2 筹建中
        ///3 暂停营业
        /// </summary>
        public int? closeStatus { get; set; }

        /// <summary>
        /// 首图信息
        /// </summary>
        public string frontImage { get; set; }

    }

    public class HotelExtendInfo
    {
        /// <summary>
        /// 酒店设施，key表示酒店设施ID（ID对应设施参看酒店设施），value="0"表示不提供该设施；value="1"表示提供该设施
        /// </summary>
        public Dictionary<int?,string> hotelFacilities { get; set; }

        /// <summary>
        /// 酒店服务，key表示酒店服务ID（ID对应服务参看酒店服务），value="0"表示不提供该服务；value="1"表示提供该服务
        /// </summary>
        public Dictionary<int?, string> hotelService { get; set; }

        /// <summary>
        /// 酒店门店扩展信息
        /// </summary>
        public PoiExtInfo poiExtInfo { get; set; }
    }

    /// <summary>
    /// 酒店门店扩展信息
    /// </summary>
    public class PoiExtInfo
    {
        /// <summary>
        /// 酒店ID
        /// </summary>
        public long? hotelId { get; set; }

        /// <summary>
        /// 开业时间，格式为yyyy/MM或yyyy/MM/dd
        /// </summary>
        public string openDate { get; set; }

        /// <summary>
        /// 装修时间，格式为yyyy/MM或yyyy/MM/dd
        /// </summary>
        public string decorationDate { get; set; }

        /// <summary>
        /// 酒店房间总数
        /// </summary>
        public int? roomNum { get; set; }

        /// <summary>
        /// 酒店楼层高度
        /// </summary>
        public int? floorNum { get; set; }


        /// <summary>
        ///    酒店星级：
        ////0 国家旅游局颁布五星级证书
        ///1 豪华（按五星级标准建造）
        ///2 国家旅游局颁布四星级证书
        ///3 高档（按四星级标准建造）
        ///4 国家旅游局颁布三星级证书
        ///5 舒适型（按三星级标准建造）
        ///6 经济型
        ///7 低档
        /// </summary>
        public int? hotelStar { get; set; }

        /// <summary>
        /// 酒店类型：
        ///0 经济型
        ///1 快捷酒店
        ///2 商务酒店
        ///3 主题酒店
        ///4 情侣酒店
        ///5 公寓
        ///6 客栈
        ///7 民宿
        ///8 青年旅社
        ///9 农家院
        ///10 家庭旅馆
        ///11 招待所
        ///12 度假酒店
        ///13 别墅
        ///最多可同时包含两项酒店类型，两项间以半角逗号分隔，如"9,10"表示该酒店即是农家院又是家庭旅馆。
        /// </summary>
        public string poiType { get; set; }


        /// <summary>
        /// 酒店主题标签：
        ///0 购物便捷
        ///1 培训学习
        ///2 蜜月出行
        ///3 休闲情调
        ///4 交通便利
        ///5 离医院近
        ///6 商旅之家
        ///7 四合院
        ///8 园林庭院
        ///9 安静优雅
        ///10 特色建筑
        ///11 周边美景
        ///12 家有萌宠
        ///13 文艺范儿
        ///14 观景露台
        ///15 古色古香
        ///最多可同时包含三项酒店主题标签，各项间以半角逗号分隔，如"9,13,15"表示该酒店安静优雅又有文艺范儿，还古色古香。
        /// </summary>
        public string themeTag { get; set; }

        /// <summary>
        /// 登记入住开始时间，格式为HH:mm，取值范围为[06:00, 23:30]，半小时为一个取值点，如"06:00"、"06:30"
        /// </summary>
        public string checkintimeBegin { get; set; }

        /// <summary>
        /// 登记入住截止时间，checkint?imeEnd="0"表示登记入住没有截止时间。非0值则格式为HH:mm，取值范围为[14:30, 23:59]，半小时为一个取值点，如"14:30"、"15:00"。当天的最后半个小时特殊处理，表示为"23:59"
        /// </summary>
        public string checkintimeEnd { get; set; }

        /// <summary>
        /// 离店时间，checkoutTime="0"表示固定小时制离店，即入住checkOutTimeHours小时之后离店；非0值则格式为HH:mm，取值范围为[06:00, 23:59]，半小时为一个取值点，如"14:30"、"15:00"。当天的最后半个小时特殊处理，表示为"23:59"
        /// </summary>
        public string checkoutTime { get; set; }

        /// <summary>
        /// 当checkoutTime="0"时必填，精度为一位小数，如checkoutTimeHours=6.5表示入住6个半小时后离店
        /// </summary>
        public float checkoutTimeHours { get; set; }

    }


    public class RoomInfo
    {
        /// <summary>
        /// 	房型基本信息
        /// </summary>
        public RoomBaseInfo roomBaseInfo { get; set; }

        /// <summary>
        /// 房型扩展信息
        /// </summary>
        public RoomExtendInfo roomExtendInfo { get; set; }

        /// <summary>
        /// 床型基本信息
        /// </summary>
        public List<RoomBedInfo> roomBedInfos { get; set; }

    }

    public class RoomBaseInfo
    {
        public int roomId { get; set; }
        public int? realRoomId { get; set; }
        public long? hotelId { get; set; }

        /// <summary>
        ///   房型类型：
        ///0 大床间（单间）
        ///1 单人间（单间）
        ///2 双床间（单间）
        ///3 三人间（单间）
        ///4 套房
        ///5 独栋
        ///6 床位房
        /// </summary>
        public int? roomType { get; set; }

        public string roomName { get; set; }

        public string roomDesc { get; set; }
        public string useableArea { get; set; }
        public string capacity { get; set; }
        public int? window { get; set; }

        public string windowView { get; set; }
        public string windowBad { get; set; }
        public int? extraBed { get; set; }
        public string floor { get; set; }
        public int? internetWay { get; set; }
        public int? weekdayPrice { get; set; }
        public int? weekendPrice { get; set; }
        /// <summary>
        /// 房型状态：
        ///0 无效
        ///1 有效 
        /// </summary>
        public int? status { get; set; }
    }

    public class RoomExtendInfo
    {
        public Dictionary<int, string> roomFacilities { get; set; }
    }

    public class RoomBedInfo
    {
        public int? roomId { get; set; }

        public string bedType { get; set; }

        public string bedDesc { get; set; }

        public string bedCount { get; set; }
    }

    public class PoiImage
    {
        public int? typeId { get; set; }

        public string typeName { get; set; }
        public string imgDesc { get; set; }
        public string url { get; set; }
        public int? roomId { get; set; }
    }


}
