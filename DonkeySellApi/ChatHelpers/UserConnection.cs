using Microsoft.AspNet.SignalR;

namespace DonkeySellApi.ChatHelpers
{ 
    public class UserConnection
    {
        public string Username { get; set; }

        public string ConnectionId { get; set; }
    }
}