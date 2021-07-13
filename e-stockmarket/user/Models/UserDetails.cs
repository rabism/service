using System.ComponentModel.DataAnnotations;

namespace user.Models
{
    public class UserDetails
    {
        [Key]
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
    }
}
