using Dominio.Dto;
using Interface.Service;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;


namespace Sistema_Agendamento.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AgendamentoController : ControllerBase
    {
        private IAgendamentoService service;
        private IValidator<AgendamentoDto> validator;

        public AgendamentoController(IAgendamentoService service, IValidator<AgendamentoDto> validator)
        {
            this.service = service;
            this.validator = validator;
        }

        [HttpPost]
        public async Task<ActionResult<AgendamentoDto>>
            addAsync(AgendamentoDto agendamentoDto)
        {

            var result = validator.Validate(agendamentoDto);
            if (result.IsValid)
            {
                var dto = await this.service.addAsync(agendamentoDto);
                return Ok(dto);
            }
            else
                return BadRequest(result);


        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AgendamentoDto>>>
           getAllAsync()
        {
            var lista = await this.service.getAllAsync(p => true);
            return Ok(lista);

        }

        [HttpGet("filtrar/{status}")]
        public async Task<ActionResult<IEnumerable<AgendamentoDto>>> getDescricaoAsync(string status)
        {
            var lista = await this.service.getAllAsync(p => p.Status.Contains(status));
            return Ok(lista);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AgendamentoDto>>
            getAsync(int id)
        {
            var cat = await this.service.getAsyc(id);
            if (cat == null)
                return NotFound(); //não encontrou
            else
                return Ok(cat);
        }

        [HttpPut]
        public async Task<ActionResult>
            updateAsync(AgendamentoDto cat)
        {


            var result = validator.Validate(cat);
            if (result.IsValid)
            {
                await this.service.updateAsync(cat);
                return NoContent();
            }
            else return BadRequest(result);

        }

        [HttpGet("cliente/{idCliente}")]
        public async Task<ActionResult<IEnumerable<AgendamentoDto>>> GetByClienteAsync(int idCliente)
        {
            var lista = await this.service.getByClienteAsync(idCliente);

            if (lista == null || !lista.Any())
                return NotFound("Nenhum agendamento encontrado para este cliente.");

            return Ok(lista);
        }

    }
}
