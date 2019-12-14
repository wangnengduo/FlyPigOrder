using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DaDuShi.Response
{
    public class RateJsonEt
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public string EffectiveDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string ExpireDate { get; set; }
        /// <summary>
        /// 税前金额，为-1时表示当前日期无价格
        /// </summary>
        public decimal AmountBeforeTax { get; set; }
        /// <summary>
        /// 税后金额，为-1时表示当前日期无价格
        /// </summary>
        public decimal AmountAfterTax { get; set; }
        /// <summary>
        /// 早餐数量
        ///-2  含早
        ///-1  不定
        ///0  不含早
        ///1  单早
        ///2  双早
        ///3  三早
        ///4:四早
        /// </summary>
        public int Breakfast { get; set; }
        /// <summary>
        /// 限制成人数,-1为无限制
        /// </summary>
        public int Adult { get; set; }
        /// <summary>
        /// 限制儿童数,-1为无限制
        /// </summary>
        public int Child { get; set; }
        /// <summary>
        /// 房型的当前房量，为-1表示当前房型不可售（关房/满房），为0也是可售的
        /// </summary>
        public int Ability { get; set; }
        public bool LosSpecified { get; set; }
    }
}