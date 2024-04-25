using System;
using System.Collections.Generic;

namespace LMS.Infrastructure.Repositories
{
    public partial class Course
    {
        public Course()
        {
            Enrollments = new HashSet<Enrollment>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int InstructorId { get; set; }
        public string? ContentUrl { get; set; }
        public int LearningPathId { get; set; }

        public virtual User Instructor { get; set; } = null!;
        public virtual LearningPath LearningPath { get; set; } = null!;
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
