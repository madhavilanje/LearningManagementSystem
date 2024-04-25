using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Application.ResponseDataModels
{
    public class EnrollmentViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("courseName")]
        public string CourseName { get; set; }

        [JsonPropertyName("learningPathName")]
        public string LearningPathName { get; set; }

        [JsonPropertyName("progress")]
        public string Progress { get; set; }

        [JsonPropertyName("StudentName")]
        public string StudentName { get; set; }
    }
}
