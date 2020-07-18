using dotnet3._1_in_docker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace dotnet3._1_in_docker.Repository
{
    public class AppRepo : IAppRepo
    {
        public int CreateAppUser(CreateUser createUser)
        {
            using (var factory = new FriendSuggestorContextFactory())
            {
                // Get a context
                using (var context = factory.CreateContext())
                {
                    var u = context.Users.FirstOrDefault(user => user.UserName == createUser.username);
                    if (u == null)
                    {
                        var user = new User() { UserName = createUser.username };
                        FriendRequest friend = new FriendRequest();
                        friend.Users = user;
                        friend.TotalFriends = 0;
                        friend.PendingRequests = 0;
                        context.FriendRequests.Add(friend);
                        context.Users.Add(user);
                        context.SaveChanges();
                        var ik = context.Users.ToList();
                        return 1;
                    }
                    else
                        return 0;
                }
            }
        }
        public int AddFriend(string userA, string userB)
        {
            int userAId = 0;
            int userBId = 0;
            int status = 0;
            using (var factory = new FriendSuggestorContextFactory())
            {
                // Get a context
                using (var context = factory.CreateContext())
                {
                    userAId = context.Users.Where(a => a.UserName == userA).Select(a => a.UserId).FirstOrDefault();
                    userBId = context.Users.Where(a => a.UserName == userB).Select(a => a.UserId).FirstOrDefault();
                    if (userAId != 0 && userBId != 0)
                    {
                        FriendRequest fr1 = context.FriendRequests.Where(a => a.UserId == userAId).FirstOrDefault();
                        FriendRequest fr2 = context.FriendRequests.Where(a => a.UserId == userBId).FirstOrDefault();
                        FriendsList fl1 = context.FriendsLists.Where(a => a.UserF == userAId && a.UserSF == userBId).FirstOrDefault();
                        FriendsList fl2 = context.FriendsLists.Where(a => a.UserF == userBId && a.UserSF == userAId).FirstOrDefault();
                        if (fl1 == null)
                        {
                            if (fl2 != null)
                            {
                                fl2.IsFriend = true;
                                fr2.PendingRequests = fr2.PendingRequests - 1;
                                fr2.TotalFriends = fr2.TotalFriends + 1;
                                fr1.TotalFriends = fr1.TotalFriends + 1;
                            }
                            else
                            {
                                fl2 = new FriendsList();
                                fl2.UserF = userAId;
                                fl2.UserSF = userBId;
                                fl2.IsFriend = false;
                                context.FriendsLists.Add(fl2);
                                fr2.PendingRequests = fr2.PendingRequests;
                            }
                            context.SaveChanges();
                            status = 1;
                        }
                        else
                        {
                            status = 2;
                        }
                    }
                }
            }
            return status;
        }
        public PendingRequest GetPendingRequests(string user)
        {
            PendingRequest pending = new PendingRequest();
            pending.friend_requests = new List<string>();
            using (var factory = new FriendSuggestorContextFactory())
            {
                // Get a context
                using (var context = factory.CreateContext())
                {
                    User u = context.Users.Where(x => x.UserName == user).FirstOrDefault();
                    if (u != null)
                        pending.friend_requests = (from l in context.FriendsLists
                                                   join r in context.Users
                                                   on l.UserF equals r.UserId
                                                   where l.UserSF == u.UserId && l.IsFriend == false
                                                   select r.UserName).ToList();
                    else
                        pending.friend_requests = null;
                }
            }
            return pending;
        }
        public AllFriends GetAllFriends(string user)
        {
            AllFriends all = new AllFriends();
            all.friends = new List<string>();
            using (var factory = new FriendSuggestorContextFactory())
            {
                // Get a context
                using (var context = factory.CreateContext())
                {
                    User u = context.Users.Where(x => x.UserName == user).FirstOrDefault();
                    if (u != null)
                    {
                        var u1 = (from l in context.FriendsLists
                                  join r in context.Users
                                  on l.UserF equals r.UserId
                                  where l.UserSF == u.UserId && l.IsFriend == true
                                  select r.UserName).Distinct().ToList();
                        if (u1.Count > 0)
                            all.friends.AddRange(u1);
                        var u2 = (from l in context.FriendsLists
                                  join r in context.Users
                                  on l.UserSF equals r.UserId
                                  where l.UserF == u.UserId && l.IsFriend == true
                                  select r.UserName).Distinct().ToList();
                        if (u2.Count > 0)
                            all.friends.AddRange(u2);
                    }
                    else
                        all.friends = null;

                }
            }
            return all;
        }
        public FriendSuggestion GetFriendSuggestions(string user)
        {
            FriendSuggestion suggestion = new FriendSuggestion();
            suggestion.suggestions = new List<string>();
            AllFriends all = GetAllFriends(user);
            if (all.friends == null)
            {
                suggestion.suggestions = null;
                return suggestion;
            }
            foreach (var friend1 in all.friends)
            {
                var l1 = GetAllFriends(friend1);
                l1.friends.Remove(user);
                if (l1.friends.Count > 0)
                {
                    suggestion.suggestions.AddRange(l1.friends.Except(all.friends));
                    foreach (var friend2 in l1.friends)
                    {
                        var l2 = GetAllFriends(friend2);
                        l2.friends.Remove(user);
                        if (l2.friends.Count > 0)
                            suggestion.suggestions.AddRange(l2.friends.Except(all.friends));
                    }
                }
            }
            return suggestion;
        }
        public void DeleteAll()
        {
            using (var factory = new FriendSuggestorContextFactory())
            {
                // Get a context
                using (var context = factory.CreateContext())
                {
                    context.FriendRequests.RemoveRange(context.FriendRequests);
                    context.FriendsLists.RemoveRange(context.FriendsLists);
                    context.Users.RemoveRange(context.Users);
                    context.SaveChanges();
                }
            }
        }
    }
}
