using Identity.Models;
using Swastika.Base.ViewModels;
using System.Collections.Generic;
using EVoucher.Lib.Models.EVoucher;
using System.Data.Entity;
using Newtonsoft.Json;
using System;
using System.Linq;

using EVoucher.Lib.Models;

namespace EVoucher.Lib.ViewModels
{
    public class UserViewModel : ViewModelBase<EvoucherEntities, AspNetUser, UserViewModel>
    {
        #region Properties
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("emailConfirmed")]
        public bool EmailConfirmed { get; set; }
        [JsonIgnore]
        public string PasswordHash { get; set; }
        [JsonIgnore]
        public string SecurityStamp { get; set; }
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }
        [JsonProperty("phoneNumberConfirmed")]
        public bool PhoneNumberConfirmed { get; set; }
        [JsonIgnore]
        public bool TwoFactorEnabled { get; set; }
        [JsonIgnore]
        public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
        [JsonIgnore]
        public bool LockoutEnabled { get; set; }
        [JsonIgnore]
        public int AccessFailedCount { get; set; }
        [JsonProperty("userName")]
        public string UserName { get; set; }
        [JsonProperty("fullname")]
        public string Fullname { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        #region Models

        #endregion

        #region Views
        [JsonProperty("roles")]
        public List<UsersRoleModel> Roles { get; set; }

        #endregion

        #endregion

        #region Contructors

        public UserViewModel() : base()
        {
        }

        public UserViewModel(AspNetUser model, EvoucherEntities _context = null, DbContextTransaction _transaction = null)
            : base(model, _context, _transaction)
        {
        }

        #endregion

        #region Overrides

        public override void ExpandView(EvoucherEntities _context = null, DbContextTransaction _transaction = null)
        {
            var roles = _context.AspNetRoles.ToList();
            Roles = new List<UsersRoleModel>();
            var user = _context.AspNetUsers.Include(u => u.AspNetRoles).First(u => u.Id == Id);
            roles.ForEach(r =>
            {
                Roles.Add(new UsersRoleModel()
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    UserId = Id,
                    IsUserInRole = user.AspNetRoles.Count(ur => ur.Id == r.Id) > 0
                });
            });
        }

        #endregion

        #region Expands

        #endregion
    }

}
