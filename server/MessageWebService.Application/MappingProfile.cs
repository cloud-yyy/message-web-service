using AutoMapper;
using MessageWebService.Domain.Dtos;
using MessageWebService.Domain.Models;

namespace MessageWebService.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Message, MessageDto>();
    }
}
