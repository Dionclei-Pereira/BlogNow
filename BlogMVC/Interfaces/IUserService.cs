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
        Task<User> GetUserByMail(string email);
        Task<User> GetUserWithFollow(string email);
        Task<User> GetUserWithAllAsNotTracking(string name);
    }
}
