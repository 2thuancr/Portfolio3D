using AutoMapper;
using Portfolio3D.Projects;

namespace Portfolio3D;

public class Portfolio3DApplicationAutoMapperProfile : Profile
{
    public Portfolio3DApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<Project, ProjectDto>();
        CreateMap<Project, ProjectListDto>();
    }
}
