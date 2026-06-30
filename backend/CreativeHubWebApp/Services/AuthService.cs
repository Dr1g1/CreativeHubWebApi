using CreativeHubWebApp.DTO;
using CreativeHubWebApp.Models;
using CreativeHubWebApp.Repositories;
using CreativeHubWebApp.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CreativeHubWebApp.Services
{
    public class AuthService
    {
        private readonly UserRepository _users;
        private readonly JwtSettings _jwt;
        private readonly IMongoClient _client;
        private readonly ResourceRepository _resources;
        private readonly ReviewRepository _reviews;
        private readonly CollectionRepository _collections;
        private readonly GridFsService _gridFs;

        public AuthService(UserRepository users, IOptions<JwtSettings> jwtOptions, IMongoClient client,
                            ResourceRepository resources, ReviewRepository reviews, CollectionRepository collections, GridFsService gridFs)
        {
            _users = users;
            _jwt = jwtOptions.Value;
            _client = client;
            _resources = resources;
            _reviews = reviews;
            _collections = collections;
            _gridFs = gridFs;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (await _users.GetByEmailAsync(dto.Email) is not null)
                throw new InvalidOperationException("Email je već u upotrebi.");
            if (await _users.GetByUsernameAsync(dto.Username) is not null)
                throw new InvalidOperationException("Username je već zauzet.");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                DisplayName = string.IsNullOrWhiteSpace(dto.DisplayName) ? dto.Username : dto.DisplayName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "user"
            };

            await _users.CreateAsync(user);

            return BuildResponse(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _users.GetByEmailAsync(dto.Email);

            // namerno ista poruka i kad email ne postoji i kad je lozinka pogresna
            // da ako neko hoce da napadne ne moze da provali koji mejl je u sistemu
            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new InvalidOperationException("Pogrešan email ili lozinka.");

            return BuildResponse(user);
        }

        private AuthResponseDto BuildResponse(User user) => new()
        {
            Token = GenerateToken(user),
            Id = user.Id,
            Username = user.Username,
            DisplayName = user.DisplayName
        };

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),        
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.Role, user.Role),                  
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())//svaki token ima id
        };

            // kljuc kojim potpisujemo token
            //isti ovaj kljuc kasnije proverava potpis
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

        public async Task<UserResponseDto?> GetProfileAsync(string id)
        {
            var user = await _users.GetByIdAsync(id);
            return user is null ? null : UserResponseDto.From(user);
        }

        public async Task<UserResponseDto?> UpdateProfileAsync(string id, UpdateProfileDto dto)
        {
            await _users.UpdateProfileAsync(id, dto.DisplayName, dto.Bio);
            var user = await _users.GetByIdAsync(id);
            return user is null ? null : UserResponseDto.From(user);
        }

        public async Task<bool> DeleteAccountAsync(string userId)
        {
            var user = await _users.GetByIdAsync(userId);
            if (user is null) return false;

            using var session = await _client.StartSessionAsync();

            await session.WithTransactionAsync(async (s, ct) =>
            {
                var resources = await _resources.GetByOwnerInSessionAsync(s, userId);

                
                await _resources.DeleteByOwnerAsync(s, userId);
                await _reviews.DeleteByUserAsync(s, userId);
                await _collections.DeleteByOwnerAsync(s, userId);
                await _users.DeleteAsync(s, userId);

                
                foreach (var r in resources)
                {
                    if (!string.IsNullOrEmpty(r.FileId))
                        await _gridFs.DeleteAsync(r.FileId);
                    foreach (var pid in r.PreviewImageIds)
                        await _gridFs.DeleteAsync(pid);
                }

                return true;
            });

            return true;
        }
    }
}
