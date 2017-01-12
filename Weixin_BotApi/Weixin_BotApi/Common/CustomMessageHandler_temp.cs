/*----------------------------------------------------------------
    Copyright (C) 2016 Senparc
    
    文件名：CustomMessageHandler.cs
    文件功能描述：自定义MessageHandler
    
    
    创建标识：Senparc - 20150312
----------------------------------------------------------------*/

using System.IO;
using System.Web.Configuration;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.MP.Helpers;
using log4net;

namespace Weixin_BotApi.Common
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public class CustomMessageHandler_temp : MessageHandler<CustomMessageContext>
    {
        /*
         * 重要提示：v1.5起，MessageHandler提供了一个DefaultResponseMessage的抽象方法，
         * DefaultResponseMessage必须在子类中重写，用于返回没有处理过的消息类型（也可以用于默认消息，如帮助信息等）；
         * 其中所有原OnXX的抽象方法已经都改为虚方法，可以不必每个都重写。若不重写，默认返回DefaultResponseMessage方法中的结果。
         */
        private string appId = WebConfigurationManager.AppSettings["AppID"];
        private string appSecret = WebConfigurationManager.AppSettings["Secret"];
        ILog loginfo = LogManager.GetLogger(typeof(CustomMessageHandler_temp));

        public CustomMessageHandler_temp(Stream inputStream, PostModel postModel, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalWeixinContext.ExpireMinutes = 3。
            WeixinContext.ExpireMinutes = 3;

            loginfo.Info("进入CustomMessageHandler");

            if (!string.IsNullOrEmpty(postModel.AppId))
            {
                loginfo.Info("appId不为空");
                appId = postModel.AppId;//通过第三方开放平台发送过来的请求
            }

            loginfo.Info("离开CustomMessageHandler方法");
            //loginfo.Info("appId为空");
            //在指定条件下，不使用消息去重
            //base.OmitRepeatedMessageFunc = requestMessage =>
            //{
            //    var textRequestMessage = requestMessage as RequestMessageText;
            //    if (textRequestMessage != null && textRequestMessage.Content == "容错")
            //    {
            //        return false;
            //    }
            //    return true;
            //};
        }

        //OnExecuting会在所有消息处理方法（如OnTextRequest，OnVoiceRequest等）执行之前执行
        public override void OnExecuting()
        {
            //测试MessageContext.StorageData
            loginfo.Info("进入OnExecuting");
            try
            {
                if (CurrentMessageContext.StorageData == null)
                {
                    CurrentMessageContext.StorageData = 0;
                }
                base.OnExecuting();
            }
            catch (System.Exception ex)
            {
                loginfo.Info("OnExecuting异常" + ex.Message);
            }
        }

        //如果OnExecuting中没有中断，当例如OnTextRequest方法执行完毕之后（或执行了默认方法），OnExecuted()方法将会触发，
        //我们也可以对应地重写。要注意的是，在OnExecuted()方法内，ResponseMessage已经被赋了返回值。
        public override void OnExecuted()
        {
            loginfo.Info("进入OnExecuted");
            base.OnExecuted();
            CurrentMessageContext.StorageData = ((int)CurrentMessageContext.StorageData) + 1;
        }

        /// <summary>
        /// 处理文字请求
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            //TODO:这里的逻辑可以交给Service处理具体信息，参考OnLocationRequest方法或/Service/LocationSercice.cs
            loginfo.Info("回复文字信息");
            var responseMessage = RequestMessage.CreateResponseMessage<ResponseMessageText>();

            responseMessage.Content = requestMessage.Content;

            loginfo.Info("回复的信息为" + responseMessage.Content);
            return responseMessage;
        }

        /// <summary>
        /// 处理事件请求（这个方法一般不用重写，这里仅作为示例出现。除非需要在判断具体Event类型以外对Event信息进行统一操作
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            /* 所有没有被处理的消息会默认返回这里的结果，
            * 因此，如果想把整个微信请求委托出去（例如需要使用分布式或从其他服务器获取请求），
            * 只需要在这里统一发出委托请求，如：
            * var responseMessage = MessageAgent.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
            * return responseMessage;
            */

            //var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            //responseMessage.Content = "这条消息来自DefaultResponseMessage。";
            //return responseMessage;
            return null;
        }
    }
}