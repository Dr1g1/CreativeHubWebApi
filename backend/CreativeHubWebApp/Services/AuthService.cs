using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CreativeHubWebApp.DTO;
using CreativeHubWebApp.Models;
using CreativeHubWebApp.Repositories;
using CreativeHubWebApp.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CreativeHubWebApp.Services
{
    public class AuthService
    {
        private readonly UserRepository _users;
        private readonly JwtSettings _jwt;

        public AuthService(UserRepository users, IOptions<JwtSettings> jwtOptions)
        {
            _users = users;
            _jwt = jwtOptions.Value;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            // provera da email ili username nisu vec zauzeti:
            if (await _users.GetByEmailAsync(dto.Email) is not null)
                throw new InvalidOperationException("Email je već u upotrebi.");
            if (await _users.GetByUsernameAsync(dto.Username) is not null)
                throw new InvalidOperationException("Username je već zauzet.");

            // hesiranje:
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                DisplayName = string.IsNullOrWhiteSpace(dto.DisplayName) ? dto.Username : dto.DisplayName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "user"
            };

            // upis u bazu
            await _users.CreateAsync(user);

            // odmah izdaj token, da bude odmah ulogovan i posle registracije
            return BuildResponse(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _users.GetByEmailAsync(dto.Email);

            // namerno ista poruka i kad email ne postoji i kad je lozinka pogresna
            // da napadac ne moze da provali koji mejl je u sistemu
            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new InvalidOperationException("Pogrešan email ili lozinka.");

            return BuildResponse(user);
        }

        // helper fja:
        private AuthResponseDto BuildResponse(User user) => new()
        {
            Token = GenerateToken(user),
            Id = user.Id,
            Username = user.Username,
            DisplayName = user.DisplayName
        };

        private string GenerateToken(User user)
        {
            // "Claims" su tvrdnje koje token nosi o korisniku
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),        // ko je korisnik (misli se na id)
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.Role, user.Role),                  // uloga (user/admin)
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // jedinstveni id tokena
        };

            // kljuc kojim potpisujemo token (isti ovaj kljuc kasnije proverava potpis)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
