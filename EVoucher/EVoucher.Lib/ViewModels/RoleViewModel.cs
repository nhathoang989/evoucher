using Identity.Models;
using Swastika.Base.ViewModels;
using System.Collections.Generic;
using EVoucher.Lib.Models.EVoucher;
using System.Data.Entity;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace EVoucher.Lib.ViewModels
{
    public class RoleViewModel : ViewModelBase<EvoucherEntities, AspNetRole, RoleViewModel>
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        #region Contructors

        public RoleViewModel() : base()
        {
        }

        public RoleViewModel(AspNetRole model, EvoucherEntities _context = null, DbContextTransaction _transaction = null)
            : base(model, _context, _transaction)
        {
        }

        #endregion

    }
}
