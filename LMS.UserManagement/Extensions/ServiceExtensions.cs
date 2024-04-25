using AutoMapper;
using LMS.Application.AutoMapperMappingProfile;


namespace LMS.UserManagement.Api.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Auto mapper configuration
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }


    }
}
