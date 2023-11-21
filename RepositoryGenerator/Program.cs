namespace RepositoryGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ProjetoDTO projeto = CriarProjetoAceleraDB();

                /*
SELECT table_name 
FROM information_schema.tables
WHERE table_type = 'BASE TABLE';
                 */

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
                string[] tabelas = entidadesParaGerar.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                int total = 0;
                bool arquivoCriado;

                foreach (var item in tabelas)
                {
                    string tableName = item;
                    string entityName = ConvertTableNameToEntityName(tableName);

                    if(tableName.IndexOf(",") != -1)
                    {
                        string[] partes = tableName.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        tableName = partes[0];
                        entityName = partes[1];
                    }

                    string iRepositoryInterfaceContent = GetTemplate(projeto.TemplateInterface, tableName, entityName);
                    string arquivoIRepositoryPath = GetFilePath(repositoryInterfacesPath, "I" + tableName + "Repository.cs");
                    arquivoCriado = CriarArquivo(arquivoIRepositoryPath, iRepositoryInterfaceContent);

                    string repositoryContent = GetTemplate(projeto.TemplateRepository, tableName, entityName);
                    string arquivoRepositoryPath = GetFilePath(repositoryPath, tableName + "Repository.cs");
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
                return true;
            }
            return false;
        }

        static string GetTemplate(string template, string tableName, string entityName)
        {
            template = template.Replace("{entityname}", entityName);
            template = template.Replace("{tablename}", tableName);
            return template;
        }

        static string ConvertTableNameToEntityName(string tableName)
        {
            if(tableName.ToLower().EndsWith("s"))
            {
                tableName = tableName.Substring(0, tableName.Length - 1);
            }
            return tableName;
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
    public interface I{tablename}Repository : IGenericRepository<{entityname}>
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
    internal class {tablename}Repository : GenericRepository<{entityname}>, I{tablename}Repository
    {
        public {tablename}Repository(StandonlineContext context, IConfiguration configuration) : base(context, configuration)
        {
        }
    }
}

"
            };
        }


    }
}