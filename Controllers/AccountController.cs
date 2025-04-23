using EmployeeManagementSystem.Web.Data;
using EmployeeManagementSystem.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var company = await _context.Companies.FirstOrDefaultAsync(c => c.Name == model.CompanyName);

                if (company == null)
                {
                    company = new Company { Name = model.CompanyName };
                    _context.Companies.Add(company);
                    await _context.SaveChangesAsync();
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    CompanyName = model.CompanyName,
                    CompanyId = company.Id,
                    MustChangePassword = false,
                    Role = "Director"
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await SignInWithClaims(user);
                    return RedirectToAction("Index", "Employee");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                    if (result.Succeeded)
                    {
                        if (user.MustChangePassword)
                            return RedirectToAction("ChangePassword");

                        await SignInWithClaims(user);
                        return RedirectToAction("Index", "Employee");
                    }
                }

                ModelState.AddModelError(string.Empty, "Neteisingas el. paštas arba slaptažodis.");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult ChangePassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return RedirectToAction("Login");

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    user.MustChangePassword = false;
                    await _userManager.UpdateAsync(user);
                    await SignInWithClaims(user);
                    return RedirectToAction("Index", "Employee");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> Invite()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.CompanyId == null) return Unauthorized();

            var model = new InviteViewModel
            {
                InvitedEmployees = await _context.Users
                    .Where(u => u.CompanyId == currentUser.CompanyId && u.Role == "Employee")
                    .ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Invite(InviteViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.CompanyId == null) return Unauthorized();

            if (ModelState.IsValid)
            {
                var tempPassword = GenerateSecurePassword();

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    CompanyId = currentUser.CompanyId,
                    CompanyName = currentUser.CompanyName,
                    MustChangePassword = true,
                    Role = "Employee"
                };

                var result = await _userManager.CreateAsync(user, tempPassword);

                if (result.Succeeded)
                {
                    model.GeneratedPassword = tempPassword;
                    model.InviteSuccessful = true;
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model.InvitedEmployees = await _context.Users
                .Where(u => u.CompanyId == currentUser.CompanyId && u.Role == "Employee")
                .ToListAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInvite(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.CompanyId != user.CompanyId) return Forbid();

            await _userManager.DeleteAsync(user);
            return RedirectToAction("Invite");
        }

        private string GenerateSecurePassword()
        {
            const int length = 12;
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@$?_-";
            var res = new StringBuilder();
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];
                while (res.Length < length)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }
            return res.ToString();
        }

        private async Task SignInWithClaims(ApplicationUser user)
        {
            var principal = await _signInManager.CreateUserPrincipalAsync(user);

            var identity = (ClaimsIdentity)principal.Identity!;
            identity.AddClaim(new Claim("CompanyName", user.CompanyName ?? ""));
            identity.AddClaim(new Claim("Role", user.Role ?? ""));

            await _signInManager.SignOutAsync(); // remove old cookie
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity));
        }
    }
}
