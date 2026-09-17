using System.ComponentModel.DataAnnotations.Schema;
using CourseHub.Domain.Common;

namespace CourseHub.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;
   
    [NotMapped]
    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;
}