using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dto
{
    public class UserRegisterDto
    {
        public int Id { get; set; }
        public string User { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string ConfirmarSenha { get; set; } = string.Empty;
        public int? ClienteId { get; set; }
        public string Nome { get; set; } = string.Empty;
    }
}
