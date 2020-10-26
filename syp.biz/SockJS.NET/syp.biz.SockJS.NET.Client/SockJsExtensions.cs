using Newtonsoft.Json;
using syp.biz.SockJS.NET.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace syp.biz.SockJS.NET.Client
{
    public static class SockJsExtensions
    {
        public static Task Send(this IClient sockjs, string topic, string data) => Send(sockjs, new TopicMessage(topic, data));
        public static Task Send(this IClient sockjs, TopicMessage message) => sockjs.SendWithTopic(message.Serialize());
    }
    public class TopicMessage
    {
        public TopicMessage()
        {
        }

        public TopicMessage(string topic, string data)
        {
            Topic = topic;
            Data = data;
        }

        public string Topic { get; set; }
        public string Data { get; set; }

       internal string Serialize() => JsonConvert.SerializeObject(this);
    }
}
