using log4net;
using System;
using System.Net;
using System.IO;
using System.Text;
using Weixin_BotApi.Common;

namespace Weixin_BotApi.Service
{
    public class BotFQAService
    {
        ILog loginfo = LogManager.GetLogger(typeof(BotFQAService));

        /// <summary>
        /// 获取bot api信息
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        public static string GetEngineResponse(string query, string sessionID)
        {
            string url = string.Format("http://huaxiabot20161206105452.azurewebsites.net/query?query={0}&sessionID={1}", query, sessionID);
            WebRequest request = WebRequest.Create(url);
            request.Method = "Get";

            //读取返回消息
            string res = string.Empty;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                res = reader.ReadToEnd();
                //整理res格式
                res = res.Replace("<br/>", "\n").Trim('"');
                reader.Close();

                //存入faq数据库
                //DapperHelper.InsertFAQInfo(null);
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}