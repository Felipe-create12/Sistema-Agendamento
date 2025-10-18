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
    public class ProfissionalRepositorio: IProfissionalRepositorio
    {
        private EmpresaContexto contexto;

        public ProfissionalRepositorio(EmpresaContexto contexto)
        {
            this.contexto = contexto;
        }

        public async Task<Profissional> addAsync(Profissional profissional)
        {
            await this.contexto.profissional.AddAsync(profissional);
            await this.contexto.SaveChangesAsync();
            return profissional;
        }

        public async Task<IEnumerable<Profissional>> getAllAsync(Expression<Func<Profissional, bool>> expression)
        {
            return await
               this.contexto.profissional
               .Where(expression)
               .Include(p => p.agendamentos)
               .OrderBy(p => p.nome)
               .ToListAsync();
        }

        public async Task<Profissional?> getAsyc(int id)
        {
            return await
               this.contexto.profissional
               .Where(p => p.Id == id)
               .Include(p => p.agendamentos)
               .FirstOrDefaultAsync();
        }

        public async Task removeAsyc(Profissional profissional)
        {
            this.contexto.profissional.Remove(profissional);
            await this.contexto.SaveChangesAsync();
        }

        public async Task updateAsync(Profissional profissional)
        {
            this.contexto.Entry(profissional).State
                = EntityState.Modified;
            await this.contexto.SaveChangesAsync();
        }
    }
}

