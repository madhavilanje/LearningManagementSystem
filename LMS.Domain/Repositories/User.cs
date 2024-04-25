using System;
using System.Collections.Generic;

namespace LMS.Infrastructure.Repositories
{
    public partial class User
    {
        public User()
        {
            Courses = new HashSet<Course>();
            Discussions = new HashSet<Discussion>();
            Enrollments = new HashSet<Enrollment>();
            LearningPaths = new HashSet<LearningPath>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;

        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<Discussion> Discussions { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<LearningPath> LearningPaths { get; set; }
    }
}
