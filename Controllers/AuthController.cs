using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Disco.Models;
using Disco.DTOs;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
  private readonly AppDbContext _context;
  private readonly IConfiguration _configuration;

  public AuthController(AppDbContext context, IConfiguration configuration)
  {
    _context = context;
    _configuration = configuration;
  }

  [HttpPost("signup")]
  public async Task<IActionResult> Signup([FromBody] SignupDTO signupDetails)
  {
    var hash = BCrypt.Net.BCrypt.HashPassword(signupDetails.Password);

    var verificationToken = Guid.NewGuid().ToString();

    var user = new User
    {
      Name = signupDetails.Username,
      Email = signupDetails.Email,
      Hashpassword = hash,
      Verificationtoken = verificationToken
    };

    try
    {
      await _context.Users.AddAsync(user);
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateException ex)
    {
      if (ex.InnerException != null && ex.InnerException.Message.Contains("duplicate"))
        return BadRequest("Já existe um usuário com esse nome");

      throw;
    }

    var userDTO = new UserDTO
    {
      Id = user.Id,
      Name = user.Name,
      Email = user.Email,
      Isverified = user.Isverified,
      Createdat = user.Createdat,
      Updatedat = user.Updatedat,
      Role = user.Role.ToString()
    };
    return Created($"/v1/users/{userDTO.Id}", userDTO);
  }

  [HttpPost("verify")]
  public async Task<IActionResult> VerifyAccount([FromQuery] string token)
  {
    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Verificationtoken == token);

    if (user == null)
      return BadRequest("Token de verificação inválido.");

    user.Isverified = true;
    user.Verificationtoken = null;

    await _context.SaveChangesAsync();

    return Ok("Conta verificada com sucesso! Agora você pode fazer login.");
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginDTO loginDetails)
  {
    var user = await _context.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Email == loginDetails.Email);

    if (user == null)
      return Unauthorized("Usuário ou senha inválidos");

    if (user.Isverified == false)
      return Unauthorized("Sua conta ainda não foi verificada");


    bool passwordValid = BCrypt.Net.BCrypt.Verify(loginDetails.Password, user.Hashpassword);

    if (!passwordValid)
      return Unauthorized("Usuário ou senha inválidos");

    var token = TokenService.GenerateToken(user, _configuration["Jwt:Key"]);

    var userDTO = new UserDTO
    {
      Id = user.Id,
      Name = user.Name,
      Email = user.Email,
      Isverified = user.Isverified,
      Createdat = user.Createdat,
      Updatedat = user.Updatedat,
      Role = user.Role.ToString()
    };

    return Ok(new LoginResponseDTO
    {
      User = userDTO,
      Token = token
    });
  }

  [HttpPost("forgot-password")]
  public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDto)
  {
    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Email == forgotPasswordDto.Email);

    user?.Resetpasswordtoken = Guid.NewGuid().ToString();

    await _context.SaveChangesAsync();

    return NoContent();
  }

  [HttpPost("reset-password")]
  public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetDto)
  {
    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Resetpasswordtoken == resetDto.Token);

    if (user == null || string.IsNullOrEmpty(resetDto.Token))
      return BadRequest("Token de recuperação inválido ou expirado.");

    user.Hashpassword = BCrypt.Net.BCrypt.HashPassword(resetDto.Password);

    user.Resetpasswordtoken = null;

    await _context.SaveChangesAsync();

    return Ok("Senha alterada com sucesso!");
  }
}