using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.Contracts.Services;
using MaleFashion.Infrastructure.Extensions;
using MaleFashion.Infrastructure.Identity;
using MaleFashion.Web.Areas.Admin.Models;
using MaleFashion.Web.Codes;
using MaleFashion.Web.Models.Account;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace MaleFashion.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IRecaptchaService _recaptchaService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailService emailService,
            IMapper mapper, 
            IRecaptchaService recaptchaService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailService = emailService;
            _mapper = mapper;
            _recaptchaService = recaptchaService;
        }
        public async Task<IActionResult> Register(string? returnUrl = null)
        {
            var model = new RegisterModel();
            model.ReturnUrl = returnUrl;
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            model.ReturnUrl ??= Url.Content("~/");
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var recaptchaToken = Request.Form["g-recaptcha-response"];
                var isRecaptchaValid = await _recaptchaService.VerifyAsync(recaptchaToken, "register");
                if (!isRecaptchaValid)
                {
                    ModelState.AddModelError(string.Empty, "reCAPTCHA verification failed. Please try again.");
                    return View(model);
                }
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.DateOfBirth = model.DateOfBirth;

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Member");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        values: new { area = "", userId = userId, code = code, returnUrl = model.ReturnUrl },
                        protocol: Request.Scheme)!;

                    var fullName = $"{user.FirstName} {user.LastName}";

                    await _emailService.SendEmailAsync(fullName, model.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    if (_userManager.Options.SignIn.RequireConfirmedEmail)
                    {
                        return RedirectToAction("RegisterConfirmation", new { email = model.Email, returnUrl = model.ReturnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(model.ReturnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> RegisterConfirmation(string email, string? returnUrl = null)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            ViewData["Email"] = email;
            return View();
        }

       
        public async Task<IActionResult> ConfirmEmail(string? userId, string? code, string? returnUrl = null)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, decodedCode);

            ViewData["StatusMessage"] = result.Succeeded
                ? "Thank you for confirming your email."
                : "Error confirming your email.";

            return View();
        }
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            var model = new LoginModel();

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            model.ReturnUrl = returnUrl;

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            model.ReturnUrl ??= Url.Content("~/");

            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var recaptchaToken = Request.Form["g-recaptcha-response"];
                var isRecaptchaValid = await _recaptchaService.VerifyAsync(recaptchaToken, "login");
                if (!isRecaptchaValid)
                {
                    ModelState.AddModelError(string.Empty, "reCAPTCHA verification failed. Please try again.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(model.ReturnUrl);
                }
                
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction("LoginWith2fa", "Account", new { ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction("Lockout", "Account");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);


                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Action(
                    "ResetPassword",
                    "Account",
                    values: new { area = "", code, email = user.Email },
                    protocol: Request.Scheme)!;
                var fullName = $"{user.FirstName} {user.LastName}";
                await _emailService.SendEmailAsync(
                    fullName,
                    model.Email,
                    "Reset your MaleFashion password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            return View(model);
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        public IActionResult ResetPassword(string? code = null, string? email = null)
        {
            if (code == null || email == null)
            {
                return BadRequest("A code and email must be supplied for password reset.");
            }

            var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var model = new ResetPasswordModel
            {
                Code = decodedCode,
                Email = email
            };
            ModelState.Clear();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        _logger.LogWarning("ModelState error on {Field}: {Error}", kvp.Key, error.ErrorMessage);
                    }
                }
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {

                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} successfully reset their password.", model.Email);
                return RedirectToAction("ResetPasswordConfirmation","Account");
            }
            foreach (var error in result.Errors)
            {
                _logger.LogWarning("ResetPassword failed: {Code} - {Description}", error.Code, error.Description);
                ModelState.AddModelError(string.Empty, error.Description);
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            return View(new ChangePasswordModel());
        }

        [HttpPost,ValidateAntiForgeryToken] 
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.GetUserAsync(User);
            if (user== null)
            {
                return Challenge();
            }
            var result = await _userManager.ChangePasswordAsync(user,model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                _signInManager.RefreshSignInAsync(user);
                _logger.LogInformation("User {UserId} changed their password", user.Id);
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Your password has been changed.", Type = ResponseTypes.Success });
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                
                    return RedirectToAction("Index", "Home", new { area = "Customer" });
                
                
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }
        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }
        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
        
        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }

}
