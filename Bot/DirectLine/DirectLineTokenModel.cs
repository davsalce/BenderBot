using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.DirectLine
{
    public class DirectLineTokenModel
    {
        public string ConversationId { get; set; }
        public string Token { get; set; }
        public int Expires_in { get; set; }
    }
}
