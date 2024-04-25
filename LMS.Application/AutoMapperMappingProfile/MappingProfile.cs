using AutoMapper;
using LMS.Application.ResponseDataModels;
using LMS.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace LMS.Application.AutoMapperMappingProfile
{
    /// <summary>
    /// This class is used for mapping source model to destination model.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Map souce model to destination model.
        /// </summary>
        public MappingProfile()
        {
            CreateMap<User, BaseUserProfileViewModel>();
            CreateMap<Course, CourseViewModel>();
            CreateMap<LearningPath, LearningPathViewModel>();
            CreateMap<Enrollment, EnrollmentViewModel>();
            CreateMap<Discussion, MessageViewModel>();





        }
    }
}
