using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudTemplate.Shared.Entities
{
    public class SiteUser
    {

        [Key]
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool Active { get; set; }
        public bool Blocked { get; set; }
        public Guid ResetToken { get; set; }
        public DateTime? LastLoginDate { get; set; }

    }
}
