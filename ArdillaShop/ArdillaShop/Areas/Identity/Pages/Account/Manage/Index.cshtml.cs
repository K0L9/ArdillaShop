using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ArdillaShop.Data;
using ArdillaShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArdillaShop.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;

        public IndexModel(
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext applicationDbContext)
        {
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            //[Phone]
            //[Display(Name = "Phone number")]
            //public string PhoneNumber { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Gender { get; set; }
            public string Email { get; set; }
            public uint Age { get; set; }

            [Display(Name="Facebook link")]
            public string FacebookProfile { get; set; }
            [Display(Name="Telegram name")]
            public string TelegramName { get; set; }
        }

        private void Load(AppUser user)
        {
            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                Age = user.Age,
                Email = user.Email,
                FacebookProfile = user.FacebookProfile,
                TelegramName = user.TelegramName
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            AppUser user2 = _applicationDbContext.AppUser.FirstOrDefault(u => u.Id == claims.Value);

            Load(user2);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            AppUser user = _applicationDbContext.AppUser.FirstOrDefault(u => u.Id == claims.Value);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{claims.Value}'.");
            }

            if (!ModelState.IsValid)
            {
                Load(user);
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}