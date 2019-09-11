using System;
using System.Linq;
using System.Text;

namespace FlyPig.Order.Core.Model
{
    ///<summary>
    ///
    ///</summary>
    public partial class AliTripHotelExtension
    {
           public AliTripHotelExtension(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int Id {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string HotelId {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int Source {get;set;}

           /// <summary>
           /// Desc:加价目录
           /// Default:
           /// Nullable:True
           /// </summary>           
           public bool? Extension {get;set;}

           /// <summary>
           /// Desc:不可取消目录
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? Extension1 {get;set;}

           /// <summary>
           /// Desc:酒店黑名单（内部设置/渠道黑名单）
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? Extension2 {get;set;}

           /// <summary>
           /// Desc:指导价名单
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? Extension3 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public int Extension4 {get;set;}

           /// <summary>
           /// Desc:是否开启红包
           /// Default:1
           /// Nullable:False
           /// </summary>           
           public int Extension5 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? CreateTime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? ModifyTime {get;set;}

    }
}
