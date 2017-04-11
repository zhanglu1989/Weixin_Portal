using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Senparc.Weixin.Entities;
using Senparc.Weixin.CommonAPIs;
using log4net;

namespace Weixin_BotApi.Service
{
    /// <summary>
    /// 客服接口
    /// </summary>
    public static class CustomerService
    {
        static ILog loginfo = LogManager.GetLogger(typeof(CustomerService));
        private const string URL_FORMAT = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}";

        /// <summary>
        /// 发送文本信息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static WxJsonResult SendText(string accessToken, string openId, string content)
        {
            var data = new
            {
                touser = openId,
                msgtype = "text",
                text = new
                {
                    content = content
                }
            };

            loginfo.Info("客服发送的内容为：" + data.ToString());
            return CommonJsonSend.Send(accessToken, URL_FORMAT, data);
        }
    }
}