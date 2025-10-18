using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dto
{
    public class AgendamentoDto
    {
        public int Id { get; set; }

        public int idServicio { get; set; }
        public int idProfissional { get; set; }
        public int idCliente { get; set; }
        public DateTime DataHora { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
