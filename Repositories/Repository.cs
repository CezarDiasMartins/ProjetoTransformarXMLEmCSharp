using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using TransformarXmlEmCSharpESalvarNoBanco.Models;
using TransformarXmlEmCSharpESalvarNoBanco.Repositories.EvtBasesTrab;
using TransformarXmlEmCSharpESalvarNoBanco.Repositories.EvtDeslig;
using TransformarXmlEmCSharpESalvarNoBanco.Repositories.EvtPgtos;
using TransformarXmlEmCSharpESalvarNoBanco.Repositories.EvtRemun;
using TransformarXmlEmCSharpESalvarNoBanco.Repositories.EvtTabRubrica;
using TransformarXmlEmCSharpESalvarNoBanco.Services;
using TransformarXmlEmCSharpESalvarNoBanco.Services.EvtBasesTrab;
using TransformarXmlEmCSharpESalvarNoBanco.Services.EvtDeslig;
using TransformarXmlEmCSharpESalvarNoBanco.Services.EvtPgtos;
using TransformarXmlEmCSharpESalvarNoBanco.Services.EvtRemun;
using TransformarXmlEmCSharpESalvarNoBanco.Services.EvtTabRubrica;

namespace TransformarXmlEmCSharpESalvarNoBanco.Repositories
{
    public class Repository
    {
        public void TransformarXmlEmCSharpESalvarNoBanco()
        {
            string connectionString = "Server=localhost;Database=projeto99freelasxmlemcsharp;User Id=root;";

            try
            {   
                var id_usuario = 1;
                var id_cadastro_arquivo = 1;
                var id_cadastro_envios = 1;

                var caminhoXMLs = @"E:/Cezar/99Freelas/ExtrairDadosXMLEstruturadoComBaseEmXSDEmCSharp/ProjetoPrincipal3/xmlsTeste";
                var xmls = Directory.GetFiles(caminhoXMLs, "*.xml");

                foreach (var xml in xmls)
                {
                    // NESSA PARTE, EU DESSERIALIZO O XML PARA PEGAR SOMENTE O CABEÇALHO DELE.
                    // ASSIM, EU PEGO O TIPO DO XML PELO NAMESPACE E CONSIGO COLOCÁ-LO EM SEU if DETERMINADO
                    var _service = new Service();
                    var evt = _service.Desserializar(xml);
                    var evtNamespaceEvento = evt.RetornoProcessamentoDownload.Evento.ESocialEvento.Namespace;
                    var evtNamespaceRecibo = evt?.RetornoProcessamentoDownload?.Recibo?.ESocialRecibo?.Namespace;

                    var ultimaBarraIndex = evtNamespaceEvento.LastIndexOf('/');
                    var penultimaBarraIndex = evtNamespaceEvento.LastIndexOf('/', ultimaBarraIndex - 1);
                    var tipoEvt = evtNamespaceEvento.Substring(penultimaBarraIndex + 1, ultimaBarraIndex - penultimaBarraIndex - 1);

                    if (tipoEvt == "evtDeslig")
                    {
                        var _evtDesligService = new EvtDesligService();
                        var evtDeslig = _evtDesligService.DesserializarEvtDeslig(xml, evtNamespaceEvento, evtNamespaceRecibo);

                        var _evtDesligRepository = new EvtDesligRepository();
                        _evtDesligRepository.InsertEvtDeslig(connectionString, evtDeslig, xml, id_cadastro_envios, id_cadastro_arquivo);

                        File.Delete(xml);
                    }
                    else if (tipoEvt == "evtPgtos")
                    {
                        var _evtPgtosService = new EvtPgtosService();
                        var evtPgtos = _evtPgtosService.DesserializarEvtPgtos(xml, evtNamespaceEvento, evtNamespaceRecibo);

                        // Verificar versão
                        var versao_layout_evtPgtos = Path.GetFileName(evtNamespaceEvento);

                        if (versao_layout_evtPgtos == "v02_05_00" || versao_layout_evtPgtos == "v02_04_00" || versao_layout_evtPgtos == "v02_04_02")
                        {
                            var _evtPgtosRepository = new EvtPgtos_V02_0_Repository();
                            _evtPgtosRepository.InsertEvtPgtos(connectionString, evtPgtos, xml, id_cadastro_envios, id_cadastro_arquivo);
                        }
                        else if (versao_layout_evtPgtos == "v_S_01_00_00")
                        {
                            var _evtPgtosRepository = new EvtPgtos_v_S_01_00_Repository();
                            _evtPgtosRepository.InsertEvtPgtos(connectionString, evtPgtos, xml, id_cadastro_envios, id_cadastro_arquivo);
                        }
                        else if (versao_layout_evtPgtos == "v_S_01_01_00")
                        {
                            var _evtPgtosRepository = new EvtPgtos_v_S_01_01_Repository();
                            _evtPgtosRepository.InsertEvtPgtos(connectionString, evtPgtos, xml, id_cadastro_envios, id_cadastro_arquivo);
                        }
                        else if (versao_layout_evtPgtos == "v_S_01_02_00")
                        {
                            var _evtPgtosRepository = new EvtPgtos_v_S_01_02_Repository();
                            _evtPgtosRepository.InsertEvtPgtos(connectionString, evtPgtos, xml, id_cadastro_envios, id_cadastro_arquivo);
                        }

                        File.Delete(xml);
                    }
                    else if (tipoEvt == "evtRemun")
                    {
                        var _evtRemunService = new EvtRemunService();
                        var evtRemun = _evtRemunService.DesserializarEvtRemun(xml, evtNamespaceEvento, evtNamespaceRecibo);

                        var _evtRemunRepository = new EvtRemunRepository();
                        _evtRemunRepository.InsertEvtDeslig(connectionString, evtRemun, xml, id_cadastro_envios, id_cadastro_arquivo);

                        File.Delete(xml);
                    }
                    else if (tipoEvt == "evtTabRubrica")
                    {
                        var _evtTabRubricaService = new EvtTabRubricaService();
                        var evtTabRubrica = _evtTabRubricaService.DesserializarEvtTabRubrica(xml, evtNamespaceEvento, evtNamespaceRecibo);

                        var _evtTabRubricaRepository = new EvtTabRubricaRepository();
                        _evtTabRubricaRepository.InsertEvtTabRubrica(connectionString, evtTabRubrica, xml, id_cadastro_envios, id_usuario);

                        File.Delete(xml);
                    }
                    else if (tipoEvt == "evtBasesTrab")
                    {
                        var _evtBasesTrabService = new EvtBasesTrabService();
                        var evtBasesTrab = _evtBasesTrabService.DesserializarEvtDeslig(xml, evtNamespaceEvento, evtNamespaceRecibo);

                        var _evtBasesTrabRepository = new EvtBasesTrabRepository();
                        _evtBasesTrabRepository.InsertEvtBasesTrab(connectionString, evtBasesTrab, xml, id_cadastro_envios);

                        File.Delete(xml);
                    }
                    else
                    {
                        Console.WriteLine("Não foi identificado nenhum tipo de evento!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
            }
        }

        public bool ExcluirRegistroAntigo(string connectionString, string tabela, int idRegistroExcluido)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = $"DELETE FROM {tabela} WHERE id = @id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", idRegistroExcluido);

                    try
                    {
                        command.ExecuteNonQuery();
                        return true;
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"Erro ao excluir da tabela {tabela} o registro: {ex.Message}");
                        return true;
                    }
                }
            }
        }

        public bool InserirArquivosExcluidos(string connectionString, int id_projeto, string tabela, int id_registro_excluido)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var sql = "INSERT INTO tb_itens_excluidos (id_projeto, tabela, id_registro_excluido) VALUES (@id_projeto, @tabela, @id_registro_excluido)";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id_projeto", id_projeto);
                    command.Parameters.AddWithValue("@tabela", tabela);
                    command.Parameters.AddWithValue("@id_registro_excluido", id_registro_excluido);

                    try
                    {
                        command.ExecuteNonQuery();
                        return true;
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine("Erro ao inserir registro na tb_itens_excluidos: " + ex.Message);
                        return false;
                    }
                }
            }
        }

        public VerificarProjetosModel VerificarProjetos(string connectionString, string competenciaProcurarProjeto, string cnpjcpf, int id_cadastro_envios)
        {
            var verificarProjetosModel = new VerificarProjetosModel();

            //USAR PRA TESTE
            competenciaProcurarProjeto = "01/2020";
            cnpjcpf = "03572731";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var sql_verifica_projetos = "SELECT * FROM projetos WHERE competencia = @competencia " +
                    "AND raiz_cnpj = @cnpjcpf AND situacao = '0' AND id_cadastro_envios = @id_cadastro_envios";

                using (MySqlCommand cmd = new MySqlCommand(sql_verifica_projetos, connection))
                {
                    cmd.Parameters.AddWithValue("@competencia", competenciaProcurarProjeto);
                    cmd.Parameters.AddWithValue("@cnpjcpf", cnpjcpf);
                    cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        var conta_projeto = 0;

                        while (reader.Read())
                        {
                            if (conta_projeto == 0)
                            {
                                verificarProjetosModel.Id_projeto = reader.GetInt32(reader.GetOrdinal("id"));
                                verificarProjetosModel.Id_usuario = reader.GetInt32(reader.GetOrdinal("id_usuario"));
                                verificarProjetosModel.Cnpj_empresa = reader.GetString(reader.GetOrdinal("cnpj_empresa"));
                            }

                            conta_projeto++;
                        }
                        verificarProjetosModel.Conta_projeto = conta_projeto;

                        return verificarProjetosModel;
                    }
                }
            }
        }

        public void InserirAquivosRejeitados(string competenciaProcurarProjeto, string cnpjcpf, string connectionString, string url, int idCadastroArquivo, int idCadastroEnvios)
        {
            var origemRejeicao = "1";
            var arquivosEnviadosRejeitado = "arquivos_enviados";
            var descricaoRejeicao = $"Não encontrato projeto com competencia = {competenciaProcurarProjeto} e raiz cnpj = {cnpjcpf}";
            var idProjeto = "0";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var sql = "INSERT INTO arquivos_rejeitados_importacao (id_projeto, origem, caminho, id_registro, nome_tabela, " +
                    "descricao_rejeicao, id_cadastro_envios) " +
                    "VALUES (@id_projeto, @origem, @caminho, @id_registro, @nome_tabela, @descricao_rejeicao, @id_cadastro_envios)";

                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@id_projeto", idProjeto);
                    cmd.Parameters.AddWithValue("@origem", origemRejeicao);
                    cmd.Parameters.AddWithValue("@caminho", url);
                    cmd.Parameters.AddWithValue("@id_registro", idCadastroArquivo);
                    cmd.Parameters.AddWithValue("@nome_tabela", arquivosEnviadosRejeitado);
                    cmd.Parameters.AddWithValue("@descricao_rejeicao", descricaoRejeicao);
                    cmd.Parameters.AddWithValue("@id_cadastro_envios", idCadastroEnvios);

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erro ao inserir arquivo na tabela arquivos_rejeitados_importacao: " + ex.Message);
                    }
                }
            }
        }

        public string BuscararNomeTabela(string sql)
        {
            // Regex para encontrar a tabela após a palavra-chave FROM
            var regex = new Regex(@"FROM\s+([a-zA-Z0-9_]+)", RegexOptions.IgnoreCase);
            var match = regex.Match(sql);
            if (match.Success)
                return match.Groups[1].Value;
            return "";
        }
    }
}