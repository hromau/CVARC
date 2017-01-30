using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CVARC.V2;

namespace Assets
{
    // MultiplayingPlayer
    public class MultiplayingPlayer
    {
        public readonly string CvarcTag;
        public readonly IWorldState PreferredWorldState;
        public readonly IMessagingClient Client;

        public MultiplayingPlayer(IMessagingClient client, string cvarcTag,
            IWorldState preferredWorldState)
        {
            this.Client = client;
            this.CvarcTag = cvarcTag;
            this.PreferredWorldState = preferredWorldState;
        }
    }
}
