// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Expense_Tracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Expense_Tracker.Areas.Identity.Pages.Account
{
    /// <summary>
    /// This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    /// directly from your code. This API may change or be removed in future releases.
    /// </summary>
   public class AccountModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "New Username")]
        public string NewUsername { get; set; }

        [Display(Name = "Profile Picture")]
        public byte[] ProfilePicture { get; set; }
        }

    public async Task<IActionResult> OnGet()
    {
        var user = await _userManager.GetUserAsync(User);

            Input = new InputModel
            {

                ProfilePicture = user.profilePicture
            };

            if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        ViewData["Username"] = user.UserName;
        ViewData["Email"] = user.Email;

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        // Update the username
        user.UserName = Input.NewUsername;
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files.FirstOrDefault();

                //check file size and extension

                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    user.profilePicture = dataStream.ToArray();
                }

            }
            var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        // Redirect to the updated user information page
        return RedirectToPage();
    }
}
}
