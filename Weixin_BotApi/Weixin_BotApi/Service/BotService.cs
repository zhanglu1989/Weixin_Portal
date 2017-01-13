using System;
using System.Net;
using System.IO;
using System.Text;
using log4net;

namespace Weixin_BotApi.Service
{
    /// <summary>
    /// 调用后台api接口信息
    /// </summary>
    public class BotService
    {
        static ILog loginfo = LogManager.GetLogger(typeof(BotService));

        /// <summary>
        /// 获取bot api信息
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        public static string GetBotInfo(string query, string sessionID)
        {
            string url = string.Format("http://huaxiabot20161206105452.azurewebsites.net/query?query={0}&sessionID={1}", query, sessionID);
            WebRequest request = WebRequest.Create(url);
            request.Method = "Get";

            //读取返回消息
            string res = string.Empty;
            //string result = string.Format("Hi, 接下来让萌萌告诉你今天{0}的天气：" + "\r\n", location);

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                res = reader.ReadToEnd();

                //整理res格式
                res = res.Replace("<br/>", "\n").Trim('"');

                reader.Close();
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 处理简单对话
        /// </summary>
        /// <param name="requestContent"></param>
        /// <returns></returns>
        public static string HandleSimpleDialog(string requestContent)
        {
            string response = "";
            if (requestContent.Length < 10 && 
                    (requestContent.Contains("hi") || requestContent.Contains("Hi") || requestContent.Contains("你好")
                                                   || requestContent.Contains("Hello") || requestContent.Contains("hello")))
            {
                response = "Hi, 我是小夏，很高兴能为您服务，请问有什么我可以帮助您的吗？ 理财问我就对啦！";
            }
            //else if
            //{

            //}

            loginfo.Info("自定义返回消息内容：" + response);
            return response;
        }

        /// <summary>
        ///  优化回复信息
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static string HandleInvalidContent(string responseContent)
        {
            if(responseContent.Contains("没有找到您想要查询的基金"))
            {
                responseContent = "对不起，小夏没有找到您想要查询的基金名称，请您更换查询条件或确认基金名称无误后重新提问。";
            }
            else if (responseContent.Contains("请给出您要查询的基金名称或者您想要查询的基金"))
            {
                responseContent = "请把您要查询的基金名称或者基金的满足条件告诉小夏, 小夏来帮您进一步查询。";
            }
            //else if (responseContent.Contains("Internal Server Error"))
            //{
            //    responseContent = "不好意思，小夏需要您给出准确的基金名称，才能帮您进一步查询哦。";
            //}

            loginfo.Info("自定义返回消息内容：" + responseContent);
            return responseContent;
        }
    }
}