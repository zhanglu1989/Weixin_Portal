using System.Configuration;

namespace Weixin_BotApi.Util
{
    public class AppSettings
    {
        public static string Wechat_AppID
        {
            get
            {
                return ConfigurationManager.AppSettings["AppID"];
            }
        }

        public static string Wechat_Secret
        {
            get
            {
                return ConfigurationManager.AppSettings["Secret"];
            }
        }
    }
}