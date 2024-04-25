using System;
using System.Collections.Generic;

namespace LMS.Infrastructure.Repositories
{
    public partial class LearningPath
    {
        public LearningPath()
        {
            Courses = new HashSet<Course>();
            Enrollments = new HashSet<Enrollment>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int InstructorId { get; set; }

        public virtual User Instructor { get; set; } = null!;
        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
