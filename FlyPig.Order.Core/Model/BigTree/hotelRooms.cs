using System;
using System.Linq;
using System.Text;

namespace FlyPig.Order.Core.Model
{
    ///<summary>
    ///酒店房间
    ///</summary>
    public partial class hotelRooms
    {
           public hotelRooms(){


           }
           /// <summary>
           /// Desc:房间编号
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int rId {get;set;}

           /// <summary>
           /// Desc:酒店编号
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int hId {get;set;}

           /// <summary>
           /// Desc:房间号码
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public string roomCode {get;set;}

           /// <summary>
           /// Desc:房型名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string roomName {get;set;}

           /// <summary>
           /// Desc:房间数
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public int roomNumber {get;set;}

           /// <summary>
           /// Desc:可选值：A,B,C,D。分别代表： A：15平米以下，B：16－30平米，C：31－50平米，D：50平米以上
           /// Default:NULL
           /// Nullable:True
           /// </summary>           
           public string roomAcreage {get;set;}

           /// <summary>
           /// Desc:层高
           /// Default:NULL
           /// Nullable:True
           /// </summary>           
           public string roomFloor {get;set;}

           /// <summary>
           /// Desc:房间设施
           /// Default:NULL
           /// Nullable:True
           /// </summary>           
           public string roomEquipment {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string roomDesc {get;set;}

           /// <summary>
           /// Desc:宽带信息(宽带服务。A,B,C,D。分别代表： A：无宽带，B：免费宽带，C：收费宽带，D：部分收费宽带)
           /// Default:A
           /// Nullable:True
           /// </summary>           
           public string netType {get;set;}

           /// <summary>
           /// Desc:0表示没有wifi，1表示有wifi
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public short wifi {get;set;}

           /// <summary>
           /// Desc:0为不能加床，1为加床
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public short isaddbed {get;set;}

           /// <summary>
           /// Desc:最大加床
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public short maxAddBed {get;set;}

           /// <summary>
           /// Desc:床型(可选值：A,B,C,D,E,F,G,H,I。分别代表：A：单人床，B：大床，C：双床，D：双床/大床，E：子母床，F：上下床，G：圆形床，H：多床，I：其他床型)
           /// Default:A
           /// Nullable:True
           /// </summary>           
           public string bedType {get;set;}

           /// <summary>
           /// Desc:床宽(可选值：A,B,C,D,E,F,G,H。分别代表：A：1米及以下，B：1.1米，C：1.2米，D：1.35米，E：1.5米，F：1.8米，G：2米，H：2.2米及以上)
           /// Default:A
           /// Nullable:True
           /// </summary>           
           public string bedWidth {get;set;}

           /// <summary>
           /// Desc:0没窗，1有窗，2部分有窗
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public short hasWindow {get;set;}

           /// <summary>
           /// Desc:是否可以吸烟(0不可，1可以)
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public short noSmoking {get;set;}

           /// <summary>
           /// Desc:备注
           /// Default:NULL
           /// Nullable:True
           /// </summary>           
           public string remark {get;set;}

           /// <summary>
           /// Desc:房态(0不可预订1可预订)
           /// Default:1
           /// Nullable:False
           /// </summary>           
           public short roomState {get;set;}

           /// <summary>
           /// Desc:
           /// Default:1900-01-01 00:01
           /// Nullable:False
           /// </summary>           
           public DateTime modifiedTime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string elong_rid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? fangcangRid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public short? isfangcangdirect {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public short? yuliu {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? personNumMax {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? IsStandardRoom {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? isHasNet {get;set;}

           /// <summary>
           /// Desc:网络收费情况
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string NetFee {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string waytype {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public short? jishi {get;set;}

           /// <summary>
           /// Desc:
           /// Default:24
           /// Nullable:True
           /// </summary>           
           public short? lastConfirm {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public short? yuliuState {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string elongroomname {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string qunarroomname {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ctriproomname {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string faxroomname {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string msgroomname {get;set;}

           /// <summary>
           /// Desc:
           /// Default:DateTime.Now
           /// Nullable:True
           /// </summary>           
           public DateTime? ReserveUpdateTime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:30
           /// Nullable:True
           /// </summary>           
           public short? lastbook {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string meituanrid {get;set;}

    }
}
