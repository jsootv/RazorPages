using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using NetCore.Web.Data;
using NetCore.Web.Utils;

namespace NetCore.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            #region 회원 클레임 정보
            // 회원 클레임 정보
            //string claimInfo = ClaimTypes.StreetAddress;
            /// <summary>
            /// 회원 이름 First name
            /// </summary>
            [Required]
            [Display(Name ="First name")]
            public string GivenName { get; set; }

            /// <summary>
            /// 회원 성 Last name
            /// </summary>
            [Required]
            [Display(Name ="Last name")]
            public string Surname { get; set; }

            /// <summary>
            /// 회원 주소
            /// </summary>
            [Display(Name ="Contact name")]
            public string ContactName { get; set; }
            #endregion

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        /// <summary>
        /// 회원가입 성공 메시지
        /// </summary>
        private const string _registerSuccessMessage = "회원가입이 성공적으로 이루어졌습니다.";

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {
                    GivenName = Input.GivenName
                    , Surname = Input.Surname
                    , ContactName = Input.ContactName
                    , UserName = Input.Email
                    , Email = Input.Email
                    , Description = MemberDescription._register
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // 회원정보 추가
                    await _userManager.AddClaimsAsync(user, new[] {
                        new Claim(ClaimTypes.GivenName, user.GivenName)
                        , new Claim(ClaimTypes.Surname, user.Surname)
                    });

                    // 회원주소가 입력되었을 때만 주소를 회원클레임에 추가
                    if (!string.IsNullOrWhiteSpace(user.ContactName))
                    {
                        await _userManager.AddClaimAsync(user
                            , new Claim(ClaimTypes.StreetAddress, user.ContactName)
                            );
                    }

                    // 회원보유권한 준사용자 권한 추가
                    await _userManager.AddToRoleAsync(user, MemberSiteRole._associateUser);

                    // 이메일 인증코드를 가입회원 이메일로 전송하는 부분
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        TempData["Message"] = _registerSuccessMessage + "<br />이제 이메일 인증만 남았습니다.";
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        TempData["Message"] = _registerSuccessMessage;
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
