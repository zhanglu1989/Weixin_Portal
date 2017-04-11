using System.IO;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using log4net;
using System.Text;
using Weixin_BotApi.Service;
using System.Threading.Tasks;
using System;

namespace Weixin_BotApi.Common
{
    public class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        ILog loginfo = LogManager.GetLogger(typeof(CustomMessageHandler));

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="postModel"></param>
        public CustomMessageHandler(Stream inputStream, PostModel postModel)
            : base(inputStream, postModel)
        {
            loginfo.Info("进入CustomMessageHandler");

            StreamReader readStream = new StreamReader(inputStream, Encoding.UTF8);
            string SourceCode = readStream.ReadToEnd();
            loginfo.Info("输入的流为：" + SourceCode);
        }

        /// <summary>
        /// 默认信息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            loginfo.Info("进入DefaultResponseMessage");
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>(); //ResponseMessageText也可以是News等其他类型
            responseMessage.Content = "对不起，小夏现在只能支持文字查询，不过小夏在很努力的学习中。";
            return responseMessage;
        }

        /// <summary>
        /// 处理用户文字信息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            try
            {
                //开启task, 实现客服接口异步回复消息。       
                Task.Factory.StartNew(() => GetBotAsyn(requestMessage));
                return null;
            }
            catch (Exception ex)
            {
                loginfo.Info("异常---------" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 处理事件信息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEventRequest(IRequestMessageEventBase requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您好，欢迎您关注华夏基金小夏，小夏非常乐意为您服务。";

            return responseMessage;
        }

        /// <summary>
        /// 在触发OnTextRequest之前进入该方法
        /// </summary>
        public override void OnExecuting()
        {
            loginfo.Info("进入OnExecuting");
            base.OnExecuting();
            //if (RequestMessage.FromUserName == "olPjZjsXuQPJoV0HlruZkNzKc91E")
            //{
            //    CancelExcute = true; //终止此用户的对话

            //    //如果没有下面的代码，用户不会收到任何回复，因为此时ResponseMessage为null

            //    //添加一条固定回复
            //    var responseMessage = CreateResponseMessage<ResponseMessageText>();
            //    responseMessage.Content = "Hey！你已经被拉黑啦！";

            //    ResponseMessage = responseMessage;//设置返回对象
            //}
        }

        /// <summary>
        /// 触发OnTextRequest后进入该方法
        /// </summary>
        public override void OnExecuted()
        {
            loginfo.Info("进入OnExecuted");
            base.OnExecuted();
        }

        /// <summary>
        /// 异步获取信息
        /// </summary>
        private void GetBotAsyn(RequestMessageText requestMessage)
        {
            try
            {
                loginfo.Info("开启异步线程");
                string tableResponse = "";
                string faqResponse = "";
                string finalResponse = "";

                //第一步：处理简单对话
                tableResponse = BotTableService.HandleSimpleDialog(requestMessage.Content);

                //第二步：进入table engine，并将结果存入数据库
                if (string.IsNullOrEmpty(tableResponse))
                {
                    tableResponse = BotTableService.GetEngineResponse(requestMessage.Content, requestMessage.FromUserName);
                    loginfo.Info("接收到table engine返回的内容：" + tableResponse);
                }
                tableResponse = BotTableService.HandleInvalidContent(tableResponse);

                //第三步：进入faq engine，并将结果存入数据库
                faqResponse = BotFQAService.GetEngineResponse(requestMessage.Content, requestMessage.FromUserName);
                loginfo.Info("接收到faq engine返回的内容：" + faqResponse);

                //第四步：返回结果。
                if (tableResponse.Equals("FAQ"))
                {
                    finalResponse = faqResponse;
                }
                else
                {
                    finalResponse = tableResponse;
                }

                var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
                responseMessage.Content = finalResponse;
                loginfo.Info("回复给用户的信息：" + responseMessage.Content);
                //获取access token
                string accessToken = CommonService.GetAccessToken();
                //发送客服消息
                CustomerService.SendText(accessToken, requestMessage.FromUserName, finalResponse);
            }
            catch (System.Exception ex)
            {
                loginfo.Info("异常--" + ex.Message + "---" + ex.StackTrace);
            }
        }
    }
}