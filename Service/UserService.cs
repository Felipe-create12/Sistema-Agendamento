using AutoMapper;
using Dominio.Dto;
using Dominio.Entidades;
using Interface.RepositorioI;
using Interface.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserService: IUserService
    {
        private IUserRepositorio repositorio;

        private IMapper mapper;

        public UserService(IUserRepositorio repositorio,
            IMapper mapper)
        {
            this.repositorio = repositorio;
            this.mapper = mapper;
        }

        public async Task<UserDto> addAsync(UserDto userDtos)
        {
            var existe = await repositorio.getAllAsync(u => u.user == userDtos.user);
            if (existe.Any())
                throw new Exception("Já existe um usuário com este nome.");

            var entidade = mapper.Map<User>(userDtos);

            // Ideal: aplicar hash na senha antes de salvar
            // entidade.senha = BCrypt.Net.BCrypt.HashPassword(userDto.senha);

            entidade = await repositorio.addAsync(entidade);
            return mapper.Map<UserDto>(entidade);
        }

        public async Task<IEnumerable<UserDto>> getAllAsync(Expression<Func<User, bool>> expression)
        {
            var listaCat =
               await this.repositorio.getAllAsync(expression);
            return mapper.Map<IEnumerable<UserDto>>(listaCat);
        }

        public async Task<UserDto?> getAsyc(int id)
        {
            var cat = await this.repositorio.getAsyc(id);
            return mapper.Map<UserDto>(cat);
        }

        public async Task removeAsyc(int id)
        {
            var cat = await this.repositorio.getAsyc(id);
            if (cat != null)
                await this.repositorio.removeAsyc(cat);
        }

        public async Task updateAsync(UserDto userDtos)
        {
            var userExistente = await repositorio.getAsyc(userDtos.Id);
            if (userExistente == null)
                throw new Exception("Usuário não encontrado.");

            // Se for alterar o nome, validar se já existe outro igual
            var duplicado = await repositorio.getAllAsync(u => u.user == userDtos.user && u.Id != userDtos.Id);
            if (duplicado.Any())
                throw new Exception("Já existe outro usuário com este nome.");

            var entidade = mapper.Map<User>(userDtos);
            await repositorio.updateAsync(entidade);
        }

        public async Task<UserDto> RegisterAsync(UserRegisterDto registerDto)
        {
            if (registerDto.Senha != registerDto.ConfirmarSenha)
                throw new Exception("As senhas não coincidem.");

            var existe = await repositorio.getAllAsync(u => u.user == registerDto.User);
            if (existe.Any())
                throw new Exception("Usuário já existe.");

            var entidade = new User
            {
                user = registerDto.User,
                senha = registerDto.Senha // ideal criptografar
            };

            entidade = await repositorio.addAsync(entidade);
            return mapper.Map<UserDto>(entidade);
        }
    }
}
