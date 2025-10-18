using Dominio.Entidades;
using InfraEstrutura.Data;
using Interface.RepositorioI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InfraEstrutura.Repositorio
{
    public class AgendamentoRepositorio : IAgendamentoRepositorio
    {
        private EmpresaContexto contexto;

        public AgendamentoRepositorio(EmpresaContexto contexto)
        {
            this.contexto = contexto;

        }

        public async Task<Agendamento> addAsync(Agendamento agendamento)
        {
            await this.contexto.agendamento.AddAsync(agendamento);
            await this.contexto.SaveChangesAsync();
            return agendamento;
        }

        public async Task<IEnumerable<Agendamento>> getAllAsync(Expression<Func<Agendamento, bool>> expression)
        {
            return await
               this.contexto.agendamento
               .Where(expression)
               .OrderBy(p => p.DataHora)
               .ToListAsync();
        }

        public async Task<Agendamento?> getAsyc(int id)
        {
            return await
              this.contexto.agendamento
              .Where(p => p.Id == id)
              .FirstOrDefaultAsync();
        }

        public async Task removeAsyc(Agendamento agendamento)
        {
            this.contexto.agendamento.Remove(agendamento);
            await this.contexto.SaveChangesAsync();
        }

        public async Task updateAsync(Agendamento agendamento)
        {
            this.contexto.Entry(agendamento).State
                = EntityState.Modified;
            await this.contexto.SaveChangesAsync();
        }
    }
}
