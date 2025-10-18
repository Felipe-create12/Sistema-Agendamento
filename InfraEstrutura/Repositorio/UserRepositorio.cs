﻿using Dominio.Entidades;
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
    public class UserRepositorio: IUserRepositorio
    {
        private EmpresaContexto contexto;

        public UserRepositorio(EmpresaContexto contexto) {
            this.contexto = contexto;

        }

        public async Task<User> addAsync(User users)
        {
            await this.contexto.users.AddAsync(users);
            await this.contexto.SaveChangesAsync();
            return users;
        }

        public async Task<IEnumerable<User>> getAllAsync(Expression<Func<User, bool>> expression)
        {
            return await
               this.contexto.users
               .Where(expression)
               .OrderBy(p => p.user)
               .ToListAsync();
        }

        public async Task<User?> getAsyc(int id)
        {
            return await
               this.contexto.users
               .Where(p => p.Id == id)
               .FirstOrDefaultAsync();
        }

        public async Task removeAsyc(User users)
        {
            this.contexto.users.Remove(users);
            await this.contexto.SaveChangesAsync();
        }

        public async Task updateAsync(User users)
        {
            this.contexto.Entry(users).State
                = EntityState.Modified;
            await this.contexto.SaveChangesAsync();
        }
    }
}
