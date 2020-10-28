using Grpc.Core;
using Grpc.Net.Client;
using SimpleChat.Proto;
using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var channel = GrpcChannel.ForAddress("https://localhost:5001"))
            {
                var client = new SimpleChat.Proto.ChatService.ChatServiceClient(channel);

                var regInfo = new RegistrationInfo { UserName = "aborisov", Age = 23, Email = "aborisov@email.com", Password = "1" };

                try
                {
                    var connectionId = client.Register(regInfo);
                }
                catch(RpcException e)
                {
                   if(e.Status.StatusCode == StatusCode.OutOfRange)
                    {
                        Console.WriteLine(e.Status.Detail);
                    }
                    else
                    {
                        throw;
                    }
                }
                //var result = client.Login(new LoginRequest() { Password = "111", UserName = "aaaa" });
            }
        }
    }
}
