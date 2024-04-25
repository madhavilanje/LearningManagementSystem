using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Application.ResponseDataModels
{
    public class CourseViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("content_url")]
        public string ContentUrl { get; set; }


        [JsonPropertyName("InstructorName")]
        public string InstructorName { get; set; }

    }
}
