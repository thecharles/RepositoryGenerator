namespace RepositoryGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string repositoryInterfacesPath = @"C:\dados\git_ivera\AceleraTurboAPI\AceleraTurbo\StandOnline.Domain\Interfaces\Repositories";
                string repositoryPath = @"C:\dados\git_ivera\AceleraTurboAPI\AceleraTurbo\StandOnline.Infrastructure\Impl\Data\Repositories";
                string entidadesParaGerar = @"
Aquecimento
AquecimentoDia
AquecimentoDialogos
Arquivos
Bases
Bots
BotsBases
BotsIntents
BotsIntentsComandos
BotsTemplates
BotsTemplatesComandos
Comandos
ContaSistemas
Contatos
ContatosBotsTemplates
ContatosMensagens
ContatosMensagensNaoProcessadas
LogsAtividades
PoolNumeros
Pools
UsuariosContaSistemas
Webhooks
";

                string[] entidades = entidadesParaGerar.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                int total = 0;
                bool arquivoCriado;

                foreach (var item in entidades)
                {
                    string entityName = item;
                    
                    string iRepositoryInterfaceContent = GetRepositoryInterfaceTemplate(entityName);
                    string arquivoIRepositoryPath = GetFilePath(repositoryInterfacesPath, entityName + "Repository.cs");
                    arquivoCriado = CriarArquivo(arquivoIRepositoryPath, iRepositoryInterfaceContent);

                    string repositoryContent = GetRepositoryTemplate(entityName);
                    string arquivoRepositoryPath = GetFilePath(repositoryPath, "I" + entityName + "Repository.cs");
                    arquivoCriado = CriarArquivo(arquivoRepositoryPath, repositoryContent);

                    if (arquivoCriado)
                    {
                        total++;
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Total gerado: " + total);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
            }
            Console.ReadKey();
        }

        static void ExplodirErroSeArquivoNaoExistir(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                throw new Exception($"O arquivo ({path}) não existe");
            }
        }

        static string GetFilePath(string path, string fileName)
        {
            return Path.Combine(path, fileName);
        }

        static bool CriarArquivo(string path, string content)
        {
            if(!System.IO.File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(content);
                }
                Console.WriteLine($"Arquivo ({path}) criado");
                return true;
            }
            return false;
        }

        static string GetRepositoryInterfaceTemplate(string entityName)
        {
            return $@"

using StandOnline.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandOnline.Domain.Interfaces.Repositories
{{
    public interface I{entityName}Repository : IGenericRepository<{entityName}>
    {{

    }}
}}


";
        }

        static string GetRepositoryTemplate(string entityName)
        {
            return $@"

using Microsoft.Extensions.Configuration;
using StandOnline.Domain.Entities;
using StandOnline.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandOnline.Infrastructure.Impl.Data.Repositories
{{
    internal class {entityName}Repository : GenericRepository<{entityName}>, I{entityName}Repository
    {{
        public {entityName}Repository(StandonlineContext context, IConfiguration configuration) : base(context, configuration)
        {{
        }}
    }}
}}


";
        }
    }
}
