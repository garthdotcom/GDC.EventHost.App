using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.App.Auth;
using GDC.EventHost.App.Services;
using GDC.EventHost.Shared.Member;
using GDC.EventHost.App.Admin.ViewModels.User;
using GDC.EventHost.App.Areas.Admin.ViewModels.User;
using GDC.EventHost.App.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.App.Areas.Admin.Controllers
{
    [Authorize(Policy = "IsAdministrator")]
    [Authorize(Policy = "CanManageUsers")]
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    [Area("Admin")]
    public class UserAdminController : Controller
    {
        private readonly UserManager<EventHostUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEventHostService _eventHostService;

        public UserAdminController(UserManager<EventHostUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            IEventHostService eventHostService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _eventHostService = eventHostService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public IActionResult UserManagement(string searchQuery, string sortOrder, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchQuery;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["UserNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "user_name_desc" : "";
            ViewData["EmailAddressSortParm"] = sortOrder == "EmailAddress" ? "email_address_desc" : "EmailAddress";

            var users = _userManager.Users
                .Where(u => u.UserName != null)
                .ToList();

            var filteredUsers = string.IsNullOrEmpty(searchQuery) ? users : 
                users.Where(s =>
                    s.UserName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    s.Email.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));

            switch (sortOrder)
            {
                case "user_name_desc":
                    filteredUsers = filteredUsers.OrderByDescending(s => s.UserName).ToList();
                    break;
                case "EmailAddress":
                    filteredUsers = filteredUsers.OrderBy(s => s.Email).ToList();
                    break;
                case "email_address_desc":
                    filteredUsers = filteredUsers.OrderByDescending(s => s.Email).ToList();
                    break;
                default:
                    filteredUsers = filteredUsers.OrderBy(s => s.UserName).ToList();
                    break;
            }

            int pageSize = 10;

            var userListViewModel = new UserListVM
            {
                UserList = new PaginatedList<EventHostUser>(filteredUsers.AsQueryable(),
                    pageNumber ?? 1, pageSize)
            };

            return View(userListViewModel);
        }

        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserVM addUserViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(addUserViewModel);
            }

            var memberId = Guid.NewGuid();
                
            var user = new EventHostUser()
            {
                UserName = addUserViewModel.UserName,
                Email = addUserViewModel.Email,
                BirthDate = addUserViewModel.BirthDate,
                MemberId = memberId
            };

            IdentityResult result = await _userManager.CreateAsync(user, addUserViewModel.Password);

            if (result.Succeeded)
            {
                var newMember = new MemberForUpdateDto
                {
                    Id = memberId,
                    IsActive = true,
                    UserId = user.Id,
                    Username = addUserViewModel.UserName,
                    EmailAddress = addUserViewModel.Email,
                    FullName = addUserViewModel.FullName
                };

                string stringData = JsonConvert.SerializeObject(newMember);

                await _eventHostService.PostOne<MemberDto>("/members", stringData);

                if (_eventHostService.Error)
                {
                    ModelState.AddModelError("", _eventHostService.Messages);
                }

                if (ModelState.IsValid)
                {
                    return RedirectToAction("UserManagement", _userManager.Users);
                } 
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(addUserViewModel);
        }

        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return RedirectToAction("UserManagement", _userManager.Users);
            }

            var claims = await _userManager.GetClaimsAsync(user);

            var member = await _eventHostService.GetOne<MemberDto>($"/members/{user.MemberId}");

            var fullName = member != null ? member.FullName : string.Empty;

            var vm = new EditUserVM() 
            { 
                Id = user.Id, 
                Email = user.Email, 
                UserName = user.UserName, 
                BirthDate = user.BirthDate ?? null, 
                MemberId = user.MemberId ?? null,
                FullName = fullName,
                UserClaims = claims.Select(c => c.Value).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserVM editUserViewModel)
        {
            var user = await _userManager.FindByIdAsync(editUserViewModel.Id);

            if (user != null)
            {
                user.Email = editUserViewModel.Email;
                user.UserName = editUserViewModel.UserName;
                user.BirthDate = editUserViewModel.BirthDate;
                user.MemberId = editUserViewModel.MemberId;

                // update the existing user

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // update the associated member

                    var memberForUpdate = await _eventHostService.GetOne<MemberForUpdateDto>($"/members/{editUserViewModel.MemberId}");

                    memberForUpdate.EmailAddress = editUserViewModel.Email;
                    memberForUpdate.Username = editUserViewModel.UserName;
                    memberForUpdate.FullName = editUserViewModel.FullName;

                    string stringData = JsonConvert.SerializeObject(memberForUpdate);

                    await _eventHostService.PutOne($"/members/{editUserViewModel.MemberId}", stringData);

                    return RedirectToAction("UserManagement", _userManager.Users);
                }

                ModelState.AddModelError("", "User was not updated, something went wrong.");

                return View(editUserViewModel);
            }

            return RedirectToAction("UserManagement", _userManager.Users);
        }

        private void UpdateTicketStatus(Guid ticketId, TicketStatusEnum ticketStatus)
        {
            var patchDocList = new List<PatchDocument>()
            {
                new PatchDocument()
                {
                    op = "replace",
                    path = "/ticketStatusId",
                    value = ticketStatus.ToString()
                }
            };

            string stringData = JsonConvert.SerializeObject(patchDocList);

            Task.Run(async () =>
            {
                await _eventHostService.PatchOne($"/tickets/{ticketId}", stringData);
            });
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    // make the member inactive

                    var existingMember = await _eventHostService.GetOne<MemberForUpdateDto>($"/members/{user.MemberId}");

                    if (existingMember.ShoppingCart != null)
                    {
                        foreach (var cartItem in existingMember.ShoppingCart.ShoppingCartItems)
                        {
                            // return the ticket
                            UpdateTicketStatus(cartItem.Ticket.Id, TicketStatusEnum.UnSold);

                            // delete the shopping cart item
                            await _eventHostService.DeleteOne($"/shoppingcartitems/{cartItem.Id}");
                        }
                    }

                    // set the active flag on the member to false

                    existingMember.IsActive = false;

                    string stringData = JsonConvert.SerializeObject(existingMember);

                    await _eventHostService.PutOne($"/members/{existingMember.Id}", stringData);

                    return RedirectToAction("UserManagement");
                }  
                else
                {
                    ModelState.AddModelError("", "Something went wrong while deleting this user.");
                }    
            }
            else
            {
                ModelState.AddModelError("", "This user can't be found");
            }

            return View("UserManagement", _userManager.Users);
        }


        // Role management

        public IActionResult RoleManagement()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }


        public IActionResult AddNewRole() => View();


        [HttpPost]
        public async Task<IActionResult> AddNewRole(AddRoleVM addRoleViewModel)
        {

            if (!ModelState.IsValid) return View(addRoleViewModel);

            var role = new IdentityRole
            {
                Name = addRoleViewModel.RoleName
            };

            IdentityResult result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction("RoleManagement", _roleManager.Roles);
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(addRoleViewModel);
        }


        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return RedirectToAction("RoleManagement", _roleManager.Roles);
            }
                
            var editRoleViewModel = new EditRoleVM
            {
                Id = role.Id,
                RoleName = role.Name,
                Users = new List<string>()
            };

            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    editRoleViewModel.Users.Add(user.UserName);
                }           
            }

            return View(editRoleViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleVM editRoleViewModel)
        {
            var role = await _roleManager.FindByIdAsync(editRoleViewModel.Id);

            if (role != null)
            {
                role.Name = editRoleViewModel.RoleName;

                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("RoleManagement", _roleManager.Roles);
                }
                    
                ModelState.AddModelError("", "Role not updated, something went wrong.");

                return View(editRoleViewModel);
            }

            return RedirectToAction("RoleManagement", _roleManager.Roles);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);

            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("RoleManagement", _roleManager.Roles);
                }     

                ModelState.AddModelError("", "Something went wrong while deleting this role.");
            }
            else
            {
                ModelState.AddModelError("", "This role can't be found.");
            }

            return View("RoleManagement", _roleManager.Roles);
        }

        //Users in roles

        public async Task<IActionResult> AddUserToRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return RedirectToAction("RoleManagement", _roleManager.Roles);
            }
                
            var addUserToRoleViewModel = new UserRoleVM 
                { 
                    RoleId = role.Id 
                };

            foreach (var user in _userManager.Users)
            {
                if (!await _userManager.IsInRoleAsync(user, role.Name))
                {
                    addUserToRoleViewModel.Users.Add(user);
                }
            }

            return View(addUserToRoleViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToRole(UserRoleVM userRoleViewModel)
        {
            var user = await _userManager.FindByIdAsync(userRoleViewModel.UserId);
            var role = await _roleManager.FindByIdAsync(userRoleViewModel.RoleId);

            var result = await _userManager.AddToRoleAsync(user, role.Name);

            if (result.Succeeded)
            {
                return RedirectToAction("RoleManagement", _roleManager.Roles);
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(userRoleViewModel);
        }


        public async Task<IActionResult> DeleteUserFromRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return RedirectToAction("RoleManagement", _roleManager.Roles);
            }

            var addUserToRoleViewModel = new UserRoleVM { RoleId = role.Id };

            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    addUserToRoleViewModel.Users.Add(user);
                }
            }

            return View(addUserToRoleViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUserFromRole(UserRoleVM userRoleViewModel)
        {
            var user = await _userManager.FindByIdAsync(userRoleViewModel.UserId);
            var role = await _roleManager.FindByIdAsync(userRoleViewModel.RoleId);

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);

            if (result.Succeeded)
            {
                return RedirectToAction("RoleManagement", _roleManager.Roles);
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(userRoleViewModel);
        }


        // Roles

        public async Task<IActionResult> ManageRolesForUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("UserManagement", _userManager.Users);
            }

            var userRoleVMs = new List<UserRoleVM2>();

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in _roleManager.Roles)
            {
                userRoleVMs.Add(new UserRoleVM2
                {
                    UserId = userId,
                    RoleName = role.Name,
                    HasRole = userRoles.Contains(role.Name)
                });
            }

            var rolesManagementViewModel = new RolesManagementVM
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserRoleVMs = userRoleVMs
            };

            return View(rolesManagementViewModel);
        }

        public async Task<IActionResult> ToggleUserRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId); 
             

            var userRoleNames = await _userManager.GetRolesAsync(user);

            if (userRoleNames.Contains(roleName))
            {
                await _userManager.RemoveFromRoleAsync(user, roleName);
            }
            else
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }

            return RedirectToAction("ManageRolesForUser", new { userId });
        }

        // Claims

        public async Task<IActionResult> ManageClaimsForUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("UserManagement", _userManager.Users);
            }

            var userClaimsList = new List<string>();
            var userClaimVMs = new List<UserClaimVM>();

            foreach (var userClaim in await _userManager.GetClaimsAsync(user))
            {
                userClaimsList.Add(userClaim.Type);          
            }

            foreach (var claim in EventHostClaimTypes.ClaimsList)
            {
                userClaimVMs.Add(new UserClaimVM
                {
                    UserId = userId,
                    ClaimValue = claim,
                    HasClaim = userClaimsList.Contains(claim)
                });
            }

            var claimsManagementViewModel = new ClaimsManagementVM 
            {   
                UserId = user.Id,
                UserName = user.UserName,
                UserClaimVMs = userClaimVMs
            };

            return View(claimsManagementViewModel);
        }

        public async Task<IActionResult> ToggleUserClaim(string userId, string claimValue)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var claim = new Claim(claimValue, claimValue);

            var userClaims = await _userManager.GetClaimsAsync(user);

            if (userClaims.Any(c => c.Value == claimValue))
            {
                await _userManager.RemoveClaimAsync(user, claim);
            }
            else
            {
                await _userManager.AddClaimAsync(user, claim);
            }

            return RedirectToAction("ManageClaimsForUser", new { userId });
        } 
    }
}