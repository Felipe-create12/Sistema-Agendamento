using Dominio.Dto;
using Interface.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sistema_Agendamento.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SegurancaController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;

        public SegurancaController(IConfiguration config, IUserService userService)
        {
            _config = config;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto loginDetalhes)
        {
            if (loginDetalhes == null || string.IsNullOrWhiteSpace(loginDetalhes.user) || string.IsNullOrWhiteSpace(loginDetalhes.senha))
                return BadRequest(new { message = "Usuário e senha são obrigatórios." });

            // 🔹 Busca o usuário no banco pelo nome de login
            var user = (await _userService.getAllAsync(u => u.user == loginDetalhes.user)).FirstOrDefault();

            if (user == null)
                return Unauthorized(new { message = "Usuário não encontrado." });

            // 🔹 Aqui poderia ser comparado um hash de senha (se você usar criptografia)
            if (user.senha != loginDetalhes.senha)
                return Unauthorized(new { message = "Senha incorreta." });

            // 🔹 Gera token JWT real com base no usuário
            var tokenString = GerarTokenJWT(user);

            return Ok(new
            {
                access_token = tokenString,
                token_type = "Bearer",
                expires_in = 60 * 60, // 60 minutos
                user = user.user
            });
        }

        private string GerarTokenJWT(UserDto user)
        {
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // 🔹 Claims baseadas no usuário real
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.user),
                new Claim(ClaimTypes.Role, "User") // Aqui você pode colocar o papel real se tiver
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
