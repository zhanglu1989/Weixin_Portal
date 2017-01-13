using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.CommonAPIs;
using Weixin_BotApi.Util;
using log4net;
using Senparc.Weixin.MP.AdvancedAPIs.Media;
using Senparc.Weixin.MP.AdvancedAPIs;

namespace Weixin_BotApi.Service
{
    public class WechatImageService
    {
        static ILog loginfo = LogManager.GetLogger(typeof(BotService));

        public static void UploadImg()
        {
            //根据appId判断获取    
            if (!AccessTokenContainer.CheckRegistered(AppSettings.Wechat_AppID))      //检查是否已经注册    
            {
                AccessTokenContainer.Register(AppSettings.Wechat_AppID, AppSettings.Wechat_Secret);   //如果没有注册则进行注册   
                loginfo.Info("进行注册"); 
            }
            string access_token = AccessTokenContainer.GetAccessTokenResult(AppSettings.Wechat_AppID).access_token;   //获取AccessToken结果 

            loginfo.Info("返回的access_token是： " + access_token);

            string media_id_img = "";     //图片media_id  

            //新增永久素材（图片）  将作为微信群发消息的封面图片  
            //UploadForeverMediaResult uploadResult_IMG = MediaApi.UploadForeverMedia(access_token, Server.MapPath(imgPathWX));
            //if (uploadResult_IMG.errcode.ToString().Contains("成功"))
            //{
            //    media_id_img = uploadResult_IMG.media_id;
            //}
            //else
            //{
            //    loginfo.Info("新增图片素材出错");
            //    return;
            //}
        }

        public static string GetAccessToken()
        {
            var ret = CommonApi.GetToken(AppSettings.Wechat_AppID, AppSettings.Wechat_Secret);

            //返回token
            return ret.access_token;
        }
    }
}