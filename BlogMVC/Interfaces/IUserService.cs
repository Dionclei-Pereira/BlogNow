using BlogMVC.Models.Exceptions;
using BlogMVC.Models;
using Microsoft.EntityFrameworkCore;
using BlogMVC.Data;

namespace BlogMVC.Interfaces {
    public interface IUserService {
        Task<User> GetUserWithPosts(string name);
        Task<User> GetUserWithAll(string name);
        Task<User> GetUserByMailNoTracking(string email);
        Task<Post> GetPostById(int id);
        Task<User> GetUserByMail(string email);
        Task<User> GetUserWithFollow(string email);
    }
}
