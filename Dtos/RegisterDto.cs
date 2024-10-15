namespace DotnetAPI.Dtos
{
    public partial class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }

        public RegisterDto()
        {
            Email ??= "";
            Password ??= "";
            PasswordConfirm ??= "";
        }
    }
}
