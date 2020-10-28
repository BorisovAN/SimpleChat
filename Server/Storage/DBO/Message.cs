using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SimpleChat.Server.Storage.DBO
{
    public class Message
    {
        //Номер сообщения в диалоге
        public ulong Id { get; set; }
        public Guid DialogId { get; set; }
        public DateTime SendingTime { get; set; }
        public string MessageText { get; set; }
    }
}
