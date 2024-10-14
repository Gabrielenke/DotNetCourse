namespace DotnetAPI.Dtos
{
    public partial class UserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }

        public UserDto()
        {
            FirstName ??= "";
            LastName ??= "";
            Email ??= "";
        }
    }
}
