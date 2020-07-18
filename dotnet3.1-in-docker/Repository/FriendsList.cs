using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet3._1_in_docker.Repository
{
    public class FriendsList
    {
        public int Id { get; set; }
        public bool IsFriend { get; set; }

        public int UserF { get; set; }
        public int UserSF { get; set; }
    }
}
