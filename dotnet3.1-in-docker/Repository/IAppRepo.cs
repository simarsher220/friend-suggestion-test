using dotnet3._1_in_docker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet3._1_in_docker.Repository
{
    public interface IAppRepo
    {
        int CreateAppUser(CreateUser createUser);
        int AddFriend(string userA, string userB);
        PendingRequest GetPendingRequests(string user);
        AllFriends GetAllFriends(string user);
        FriendSuggestion GetFriendSuggestions(string user);
        void DeleteAll();
    }
}
