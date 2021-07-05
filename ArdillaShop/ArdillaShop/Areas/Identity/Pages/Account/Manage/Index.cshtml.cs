using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ArdillaShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArdillaShop.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public IndexModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string PhoneNumber { get; set; }


            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Gender { get; set; }
            public uint Age { get; set; }
            [Display(Name = "Telegram name")]
            public string TelegramName { get; set; }
            [Display(Name = "Facebook link")]
            public string FacebookProfile { get; set; }
        }

        private async Task LoadAsync(AppUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            AppUser appUser = new AppUser();
            var name = appUser.Name;

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }


        //private async Task LoadAsync(IdentityUser user)
        //{
        //    var userName = await _userManager.GetUserNameAsync(user);
        //    var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

        //    //AppUser appUser = new AppUser();
        //    //string name = user.Name;
        //    //string surname = user.Surname;
        //    //string age = user.Age.ToString();
        //    //string gender = user.Gender;
        //    //string fb = user.FacebookProfile;
        //    //string tg = user.TelegramName;


        //    Input = new InputModel
        //    {
        //        PhoneNumber = Input.PhoneNumber
        //        //Email = Input.Email,
        //        //Name = Input.Name,
        //        //Surname = Input.Surname,
        //        //Age = Input.Age,
        //        //Gender = Input.Gender,
        //        //TelegramName = Input.TelegramName,
        //        //FacebookProfile = Input.FacebookProfile
        //    };
        //}

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
