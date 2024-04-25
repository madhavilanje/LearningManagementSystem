using System;
using System.Collections.Generic;

namespace LMS.Infrastructure.Repositories
{
    public partial class Enrollment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int LearningPathId { get; set; }
        public decimal? Progress { get; set; }

        public virtual Course Course { get; set; } = null!;
        public virtual LearningPath LearningPath { get; set; } = null!;
        public virtual User Student { get; set; } = null!;
    }
}
