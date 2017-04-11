using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Weixin_BotApi.Models
{
    public class TableModel
    {
        /*
        若属性名和数据库字段不一致（不区分大小写）则查询不出数据，如果使用EF则可以通过Column特性
        建立属性和数据表字段之间的映射关系，Dapper则不行
        */
        public Guid ID { get; set; }

        //客户问的问题
        public string Question { get; set; }

        //table engine 给出的答案
        public string Answer { get; set; }

        //table engine 给出的评分
        public string Score { get; set; }

        //table engine 给出的门限
        public string Threshold { get; set; }

        //提问时间
        public DateTime Datetime { get; set; }
    }
}