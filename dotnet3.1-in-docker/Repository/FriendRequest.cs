using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet3._1_in_docker.Repository
{
    public class FriendRequest
    {
        public int Id { get; set; }
        public int PendingRequests { get; set; }
        public int TotalFriends { get; set; }

        public int UserId { get; set; }
        public User Users { get; set; }
    }
}
