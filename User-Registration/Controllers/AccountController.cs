using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using User_Registration.Dal;
using User_Registration.Models;

namespace User_Registration.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        // GET: AccountController
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> UserProfile(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var user = await GetUser(email);
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.Image))
                    {
                        user.Image = Url.Content("~/UserImages/" + Path.GetFileName(user.Image));
                    }
                    return View(user); // Pass the user object to the view
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return RedirectToAction("Login", "Account");
            }
        }


        // GET: AccountController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AccountController/Create
        public ActionResult Create()
        {
            return View();
        }

        public async Task<UserProfile> GetUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var userProfile = new UserProfile()
            {
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Id = user.Id,
                Image = user.ImageUrl,
                MaritalStatus = user.MaritalStatus,
                Name = user.FullName
            };

            return userProfile;

        }


        public async Task<ActionResult> CreateToken(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("5897279b-5248-40a0-814c-fa855ab1895d"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken("test",
            "test",
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        // POST: AccountController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateUser createUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var errorMessages = string.Empty;

                    var user = new ApplicationUser { FullName = createUser.Name, UserName = createUser.Email, Email = createUser.Email, DateOfBirth = createUser.DateOfBirth, Gender = createUser.Gender, MaritalStatus = createUser.MaritalStatus };
                    var userCreationResult = await _userManager.CreateAsync(user, createUser.Password);

                    if (!userCreationResult.Succeeded)
                    {
                        foreach (var error in userCreationResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(createUser);
                    }

                    if (createUser.Image != null && createUser.Image.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(createUser.Image.FileName);

                        var rootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "UserImages");

                        if (!Directory.Exists(rootDirectory))
                        {
                            Directory.CreateDirectory(rootDirectory);
                        }

                        var filePath = Path.Combine(rootDirectory, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await createUser.Image.CopyToAsync(stream);
                        }

                        user.ImageUrl = filePath;
                        await _userManager.UpdateAsync(user);
                    }

                    if (userCreationResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        user = await _userManager.FindByEmailAsync(createUser.Email);

                        await _userManager.AddToRoleAsync(user, "User");
                    }
                    else
                    {
                        return View(createUser);
                    }

                    return RedirectToAction(nameof(UserProfile), new { email = createUser.Email });
                }
                else
                {
                    return View(createUser);
                }
            }
            catch
            {
                return View();
            }
        }

        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.FullName = model.Name;
                user.Gender = model.Gender;
                user.MaritalStatus = model.MaritalStatus;
                user.DateOfBirth = model.DateOfBirth;

                if (model.Image != null && model.Image.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);

                    var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "UserImages");

                    if (!Directory.Exists(uploadDirectory))
                    {
                        Directory.CreateDirectory(uploadDirectory);
                    }

                    var filePath = Path.Combine(uploadDirectory, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(stream);
                    }

                    user.ImageUrl = "/UserImages/" + fileName;
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("UserProfile", new { email = user.Email });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View("UserProfile", model);
        }

        // POST: AccountController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AccountController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AccountController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
