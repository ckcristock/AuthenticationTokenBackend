namespace AuthenticationTokenBackend.Models.DTOs
{
    public class LoginResponseDto
    {
        public int Id { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime FechaExpiracion { get; set; }
    }
}
