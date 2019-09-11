using System;
using System.Linq;
using System.Text;

namespace FlyPig.Order.Core.Model
{
    ///<summary>
    ///
    ///</summary>
    public partial class OperateRemark
    {
           public OperateRemark(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int id {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int hid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public int? rid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public int? rateplanid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string caozuo {get;set;}

           /// <summary>
           /// Desc:1为操作日志，2为操作原因
           /// Default:1
           /// Nullable:True
           /// </summary>           
           public short? type {get;set;}

           /// <summary>
           /// Desc:1为酒店操作，2为房型操作，3为价格计划操作
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public short? operatetype {get;set;}

           /// <summary>
           /// Desc:1为房价记录，2为房态记录
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public short? detailoperate {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? stdate {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? endate {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? addtime {get;set;}

    }
}
