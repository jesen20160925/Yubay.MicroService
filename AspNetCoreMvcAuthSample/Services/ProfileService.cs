using AspNetCoreMvcAuthSample.Models;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCoreMvcAuthSample.Services
{
    //IProfileService接口，他是专门用来装载我们需要的Claim信息的，比如在token创建期间和请求用户信息终结点
    //是会调用它的GetProfileDataAsync方法来根据请求需要的Claim类型，来为我们装载信息，
    public class ProfileService : IProfileService
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationUserRole> _roleManager;

        public ProfileService(UserManager<ApplicationUser> userManager
            ,RoleManager<ApplicationUserRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        private async Task<List<Claim>> GetClaimsFromUser(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject,user.Id.ToString()),
                new Claim(JwtClaimTypes.PreferredUserName,user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach(var role in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));
            }

            if (!string.IsNullOrWhiteSpace(user.Avatar))
            {
                claims.Add(new Claim("avatar",user.Avatar));
            }

            return claims;
        }

        /// <summary>
        /// 只要有关用户的身份信息单元被请求（例如在令牌创建期间或通过用户信息终点），就会调用此方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.Claims.Where(c => c.Type == "sub").FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(subjectId);

            context.IssuedClaims = await GetClaimsFromUser(user);
        }

        /// <summary>
        /// 验证用户是否有效，例如：token创建或者验证
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = false;
            var subjectId = context.Subject.Claims.Where(c => c.Type == "sub").FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(subjectId);
            if(user != null)
            {
                context.IsActive = true;
            }
            //还可以实现用户锁定
        }
    }
}
