using AutoMapper;
using EVoucher.Controllers;
using EVoucher.Lib.ViewModels;
using EVoucher.Models;
using Identity.Models;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EVoucher.Startup))]
namespace EVoucher
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            Mapper.Initialize(cfg => {
                cfg.CreateMap<Controllers.Register, BSRegisterViewModel>();
                cfg.CreateMap<ClaimProduct, ClaimProductViewModel>();
                cfg.CreateMap<Models.RegisterViewModel, ApplicationUser> ();
            });
        }
    }
}
