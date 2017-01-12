using System.IO;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using log4net;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;
using Weixin_BotApi.Service;

namespace Weixin_BotApi.Common
{
    public class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        ILog loginfo = LogManager.GetLogger(typeof(CustomMessageHandler));

        public CustomMessageHandler(Stream inputStream, PostModel postModel)
            : base(inputStream, postModel)
        {
            loginfo.Info("进入CustomMessageHandler");

            StreamReader readStream = new StreamReader(inputStream, Encoding.UTF8);
            string SourceCode = readStream.ReadToEnd();
            loginfo.Info("输入的流为：" + SourceCode);
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            loginfo.Info("进入DefaultResponseMessage");
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>(); //ResponseMessageText也可以是News等其他类型
            responseMessage.Content = "这条消息来自DefaultResponseMessage。";
            return responseMessage;
        }

        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            loginfo.Info("进入OnTextRequest");
            loginfo.Info("用户openid：" + requestMessage.FromUserName);
            loginfo.Info("发送的问题：" + requestMessage.Content);

            string botInfo = "";

            //处理简单对话
            botInfo = BotService.HandleSimpleDialog(requestMessage.Content);
            if (string.IsNullOrEmpty(botInfo))
            {
                //调用api接口，等待返回。
                botInfo = BotService.GetBotInfo(requestMessage.Content, requestMessage.FromUserName);
                loginfo.Info("接收到huaxiabot返回的内容：" + botInfo);
            }

            botInfo = BotService.HandleInvalidContent(botInfo);
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = botInfo;

            loginfo.Info("回复给用户的信息：" + responseMessage.Content);

            //如果超过5s则失败。
            //可以用客服机制解决这个问题（48小时内有效），主动推送信息。这条回复空即可。

            return responseMessage;
        }

        public override void OnExecuting()
        {
            loginfo.Info("进入OnExecuting");
            if (RequestMessage.FromUserName == "olPjZjsXuQPJoV0HlruZkNzKc91E")
            {
                CancelExcute = true; //终止此用户的对话

                //如果没有下面的代码，用户不会收到任何回复，因为此时ResponseMessage为null

                //添加一条固定回复
                var responseMessage = CreateResponseMessage<ResponseMessageText>();
                responseMessage.Content = "Hey！你已经被拉黑啦！";

                ResponseMessage = responseMessage;//设置返回对象
            }
        }
    }
}