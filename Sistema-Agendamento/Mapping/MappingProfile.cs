using AutoMapper;
using Dominio.Dto;
using Dominio.Entidades;

namespace Sistema_Agendamento.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Agendamento, AgendamentoDto>().ReverseMap();
            CreateMap<Cliente, ClienteDto>().ReverseMap();
            CreateMap<Profissional, ProfissionalDto>().ReverseMap();
            CreateMap<Servico, ServicoDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<UserRegisterDto, User>().ReverseMap();
            CreateMap<Empresa, EmpresaDto>().ReverseMap();

        }
    }
}
