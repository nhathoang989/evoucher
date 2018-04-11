using AutoMapper;
using EVoucher.Lib;
using EVoucher.Lib.Models.EVoucher;
using EVoucher.Lib.ViewModels;
using Identity.Models;
using Newtonsoft.Json;
using Swastika.Base.ViewModels;
using Swastika.Common.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http;
using static Swastika.Common.Utility.Enums;

namespace EVoucher.Controllers
{
    [RoutePrefix("api/e-voucher")]
    public class ApiEvoucherController : BaseApiController
    {
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("users")]
        public RepositoryResponse<PaginationModel<UserViewModel>> Users(RequestPaging request)
        {
            var users = UserViewModel.Repository.GetModelListBy(r =>
                    !r.AspNetRoles.Any(ur => ur.Name == "Admin") &&
                    (
                        string.IsNullOrEmpty(request.Keyword) ||
                        (
                            r.Fullname.Contains(request.Keyword)
                            || r.Fullname.Contains(request.Keyword)
                            || r.Address.Contains(request.Keyword)
                            || r.UserName.Contains(request.Keyword)
                        )
                   ),
                    request.OrderBy, request.Direction,
                    request.PageSize, request.PageIndex
                );
            return users;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("remove-user")]
        public async Task<RepositoryResponse<bool>> RemoveUser(ApplicationUser manager)
        {
            var user = await UserViewModel.Repository.GetSingleModelAsync(u => u.Id == manager.Id);
            if (user.IsSucceed)
            {
                var result = await UserManager.RemoveFromRolesAsync(manager.Id, new string[] { "Admin", "Manager" });
                return await user.Data.RemoveModelAsync();
            }
            return new RepositoryResponse<bool>();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("import-users")]
        public async Task<RepositoryResponse<List<UserViewModel>>> ImportUsersAsync(ImportUsers file)
        {

            List<int> failed = new List<int>();
            List<string> strUsers = Swastika.Common.Helper.CommonHelper.LoadImportExcelRecords(file.Base64.Split(',')[1], 3, 2, 4, out failed);
            bool isSucceed = true;
            foreach (var item in strUsers)
            {
                string[] strUser = item.Split('|');
                string password = strUser[1];
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = strUser[0],
                    Email = $"{strUser[0]}@khuyenmaibridgestone.com",
                    Fullname = strUser[2],
                    Address = strUser[3]
                };
                var result = await UserManager.CreateAsync(user, password);
                isSucceed = isSucceed && result.Succeeded;
            }
            if (isSucceed)
            {
                return UserViewModel.Repository.GetModelList();
            }
            else
            {
                return new RepositoryResponse<List<UserViewModel>>();
            }
        }

        [HttpPost]
        [Route("register/list")]
        public RepositoryResponse<PaginationModel<BSRegisterViewModel>> Registers(RequestPaging request)
        {
            var conditions = GetConditions(request);
            var users = BSRegisterViewModel.Repository.GetModelListBy(
                conditions,
                    request.OrderBy, request.Direction,
                    request.PageSize, request.PageIndex
                );
            return users;
        }


        [HttpPost]
        [Route("register/list/{claimed:int}")]
        public RepositoryResponse<PaginationModel<BSRegisterViewModel>> Registers(RequestPaging request, int claimed)
        {
            var conditions = GetConditions(request);
            var users = BSRegisterViewModel.Repository.GetModelListBy(
                conditions,
                    request.OrderBy, request.Direction,
                    null, request.PageIndex
                );
            users.Data.Items = users.Data.Items.Where(r => r.Products.Sum(p=>p.Quantity) == claimed).ToList();
            users.Data.TotalItems = users.Data.Items.Count;
            return users;
        }

        public Expression<Func<BridgeStone_Register, bool>> GetConditions(RequestPaging request)
        {
            DateTime? from = request.FromDate.HasValue ? new DateTime(request.FromDate.Value.Year, request.FromDate.Value.Month, request.FromDate.Value.Day).ToUniversalTime()
                : default(DateTime?);
            DateTime? to = request.ToDate.HasValue ? new DateTime(request.ToDate.Value.Year, request.ToDate.Value.Month, request.ToDate.Value.Day).ToUniversalTime().AddDays(1)
                : default(DateTime?);
            bool isAdmin = User.IsInRole("Admin");
            Expression<Func<BridgeStone_Register, bool>> conditions;
            if (isAdmin)
            {
                    conditions =
                   r =>
                      r.Status != (int)SWStatus.Deleted &&
                      (
                           (string.IsNullOrEmpty(request.Key) || r.Code == request.Key)
                      &&
                       (string.IsNullOrEmpty(request.Keyword) || r.Phone == request.Keyword)
                      && (!from.HasValue
                       || (r.CreatedDate >= from.Value)
                       )
                       && (!to.HasValue
                           || (r.CreatedDate <= to.Value)

                       ));
            }
            else
            {
                conditions =
               r =>
                  r.Status != (int)SWStatus.Deleted &&
                  (
                       (r.Code == request.Key)
                  ||
                   (r.Phone == request.Keyword)
                   );
            }
            return conditions;
            
        }

        [HttpPost]
        [Route("register/export")]
        public RepositoryResponse<string> ExportRegisters(RequestPaging request)
        {
            var conditions = GetConditions(request);
            var result = BSRegisterViewModel.Repository.GetModelListBy(
                conditions,
                    request.OrderBy, request.Direction,
                    request.PageSize, request.PageIndex
                );

            string FolderPath = "/Exports";
            List<Register> lstData = new List<Register>();
            foreach (var item in result.Data.Items)
            {
                lstData.Add(Mapper.Map<Register>(item));
            }
            string SavedPath = CommonHelper.ExportToExcel(lstData, "Registers", FolderPath, "BridgeStone_registers_", out string error);
            return new RepositoryResponse<string>()
            {
                IsSucceed = !string.IsNullOrEmpty(SavedPath),
                Data = SavedPath
            };
        }


        [HttpPost]        
        [Route("register/export/{claimed:int}")]
        public RepositoryResponse<string> ExportRegisters(RequestPaging request, int claimed)
        {
            var conditions = GetConditions(request);
            var users = BSRegisterViewModel.Repository.GetModelListBy(
                conditions,
                    request.OrderBy, request.Direction,
                    null, request.PageIndex
                );
            users.Data.Items = users.Data.Items.Where(r => r.Products.Sum(p => p.Quantity) == claimed).ToList();
            users.Data.TotalItems = users.Data.Items.Count;

            string FolderPath = "/Exports";
            List<Register> lstData = new List<Register>();
            foreach (var item in users.Data.Items)
            {
                lstData.Add(Mapper.Map<Register>(item));
            }
            string SavedPath = CommonHelper.ExportToExcel(lstData, "Registers", FolderPath, "BridgeStone_registers_", out string error);
            return new RepositoryResponse<string>()
            {
                IsSucceed = !string.IsNullOrEmpty(SavedPath),
                Data = SavedPath
            };
        }

        [HttpGet]
        [Route("init-register")]
        public BSRegisterViewModel InitRegister()
        {
            return new BSRegisterViewModel();
        }

        [HttpPost]
        [Route("register/save")]
        public async Task<RepositoryResponse<BSRegisterViewModel>> SubmitRegisterAsync(Register model)
        {
            List<string> errors = new List<string>();
            if (model != null && !string.IsNullOrEmpty(model.Phone)
                && !BSRegisterViewModel.Repository.CheckIsExists(r => r.Phone == model.Phone && r.Status != (int)SWStatus.Deleted))
            {
                model.Code = BSHelper.GenerateCode();

                while (BSRegisterViewModel.Repository.CheckIsExists(r => r.Code == model.Code))
                {
                    model.Code = BSHelper.GenerateCode();
                }

                model.SendCodeStatus = await BSHelper.SendMessage(model.Phone, model.Code);
                model.SendCodeDate = DateTime.UtcNow;
                var register = Mapper.Map<BSRegisterViewModel>(model);
                register.Status = SWStatus.Preview;
                return await register.SaveModelAsync();
            }
            else
            {
                errors.Add("Số điện thoại này đã được đăng ký");
            }
            return new RepositoryResponse<BSRegisterViewModel>()
            {
                IsSucceed = false,
                Data = null,
                Errors = errors
            };

        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("register/delete")]
        public async Task<RepositoryResponse<BSRegisterViewModel>> DeleteRegisterAsync(Register model)
        {
            List<string> errors = new List<string>();
            if (model != null)
            {
                var register = await BSRegisterViewModel.Repository.GetSingleModelAsync(r => r.Id == model.Id);
                if (register.IsSucceed)
                {
                    register.Data.Status = SWStatus.Deleted;
                    register.Data.UpdatedDate = DateTime.UtcNow;
                    return await register.Data.SaveModelAsync();
                }
            }
            else
            {
                errors.Add("Không tìm thấy!");
            }
            return new RepositoryResponse<BSRegisterViewModel>()
            {
                IsSucceed = false,
                Errors = errors
            };

        }

        [HttpPost]
        [Route("claim")]
        public async Task<RepositoryResponse<ClaimProductViewModel>> ClaimProduct(ClaimProduct model)
        {
            List<string> errors = new List<string>();
            if (model != null)
            {
                var vm = Mapper.Map<ClaimProductViewModel>(model);
                vm.CreatedBy = User.Identity.Name;
                vm.ClaimedDate = DateTime.UtcNow;
                return await vm.SaveModelAsync();
            }

            errors.Add("Vui lòng nhập đầy đủ thông tin!");
            return new RepositoryResponse<ClaimProductViewModel>()
            {
                Errors = errors
            };
        }
    }

    public class ImportUsers
    {
        [JsonProperty("strBase64")]
        public string Base64 { get; set; }
    }
    public class ClaimProduct
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
        [JsonProperty("codeVoucher")]
        public string CodeVoucher { get; set; }
    }
    public class Register
    {
        #region Models

        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("fullname")]
        public string Fullname { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; }
        [JsonProperty("automaker")]
        public string Automaker { get; set; }
        [JsonProperty("carModel")]
        public string CarModel { get; set; }
        [JsonProperty("license")]
        public string License { get; set; }
        [JsonProperty("createdDate")]
        public System.DateTime CreatedDate { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("sendCodeStatus")]
        public string SendCodeStatus { get; set; }
        [JsonProperty("updatedDate")]
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        [JsonProperty("sendCodeDate")]
        public Nullable<System.DateTime> SendCodeDate { get; set; }

        #endregion
    }
}
