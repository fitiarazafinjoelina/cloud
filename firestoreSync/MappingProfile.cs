using cloud.user;

namespace cloud.firestoreSync;

using AutoMapper;
using System;
using System.Collections.Generic;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Dictionary<string, object>, UserInscriptionDTO>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.ContainsKey("Username") ? src["Username"].ToString() : null))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.ContainsKey("Password") ? Convert.ToInt32(src["Password"]) : 0))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.ContainsKey("Email") ? src["Email"].ToString() : null));
    }
}
