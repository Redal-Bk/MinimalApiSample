public record LoginDTO(string Username, string Password);
public record AuthResponse(string Token, DateTime ExpiresAt);