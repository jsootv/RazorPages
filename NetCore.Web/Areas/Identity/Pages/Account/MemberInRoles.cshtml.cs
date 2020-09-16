using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetCore.Web.Data;
using NetCore.Web.Utils;

namespace NetCore.Web.Areas.Identity.Pages.Account
{
    /// <summary>
    /// 향상된 사용자와 시스템사용자만 페이지에 접근할 수 있도록 허용.
    /// </summary>
    [Authorize(Roles =MemberSiteRole._superUser+","+MemberSiteRole._systemUser)]
    public class MemberInRolesModel : PageModel
    {
        /// <summary>
        /// 회원 관리자
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// 권한 관리자
        /// </summary>
        private readonly RoleManager<ApplicationRole> _roleManager;

        public MemberInRolesModel(UserManager<ApplicationUser> userManager
                                    , RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// 로그인 회원 정보
        /// </summary>
        public ApplicationUser _loginMember { get; set; }

        /// <summary>
        /// 회원 리스트
        /// </summary>
        public IEnumerable<ApplicationUser> _members { get; set; }

        /// <summary>
        /// 권한 리스트
        /// </summary>
        public IEnumerable<ApplicationRole> _roles { get; set; }

        /// <summary>
        /// 회원검색정보
        /// </summary>
        [BindProperty]  //Get에서 Post로 넘어가는 바인딩 모델로 사용하기 위해
        public SearchModel Search { get; set; }

        /// <summary>
        /// 회원 검색모델
        /// </summary>
        public class SearchModel
        {
            public SearchModel() : this(null)
            {
            }

            public SearchModel(string memberEmail)
            {
                _memberEmail = memberEmail;
            }

            /// <summary>
            /// 회원 이메일
            /// </summary>
            [Display(Name ="회원 이메일")]
            public string _memberEmail { get; set; }
        }

        /// <summary>
        /// 회원변경 정보
        /// </summary>
        [BindProperty]
        public ChangeRoleModel ChangeRole { get; set; } = new ChangeRoleModel();

        /// <summary>
        /// 회원정보 모델
        /// </summary>
        public class ChangeRoleModel
        {
            /// <summary>
            /// 권한변경 회원 이메일
            /// </summary>
            [Required]
            public string _memberEmail { get; set; }

            /// <summary>
            /// 기존 보유권한명
            /// </summary>
            [Required]
            public string _originalRoleName { get; set; }

            /// <summary>
            /// 변경 권한명
            /// </summary>
            [Required]
            public string _changeRoleName { get; set; }
        }

        /// <summary>
        /// 상태 메시지
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            await SearchListAsync(new SearchModel(), User);
        }

        /// <summary>
        /// 회원 검색리스트
        /// </summary>
        /// <param name="search">회원검색모델</param>
        /// <param name="user">로그인 회원의 Claim Identity 정보</param>
        /// <returns></returns>
        public async Task SearchListAsync(SearchModel search, ClaimsPrincipal user)
        {
            var httpMethod = this.HttpContext.Request.Method;// GET, POST

            if (httpMethod == "GET")
            {
                Search = search;
            }

            #region 로그인 회원 정보 가져오기
            // 로그인 회원 정보
            _loginMember = await _userManager.FindByNameAsync(user.Identity.Name);
            _loginMember.MemberInRole = await _roleManager.FindByNameAsync(user.FindFirst(ClaimTypes.Role).Value);
            #endregion

            #region 회원리스트
            _members = _userManager.Users.ToList();

            // 회원 이메일 값으로 검색할 때
            // 필터링한다.
            if (!string.IsNullOrWhiteSpace(search._memberEmail))
            {
                _members = _members.Where(m => m.UserName.Contains(search._memberEmail));
            }

            foreach (ApplicationUser member in _members)
            {
                //회원 유형
                //member.Description
                //회원 클레임
                IList<Claim> claims = await _userManager.GetClaimsAsync(member);

                foreach (Claim claim in claims)
                {
                    // 클레임 유형
                    //claim.Type
                    // 클레임 값
                    //claim.Value
                    if (claim.Type == ClaimTypes.GivenName)
                    {
                        member.GivenName = claim.Value;
                    }
                    else if (claim.Type == ClaimTypes.Surname)
                    {
                        member.Surname = claim.Value;
                    }
                }

                //회원 성명
                //member.GivenName, member.Surname
                //회원 이메일
                //member.UserName
                // 회원 보유권한 정보
                // 회원 보유권한이름들
                IList<string> memberInRoles = await _userManager.GetRolesAsync(member);
                member.MemberInRole = await _roleManager.FindByNameAsync(memberInRoles.FirstOrDefault());
                //권한명
                //member.MemberInRole.Name
                //권한 설명
                //member.MemberInRole.Description
                //권한 변경 금지여부
                member.IsTheMemberWhoCannotChangeRole = IsTheMemberWhoCannotChangeRole(member.UserName);
            }

            // 회원리스트 : 권한 우선순위 내림차순, 회원 이메일 오름차순 정렬
            _members = _members.OrderByDescending(m => m.MemberInRole.RolePriority)
                               .ThenBy(m => m.UserName);
            #endregion

            #region 권한리스트
            _roles = _roleManager.Roles
                                 // 준사용자(1) 권한으로 변경하는 것은 의미가 없다.
                                 // 원활한 사이트 이용을 위해서 일반사용자(2) 이상의 권한이 필요하다.
                                 .Where(r => r.RolePriority > 1
                                        && r.RolePriority <= _loginMember.MemberInRole.RolePriority)
                                 // 권한 우선순위 내림차순 정렬
                                 .OrderByDescending(r => r.RolePriority)
                                 .ToList();
            #endregion
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //OnGetAsync 내용과 동일
            //Get과 Post의 공통내용을 메서드로 분리
            await SearchListAsync(Search, User);

            return Page();
        }

        public async Task<IActionResult> OnPostChangeRoleAsync()
        {
            if (string.IsNullOrWhiteSpace(ChangeRole._memberEmail))
            {
                StatusMessage = "권한을 변경할 회원의 이메일 정보가 올바르지 않습니다.";
                return Page();
            }

            ApplicationUser member = await _userManager.FindByNameAsync(ChangeRole._memberEmail);

            if (ModelState.IsValid)
            {
                // 기존 보유권한 삭제
                await _userManager.RemoveFromRoleAsync(member, ChangeRole._originalRoleName);

                // 변경 권한 추가
                await _userManager.AddToRoleAsync(member, ChangeRole._changeRoleName);

                ApplicationRole originalRole = await _roleManager.FindByNameAsync(ChangeRole._originalRoleName);
                ApplicationRole changeRole = await _roleManager.FindByNameAsync(ChangeRole._changeRoleName);

                StatusMessage = $"{ApplyRedFontStyle(member.UserName)}회원의 권한이 {ApplyRedFontStyle(originalRole.Description)}에서 {ApplyRedFontStyle(changeRole.Description)}(으)로 변경되었습니다.";
                return RedirectToPage("/Account/MemberInRoles", new { Area = "Identity" });
            }

            StatusMessage = $"{ApplyRedFontStyle(member.UserName)}회원의 권한변경이 이루어지지 않았습니다.";
            return Page();
        }

        /// <summary>
        /// 빨간 폰트 스타일 입히기
        /// </summary>
        /// <param name="itemName">항목명</param>
        /// <returns></returns>
        private string ApplyRedFontStyle(string itemName)
        {
            return $"<span class=\"text-danger\">{itemName}</span>";
        }

        /// <summary>
        /// 권한변경이 금지된 회원인가?(true:금지됨, false:금지안됨)
        /// </summary>
        /// <param name="memberEmail">회원 이메일</param>
        /// <returns></returns>
        private bool IsTheMemberWhoCannotChangeRole(string memberEmail)
        {
            List<string> membersWhoCannotChangeRole = new List<string>();
            membersWhoCannotChangeRole.Add("admin@netcore.com".ToLower());

            return membersWhoCannotChangeRole.Where(m => m.Contains(memberEmail.ToLower())).Any();
        }
    }
}