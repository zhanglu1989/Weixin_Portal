using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using Weixin_BotApi.Models;

namespace Weixin_BotApi.Service
{
    public class WeatherService
    {
        private static readonly string key = "fe9e46d929f4ab31664defcdb4efdfc0";

        public static string GetWeatherAPI(string location)
        {
            string url = string.Format("http://v.juhe.cn/weather/index?key={0}&dtype=&format=&cityname={1}", key, location);
            WebRequest request = WebRequest.Create(url);
            request.Method = "Get";

            //读取返回消息
            string res = string.Empty;
            string result = string.Format("Hi, 接下来让萌萌告诉你今天{0}的天气：" + "\r\n", location);

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                res = reader.ReadToEnd();
                reader.Close();

                //解析json
                JObject jsonObj = JObject.Parse(res);
                var resultJson = jsonObj["result"];
                var today = resultJson["today"];

                WeatherInfoModel weatherModel = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherInfoModel>(today.ToString());

                //foreach (var item in plist)
                //{
                //    result += item.des + "\r\n";
                //}

                result += "今天日期为" + weatherModel.date_y + ",温度为:" + weatherModel.temperature + "," + weatherModel.weather
                    + "," + weatherModel.wind;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return result;
        }
    }
}