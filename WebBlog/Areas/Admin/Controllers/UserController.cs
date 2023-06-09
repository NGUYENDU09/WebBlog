﻿using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBlog.Models;
using WebBlog.ViewModels;

namespace WebBlog.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INotyfService _notyfService;

        public UserController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, INotyfService notyfService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notyfService = notyfService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("Login")]
        public IActionResult Login()
        {
            if (!HttpContext.User.Identity!.IsAuthenticated) //kiểm tra xem người dùng đã xác thực(đăng nhập) hay chưa
            {
                return View(new LoginVM());
            }
            return RedirectToAction("Index", "User", new { area = "Admin" });
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }
            var existingUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginVM.UserName);
            if(existingUser == null)
            {
                _notyfService.Error("Username does not exist");
                return View(loginVM);
            }
            var verifyPassword = await _userManager.CheckPasswordAsync(existingUser, loginVM.Password);
            if (!verifyPassword)
            {
                _notyfService.Error("Password does not match");
                return View(loginVM);
            }
            await _signInManager.PasswordSignInAsync(loginVM.UserName, loginVM.Password, loginVM.RememberMe, true);
            _notyfService.Success("Login Successful");
            return RedirectToAction("Index", "User", new { area = "Admin" });
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            _notyfService.Success("You are logged successfully");
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
 