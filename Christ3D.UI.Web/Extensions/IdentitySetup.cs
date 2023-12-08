using System;
using Christ3D.Infrastruct;
using Christ3D.Infrastruct.Identity.Data;
using Christ3D.Infrastruct.Identity.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Christ3D.UI.Web.Extensions
{
    /// <summary>
    /// AutoMapper 的启动服务
    /// </summary>
    public static class IdentitySetup
    {

        public static void AddIdentitySetup(this IServiceCollection services, IConfiguration Configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));


            services.AddDbContext<ApplicationDbContext>(options =>
            {
                if (Configuration.GetConnectionString("IsMysql").ObjToBool())
                {
                    options.UseMySql(DbConfig.InitConn(Configuration.GetConnectionString("DefaultConnection_file"), Configuration.GetConnectionString("DefaultConnection")));
                }
                else
                {
                    options.UseSqlServer(DbConfig.InitConn(Configuration.GetConnectionString("DefaultConnection_file"), Configuration.GetConnectionString("DefaultConnection")));
                }
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()//IdentityUser(ApplicationUser是新建的，它的子类), IdentityRole
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            //之后就可以在Controller中注入UserManager<IdentityUser>和RoleManager<IdentityRole>了
            //RoleManager<IdentityRole>用于管理角色
            //UserManager<IdentityUser>用于管理用户，也可添加用户角色、添加权限声明

            /*
             * https://www.cnblogs.com/axel10/p/9689807.html 自定义User(IdentityUser)和Role(IdentityRole)的主键类型
             * 这里需要注意的一点是，如果以后在ApplicationDbContext里注册新的类，并修改OnModelCreating方法的时候，记得调用base.OnModelCreating(modelBuilder); 不然会报错。

这时候启动网站，发现报错了。提示No service for type 'Microsoft.AspNetCore.Identity.UserManager`1[Microsoft.AspNetCore.Identity.IdentityUser]' has been registered.  意思就是UserManager<IdentityUser>没注册。因为我们之前在startup中将services.Addidentity<IdentityUser, IdentityRole>()改成了services.AddIdentity<User,Role>()，所以UserManager<IdentityUser>自然就没注册了。解决方法是全局搜索UserManager<IdentityUser>和，RoleManager<IdentityRole>，将其改成UserManager<User>和RoleManager<Role>就行了。

这时启动网站，访问登录页面，发现提示404。这也是由于我们修改了services.Addidentity<IdentityUser, IdentityRole>()导致的问题，解决方案是自己实现login和register。
             */

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/oauth2/authorize";
                options.AccessDeniedPath = "/account/access-denied";
                options.SlidingExpiration = true;
            });
        }


    }
}