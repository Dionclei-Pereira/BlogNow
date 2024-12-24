using BlogMVC.Models.Exceptions;
using BlogMVC.Models;
using Microsoft.EntityFrameworkCore;
using BlogMVC.Data;
using BlogMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BlogMVC.Interfaces {
    public interface IUserService {
        Task<User> GetUserWithPosts(string name);
        Task<User> GetUserWithAll(string name);
        Task<User> GetUserByMailNoTracking(string email);
        Task<Post> GetPostById(int id);
        Task<User> GetUserByMail(string email);
        Task<User> GetUserWithFollow(string email);
        Task<User> GetUserWithAllAsNotTracking(string name);
        Task<List<FollowingModel>> GetFollowingByUserId(string id);
        Task<List<FollowedModel>> GetFollowedByUserId(string id);
        Task<List<Post>> GetAllPosts();
        Task RemovePost(Post post);
        Task AddPost(CreateViewModel model);
        Task<object> ToggleLike(int postID, string email, string authenticatedEmail);
    }
}
