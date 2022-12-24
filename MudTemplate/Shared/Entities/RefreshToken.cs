using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudTemplate.Shared.Entities
{
    public class RefreshToken
    {
        [Key]
        public int UserRefreshTokenId { get; set; }
        public string Token { get; set; }
        public string UserRefreshToken { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        [NotMapped]
        public bool IsActive { get { return ExpirationDate > DateTime.UtcNow; } }
        public string IpAddress { get; set; }
        public bool IsInvalidated { get; set; }
        public int UserId { get; set; }
    }
}
