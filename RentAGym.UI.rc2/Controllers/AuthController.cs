using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RentAGym.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;


//[ApiController]
//[Route("api/[controller]")]
//public class AuthController : ControllerBase
//{
//    private readonly IConfiguration _configuration;
//    private readonly UserManager<IdentityUser> _userManager;

//    public AuthController(IConfiguration configuration, UserManager<IdentityUser> userManager)
//    {
//        _configuration = configuration;
//        _userManager = userManager;
//    }

//    [HttpPost("login")]
//    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
//    {
//        // Получаем пользователя по имени
//        var user = await _userManager.FindByEmailAsync(loginDto.Username);
//        if (user != null)
//        {
//            // Проверяем пароль
//            var passwordCheck = await _userManager.CheckPasswordAsync(user, loginDto.Password);
//            if (passwordCheck)
//            {
//                var token = GenerateJwtToken(user.UserName);
//                return Ok(new { Token = token });
//            }
//        }

//        return Unauthorized();
//    }

//    private string GenerateJwtToken(string username)
//    {
//        var jwtSettings = _configuration.GetSection("Jwt");
//        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]));

//        var claims = new[]
//        {
//            new Claim(JwtRegisteredClaimNames.Sub, username),
//            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
//        };

//        var token = new JwtSecurityToken(
//            issuer: jwtSettings["Issuer"],
//            audience: jwtSettings["Audience"],
//            claims: claims,
//            expires: DateTime.UtcNow.AddHours(1),
//            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
//        );

//        return new JwtSecurityTokenHandler().WriteToken(token);
//    }
//}


//public class LoginDto
//{
//    public string Username { get; set; }
//    public string Password { get; set; }
//}




[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    private readonly IUserStore<IdentityUser> _userStore;
    private readonly IEmailSender _emailSender;

    public AuthController(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IConfiguration configuration,
        ILogger<AuthController> logger,

        IUserStore<IdentityUser> userStore,
        IEmailSender emailSender)

    {
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;

        _userStore = userStore;
        _emailSender = emailSender;
       
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Unauthorized("Invalid email or password");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return Unauthorized("Invalid email or password");

        var claims = await _userManager.GetClaimsAsync(user);
        var isLandlord = claims.Any(c => c.Type == "landLord" && c.Value == "true");


        var token = GenerateJwtToken(user, isLandlord);

        return Ok(new LoginResponse
        {
            Token = token,
            Email = user.Email,
            UserId = user.Id,
            UserRole = isLandlord ? "Landlord" : "Tenant"
        });
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest model)
    {
        IdentityUser user = model.IsLandlord ? new Landlord() : new Tenant();

        if (model.IsLandlord)
            (user as Landlord)!.DateOfRegistration = DateTime.UtcNow;
        else
            (user as Tenant)!.DateOfRegistration = DateTime.UtcNow;

        await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
        if (_userStore is IUserEmailStore<IdentityUser> emailStore)
        {
            await emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);
        }
        else
        {
            return BadRequest("Email store is not available.");
        }

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        // Добавляем claims
        var claimType = model.IsLandlord ? "landLord" : "tenant";
        await _userManager.AddClaimAsync(user, new Claim(claimType, "true", ClaimValueTypes.Boolean));
        await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, model.Email));

        _logger.LogInformation("User created a new account with password.");

        // Генерация email подтверждения
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var userId = await _userManager.GetUserIdAsync(user);

        // Здесь можно сформировать ссылку на подтверждение почты (если нужно)
        var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));       

        var callbackUrl = $"http://rentagym.runasp.net/Account/ConfirmEmail?userId={userId}&code={encodedCode}";

        await _emailSender.SendEmailAsync(
            model.Email,
            "Подтвердите вашу почту",
            $"Пожалуйста, подтвердите ваш аккаунт, перейдя по ссылке: {callbackUrl}");

        return Ok(new { Message = "Регистрация прошла успешно. Проверьте email для подтверждения." });
    }

    private string GenerateJwtToken(IdentityUser user, bool isLandlord)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Role, isLandlord.ToString())


        };

        foreach (var claim in claims)
        {
            Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
            Debug.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddDays(7);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string UserRole { get; set; } = null!;
    }

    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;

        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; } = null!;

        public bool IsLandlord { get; set; }
    }
}