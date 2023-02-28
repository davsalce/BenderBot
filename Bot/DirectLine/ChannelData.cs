using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace Bot.DirectLine
{
    public partial class ChannelData
    {
        public string Token { get; set; }

        private TokenResponse _tokenResponse;

        public TokenResponse TokenResponse
        {
            get
            {
                if (_tokenResponse is null && !string.IsNullOrEmpty(Token))
                {
                    _tokenResponse = new TokenResponse(token: Token);
                }
                return _tokenResponse;
            }
            set { _tokenResponse = value; }
        }

    }
}
