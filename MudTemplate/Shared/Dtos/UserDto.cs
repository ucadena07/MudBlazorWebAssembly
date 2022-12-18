using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudTemplate.Shared.Dtos
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool Active { get; set; }
        public bool Blocked { get; set; }
        public Guid? ResetToken { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
