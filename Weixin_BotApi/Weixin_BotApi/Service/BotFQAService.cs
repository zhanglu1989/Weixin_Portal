using log4net;
using System;
using System.Net;
using System.IO;
using System.Text;
using Weixin_BotApi.Common;
using Newtonsoft.Json.Linq;
using Weixin_BotApi.Models;

namespace Weixin_BotApi.Service
{
    public class BotFQAService
    {
        static ILog loginfo = LogManager.GetLogger(typeof(BotFQAService));

        //得分阈值
        private const int Threshold = 5;

        /// <summary>
        /// 获取bot engine信息
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public static string GetEngineResponse(string question)
        {
            String url = "http://40.74.113.149:8850/q=" + question;
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")); //reader.ReadToEnd() 表示取得网页的源码

            string[] results = System.Text.RegularExpressions.Regex.Split(reader.ReadToEnd().ToString(), "#");
            JObject jsonObj = JObject.Parse(results[0]);

            //对应的标准问题
            string stQuestionContent = jsonObj["ResponseText"].ToString().Split('\t')[0].Trim();
            string stQuestion = stQuestionContent;
            if (stQuestionContent.Contains(":"))
            {
                int pos = stQuestionContent.LastIndexOf(':');
                stQuestion = stQuestionContent.Substring(pos, stQuestionContent.Length);
            }
            loginfo.Info(string.Format("the stQuestion answer is：{0}", stQuestion));
            //标准答案
            string content = jsonObj["ResponseText"].ToString().Split('\t')[1].Trim();
            string answer = content.Split('$')[0].Trim();
            loginfo.Info(string.Format("the answer is：{0}", answer));
            //获取得分
            string contentScore = content.Split('$')[1].Replace("score", "").Trim();
            string score = contentScore;
            if (contentScore.Contains(","))
            {
                score = contentScore.Split(',')[0].Trim();
            }
            loginfo.Info(string.Format("the score is：{0}", score));

            //得分小于阈值
            if (Convert.ToDouble(score) < Threshold)
            {
                answer += "如果您对小夏的回答不满意，可以接入人工客服哟。";
            }
            //存入faq数据库,
            FAQModel faq = new FAQModel() { Question = question, StandardQuestion = stQuestion, Answer = answer, Score = score, Threshold = Threshold.ToString(), Datetime = DateTime.Now };
            DapperHelper.InsertFAQInfo(faq);

            loginfo.Info("exist faq engine！");
            return answer;
        }
    }
}