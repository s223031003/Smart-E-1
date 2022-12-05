using Microsoft.AspNetCore.SignalR;

namespace Smart_E.Data
{
    public class ChatRoom
    {
        public Guid Id { get; set; }

        public DateTime DateTime { get; set; }

        public string UserId { get; set; }

        public string Comment { get; set; }
    }
}
