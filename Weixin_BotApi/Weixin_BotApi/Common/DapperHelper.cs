using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Weixin_BotApi.Models;

namespace Weixin_BotApi.Common
{
    public class DapperHelper
    {
        protected static string connectSql = ConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;

        #region FAQ engine CURD
        //将faq engine的回答记录添加到数据库
        public static void InsertFAQInfo(FAQModel faqModel)
        {
            string connStr = connectSql;
            IDbConnection conn = new SqlConnection(connStr);
            string query = "INSERT INTO FAQEngine(Question, StandardQuestion, Answer, Score, Threshold, Datetime) VALUES(@question, @standardquestion, @answer, @score, @threshold, @datetime)";
            conn.Execute(query, faqModel);
            conn.Close();
        }
        #endregion

        #region Table engine CURD
        //将table engine的回答记录添加到数据库
        public static void InsertTableInfo(TableModel tableModel)
        {
            string connStr = connectSql;
            IDbConnection conn = new SqlConnection(connStr);
            string query = "INSERT INTO TableEngine(Question, Answer, Score, Threshold, Datetime) VALUES(@question, @answer, @score, @threshold, @datetime)";
            conn.Execute(query, tableModel);
            conn.Close();
        }
        #endregion
    }
}