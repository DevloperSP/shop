using System.Collections.Generic;

namespace BlazorAdmin.Authorization;

public class UserInfo
{
    public static readonly UserInfo Anonymous = new();

    public bool IsAuthenticated { get; init; }
    public string? NameClaimType { get; init; }
    public string? RoleClaimType { get; init; }
    public string? Token { get; init; }
    public IEnumerable<ClaimValue> Claims { get; init; } = new List<ClaimValue>();
}
