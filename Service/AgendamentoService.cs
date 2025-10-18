using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Dto;
using Interface.RepositorioI;
using Interface.Service;


namespace Service
{
    public class AgendamentoService: IAgendamentoService
    {
        private IAgendamentoRepositorio repositorio;
        private  IClienteService _clienteService;
        private  IProfissionalService _profissionalService;
        private  IServicoService _servicoService;

        private IMapper mapper;

        public AgendamentoService(IAgendamentoRepositorio repositorio,
            IMapper mapper, IClienteService clienteService, IProfissionalService profissionalService, IServicoService servicoService)
        {
            this.repositorio = repositorio;
            this.mapper = mapper;
            this._clienteService = clienteService;
            this._profissionalService = profissionalService;
            this._servicoService = servicoService;
        }

        public async Task<AgendamentoDto> addAsync(AgendamentoDto agendamentoDto)
        {
            var cliente = await _clienteService.getAsyc(agendamentoDto.idCliente);
            if (cliente == null)
                throw new Exception("Cliente informado não existe.");

            var profissional = await _profissionalService.getAsyc(agendamentoDto.idProfissional);
            if (profissional == null)
                throw new Exception("Profissional informado não existe.");

            var servico = await _servicoService.getAsyc(agendamentoDto.idServicio);
            if (servico == null)
                throw new Exception("Serviço informado não existe.");

            var conflito = (await repositorio.getAllAsync(a =>
                a.idProfissional == agendamentoDto.idProfissional &&
                a.DataHora == agendamentoDto.DataHora)).Any();

            if (conflito)
                throw new Exception("O profissional já possui um agendamento nesse horário.");


            var entidade = mapper.Map<Agendamento>(agendamentoDto);
            entidade = await this.repositorio.addAsync(entidade);
            return mapper.Map<AgendamentoDto>(entidade);
        }

        public async Task<IEnumerable<AgendamentoDto>> getAllAsync(Expression<Func<Agendamento, bool>> expression)
        {
            var listaCat =
               await this.repositorio.getAllAsync(expression);
            return mapper.Map<IEnumerable<AgendamentoDto>>(listaCat);
        }

        public async Task<AgendamentoDto> getAsyc(int id)
        {
            var cat = await this.repositorio.getAsyc(id);
            return mapper.Map<AgendamentoDto>(cat);
        }

        public async Task removeAsyc(int id)
        {
            var cat = await this.repositorio.getAsyc(id);
            if (cat != null)
                await this.repositorio.removeAsyc(cat);
        }

        public async Task updateAsync(AgendamentoDto agendamentoDto)
        {
            var existente = await repositorio.getAsyc(agendamentoDto.Id);
            if (existente == null)
                throw new Exception("Agendamento não encontrado para atualização.");

            if (existente.Status == "Executado" && agendamentoDto.Status == "Cancelado")
                throw new Exception("Não é permitido cancelar um agendamento já executado.");
            var cat = mapper.Map<Agendamento>(agendamentoDto);
            await this.repositorio.updateAsync(cat);
        }

    }
}

