using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SimpleChat.Proto;
using SimpleChat.Server.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleChat.Server.Services
{
    public class ChatService : SimpleChat.Proto.ChatService.ChatServiceBase
    {
        static IStorage _storage = new FileStorage();
        static Dictionary<string, Guid> _connections = new Dictionary<string, Guid>();

        /// <summary>
        /// Возвращает ИД пользователя либо бросает ошибку, если соединения с таким ИД нет
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        Guid CheckConnection(GUID connectionId) 
        {
            if (!_connections.ContainsKey(connectionId.Value))
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Небходимо войти"));
            return _connections[connectionId.Value];
        }

        public override Task<MessagesList> GetMessages(GetMessagesRequest request, ServerCallContext context)
        {
            return base.GetMessages(request, context);
        }

        public override Task<GUID> Login(LoginRequest request, ServerCallContext context)
        {
            var userId = _storage.Authenticate(request.UserName, request.Password);

            if (userId == Guid.Empty)
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Пользователь не найден"));

            var connectionId = Guid.NewGuid().ToString();
            _connections.Add(connectionId, userId);

            return Task.FromResult(new GUID { Value = connectionId });
        }

        public override Task<GUID> Register(RegistrationInfo info, ServerCallContext context)
        {
            Guid userId;
            string connectionId;
            try
            {
                userId = _storage.Register(info);
            }
            catch(ArgumentException e)
            {
                throw new Grpc.Core.RpcException(new Status(StatusCode.OutOfRange, e.Message));
            }
            connectionId = Guid.NewGuid().ToString();
            _connections.Add(connectionId, userId);
            return Task<GUID>.FromResult(new GUID {Value = connectionId});
        }

        public override Task<Empty> SendMessage(SendMessageRequest request, ServerCallContext context)
        {
            //вызвать storage.CreateDialog, затем storage.SaveMessage
            return base.SendMessage(request, context);
        }

        public override Task<Empty> UpdateUserInfo(UpdateUserInfoReqiest request, ServerCallContext context)
        {
            return base.UpdateUserInfo(request, context);
        }

        public override Task<UserInfo> GetUserInfo(GetUserInfoRequest request, ServerCallContext context)
        {
            return base.GetUserInfo(request, context);
        }

        public override Task<DialogsList> GetDialogs(GetDialogsRequest request, ServerCallContext context)
        {
            return base.GetDialogs(request, context);
        }
    }
}
