using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using syp.biz.SockJS.NET.Client;
using syp.biz.SockJS.NET.Common.Interfaces;

namespace syp.biz.SockJS.NET.Test
{
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    internal class ClientTester : ITestModule
    {
        public async Task Execute()
        {
            var tcs = new TaskCompletionSource<bool>();
            try
            {
               var config = Configuration.Factory.BuildDefault("http://localhost:9999/echo");
             
           
                config.Logger = new ConsoleLogger();

                config.DefaultHeaders = new WebHeaderCollection
                {
                    //{HttpRequestHeader.UserAgent, "Custom User Agent"},
                    //{"application-key", "foo-bar"}
                  // {HttpRequestHeader.Authorization,"Basic c2FpLmthbm5lOmRXcSUyRklvSVBCNWhWdUFzVTAlMkJpU05IVk4zV2FrZUZhdmZEZG9VNVJsQkx3JTNE"}
                        {HttpRequestHeader.Authorization,"Basic c2hhbm11a2guam9nYTpETzJsWWhhcW13RFpjWlNGbTByZWVEeXF6UEJ6ZkJlUlZIJTJiMG01QXlLekUlM2Q="}
                    
                    //Basic c2hhbm11a2guam9nYTo1MGxLdE50TDQlMmZLSTIxVTRLTVpFWHZwbzRpWm0lMmJSdyUyYldvdVN0VHRBTG80JTNk

                };
                // xhr.setRequestHeader('Authorization', make_base_auth("sai.kanne", "dWq%2FIoIPB5hVuAsU0%2BiSNHVN3WakeFavfDdoU5RlBLw%3D"))
               
                var sockJs = (IClient)new Client.SockJS(config);

                sockJs.Connected += async (sender, e) =>
                {
                    try
                    {
                        Console.WriteLine("****************** Main: Open");
                        //await sockJs.Send(JsonConvert.SerializeObject(new { foo = "bar" }));
                        //await sockJs.Send("test");
                        TopicMessage topicMessage = new TopicMessage();
                        topicMessage.Topic = "/app/user/cdcevents";
                        topicMessage.Data = "4608";
                        TopicMessage topicMessage2 = new TopicMessage();
                        topicMessage2.Topic = "/app/user/wells";
                        topicMessage2.Data = "5085,5086";
                        TopicMessage topicMessage3 = new TopicMessage();
                        topicMessage3.Topic = "/app/user/tags";
                        topicMessage3.Data = "1,2,3,4";
                   
                        await SockJsExtensions.Send(sockJs, topicMessage);
                        await SockJsExtensions.Send(sockJs, topicMessage2);
                        await SockJsExtensions.Send(sockJs, topicMessage3);
                        //TopicMessage topicMessage4 = new TopicMessage();
                        //topicMessage4.Topic = "/user/queue/eventdata";
                        //await SockJsExtensions.Send(sockJs, topicMessage4);

                        // await sockJs.RecieveWithTopic(JsonConvert.SerializeObject(topicMessage4));

                       
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                };
                sockJs.Message += async (sender, msg) =>
                {
                    try
                    {
                        Console.WriteLine($"****************** Main: Message: {msg}");

                      
                            //TopicMessage topicMessage4 = new TopicMessage();
                            //topicMessage4.Topic = "/user/queue/eventdata";

                            var topicM = JsonConvert.DeserializeObject<TopicMessage>(msg);
                            // successful deserialization -> this is a TopicMessage
                         //   InvokeTopicEvent(topicMessage);
                        
                      

                        Console.WriteLine("****************** Main: Got back echo -> sending shutdown");
                       // await sockJs.Disconnect();
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                };
                sockJs.Disconnected += (sender, e) =>
                {
                    try
                    {
                        Console.WriteLine("****************** Main: Closed");
                        tcs.TrySetResult(true);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                };

                await sockJs.Connect();
               


            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
                throw;
            }

            await tcs.Task;
        }
    }
}
