using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryGenerator
{
    public class ProjetoDTO
    {
        public string RepositoryFolder { get; set; }
        public string RepositoryInterfaceFolder { get; set; }
        public string TemplateInterface { get; set; }
        public string TemplateRepository { get; set; }
    }
}
