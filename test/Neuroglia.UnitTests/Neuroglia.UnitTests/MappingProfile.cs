using AutoMapper;
using Neuroglia.UnitTests.Data.Events;

namespace Neuroglia.UnitTests;

internal class MappingProfile
    : Profile
{

    public MappingProfile()
    {
        this.CreateMap<UserCreatedEvent, UserCreatedEventV2>();
        this.CreateMap<UserCreatedEventV2, UserCreatedEventV3>();
    }

}
