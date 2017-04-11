using log4net;
using Senparc.Weixin.MP.CommonAPIs;
using Weixin_BotApi.Util;

namespace Weixin_BotApi.Service
{
    public static class CommonService
    {
        static ILog loginfo = LogManager.GetLogger(typeof(CommonService));

        /// <summary>
        /// 获取access token
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken()
        {
            var ret = CommonApi.GetToken(AppSettings.Wechat_AppID, AppSettings.Wechat_Secret);
            loginfo.Info("获取的accesstoken值为：" + ret.access_token);
            return ret.access_token;
        }

        /// <summary>
        /// 获取open id
        /// </summary>
        /// <returns></returns>
        public static string GetOpenId()
        {
            var ret = CommonApi.GetUserInfo(AppSettings.Wechat_AppID, AppSettings.Wechat_Secret);
            loginfo.Info("获取的openid值为：" + ret);
            return ret.openid;
        }
    }
}