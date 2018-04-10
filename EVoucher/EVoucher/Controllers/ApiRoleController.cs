using EVoucher.Lib.Models;
using EVoucher.Lib.ViewModels;
using EVoucher.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Swastika.Base.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace EVoucher.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="TTXServices.Controllers.BaseApiController" />
    [Authorize(Roles ="Admin")]
    [RoutePrefix("api/roles")]
    public class ApiRoleController : BaseApiController
    {

        /// <summary>
        /// Gets the role.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        [Route("{id:guid}", Name = "GetRoleById")]
        public async Task<RepositoryResponse<RoleViewModel>> GetRole(string Id)
        {
            var role = await RoleViewModel.Repository.GetSingleModelAsync(r => r.Id == Id);
            return role;
        }

        /// <summary>
        /// Gets all roles.
        /// </summary>
        /// <returns></returns>
        [Route("", Name = "GetAllRoles")]
        public async Task<RepositoryResponse<List<RoleViewModel>>> GetAllRolesAsync()
        {
            var roles = await RoleViewModel.Repository.GetModelListAsync();
            return roles;
        }

        /// <summary>
        /// Updates the user role.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [Route("UpdateUserRole")]
        public async Task<IHttpActionResult> UpdateUserRole(UsersRoleModel model)
        {
            var role = await this.AppRoleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                ModelState.AddModelError("", "Role does not exist");
                return BadRequest(ModelState);
            }
            else
            {
                if (!string.IsNullOrEmpty(model.UserId))
                {
                    var appUser = await this.UserManager.FindByIdAsync(model.UserId);

                    if (appUser == null)
                    {
                        ModelState.AddModelError("", String.Format("User: {0} does not exists", model.UserId));
                    }

                    if (model.IsUserInRole)
                    {

                        if (!this.UserManager.IsInRole(model.UserId, role.Name))
                        {
                            IdentityResult result = await this.UserManager.AddToRoleAsync(model.UserId, role.Name);

                            if (!result.Succeeded)
                            {
                                ModelState.AddModelError("", String.Format("User: {0} could not be added to role", model.UserId));
                            }

                        }
                    }
                    else
                    {
                        if (this.UserManager.IsInRole(model.UserId, role.Name))
                        {
                            IdentityResult result = await this.UserManager.RemoveFromRoleAsync(model.UserId, role.Name);

                            if (!result.Succeeded)
                            {
                                ModelState.AddModelError("", String.Format("User: {0} could not be added to role", model.UserId));
                            }

                        }
                    }
                }
            }
            return Ok(true);

        }

        /// <summary>
        /// Manages the users in role.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [Route("ManageUsersInRole")]
        public async Task<IHttpActionResult> ManageUsersInRole(UsersInRoleModel model)
        {
            var role = await this.AppRoleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ModelState.AddModelError("", "Role does not exist");
                return BadRequest(ModelState);
            }

            if (model.EnrolledUsers != null)
            {

                foreach (string user in model.EnrolledUsers)
                {
                    var appUser = await this.UserManager.FindByIdAsync(user);

                    if (appUser == null)
                    {
                        ModelState.AddModelError("", String.Format("User: {0} does not exists", user));
                        continue;
                    }

                    if (!this.UserManager.IsInRole(user, role.Name))
                    {
                        IdentityResult result = await this.UserManager.AddToRoleAsync(user, role.Name);

                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", String.Format("User: {0} could not be added to role", user));
                        }

                    }
                }
            }
            if (model.RemovedUsers != null)
            {
                foreach (string user in model.RemovedUsers)
                {
                    var appUser = await this.UserManager.FindByIdAsync(user);

                    if (appUser == null)
                    {
                        ModelState.AddModelError("", String.Format("User: {0} does not exists", user));
                        continue;
                    }

                    IdentityResult result = await this.UserManager.RemoveFromRoleAsync(user, role.Name);

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", String.Format("User: {0} could not be removed from role", user));
                    }
                }
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(true);
        }
    }
}