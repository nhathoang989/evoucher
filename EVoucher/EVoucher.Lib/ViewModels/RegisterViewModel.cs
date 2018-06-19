using Identity.Models;
using Swastika.Base.ViewModels;
using System.Collections.Generic;
using EVoucher.Lib.Models.EVoucher;
using System.Data.Entity;
using Newtonsoft.Json;
using System;
using System.Linq;
using EVoucher.Lib.Models;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EVoucher.Lib.ViewModels
{
    public class BSRegisterViewModel : ViewModelBase<EvoucherEntities, BridgeStone_Register, BSRegisterViewModel>
    {
        #region Properties
        #region Models

        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("bid")]
        public int BId { get; set; }
        [JsonProperty("fullname")]
        public string Fullname { get; set; }
        [Phone]
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
        [JsonProperty("from")]
        public string From { get; set; }
        [JsonProperty("sendCodeStatus")]
        public string SendCodeStatus { get; set; }
        [JsonProperty("updatedDate")]
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        [JsonProperty("sendCodeDate")]
        public Nullable<System.DateTime> SendCodeDate { get; set; }

        #endregion

        #region Views

        [JsonProperty("products")]
        public List<ClaimProductViewModel> Products { get; set; }

        [JsonProperty("claimStatus")]
        public string ClaimStatus
        {
            get
            {
                if (Products.Count > 0)
                {
                    int quantity = Products.Sum(p => p.Quantity);
                    return $"Đã đổi {quantity} sản phẩm";
                }
                return "";
            }
        }

        [JsonProperty("canClaim")]
        public bool CanClaim
        {
            get
            {
                int total = 0;
                if (Products != null && Products.Count > 0)
                {
                    total = Products.Sum(p => p.Quantity);
                }
                return total < 2;
            }
        }
        #endregion

        #endregion

        #region Contructors

        public BSRegisterViewModel() : base()
        {
        }

        public BSRegisterViewModel(BridgeStone_Register model, EvoucherEntities _context = null, DbContextTransaction _transaction = null)
            : base(model, _context, _transaction)
        {
        }

        #endregion

        #region Overrides

        public override BridgeStone_Register ParseModel()
        {
            if (Id == 0)
            {
                CreatedDate = DateTime.UtcNow;
            }
            return base.ParseModel();
        }

        public override void ExpandView(EvoucherEntities _context = null, DbContextTransaction _transaction = null)
        {
            Products = ClaimProductViewModel.Repository.GetModelListBy(p => p.CodeVoucher == Code, _context, _transaction).Data;
        }

        public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(BSRegisterViewModel view, EvoucherEntities _context = null, DbContextTransaction _transaction = null)
        {
            bool isSucceed = true;
            foreach (var product in view.Products)
            {
                isSucceed = isSucceed && (await product.RemoveModelAsync(_context: _context, _transaction: _transaction)).Data;
            }
            return new RepositoryResponse<bool>() { IsSucceed = isSucceed, Data = isSucceed };
        }

        public override RepositoryResponse<bool> RemoveRelatedModels(BSRegisterViewModel view, EvoucherEntities _context = null, DbContextTransaction _transaction = null)
        {
            bool isSucceed = true;
            foreach (var product in view.Products)
            {
                isSucceed = isSucceed && (product.RemoveModel(_context: _context, _transaction: _transaction)).Data;
            }
            return new RepositoryResponse<bool>() { IsSucceed = isSucceed, Data = isSucceed };
        }
        #endregion

        #region Expands

        #endregion
    }

}
