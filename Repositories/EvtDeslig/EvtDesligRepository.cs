using MySql.Data.MySqlClient;
using System.Data;
using System.Globalization;
using TransformarXmlEmCSharpESalvarNoBanco.Models.EvtDeslig;

namespace TransformarXmlEmCSharpESalvarNoBanco.Repositories.EvtDeslig
{
    public class EvtDesligRepository
    {
        public void InsertEvtDeslig(string connectionString, ESocialEvtDeslig eSocialEvtDeslig, string arquivo, int id_cadastro_envios, int id_cadastro_arquivo)
        {
            var evtDeslig_id = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.Id;
            var ideEvento_indRetif = Convert.ToInt32(eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.IdeEvento?.IndRetif);
            var ideEvento_nrRecibo = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.IdeEvento?.NrRecibo;
            var ideEvento_tpAmb = Convert.ToInt32(eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.IdeEvento?.TpAmb);
            var ideEvento_procEmi = Convert.ToInt32(eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.IdeEvento?.ProcEmi);
            var ideEvento_verProc = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.IdeEvento?.VerProc;
            var ideEmpregador_tpInsc = Convert.ToInt32(eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.IdeEmpregador?.TpInsc);
            var ideEmpregador_nrInsc = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.IdeEmpregador?.NrInsc;
            var ideVinculo_cpfTrab = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.IdeVinculo?.CpfTrab;
            var ideVinculo_nisTrab = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.IdeVinculo?.NisTrab;
            var ideVinculo_matricula = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.IdeVinculo?.Matricula;
            var infoDeslig_mtvDeslig = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.MtvDeslig;
            var infoDeslig_dtDeslig = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.DtDeslig;
            var infoDeslig_indPagtoAPI = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.IndPagtoAPI;
            var infoDeslig_dtProjFimAPI = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.DtProjFimAPI;
            var infoDeslig_pensAlim = Convert.ToInt32(eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.PensAlim);
            var infoDeslig_percAliment = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.PercAliment;
            var infoDeslig_vrAlim = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.VrAlim;
            var infoDeslig_nrCertObito = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.NrCertObito;
            var infoDeslig_nrProcTrab = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.NrProcTrab;
            var infoDeslig_indPDV = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.IndPDV;
            var infoDeslig_indCumprParc = Convert.ToInt32(eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.IndCumprParc);
            var infoDeslig_sucessaoVinc_tpInscSuc = Convert.ToInt32(eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.SucessaoVinc?.TpInscSuc);
            var infoDeslig_sucessaoVinc_cnpjSucessora = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.SucessaoVinc?.CnpjSucessora;
            var infoDeslig_transfTit_cpfSubstituto = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.TransfTit?.CpfSubstituto;
            var infoDeslig_transfTit_dtNascto = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.TransfTit?.DtNascto;
            var infoDeslig_mudancaCPF_novoCPF = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.MudancaCPF?.NovoCPF;
            var infoDeslig_quarentena_dtFimQuar = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.Quarentena?.DtFimQuar;
            var recepcao_processamento_dhProcessamento = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Recibo?.ESocial?.RetornoEvento?.Processamento?.DhProcessamento;
            var original_numero_lote = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Recibo?.ESocial?.RetornoEvento?.Recepcao?.ProtocoloEnvioLote;
            var recibo_nrRecibo = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Recibo?.ESocial?.RetornoEvento?.ReciboRetornoEvento?.NrRecibo;
            var recibo_hash = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Recibo?.ESocial?.RetornoEvento?.ReciboRetornoEvento?.Hash;

            var infoDeslig_remunAposDeslig_indRemun = 0;
            var infoDeslig_remunAposDeslig_dtFimRemun = "";
            var infoDeslig_remunAposDeslig_verifica = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.RemunAposDeslig;
            if (infoDeslig_remunAposDeslig_verifica != null && infoDeslig_remunAposDeslig_verifica.Count > 0)
            {
                foreach (var item_remunAposDeslig in infoDeslig_remunAposDeslig_verifica)
                {
                    infoDeslig_remunAposDeslig_indRemun = Convert.ToInt32(item_remunAposDeslig.IndRemun);
                    infoDeslig_remunAposDeslig_dtFimRemun = item_remunAposDeslig.DtFimRemun ?? "";
                }
            }

            var data_extracao = DateTime.Now;
            var versao_layout_evtDeslig = Path.GetFileName(eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.Namespace);
            var nome_arquivo_importado = Path.GetFileName(arquivo);

            string[] competencia_procurar_projeto = infoDeslig_dtDeslig.Split('-');
            var competenciaProcurarProjeto = competencia_procurar_projeto[1] + "/" + competencia_procurar_projeto[0];

            var cnpjcpf = new string(ideEmpregador_nrInsc.Where(char.IsDigit).ToArray()).Substring(0, 8);

            var repository = new Repository();
            var verificarProjetos = repository.VerificarProjetos(connectionString, competenciaProcurarProjeto, cnpjcpf, id_cadastro_envios);

            if (verificarProjetos.Conta_projeto > 0)
            {
                var id_arquivo_ja_cadastrado = 0;
                var data_arquivo_ja_cadastrado = new DateTime();
                var data_arquivo_novo = Convert.ToDateTime(recepcao_processamento_dhProcessamento);
                var sql_verifica_duplicidade = "SELECT id, ideVinculo_cpfTrab, recepcao_processamento_dhProcessamento " +
                        "FROM s_2299_evtdeslig WHERE id_projeto = @id_projeto AND ideVinculo_cpfTrab = @ideVinculo_cpfTrab";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql_verifica_duplicidade, connection))
                    {
                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                        cmd.Parameters.AddWithValue("@ideVinculo_cpfTrab", ideVinculo_cpfTrab);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            var conta_projeto = 0;
                            while (reader.Read())
                            {
                                if (conta_projeto == 0)
                                {
                                    data_arquivo_ja_cadastrado = reader.GetDateTime("recepcao_processamento_dhProcessamento");
                                    id_arquivo_ja_cadastrado = reader.GetInt32(reader.GetOrdinal("id"));
                                }
                                conta_projeto++;
                            }
                        }
                    }
                }

                if (data_arquivo_novo > data_arquivo_ja_cadastrado)
                {
                    // EXCLUIR O ARQUIVO ANTIGO
                    var tabela = repository.BuscararNomeTabela(sql_verifica_duplicidade);
                    var exclusao = repository.ExcluirRegistroAntigo(connectionString, tabela, id_arquivo_ja_cadastrado);
                    var inserirArquivoAntigoNaTabelaDeExcluidos = repository.InserirArquivosExcluidos(connectionString, verificarProjetos.Id_projeto, tabela, id_arquivo_ja_cadastrado);

                    if (exclusao == true && inserirArquivoAntigoNaTabelaDeExcluidos == true)
                    {
                        // INSERIR O ARQUIVO NOVO
                        long id_evtdeslig = 0;
                        using (MySqlConnection connectionInsert_s_2299_evtdeslig = new MySqlConnection(connectionString))
                        {
                            connectionInsert_s_2299_evtdeslig.Open();
                            var sqlInsert_s_2299_evtdeslig = @"INSERT INTO s_2299_evtdeslig (data_extracao, id_projeto, id_usuario, 
            evtDeslig_id, ideEvento_indRetif, ideEvento_nrRecibo, ideEvento_tpAmb, ideEvento_procEmi, ideEvento_verProc, 
            ideEmpregador_tpInsc, ideEmpregador_nrInsc, ideVinculo_cpfTrab, ideVinculo_nisTrab, ideVinculo_matricula, 
            infoDeslig_mtvDeslig, infoDeslig_dtDeslig, infoDeslig_indPagtoAPI, infoDeslig_dtProjFimAPI, infoDeslig_pensAlim, 
            infoDeslig_percAliment, infoDeslig_vrAlim, infoDeslig_nrCertObito, infoDeslig_nrProcTrab, infoDeslig_indPDV, infoDeslig_indCumprParc, 
            infoDeslig_sucessaoVinc_tpInscSuc, infoDeslig_sucessaoVinc_cnpjSucessora, 
            infoDeslig_transfTit_cpfSubstituto, infoDeslig_transfTit_dtNascto, infoDeslig_mudancaCPF_novoCPF, 
            infoDeslig_quarentena_dtFimQuar, infoDeslig_remunAposDeslig_indRemun, infoDeslig_remunAposDeslig_dtFimRemun, 
            recepcao_processamento_dhProcessamento, recibo_nrRecibo, recibo_hash, original_numero_lote, versao_layout_evtDeslig,
            id_cadastro_envios, nome_arquivo_importado)
            VALUES(@data_extracao, @id_projeto, @id_usuario, @evtDeslig_id, 
            @ideEvento_indRetif, @ideEvento_nrRecibo, @ideEvento_tpAmb, @ideEvento_procEmi, 
            @ideEvento_verProc, @ideEmpregador_tpInsc, @ideEmpregador_nrInsc, @ideVinculo_cpfTrab, @ideVinculo_nisTrab, 
            @ideVinculo_matricula, @infoDeslig_mtvDeslig, @infoDeslig_dtDeslig, @infoDeslig_indPagtoAPI, @infoDeslig_dtProjFimAPI, 
            @infoDeslig_pensAlim, @infoDeslig_percAliment, @infoDeslig_vrAlim, @infoDeslig_nrCertObito, @infoDeslig_nrProcTrab, @infoDeslig_indPDV,
            @infoDeslig_indCumprParc, @infoDeslig_sucessaoVinc_tpInscSuc, @infoDeslig_sucessaoVinc_cnpjSucessora, 
            @infoDeslig_transfTit_cpfSubstituto, @infoDeslig_transfTit_dtNascto, @infoDeslig_mudancaCPF_novoCPF, @infoDeslig_quarentena_dtFimQuar, 
            @infoDeslig_remunAposDeslig_indRemun, @infoDeslig_remunAposDeslig_dtFimRemun, @recepcao_processamento_dhProcessamento, 
            @recibo_nrRecibo, @recibo_hash, @original_numero_lote, @versao_layout_evtDeslig, @id_cadastro_envios, @nome_arquivo_importado)";


                            using (MySqlCommand cmdInsert_s_2299_evtdeslig = new MySqlCommand(sqlInsert_s_2299_evtdeslig, connectionInsert_s_2299_evtdeslig))
                            {
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@data_extracao", data_extracao.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@evtDeslig_id", evtDeslig_id ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@ideEvento_indRetif", ideEvento_indRetif);
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@ideEvento_nrRecibo", recibo_nrRecibo ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@ideEvento_tpAmb", ideEvento_tpAmb);
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@ideEvento_procEmi", ideEvento_procEmi);
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@ideEvento_verProc", ideEvento_verProc ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@ideEmpregador_tpInsc", ideEmpregador_tpInsc);
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@ideEmpregador_nrInsc", ideEmpregador_nrInsc ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@ideVinculo_cpfTrab", ideVinculo_cpfTrab ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@ideVinculo_nisTrab", ideVinculo_nisTrab ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@ideVinculo_matricula", ideVinculo_matricula ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@infoDeslig_mtvDeslig", infoDeslig_mtvDeslig ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@infoDeslig_indPagtoAPI", infoDeslig_indPagtoAPI ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@infoDeslig_dtProjFimAPI", infoDeslig_dtProjFimAPI ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@infoDeslig_pensAlim", infoDeslig_pensAlim);
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@infoDeslig_percAliment", infoDeslig_percAliment ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@infoDeslig_vrAlim", infoDeslig_vrAlim ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@infoDeslig_nrCertObito", infoDeslig_nrCertObito ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@infoDeslig_nrProcTrab", infoDeslig_nrProcTrab ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@infoDeslig_indPDV", infoDeslig_indPDV ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@infoDeslig_indCumprParc", infoDeslig_indCumprParc);
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@infoDeslig_sucessaoVinc_tpInscSuc", infoDeslig_sucessaoVinc_tpInscSuc);
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@infoDeslig_sucessaoVinc_cnpjSucessora", infoDeslig_sucessaoVinc_cnpjSucessora ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@infoDeslig_transfTit_cpfSubstituto", infoDeslig_transfTit_cpfSubstituto ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@infoDeslig_transfTit_dtNascto", infoDeslig_transfTit_dtNascto ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@infoDeslig_mudancaCPF_novoCPF", infoDeslig_mudancaCPF_novoCPF ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@infoDeslig_quarentena_dtFimQuar", infoDeslig_quarentena_dtFimQuar ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@infoDeslig_remunAposDeslig_indRemun", infoDeslig_remunAposDeslig_indRemun);
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@infoDeslig_remunAposDeslig_dtFimRemun", infoDeslig_remunAposDeslig_dtFimRemun ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@recibo_nrRecibo", recibo_nrRecibo ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@recibo_hash", recibo_hash ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@original_numero_lote", original_numero_lote ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@versao_layout_evtDeslig", versao_layout_evtDeslig ?? "");
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@nome_arquivo_importado", nome_arquivo_importado ?? "");

                                // Convertendo infoDeslig_dtDeslig para o formato "yyyy-MM-dd"
                                DateTime infoDesligDtDeslig = DateTime.ParseExact(infoDeslig_dtDeslig, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@infoDeslig_dtDeslig", infoDesligDtDeslig.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "");

                                // Convertendo recepcao_processamento_dhProcessamento para o formato "yyyy-MM-dd HH:mm:ss" ou "yyyy-MM-dd HH:mm:sss"
                                int indexOfDot = recepcao_processamento_dhProcessamento.LastIndexOf('.');
                                if (indexOfDot != -1 && indexOfDot + 1 < recepcao_processamento_dhProcessamento.Length)
                                {
                                    string millisecondsPart = recepcao_processamento_dhProcessamento.Substring(indexOfDot + 1);

                                    if (millisecondsPart.Length == 2)
                                    {
                                        DateTime recepcaoProcessamentoDhProcessamento = DateTime.ParseExact(recepcao_processamento_dhProcessamento, "yyyy-MM-ddTHH:mm:ss.ff", CultureInfo.InvariantCulture);
                                        cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@recepcao_processamento_dhProcessamento", recepcaoProcessamentoDhProcessamento.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) ?? "");
                                    }
                                    if (millisecondsPart.Length == 3)
                                    {
                                        DateTime recepcaoProcessamentoDhProcessamento = DateTime.ParseExact(recepcao_processamento_dhProcessamento, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
                                        cmdInsert_s_2299_evtdeslig.Parameters.AddWithValue("@recepcao_processamento_dhProcessamento", recepcaoProcessamentoDhProcessamento.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) ?? "");
                                    }
                                }

                                try
                                {
                                    cmdInsert_s_2299_evtdeslig.ExecuteNonQuery();
                                    id_evtdeslig = cmdInsert_s_2299_evtdeslig.LastInsertedId;
                                    Console.WriteLine($"EvtDeslig({id_evtdeslig}) inserido na tabela s_2299_evtdeslig com sucesso!");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig: " + ex.Message);
                                }
                            }
                        }

                        var infoDeslig_infoInterm = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.InfoInterm;
                        if (infoDeslig_infoInterm != null && infoDeslig_infoInterm.Count > 0)
                        {
                            foreach (var itemInfoInterm in infoDeslig_infoInterm)
                            {
                                var infoIntermDia = Convert.ToInt32(itemInfoInterm?.Dia);

                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                {
                                    connection.Open();
                                    var query = "INSERT INTO s_2299_evtdeslig_infointerm (id_projeto, id_evtdeslig, infoInterm_dia, id_cadastro_envios) " +
                                        "VALUES (@id_projeto, @id_evtdeslig, @infoInterm_dia, @id_cadastro_envios)";

                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                    {
                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                        cmd.Parameters.AddWithValue("@id_evtdeslig", id_evtdeslig);
                                        cmd.Parameters.AddWithValue("@infoInterm_dia", infoIntermDia);
                                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                            Console.WriteLine("EvtDeslig inserido na tabela s_2299_evtdeslig_infointerm com sucesso!");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig_infointerm: " + ex.Message);
                                        }
                                    }
                                }
                            }
                        }

                        var observacoes_observacao_verifica = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.Observacoes;
                        if (observacoes_observacao_verifica != null && observacoes_observacao_verifica.Count > 0)
                        {
                            foreach (var item_observacoes in observacoes_observacao_verifica)
                            {
                                var observacoes_observacao = item_observacoes?.Observacao ?? "";

                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                {
                                    connection.Open();
                                    var query = "INSERT INTO s_2299_evtdeslig_obs (id_projeto, id_evtdeslig, observacoes_observacao) " +
                                        "VALUES(@id_projeto, @id_evtdeslig, @observacoes_observacao)";

                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                    {
                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                        cmd.Parameters.AddWithValue("@id_evtdeslig", id_evtdeslig);
                                        cmd.Parameters.AddWithValue("@observacoes_observacao", observacoes_observacao);

                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                            Console.WriteLine("EvtDeslig inserido na tabela s_2299_evtdeslig_obs com sucesso!");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig_obs: " + ex.Message);
                                        }
                                    }
                                }
                            }
                        }

                        var s_2299_evtdeslig_remunAposDeslig_verifica = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.RemunAposDeslig;
                        if (s_2299_evtdeslig_remunAposDeslig_verifica != null && s_2299_evtdeslig_remunAposDeslig_verifica.Count > 0)
                        {
                            foreach (var item_remunAposDeslig in s_2299_evtdeslig_remunAposDeslig_verifica)
                            {
                                var remunAposDeslig_indRemun = Convert.ToInt32(item_remunAposDeslig?.IndRemun);
                                var remunAposDeslig_dtFimRemun = item_remunAposDeslig?.DtFimRemun ?? "";

                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                {
                                    connection.Open();
                                    var query = "INSERT INTO s_2299_evtdeslig_remunaposdeslig (id_projeto, id_evtdeslig, remunAposDeslig_indRemun, remunAposDeslig_dtFimRemun) " +
                                        "VALUES(@id_projeto, @id_evtdeslig, @remunAposDeslig_indRemun, @remunAposDeslig_dtFimRemun)";

                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                    {
                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                        cmd.Parameters.AddWithValue("@id_evtdeslig", id_evtdeslig);
                                        cmd.Parameters.AddWithValue("@remunAposDeslig_indRemun", remunAposDeslig_indRemun);
                                        cmd.Parameters.AddWithValue("@remunAposDeslig_dtFimRemun", remunAposDeslig_dtFimRemun);

                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                            Console.WriteLine("EvtDeslig inserido na tabela s_2299_evtdeslig_remunaposdeslig com sucesso!");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig_remunaposdeslig: " + ex.Message);
                                        }
                                    }
                                }
                            }
                        }

                        var s_2299_evtdeslig_consigfgts_verifica = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.ConsigFGTS;
                        if (s_2299_evtdeslig_consigfgts_verifica != null && s_2299_evtdeslig_consigfgts_verifica.Count > 0)
                        {
                            foreach (var item_consigFGTS in s_2299_evtdeslig_consigfgts_verifica)
                            {
                                var consigFGTS_insConsig = item_consigFGTS?.InsConsig ?? "";
                                var consigFGTS_nrContr = item_consigFGTS?.NrContr ?? "";

                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                {
                                    connection.Open();
                                    var query = "INSERT INTO s_2299_evtdeslig_consigfgts (id_projeto, id_evtdeslig, consigFGTS_insConsig, consigFGTS_nrContr) " +
                                        "VALUES(@id_projeto, @id_evtdeslig, @consigFGTS_insConsig, @consigFGTS_nrContr)";

                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                    {
                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                        cmd.Parameters.AddWithValue("@id_evtdeslig", id_evtdeslig);
                                        cmd.Parameters.AddWithValue("@consigFGTS_insConsig", consigFGTS_insConsig);
                                        cmd.Parameters.AddWithValue("@consigFGTS_nrContr", consigFGTS_nrContr);

                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                            Console.WriteLine("EvtDeslig inserido na tabela s_2299_evtdeslig_consigfgts com sucesso!");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig_consigfgts: " + ex.Message);
                                        }
                                    }
                                }
                            }
                        }

                        var s_2299_evtdeslig_dmdev_verifica = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.VerbasResc?.DmDev;
                        if (s_2299_evtdeslig_dmdev_verifica != null && s_2299_evtdeslig_dmdev_verifica != null && s_2299_evtdeslig_dmdev_verifica.Count > 0)
                        {
                            foreach (var item_dmDev in s_2299_evtdeslig_dmdev_verifica)
                            {
                                var dmDev_ideDmDev = item_dmDev?.IdeDmDev ?? "";
                                var dmDev_indRRA = item_dmDev?.IndRRA ?? "";
                                long id_evtdeslig_dmdev = 0;

                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                {
                                    connection.Open();
                                    var query = "INSERT INTO s_2299_evtdeslig_dmdev (id_projeto, id_evtdeslig, dmDev_ideDmDev, dmDev_indRRA, id_cadastro_envios) " +
                                        "VALUES(@id_projeto, @id_evtdeslig, @dmDev_ideDmDev, @dmDev_indRRA, @id_cadastro_envios)";

                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                    {
                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                        cmd.Parameters.AddWithValue("@id_evtdeslig", id_evtdeslig);
                                        cmd.Parameters.AddWithValue("@dmDev_ideDmDev", dmDev_ideDmDev);
                                        cmd.Parameters.AddWithValue("@dmDev_indRRA", dmDev_indRRA);
                                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                            id_evtdeslig_dmdev = cmd.LastInsertedId;
                                            Console.WriteLine("EvtDeslig inserido na tabela s_2299_evtdeslig_dmdev com sucesso!");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig_dmdev: " + ex.Message);
                                        }
                                    }
                                }

                                var s_2299_evtdeslig_infoRRA_verifica = item_dmDev?.InfoRRA;
                                if (s_2299_evtdeslig_infoRRA_verifica != null && s_2299_evtdeslig_infoRRA_verifica.Count > 0)
                                {
                                    foreach (var item_infoRRA in s_2299_evtdeslig_infoRRA_verifica)
                                    {
                                        var infoRRA_tpProcRRA = item_infoRRA?.TpProcRRA ?? "";
                                        var infoRRA_nrProcRRA = item_infoRRA?.NrProcRRA ?? "";
                                        var infoRRA_descRRA = item_infoRRA?.DescRRA ?? "";
                                        var infoRRA_qtdMesesRRA = item_infoRRA?.QtdMesesRRA ?? "";
                                        long id_cadastra_infoRRA = 0;

                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            var query = "INSERT INTO s_2299_evtdeslig_dmdev_inforra (id_projeto, id_evtdeslig, id_dmdev, tpProcRRA, nrProcRRA, descRRA, qtdMesesRRA) " +
                                                "VALUES(@id_projeto, @id_evtdeslig, @id_dmdev, @tpProcRRA, @nrProcRRA, @descRRA, @qtdMesesRRA)";

                                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                            {
                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                cmd.Parameters.AddWithValue("@id_evtdeslig", id_evtdeslig);
                                                cmd.Parameters.AddWithValue("@id_dmdev", id_evtdeslig_dmdev);
                                                cmd.Parameters.AddWithValue("@tpProcRRA", infoRRA_tpProcRRA);
                                                cmd.Parameters.AddWithValue("@nrProcRRA", infoRRA_nrProcRRA);
                                                cmd.Parameters.AddWithValue("@descRRA", infoRRA_descRRA);
                                                cmd.Parameters.AddWithValue("@qtdMesesRRA", infoRRA_qtdMesesRRA);

                                                try
                                                {
                                                    cmd.ExecuteNonQuery();
                                                    id_cadastra_infoRRA = cmd.LastInsertedId;
                                                    Console.WriteLine("EvtDeslig inserido na tabela s_2299_evtdeslig_dmdev_inforra com sucesso!");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig_dmdev_inforra: " + ex.Message);
                                                }
                                            }
                                        }

                                        var s_2299_evtdeslig_despProcJud_verifica = item_infoRRA?.DespProcJud;
                                        if (s_2299_evtdeslig_despProcJud_verifica != null && s_2299_evtdeslig_despProcJud_verifica.Count > 0)
                                        {
                                            foreach (var item_despProcJud in s_2299_evtdeslig_despProcJud_verifica)
                                            {
                                                var despProcJud_vlrDespCustas = Convert.ToDecimal(item_despProcJud?.VlrDespCustas);
                                                var despProcJud_vlrDespAdvogados = Convert.ToDecimal(item_despProcJud?.VlrDespAdvogados);

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var query = "INSERT INTO s_2299_evtdeslig_dmdev_inforra_despprocjud (id_projeto, id_evtdeslig, id_dmdev, id_inforra, despProcJud_vlrDespCustas, despProcJud_vlrDespAdvogados) " +
                                                        "VALUES(@id_projeto, @id_evtdeslig, @id_dmdev, @id_inforra, @despProcJud_vlrDespCustas, @despProcJud_vlrDespAdvogados)";

                                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_evtdeslig", id_evtdeslig);
                                                        cmd.Parameters.AddWithValue("@id_dmdev", id_evtdeslig_dmdev);
                                                        cmd.Parameters.AddWithValue("@id_inforra", id_cadastra_infoRRA);
                                                        cmd.Parameters.AddWithValue("@despProcJud_vlrDespCustas", despProcJud_vlrDespCustas);
                                                        cmd.Parameters.AddWithValue("@despProcJud_vlrDespAdvogados", despProcJud_vlrDespAdvogados);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            Console.WriteLine("EvtDeslig inserido na tabela s_2299_evtdeslig_dmdev_inforra_despprocjud com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig_dmdev_inforra_despprocjud: " + ex.Message);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        var s_2299_evtdeslig_ideAdv_verifica = item_infoRRA?.IdeAdv;
                                        if (s_2299_evtdeslig_ideAdv_verifica != null && s_2299_evtdeslig_ideAdv_verifica.Count > 0)
                                        {
                                            foreach (var item_ideAdv in s_2299_evtdeslig_ideAdv_verifica)
                                            {
                                                var ideAdv_tpInsc = item_ideAdv?.TpInsc ?? "";
                                                var ideAdv_nrInsc = item_ideAdv?.NrInsc ?? "";
                                                var ideAdv_vlrAdv = Convert.ToDecimal(item_ideAdv?.VlrAdv);

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var query = "INSERT INTO s_2299_evtdeslig_dmdev_inforra_ideadv (id_projeto, id_evtdeslig, id_dmdev, id_inforra, " +
                                                        "ideAdv_tpInsc, ideAdv_nrInsc, ideAdv_vlrAdv) " +
                                                        "VALUES(@id_projeto, @id_evtdeslig, @id_dmdev, @id_inforra, @ideAdv_tpInsc, @ideAdv_nrInsc, " +
                                                        "@ideAdv_vlrAdv)";

                                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_evtdeslig", id_evtdeslig);
                                                        cmd.Parameters.AddWithValue("@id_dmdev", id_evtdeslig_dmdev);
                                                        cmd.Parameters.AddWithValue("@id_inforra", id_cadastra_infoRRA);
                                                        cmd.Parameters.AddWithValue("@ideAdv_tpInsc", ideAdv_tpInsc);
                                                        cmd.Parameters.AddWithValue("@ideAdv_nrInsc", ideAdv_nrInsc);
                                                        cmd.Parameters.AddWithValue("@ideAdv_vlrAdv", ideAdv_vlrAdv);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            Console.WriteLine("EvtDeslig inserido na tabela s_2299_evtdeslig_dmdev_inforra_ideadv com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig_dmdev_inforra_ideadv: " + ex.Message);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                var s_2299_evtdeslig_infoPerApur_ideEstabLot = item_dmDev?.InfoPerApur?.IdeEstabLot;
                                if (s_2299_evtdeslig_infoPerApur_ideEstabLot != null && s_2299_evtdeslig_infoPerApur_ideEstabLot.Count > 0)
                                {
                                    foreach (var item_dmDev_infoPerApur_ideEstabLot in s_2299_evtdeslig_infoPerApur_ideEstabLot)
                                    {
                                        var ideEstabLot_tpInsc = Convert.ToInt32(item_dmDev_infoPerApur_ideEstabLot?.TpInsc);
                                        var ideEstabLot_nrInsc = item_dmDev_infoPerApur_ideEstabLot?.NrInsc ?? "";
                                        var ideEstabLot_codLotacao = item_dmDev_infoPerApur_ideEstabLot?.CodLotacao ?? "";
                                        var infoSimples_indSimples = item_dmDev_infoPerApur_ideEstabLot?.InfoSimples?.IndSimples ?? "";
                                        var infoAgNocivo_grauExp = item_dmDev_infoPerApur_ideEstabLot?.InfoAgNocivo?.GrauExp ?? "";
                                        long id_infoPerApur_ideEstabLot = 0;

                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            var query = "INSERT INTO s_2299_evtdeslig_dmdev_infoperapur_ideestablot (id_projeto, " +
                                                "id_evtdeslig, id_dmdev, ideEstabLot_tpInsc, ideEstabLot_nrInsc, ideEstabLot_codLotacao, " +
                                                "infoAgNocivo_grauExp, infoSimples_indSimples, id_cadastro_envios) " +
                                                "VALUES(@id_projeto, @id_evtdeslig, @id_dmdev, @ideEstabLot_tpInsc, @ideEstabLot_nrInsc, " +
                                                "@ideEstabLot_codLotacao, @infoAgNocivo_grauExp, @infoSimples_indSimples, " +
                                                "@id_cadastro_envios)";

                                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                            {
                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                cmd.Parameters.AddWithValue("@id_evtdeslig", id_evtdeslig);
                                                cmd.Parameters.AddWithValue("@id_dmdev", id_evtdeslig_dmdev);
                                                cmd.Parameters.AddWithValue("@ideEstabLot_tpInsc", ideEstabLot_tpInsc);
                                                cmd.Parameters.AddWithValue("@ideEstabLot_nrInsc", ideEstabLot_nrInsc);
                                                cmd.Parameters.AddWithValue("@ideEstabLot_codLotacao", ideEstabLot_codLotacao);
                                                cmd.Parameters.AddWithValue("@infoAgNocivo_grauExp", infoAgNocivo_grauExp);
                                                cmd.Parameters.AddWithValue("@infoSimples_indSimples", infoSimples_indSimples);
                                                cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                try
                                                {
                                                    cmd.ExecuteNonQuery();
                                                    id_infoPerApur_ideEstabLot = cmd.LastInsertedId;
                                                    Console.WriteLine("EvtDeslig inserido na tabela s_2299_evtdeslig_dmdev_infoperapur_ideestablot com sucesso!");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig_dmdev_infoperapur_ideestablot: " + ex.Message);
                                                }
                                            }
                                        }

                                        var s_2299_evtdeslig_atual_verbas_verifica = item_dmDev_infoPerApur_ideEstabLot?.DetVerbas;
                                        if (s_2299_evtdeslig_atual_verbas_verifica != null && s_2299_evtdeslig_atual_verbas_verifica.Count > 0)
                                        {
                                            foreach (var item_atual_detVerbas in s_2299_evtdeslig_atual_verbas_verifica)
                                            {
                                                var detVerbas_codRubr = item_atual_detVerbas?.CodRubr ?? "";
                                                var detVerbas_ideTabRubr = item_atual_detVerbas?.IdeTabRubr ?? "";
                                                var detVerbas_qtdRubr = Convert.ToDecimal(item_atual_detVerbas?.QtdRubr);
                                                var detVerbas_fatorRubr = Convert.ToDecimal(item_atual_detVerbas?.FatorRubr);
                                                var detVerbas_vrUnit = Convert.ToDecimal(item_atual_detVerbas?.VrUnit);
                                                var detVerbas_vrRubr = Convert.ToDecimal(item_atual_detVerbas?.VrRubr);
                                                var situacao_original = "0";

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var query = "INSERT INTO s_2299_evtdeslig_atual_verbas (id_projeto, id_evtdeslig, id_dmdev, " +
                                                        "id_infoperapur_ideestablot, detVerbas_codRubr, detVerbas_ideTabRubr, detVerbas_qtdRubr, " +
                                                        "detVerbas_fatorRubr, detVerbas_vrUnit, detVerbas_vrRubr, situacao, id_cadastro_envios) " +
                                                        "VALUES(@id_projeto, @id_evtdeslig, @id_dmdev, @id_infoperapur_ideestablot, @detVerbas_codRubr, " +
                                                        "@detVerbas_ideTabRubr, @detVerbas_qtdRubr, @detVerbas_fatorRubr, @detVerbas_vrUnit, @detVerbas_vrRubr, " +
                                                        "@situacao, @id_cadastro_envios)";

                                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_evtdeslig", id_evtdeslig);
                                                        cmd.Parameters.AddWithValue("@id_dmdev", id_evtdeslig_dmdev);
                                                        cmd.Parameters.AddWithValue("@id_infoPerApur_ideEstabLot", id_infoPerApur_ideEstabLot);
                                                        cmd.Parameters.AddWithValue("@detVerbas_codRubr", detVerbas_codRubr);
                                                        cmd.Parameters.AddWithValue("@detVerbas_ideTabRubr", detVerbas_ideTabRubr);
                                                        cmd.Parameters.AddWithValue("@detVerbas_qtdRubr", detVerbas_qtdRubr);
                                                        cmd.Parameters.AddWithValue("@detVerbas_fatorRubr", detVerbas_fatorRubr);
                                                        cmd.Parameters.AddWithValue("@detVerbas_vrUnit", detVerbas_vrUnit);
                                                        cmd.Parameters.AddWithValue("@detVerbas_vrRubr", detVerbas_vrRubr);
                                                        cmd.Parameters.AddWithValue("@situacao", situacao_original);
                                                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            Console.WriteLine("EvtDeslig inserido na tabela s_2299_evtdeslig_atual_verbas com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig_atual_verbas: " + ex.Message);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        var infoPerApur_ideEstabLot_infoSaudeColet_verifica = item_dmDev_infoPerApur_ideEstabLot?.InfoSaudeColet;
                                        if (infoPerApur_ideEstabLot_infoSaudeColet_verifica != null && infoPerApur_ideEstabLot_infoSaudeColet_verifica.Count > 0)
                                        {
                                            foreach (var item_atual_detOper in infoPerApur_ideEstabLot_infoSaudeColet_verifica)
                                            {
                                                if (item_atual_detOper != null && item_atual_detOper?.DetOper?.Count > 0)
                                                {
                                                    foreach (var item_atual_detOper_detOper in item_atual_detOper.DetOper)
                                                    {
                                                        var detOper_cnpjOper = item_atual_detOper_detOper?.CnpjOper ?? "";
                                                        var detOper_regANS = item_atual_detOper_detOper?.RegANS ?? "";
                                                        var detOper_vrPgTit = Convert.ToDecimal(item_atual_detOper_detOper?.VrPgTit);
                                                        long id_atual_infoSaudeColet = 0;

                                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                        {
                                                            connection.Open();
                                                            var query = "INSERT INTO s_2299_evtdeslig_infoperapur_ideestablot_infosaudecolet (id_projeto, " +
                                                                "id_evtdeslig, id_dmdev, id_infoperapur_ideestablot, detOper_cnpjOper, detOper_regANS, " +
                                                                "detOper_vrPgTit) " +
                                                                "VALUES(@id_projeto, @id_evtdeslig, @id_dmdev, @id_infoperapur_ideestablot, @detOper_cnpjOper, " +
                                                                "@detOper_regANS, @detOper_vrPgTit)";

                                                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                                            {
                                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                                cmd.Parameters.AddWithValue("@id_evtdeslig", id_evtdeslig);
                                                                cmd.Parameters.AddWithValue("@id_dmdev", id_evtdeslig_dmdev);
                                                                cmd.Parameters.AddWithValue("@id_infoPerApur_ideEstabLot", id_infoPerApur_ideEstabLot);
                                                                cmd.Parameters.AddWithValue("@detOper_cnpjOper", detOper_cnpjOper);
                                                                cmd.Parameters.AddWithValue("@detOper_regANS", detOper_regANS);
                                                                cmd.Parameters.AddWithValue("@detOper_vrPgTit", detOper_vrPgTit);

                                                                try
                                                                {
                                                                    cmd.ExecuteNonQuery();
                                                                    id_atual_infoSaudeColet = cmd.LastInsertedId;
                                                                    Console.WriteLine("EvtDeslig inserido na tabela s_2299_evtdeslig_infoperapur_ideestablot_infosaudecolet com sucesso!");
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig_infoperapur_ideestablot_infosaudecolet: " + ex.Message);
                                                                }
                                                            }
                                                        }

                                                        if (item_atual_detOper_detOper != null && item_atual_detOper_detOper?.DetPlano.Count > 0)
                                                        {
                                                            foreach (var item_atual_detPlano in item_atual_detOper_detOper?.DetPlano)
                                                            {
                                                                var detPlano_tpDep = item_atual_detPlano?.TpDep ?? "";
                                                                var detPlano_cpfDep = item_atual_detPlano?.CpfDep ?? "";
                                                                var detPlano_nmDep = item_atual_detPlano?.NmDep ?? "";
                                                                var detPlano_dtNascto = item_atual_detPlano?.DtNascto ?? "";
                                                                var detPlano_vlrPgDep = Convert.ToDecimal(item_atual_detPlano?.VlrPgDep);

                                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                                {
                                                                    connection.Open();
                                                                    var query = "INSERT INTO s_2299_evtdeslig_infoperapur_ideestablot_infosaudecolet_detplano " +
                                                                        "(id_projeto, id_evtdeslig, id_dmdev, id_infoperapur_ideestablot, id_periodo_infoSaudeC, " +
                                                                        "detPlano_tpDep, detPlano_cpfDep, detPlano_nmDep, detPlano_dtNascto, detPlano_vlrPgDep) " +
                                                                        "VALUES(@id_projeto, @id_evtdeslig, @id_dmdev, @id_infoperapur_ideestablot, " +
                                                                        "@id_periodo_infoSaudeC, @detPlano_tpDep, @detPlano_cpfDep, @detPlano_nmDep, " +
                                                                        "@detPlano_dtNascto, @detPlano_vlrPgDep)";

                                                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                                                    {
                                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                                        cmd.Parameters.AddWithValue("@id_evtdeslig", id_evtdeslig);
                                                                        cmd.Parameters.AddWithValue("@id_dmdev", id_evtdeslig_dmdev);
                                                                        cmd.Parameters.AddWithValue("@id_infoPerApur_ideEstabLot", id_infoPerApur_ideEstabLot);
                                                                        cmd.Parameters.AddWithValue("@id_periodo_infoSaudeC", id_atual_infoSaudeColet);
                                                                        cmd.Parameters.AddWithValue("@detPlano_tpDep", detPlano_tpDep);
                                                                        cmd.Parameters.AddWithValue("@detPlano_cpfDep", detPlano_cpfDep);
                                                                        cmd.Parameters.AddWithValue("@detPlano_nmDep", detPlano_nmDep);
                                                                        cmd.Parameters.AddWithValue("@detPlano_dtNascto", detPlano_dtNascto);
                                                                        cmd.Parameters.AddWithValue("@detPlano_vlrPgDep", detPlano_vlrPgDep);

                                                                        try
                                                                        {
                                                                            cmd.ExecuteNonQuery();
                                                                            Console.WriteLine("EvtDeslig inserido na tabela s_2299_evtdeslig_infoperapur_ideestablot_infosaudecolet_detplano com sucesso!");
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig_infoperapur_ideestablot_infosaudecolet_detplano: " + ex.Message);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                var s_2299_evtdeslig_dmdev_infoPerAnt = item_dmDev?.InfoPerAnt;
                                if (s_2299_evtdeslig_dmdev_infoPerAnt != null && s_2299_evtdeslig_dmdev_infoPerAnt.Count > 0)
                                {
                                    foreach (var item_dmdev_infoPerAnt in s_2299_evtdeslig_dmdev_infoPerAnt)
                                    {
                                        var s_2299_evtdeslig_dmdev_infoPerAnt_ideADC = item_dmdev_infoPerAnt?.IdeADC;
                                        if (s_2299_evtdeslig_dmdev_infoPerAnt_ideADC != null && s_2299_evtdeslig_dmdev_infoPerAnt_ideADC.Count > 0)
                                        {
                                            foreach (var item_dmDev_infoPerAnt_ideADC in s_2299_evtdeslig_dmdev_infoPerAnt_ideADC)
                                            {
                                                var infoPerAnt_ideADC_dtAcConv = item_dmDev_infoPerAnt_ideADC?.DtAcConv ?? "";
                                                var infoPerAnt_ideADC_tpAcConv = item_dmDev_infoPerAnt_ideADC?.TpAcConv ?? "";
                                                var infoPerAnt_ideADC_compAcConv = item_dmDev_infoPerAnt_ideADC?.CompAcConv ?? "";
                                                var infoPerAnt_ideADC_dtEfAcConv = item_dmDev_infoPerAnt_ideADC?.DtEfAcConv ?? "";
                                                var infoPerAnt_ideADC_dsc = item_dmDev_infoPerAnt_ideADC?.Dsc ?? "";
                                                long id_infoPerAnt_ideADC = 0;

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var query = "INSERT INTO s_2299_evtdeslig_dmdev_infoperant_ideadc (id_projeto, id_evtdeslig, id_dmdev, " +
                                                        "infoPerAnt_ideADC_dtAcConv, infoPerAnt_ideADC_tpAcConv, infoPerAnt_ideADC_compAcConv, " +
                                                        "infoPerAnt_ideADC_dtEfAcConv, infoPerAnt_ideADC_dsc) " +
                                                        "VALUES(@id_projeto, @id_evtdeslig, @id_dmdev, @infoPerAnt_ideADC_dtAcConv, @infoPerAnt_ideADC_tpAcConv, " +
                                                        "@infoPerAnt_ideADC_compAcConv, @infoPerAnt_ideADC_dtEfAcConv, @infoPerAnt_ideADC_dsc)";

                                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_evtdeslig", id_evtdeslig);
                                                        cmd.Parameters.AddWithValue("@id_dmdev", id_evtdeslig_dmdev);
                                                        cmd.Parameters.AddWithValue("@infoPerAnt_ideADC_dtAcConv", infoPerAnt_ideADC_dtAcConv);
                                                        cmd.Parameters.AddWithValue("@infoPerAnt_ideADC_tpAcConv", infoPerAnt_ideADC_tpAcConv);
                                                        cmd.Parameters.AddWithValue("@infoPerAnt_ideADC_compAcConv", infoPerAnt_ideADC_compAcConv);
                                                        cmd.Parameters.AddWithValue("@infoPerAnt_ideADC_dtEfAcConv", infoPerAnt_ideADC_dtEfAcConv);
                                                        cmd.Parameters.AddWithValue("@infoPerAnt_ideADC_dsc", infoPerAnt_ideADC_dsc);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            id_infoPerAnt_ideADC = cmd.LastInsertedId;
                                                            Console.WriteLine("EvtDeslig inserido na tabela s_2299_evtdeslig_dmdev_infoperant_ideadc com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig_dmdev_infoperant_ideadc: " + ex.Message);
                                                        }
                                                    }
                                                }

                                                var dmDev_infoPerAnt_ideADC_IdePeriodo = item_dmDev_infoPerAnt_ideADC?.IdePeriodo;
                                                if (dmDev_infoPerAnt_ideADC_IdePeriodo != null && dmDev_infoPerAnt_ideADC_IdePeriodo.Count > 0)
                                                {
                                                    foreach (var item_dmDev_infoPerAnt_ideADC_IdePeriodo in dmDev_infoPerAnt_ideADC_IdePeriodo)
                                                    {
                                                        var idePeriodo_perRef = item_dmDev_infoPerAnt_ideADC_IdePeriodo?.PerRef ?? "";
                                                        long id_infoPerAnt_ideADC_perRef = 0;

                                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                        {
                                                            connection.Open();
                                                            var query = "INSERT INTO s_2299_evtdeslig_dmdev_infoperant_ideadc_ideperiodo_perref (id_projeto, " +
                                                                "id_evtdeslig, id_dmdev, id_infoperant_ideadc, idePeriodo_perRef) " +
                                                                "VALUES(@id_projeto, @id_evtdeslig, @id_dmdev, @id_infoperant_ideadc, @idePeriodo_perRef)";

                                                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                                            {
                                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                                cmd.Parameters.AddWithValue("@id_evtdeslig", id_evtdeslig);
                                                                cmd.Parameters.AddWithValue("@id_dmdev", id_evtdeslig_dmdev);
                                                                cmd.Parameters.AddWithValue("@id_infoPerAnt_ideADC", id_infoPerAnt_ideADC);
                                                                cmd.Parameters.AddWithValue("@idePeriodo_perRef", idePeriodo_perRef);

                                                                try
                                                                {
                                                                    cmd.ExecuteNonQuery();
                                                                    id_infoPerAnt_ideADC_perRef = cmd.LastInsertedId;
                                                                    Console.WriteLine("EvtDeslig inserido na tabela s_2299_evtdeslig_dmdev_infoperant_ideadc_ideperiodo_perref com sucesso!");
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig_dmdev_infoperant_ideadc_ideperiodo_perref: " + ex.Message);
                                                                }
                                                            }
                                                        }

                                                        var dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot = item_dmDev_infoPerAnt_ideADC_IdePeriodo.IdeEstabLot;
                                                        if (dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot != null && dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot.Count > 0)
                                                        {
                                                            foreach (var item_dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot in dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot)
                                                            {
                                                                var ideEstabLot_tpInsc = Convert.ToInt32(item_dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot?.TpInsc);
                                                                var ideEstabLot_nrInsc = item_dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot?.NrInsc ?? "";
                                                                var ideEstabLot_codLotacao = item_dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot?.CodLotacao ?? "";
                                                                var infoAgNocivo_grauExp = item_dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot?.InfoAgNocivo?.GrauExp ?? "";
                                                                var infoSimples_indSimples = item_dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot?.InfoSimples?.IndSimples ?? "";
                                                                long id_infoPerApur_ideEstabLot = 0;
                                                                long id_ant_ideEstabLot = 0;

                                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                                {
                                                                    connection.Open();
                                                                    var query = "INSERT INTO s_2299_evtdeslig_dmdev_infoperant_ideestablot (id_projeto, " +
                                                                        "id_evtdeslig, id_dmdev, id_infoperant_ideadc_idePeriodo_perRef, ideEstabLot_tpInsc, " +
                                                                        "ideEstabLot_nrInsc, ideEstabLot_codLotacao, infoAgNocivo_grauExp, infoSimples_indSimples) " +
                                                                        "VALUES(@id_projeto, @id_evtdeslig, @id_dmdev, @id_infoperant_ideadc_idePeriodo_perRef, " +
                                                                        "@ideEstabLot_tpInsc, @ideEstabLot_nrInsc, @ideEstabLot_codLotacao, @infoAgNocivo_grauExp, " +
                                                                        "@infoSimples_indSimples)";

                                                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                                                    {
                                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                                        cmd.Parameters.AddWithValue("@id_evtdeslig", id_evtdeslig);
                                                                        cmd.Parameters.AddWithValue("@id_dmdev", id_evtdeslig_dmdev);
                                                                        cmd.Parameters.AddWithValue("@id_infoperant_ideadc_idePeriodo_perRef", id_infoPerAnt_ideADC_perRef);
                                                                        cmd.Parameters.AddWithValue("@ideEstabLot_tpInsc", ideEstabLot_tpInsc);
                                                                        cmd.Parameters.AddWithValue("@ideEstabLot_nrInsc", ideEstabLot_nrInsc);
                                                                        cmd.Parameters.AddWithValue("@ideEstabLot_codLotacao", ideEstabLot_codLotacao);
                                                                        cmd.Parameters.AddWithValue("@infoAgNocivo_grauExp", infoAgNocivo_grauExp);
                                                                        cmd.Parameters.AddWithValue("@infoSimples_indSimples", infoSimples_indSimples);

                                                                        try
                                                                        {
                                                                            cmd.ExecuteNonQuery();
                                                                            id_ant_ideEstabLot = cmd.LastInsertedId;
                                                                            Console.WriteLine("EvtDeslig inserido na tabela s_2299_evtdeslig_dmdev_infoperant_ideestablot com sucesso!");
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig_dmdev_infoperant_ideestablot: " + ex.Message);
                                                                        }
                                                                    }
                                                                }

                                                                var dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot_DetVerbas = item_dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot.DetVerbas;
                                                                if (dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot_DetVerbas != null && dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot.Count > 0)
                                                                {
                                                                    foreach (var item_dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot_DetVerbas in dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot_DetVerbas)
                                                                    {
                                                                        var detVerbas_codRubr = item_dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot_DetVerbas?.CodRubr ?? "";
                                                                        var detVerbas_ideTabRubr = item_dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot_DetVerbas?.IdeTabRubr ?? "";
                                                                        var detVerbas_qtdRubr = item_dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot_DetVerbas?.QtdRubr ?? "";
                                                                        var detVerbas_fatorRubr = item_dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot_DetVerbas?.FatorRubr ?? "";
                                                                        var detVerbas_vrUnit = item_dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot_DetVerbas?.VrUnit ?? "";
                                                                        var detVerbas_vrRubr = item_dmDev_infoPerAnt_ideADC_IdePeriodo_IdeEstabLot_DetVerbas?.VrRubr ?? "";

                                                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                                        {
                                                                            connection.Open();
                                                                            var query = "INSERT INTO s_2299_evtdeslig_anterior_verbas (id_projeto, id_evtdeslig, " +
                                                                                "id_dmdev, id_infoPerAnt_ideestablot, detVerbas_codRubr, detVerbas_ideTabRubr, " +
                                                                                "detVerbas_qtdRubr, detVerbas_fatorRubr, detVerbas_vrUnit, detVerbas_vrRubr) " +
                                                                                "VALUES(@id_projeto, @id_evtdeslig, @id_dmdev, @id_infoPerAnt_ideestablot, " +
                                                                                "@detVerbas_codRubr, @detVerbas_ideTabRubr, @detVerbas_qtdRubr, " +
                                                                                "@detVerbas_fatorRubr, @detVerbas_vrUnit, @detVerbas_vrRubr)";

                                                                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                                                            {
                                                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                                                cmd.Parameters.AddWithValue("@id_evtdeslig", id_evtdeslig);
                                                                                cmd.Parameters.AddWithValue("@id_dmdev", id_evtdeslig_dmdev);
                                                                                cmd.Parameters.AddWithValue("@id_infoPerAnt_ideestablot", id_ant_ideEstabLot);
                                                                                cmd.Parameters.AddWithValue("@detVerbas_codRubr", detVerbas_codRubr);
                                                                                cmd.Parameters.AddWithValue("@detVerbas_ideTabRubr", detVerbas_ideTabRubr);
                                                                                cmd.Parameters.AddWithValue("@detVerbas_qtdRubr", detVerbas_qtdRubr);
                                                                                cmd.Parameters.AddWithValue("@detVerbas_fatorRubr", detVerbas_fatorRubr);
                                                                                cmd.Parameters.AddWithValue("@detVerbas_vrUnit", detVerbas_vrUnit);
                                                                                cmd.Parameters.AddWithValue("@detVerbas_vrRubr", detVerbas_vrRubr);

                                                                                try
                                                                                {
                                                                                    cmd.ExecuteNonQuery();
                                                                                    Console.WriteLine("EvtDeslig inserido na tabela s_2299_evtdeslig_anterior_verbas com sucesso!");
                                                                                }
                                                                                catch (Exception ex)
                                                                                {
                                                                                    Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig_anterior_verbas: " + ex.Message);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        var s_2299_evtdeslig_procJudTrab_verifica = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.VerbasResc?.ProcJudTrab;
                        if (s_2299_evtdeslig_procJudTrab_verifica != null && s_2299_evtdeslig_procJudTrab_verifica.Count > 0)
                        {
                            foreach (var item_procJudTrab in s_2299_evtdeslig_procJudTrab_verifica)
                            {
                                var procJudTrab_tpTrib = item_procJudTrab?.TpTrib ?? "";
                                var procJudTrab_nrProcJud = item_procJudTrab?.NrProcJud ?? "";
                                var procJudTrab_codSusp = item_procJudTrab?.CodSusp ?? "";

                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                {
                                    connection.Open();
                                    var query = "INSERT INTO s_2299_evtdeslig_procjudtrab (id_projeto, id_evtdeslig, procJudTrab_tpTrib, procJudTrab_nrProcJud, " +
                                        "procJudTrab_codSusp) " +
                                        "VALUES(@id_projeto, @id_evtdeslig, @procJudTrab_tpTrib, @procJudTrab_nrProcJud, @procJudTrab_codSusp)";

                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                    {
                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                        cmd.Parameters.AddWithValue("@id_evtdeslig", id_evtdeslig);
                                        cmd.Parameters.AddWithValue("@procJudTrab_tpTrib", procJudTrab_tpTrib);
                                        cmd.Parameters.AddWithValue("@procJudTrab_nrProcJud", procJudTrab_nrProcJud);
                                        cmd.Parameters.AddWithValue("@procJudTrab_codSusp", procJudTrab_codSusp);

                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                            Console.WriteLine("EvtDeslig inserido na tabela s_2299_evtdeslig_procjudtrab com sucesso!");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig_procjudtrab: " + ex.Message);
                                        }
                                    }
                                }
                            }
                        }

                        var s_2299_evtdeslig_infomv_verifica = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.VerbasResc?.InfoMV;
                        if (s_2299_evtdeslig_infomv_verifica != null && s_2299_evtdeslig_infomv_verifica.Count > 0)
                        {
                            foreach (var item_infoMV in s_2299_evtdeslig_infomv_verifica)
                            {
                                var infoMV_indMV = item_infoMV?.IndMV ?? "";
                                long id_evtdeslig_infomv = 0;

                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                {
                                    connection.Open();
                                    var query = "INSERT INTO s_2299_evtdeslig_procjudtrab (id_projeto, id_evtdeslig, procJudTrab_tpTrib, procJudTrab_nrProcJud, " +
                                        "procJudTrab_codSusp) " +
                                        "VALUES(@id_projeto, @id_evtdeslig, @procJudTrab_tpTrib, @procJudTrab_nrProcJud, @procJudTrab_codSusp)";

                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                    {
                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                        cmd.Parameters.AddWithValue("@id_evtdeslig", id_evtdeslig);
                                        cmd.Parameters.AddWithValue("@infoMV_indMV", infoMV_indMV);

                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                            id_evtdeslig_infomv = cmd.LastInsertedId;
                                            Console.WriteLine("EvtDeslig inserido na tabela s_2299_evtdeslig_procjudtrab com sucesso!");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig_procjudtrab: " + ex.Message);
                                        }
                                    }
                                }

                                var s_2299_evtdeslig_infomv_remunoutrempr_verifica = item_infoMV?.RemunOutrEmpr;
                                if (s_2299_evtdeslig_infomv_remunoutrempr_verifica != null && s_2299_evtdeslig_infomv_remunoutrempr_verifica.Count > 0)
                                {
                                    foreach (var item_infoMV_remunOutrEmpr in s_2299_evtdeslig_infomv_remunoutrempr_verifica)
                                    {
                                        var infoMV_remuneracao_tpInsc = item_infoMV_remunOutrEmpr?.TpInsc ?? "";
                                        var infoMV_remuneracao_nrInsc = item_infoMV_remunOutrEmpr?.NrInsc ?? "";
                                        var infoMV_remuneracao_codCateg = item_infoMV_remunOutrEmpr?.CodCateg ?? "";
                                        var infoMV_remuneracao_vlrRemunOE = Convert.ToDecimal(item_infoMV_remunOutrEmpr?.VlrRemunOE);

                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            var query = "INSERT INTO s_2299_evtdeslig_infomv_remunoutrempr (id_projeto, id_evtdeslig, id_evtdeslig_infomv, " +
                                                "infoMV_remuneracao_tpInsc, infoMV_remuneracao_nrInsc, infoMV_remuneracao_codCateg, infoMV_remuneracao_vlrRemunOE) " +
                                                "VALUES(@id_projeto, @id_evtdeslig, @id_evtdeslig_infomv, @infoMV_remuneracao_tpInsc, @infoMV_remuneracao_nrInsc, " +
                                                "@infoMV_remuneracao_codCateg, @infoMV_remuneracao_vlrRemunOE)";

                                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                            {
                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                cmd.Parameters.AddWithValue("@id_evtdeslig", id_evtdeslig);
                                                cmd.Parameters.AddWithValue("@infoMV_indMV", infoMV_indMV);
                                                cmd.Parameters.AddWithValue("@infoMV_remuneracao_tpInsc", infoMV_remuneracao_tpInsc);
                                                cmd.Parameters.AddWithValue("@infoMV_remuneracao_nrInsc", infoMV_remuneracao_nrInsc);
                                                cmd.Parameters.AddWithValue("@infoMV_remuneracao_codCateg", infoMV_remuneracao_codCateg);
                                                cmd.Parameters.AddWithValue("@infoMV_remuneracao_vlrRemunOE", infoMV_remuneracao_vlrRemunOE);

                                                try
                                                {
                                                    cmd.ExecuteNonQuery();
                                                    Console.WriteLine("EvtDeslig inserido na tabela s_2299_evtdeslig_infomv_remunoutrempr com sucesso!");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig_infomv_remunoutrempr: " + ex.Message);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        var s_2299_evtdeslig_procCS_verifica = eSocialEvtDeslig?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtDeslig?.InfoDeslig?.VerbasResc?.ProcCS;
                        if (s_2299_evtdeslig_procCS_verifica != null && s_2299_evtdeslig_procCS_verifica.Count > 0)
                        {
                            foreach (var item_procCS in s_2299_evtdeslig_procCS_verifica)
                            {
                                var procCS_nrProcJud = item_procCS?.NrProcJud ?? "";

                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                {
                                    connection.Open();
                                    var query = "INSERT INTO s_2299_evtdeslig_proccs (id_projeto, id_evtdeslig, procCS_nrProcJud) " +
                                        "VALUES(@id_projeto, @id_evtdeslig, @procCS_nrProcJud)";

                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                    {
                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                        cmd.Parameters.AddWithValue("@id_evtdeslig", id_evtdeslig);
                                        cmd.Parameters.AddWithValue("@procCS_nrProcJud", procCS_nrProcJud);

                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                            Console.WriteLine("EvtDeslig inserido na tabela s_2299_evtdeslig_proccs com sucesso!");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Erro ao inserir EvtDeslig na tabela s_2299_evtdeslig_proccs: " + ex.Message);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    repository.InserirAquivosRejeitados(competenciaProcurarProjeto, cnpjcpf, connectionString, arquivo, id_cadastro_arquivo, id_cadastro_envios);
                }
            }
            else
            {
                repository.InserirAquivosRejeitados(competenciaProcurarProjeto, cnpjcpf, connectionString, arquivo, id_cadastro_arquivo, id_cadastro_envios);
            }
        }
    }
}