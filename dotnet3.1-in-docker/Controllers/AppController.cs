using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
using dotnet3._1_in_docker.Models;
using dotnet3._1_in_docker.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace dotnet3._1_in_docker.Controllers
{
    //[Route("[controller]")]
    [ApiController]
    public class AppController : ControllerBase
    {

        private readonly ILogger<AppController> _logger;
        private IAppRepo _repo;
        public AppController(ILogger<AppController> logger)
        {
            _logger = logger;
            _repo = new AppRepo();
        }
        [Route("create")]
        [HttpPost]
        public IActionResult Create(CreateUser user)
        {
            _logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(user));
            try
            {
                if (_repo.CreateAppUser(user) == 1)
                    return Created("User Created", user);
                else
                    return BadRequest(new AppErrorResponse { status = "failure", reason = "User exists with the same name" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.InnerException);
                return BadRequest(new AppErrorResponse { status = "failure", reason = "User exists with the same name" });
            }
        }
        [Route("add/{userA}/{userB}")]
        [HttpPost]
        public IActionResult AddFriend(string userA, string userB)
        {
            _logger.LogInformation(userA + " " + userB);
            try
            {
                int status = _repo.AddFriend(userA, userB);
                _logger.LogInformation(status.ToString());
                switch (status)
                {
                    case 0:
                        return BadRequest(new AppErrorResponse { status = "failure", reason = "User does not exists" });
                    case 1:
                        return Accepted(new AppSuccessResponse { status = "success" });
                    case 2:
                        return BadRequest(new AppErrorResponse { status = "failure", reason = "User does not exists" });
                    default:
                        return BadRequest(new AppErrorResponse { status = "failure", reason = "User does not exists" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.InnerException);
                return BadRequest(new AppErrorResponse());
            }
        }
        [Route("friendRequests/{user}")]
        [HttpGet]
        public IActionResult GetPendingRequests(string user)
        {
            try
            {
                PendingRequest pending = _repo.GetPendingRequests(user);
                if (pending.friend_requests == null)
                    return BadRequest(new AppErrorResponse { status = "failure", reason = "User does not exist" });
                else if (pending.friend_requests.Count() == 0)
                    return NotFound(new AppErrorResponse { status = "failure", reason = "User does not have any requests" });
                else
                    return Ok(pending);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.InnerException);
                return BadRequest(new AppErrorResponse());
            }
        }
        [Route("friends/{user}")]
        [HttpGet]
        public IActionResult GetAllFriends(string user)
        {
            try
            {
                AllFriends all = _repo.GetAllFriends(user);
                if (all.friends == null)
                    return BadRequest(new AppErrorResponse { status = "failure", reason = "User does not exist" });
                else if (all.friends.Count() == 0)
                    return NotFound(new AppErrorResponse { status = "failure", reason = "User does not have any friends" });
                else
                {
                    all.friends = all.friends.Distinct().ToList();
                    return Ok(all);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.InnerException);
                return BadRequest(new AppErrorResponse());
            }
        }
        [Route("suggestions/{user}")]
        [HttpGet]
        public IActionResult GetFriendSuggestions(string user)
        {
            try
            {
                FriendSuggestion suggestion = _repo.GetFriendSuggestions(user);
                if (suggestion.suggestions == null)
                    return BadRequest(new AppErrorResponse { status = "failure", reason = "User does not exist" });
                else if (suggestion.suggestions.Count() == 0)
                    return NotFound(new AppErrorResponse { status = "failure", reason = "User does not have any friends" });
                else
                {
                    suggestion.suggestions.Remove(user);
                    suggestion.suggestions = suggestion.suggestions.Distinct().ToList();
                    return Ok(suggestion);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.InnerException);
                return BadRequest(new AppErrorResponse());
            }
        }
        [Route("deleteAll")]
        [HttpPost]
        public IActionResult DeleteDb()
        {
            try
            {
                _repo.DeleteAll();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.InnerException);
                return BadRequest(new AppErrorResponse());
            }
        }
    }
}
