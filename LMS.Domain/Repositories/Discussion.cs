using System;
using System.Collections.Generic;

namespace LMS.Infrastructure.Repositories
{
    public partial class Discussion
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UserId { get; set; }

        public virtual User? User { get; set; }
    }
}
