using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleChat.Storage.DBO
{
    [Serializable]
    public class Dialog
    {
        public Guid Id { get; set; }
        public Guid User1 { get; set; }
        public Guid User2 { get; set; }
    }
}
