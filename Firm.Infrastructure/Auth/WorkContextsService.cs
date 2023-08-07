﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firm.Infrastructure.Auth
{
    public class WorkContextsService : IWorkContext
    {
        private const string UserGuidCookiesName = "TBDUser";
        private ApplicationUser _currentUser;
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private HttpContext _httpContext;
        private readonly IConfiguration _configuration;

        public WorkContextsService(UserManager<ApplicationUser> userManager,
                           SignInManager<ApplicationUser> signInManager,
                           IHttpContextAccessor contextAccessor,
                           IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContext = contextAccessor.HttpContext;
            _configuration = configuration;
        }

        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            if (_currentUser != null)
            {
                return _currentUser;
            }

            var contextUser = _httpContext.User;
            _currentUser = await _userManager.GetUserAsync(contextUser);

            if (_currentUser != null)
            {
                return _currentUser;
            }

            var userGuid = GetUserGuidFromCookies();
            if (_currentUser != null && await _userManager.IsInRoleAsync(_currentUser, "Admin"))
            {
                return _currentUser;
            }
            return _currentUser;
        }
        public async Task<ApplicationUser> CurrentUserAsync()
        {
            if (_currentUser != null)
            {
                return _currentUser;
            }

            var contextUser = _httpContext.User;
            _currentUser = await _userManager.GetUserAsync(contextUser);

            if (_currentUser != null)
            {
                return _currentUser;
            }
            return _currentUser;
        }
        private Guid? GetUserGuidFromCookies()
        {
            if (_httpContext.Request.Cookies.ContainsKey(UserGuidCookiesName))
            {
                return Guid.Parse(_httpContext.Request.Cookies[UserGuidCookiesName]);
            }

            return null;
        }

        private void SetUserGuidCookies()
        {
            _httpContext.Response.Cookies.Append(UserGuidCookiesName, _currentUser.Id.ToString(), new CookieOptions
            {
                Expires = DateTime.UtcNow.AddYears(5),
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Strict
            });
        }

        public async Task<bool> IsUserSignedIn()
        {
            var contextUser = _httpContext.User;
            _currentUser = await _userManager.GetUserAsync(contextUser);

            if (_currentUser != null)
            {
                return true;
            }
            else
                return false;
        }
    }
}
