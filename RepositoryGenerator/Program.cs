namespace RepositoryGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ProjetoDTO projeto = CriarProjetoAceleraDB();

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

                #region . Execution .

                string repositoryInterfacesPath = projeto.RepositoryInterfaceFolder;
                string repositoryPath = projeto.RepositoryFolder;
                string[] entidades = entidadesParaGerar.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                int total = 0;
                bool arquivoCriado;

                foreach (var item in entidades)
                {
                    string entityName = item;

                    string iRepositoryInterfaceContent = GetTemplate(projeto.TemplateInterface, entityName);
                    string arquivoIRepositoryPath = GetFilePath(repositoryInterfacesPath, entityName + "Repository.cs");
                    arquivoCriado = CriarArquivo(arquivoIRepositoryPath, iRepositoryInterfaceContent);

                    string repositoryContent = GetTemplate(projeto.TemplateRepository, entityName);
                    string arquivoRepositoryPath = GetFilePath(repositoryPath, "I" + entityName + "Repository.cs");
                    arquivoCriado = CriarArquivo(arquivoRepositoryPath, repositoryContent);

                    if (arquivoCriado)
                    {
                        total++;
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Total gerado: " + total);

                #endregion
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
            if (!System.IO.File.Exists(path))
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

        static string GetTemplate(string template, string entityName)
        {
            template = template.Replace("{entityname}", entityName);
            return template;
        }

        static ProjetoDTO CriarProjetoStandOnline()
        {
            return new ProjetoDTO();
        }

        static ProjetoDTO CriarProjetoAceleraDB()
        {
            return new ProjetoDTO()
            {
                RepositoryFolder = @"C:\dados\git_ivera\AceleraTurboAPI\AceleraTurbo\StandOnline.Infrastructure\Impl\Data\Repositories",

                RepositoryInterfaceFolder = @"C:\dados\git_ivera\AceleraTurboAPI\AceleraTurbo\StandOnline.Domain\Interfaces\Repositories",

                TemplateInterface = @"

using StandOnline.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandOnline.Domain.Interfaces.Repositories
{
    public interface I{entityname}Repository : IGenericRepository<{entityname}>
    {

    }
}

"
                                ,
                TemplateRepository = @"
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StandOnline.Domain.Entities;
using StandOnline.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandOnline.Infrastructure.Impl.Data.Repositories
{
    internal class {entityname}Repository : GenericRepository<{entityname}>, I{entityname}Repository
    {
        public {entityname}Repository(StandonlineContext context, IConfiguration configuration) : base(context, configuration)
        {
        }
    }
}

"
            };
        }


    }
}