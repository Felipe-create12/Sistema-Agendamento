using AutoMapper;
using Dominio.Dto;
using Dominio.Entidades;
using Interface.RepositorioI;
using Interface.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class EmpresaService : IEmpresaService
    {
        private IEmpresaRepositorio repositorio;

        private IMapper mapper;

        public EmpresaService(IEmpresaRepositorio repositorio,
            IMapper mapper)
        {
            this.repositorio = repositorio;
            this.mapper = mapper;
        }

        public async Task<EmpresaDto> addAsync(EmpresaDto empresaDto)
        {
            var entidade = mapper.Map<Empresa>(empresaDto);
            entidade = await this.repositorio.addAsync(entidade);
            return mapper.Map<EmpresaDto>(entidade);
        }

        public async Task<IEnumerable<EmpresaDto>> getAllAsync(Expression<Func<Empresa, bool>> expression)
        {
            var listaCat =
               await this.repositorio.getAllAsync(expression);
            return mapper.Map<IEnumerable<EmpresaDto>>(listaCat);
        }

        public async Task<EmpresaDto?> getAsyc(int id)
        {
            var empresa = await repositorio.getAsyc(id); // já faz Include de Servicos e Profissionais
            if (empresa == null) return null;

            return new EmpresaDto
            {
                Id = empresa.Id,
                Nome = empresa.Nome,
                Endereco = empresa.Endereco,
                Cidade = empresa.Cidade,
                Estado = empresa.Estado,
                Cep = empresa.Cep,
                Telefone = empresa.Telefone,
                Servicos = empresa.Servicos?.Select(s => new ServicoDto
                {
                    Id = s.Id,
                    nome = s.nome,
                    preco = s.preco,
                    DuracaoEmMinutos = s.DuracaoEmMinutos
                }).ToList() ?? new List<ServicoDto>(),
                Profissionais = empresa.Profissionais?.Select(p => new ProfissionalDto
                {
                    Id = p.Id,
                    nome = p.nome,
                    especialidade = p.especialidade
                }).ToList() ?? new List<ProfissionalDto>()
            };
        }



        public async Task removeAsyc(int id)
        {
            var cat = await this.repositorio.getAsyc(id);
            if (cat != null)
                await this.repositorio.removeAsyc(cat);
        }

        public async Task updateAsync(EmpresaDto empresaDto)
        {
            var cat = mapper.Map<Empresa>(empresaDto);
            await this.repositorio.updateAsync(cat);
        }

        public async Task<IEnumerable<EmpresaDto>> GetEmpresasProximasAsync(double latitude, double longitude, double raioKm)
        {
            var empresas = await this.repositorio.getAllAsync(e => true);

            /*

              var proximas = empresas

                  .Where(e => CalcularDistancia(latitude, longitude, e.Latitude, e.Longitude) <= raioKm)
                  .OrderBy(e => e.Nome);
            */
            var proximas = empresas
              .Select(e =>
              {
                  var distancia = CalcularDistancia(latitude, longitude, e.Latitude, e.Longitude);
                  var dto = mapper.Map<EmpresaDto>(e);
                  dto.Distancia = Math.Round(distancia, 2); // arredonda p/ 2 casas
                  return dto;
              })
              .Where(dto => dto.Distancia <= raioKm) // filtra dentro do raio
              .OrderBy(dto => dto.Distancia) // ordena pelas mais próximas
              .ToList();
            return mapper.Map<IEnumerable<EmpresaDto>>(proximas);
        }

        // 🔹 Função Haversine para calcular distância
        private double CalcularDistancia(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371; // Raio da Terra em km
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double deg) => deg * (Math.PI / 180);

        public async Task<IEnumerable<EmpresaDto>> GetEmpresasPorCidadeAsync(string cidade)
        {
            var empresas = await this.repositorio.getAllAsync(e => e.Cidade.ToLower().Contains(cidade.ToLower()));
            return mapper.Map<IEnumerable<EmpresaDto>>(empresas);
        }

    }
}
