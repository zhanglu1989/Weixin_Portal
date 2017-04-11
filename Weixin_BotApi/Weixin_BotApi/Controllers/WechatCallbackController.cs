using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities.Request;
using System.Web.Configuration;
using System.Web.Mvc;
using Weixin_BotApi.Common;
using log4net;

namespace Weixin_BotApi.Controllers
{
     public partial class WechatCallbackController : Controller
     {
        public static readonly string Token = WebConfigurationManager.AppSettings["Token"];//与微信公众账号后台的Token设置保持一致，区分大小写。
        public static readonly string EncodingAESKey = WebConfigurationManager.AppSettings["EncodingAESKey"];//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string AppId = WebConfigurationManager.AppSettings["AppID"];//与微信公众账号后台的AppId设置保持一致，区分大小写。
        ILog loginfo = LogManager.GetLogger(typeof(WechatCallbackController));

        /// <summary>
        /// 微信后台验证地址（使用Get），微信后台的“接口配置信息”的Url填写如：http://weixin.senparc.com/weixin
        /// </summary>
        [HttpGet]
         [ActionName("Index")]
         public ActionResult Get(PostModel postModel, string echostr)
         {
             if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
             {
                return Content(echostr); //返回随机字符串则表示验证通过
             }
             else
             {
                 return Content("failed:" + postModel.Signature + "," + CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, Token) + "。" +
                     "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
             }
         }
 
         /// <summary>
         /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
         /// PS：此方法为简化方法，效果与OldPost一致。
         /// v0.8之后的版本可以结合Senparc.Weixin.MP.MvcExtension扩展包，使用WeixinResult，见MiniPost方法。
         /// </summary>
         [HttpPost]
         [ActionName("Index")]
         public ActionResult Post(PostModel postModel)
         {
             if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
             {
                 return Content("参数错误！");
             }

             postModel.Token = Token;//根据自己后台的设置保持一致
             postModel.EncodingAESKey = EncodingAESKey;//根据自己后台的设置保持一致
             postModel.AppId = AppId;//根据自己后台的设置保持一致
             loginfo.Info("参数信息:" + postModel.Token + " ," + postModel.EncodingAESKey + " ," + postModel.AppId);

             //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
             var messageHandler = new CustomMessageHandler(Request.InputStream, postModel);//接收消息
            messageHandler.Execute();//执行微信处理过程

            //直接返回空串，再利用客服接口推送消息
            return null;

            //return new WeixinResult(messageHandler);//返回结果
            //return Content(messageHandler.ResponseDocument.ToString());//返回结果
        }
    }
}