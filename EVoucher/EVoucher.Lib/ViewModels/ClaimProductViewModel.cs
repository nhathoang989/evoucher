using Identity.Models;
using Swastika.Base.ViewModels;
using System.Collections.Generic;
using EVoucher.Lib.Models.EVoucher;
using System.Data.Entity;
using Newtonsoft.Json;
using System;
using System.Linq;
using EVoucher.Lib.Models;
using System.ComponentModel.DataAnnotations;

namespace EVoucher.Lib.ViewModels
{
    public class ClaimProductViewModel : ViewModelBase<EvoucherEntities, BridgeStone_ClaimProduct, ClaimProductViewModel>
    {
        #region Properties
        #region Models

        [JsonProperty("id")]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Nhập tên sản phẩm")]
        [DataType(DataType.Text)]
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("claimedDate")]
        public System.DateTime ClaimedDate { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("codeVoucher")]
        public string CodeVoucher { get; set; }
        #endregion

        #region Views

        #endregion

        #endregion

        #region Contructors

        public ClaimProductViewModel() : base()
        {
        }

        public ClaimProductViewModel(BridgeStone_ClaimProduct model, EvoucherEntities _context = null, DbContextTransaction _transaction = null)
            : base(model, _context, _transaction)
        {
        }

        #endregion

        #region Overrides

        public override void Validate(EvoucherEntities _context = null, DbContextTransaction _transaction = null)
        {
            if (_context != null)
            {
                base.Validate(_context, _transaction);
                if (IsValid)
                {
                    IsValid = 0< Quantity && Quantity < 3;
                    if (IsValid)
                    {
                        if (_context.BridgeStone_ClaimProduct.Any(p => p.CodeVoucher == CodeVoucher))
                        {
                            IsValid = _context.BridgeStone_ClaimProduct.Where(p => p.CodeVoucher == CodeVoucher).Sum(p => p.Quantity) + Quantity < 3;
                        }
                    }
                    if (!IsValid)
                    {
                        Errors.Add("Số lượng không hợp lệ");
                        Errors.Add("Mỗi mã code chỉ được sử dụng 2 lần");
                    }
                }
            }
        }

        #endregion

        #region Expands

        #endregion
    }

}
