using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EVoucher.Lib.Models
{
    // Models used as parameters to AccountController actions.

    public class AddExternalLoginBindingModel
    {
        [Required]
        [Display(Name = "External access token")]
        public string ExternalAccessToken { get; set; }
    }

    public class ChangePasswordBindingModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterBindingModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterExternalBindingModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Provider { get; set; }

        [Required]
        public string ExternalAccessToken { get; set; }

    }


    public class RemoveLoginBindingModel
    {
        [Required]
        [Display(Name = "Login provider")]
        public string LoginProvider { get; set; }

        [Required]
        [Display(Name = "Provider key")]
        public string ProviderKey { get; set; }
    }

    public class SetPasswordBindingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterModel
    {
        [Display(Name = "User name")]
        [JsonProperty("userName")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Device Id")]
        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty("notificationId")]
        public string Notificationid { get; set; }
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [JsonProperty("password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [JsonProperty("confirmPassword")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Email")]
        [Required]
        [JsonProperty("email")]
        public string Email { get; set; }


        [Display(Name = "WeiboId")]
        [Required]
        [JsonProperty("weiboId")]
        public string WeiboId { get; set; }

        [Display(Name = "FacebookId")]
        [Required]
        [JsonProperty("facebookId")]
        public string FacebookId { get; set; }
        [JsonProperty("culture")]
        public string Culture { get; set; }

    }

    public class CreateRoleBindingModel
    {

        [Required]
        [StringLength(256, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [Display(Name = "Role Name")]
        [JsonProperty("name")]
        public string Name { get; set; }

    }

    public class UsersInRoleModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("EnrolledUsers")]
        public List<string> EnrolledUsers { get; set; }
        [JsonProperty("removedUsers")]
        public List<string> RemovedUsers { get; set; }
    }

    public class UsersRoleModel
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }
        [JsonProperty("roleId")]
        public string RoleId { get; set; }
        [JsonProperty("roleName")]
        public string RoleName { get; set; }
        [JsonProperty("isUserInRole")]
        public bool IsUserInRole { get; set; }
    }
}
