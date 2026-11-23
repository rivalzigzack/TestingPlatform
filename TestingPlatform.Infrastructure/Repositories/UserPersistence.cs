using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Exceptions;
namespace TestingPlatform.Infrastructure.Repositories;
internal static class UserPersistence
{
    internal static async Task ValidateAsync(AppDbContext db, UserDto dto, int id, bool creating)
    {
        if (await db.Users.AnyAsync(u => u.Id != id && (u.Login == dto.Login || u.Email == dto.Email)))
            throw new EntityConflictException("Логин или email уже используется.");
        if (creating && string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Для нового пользователя требуется пароль.");
    }

    internal static void SetPassword(User user, string? password)
    {
        if (string.IsNullOrWhiteSpace(password)) return;
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        user.PasswordHash = $"PBKDF2-SHA256$100000${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }
}
