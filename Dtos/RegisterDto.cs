namespace DotnetAPI.Dtos
{
    public partial class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public decimal Salary { get; set; }
        public bool Active { get; set; }

        public RegisterDto()
        {
            Email ??= "";
            Password ??= "";
            PasswordConfirm ??= "";
            FirstName ??= "";
            LastName ??= "";
            JobTitle ??= "";
            Department ??= "";
        }
    }
}
