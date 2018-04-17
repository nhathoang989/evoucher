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
                cfg.CreateMap<Register, BSRegisterViewModel>();
                cfg.CreateMap<BSRegisterViewModel, ExportViewModel>()
                .AfterMap((src, dest) =>
                {
                    dest.Product1 = (src.Products.Count > 0 ? $"Sản phẩm đã mua: {src.Products[0].Name} - Số lượng (cặp lốp): {src.Products[0].Quantity} - Bởi: {src.Products[0].CreatedBy} - { src.Products[0].ClaimedDate.ToLocalTime()}" : string.Empty);
                    dest.Product2 = (src.Products.Count > 1 ? $"Sản phẩm đã mua: {src.Products[1].Name} - Số lượng (cặp lốp): {src.Products[1].Quantity} - Bởi: {src.Products[1].CreatedBy} - { src.Products[1].ClaimedDate.ToLocalTime()}" : string.Empty);
                    dest.CreatedDate = src.CreatedDate.ToLocalTime();
                    dest.SendCodeDate = src.SendCodeDate?.ToLocalTime();
                });
                cfg.CreateMap<ClaimProduct, ClaimProductViewModel>();
                cfg.CreateMap<Models.RegisterViewModel, ApplicationUser> ();
            });
        }
    }
}
