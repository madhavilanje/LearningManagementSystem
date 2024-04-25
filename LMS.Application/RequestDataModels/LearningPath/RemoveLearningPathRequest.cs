using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Application.RequestDataModels.LearningPath
{
    public class RemoveLearningPathRequest
    {
        [Required]
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
