using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleChat.Server.Storage.DBO
{
    [Serializable]
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public uint Age { get; set; }
        public List<Guid> DialogIds { get; set; } = new List<Guid>();
    }
}
