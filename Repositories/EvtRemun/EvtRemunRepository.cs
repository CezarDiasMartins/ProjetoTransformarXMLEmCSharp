using MySql.Data.MySqlClient;
using System.Globalization;
using TransformarXmlEmCSharpESalvarNoBanco.Models.EvtDeslig;
using TransformarXmlEmCSharpESalvarNoBanco.Models.EvtRemun;

namespace TransformarXmlEmCSharpESalvarNoBanco.Repositories.EvtRemun
{
    public class EvtRemunRepository
    {
        public void InsertEvtDeslig(string connectionString, ESocialEvtRemun eSocialEvtRemun, string arquivo, int id_cadastro_envios, int id_cadastro_arquivo)
        {
            var evtRemun_id = eSocialEvtRemun?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtRemun?.Id;
            var ideEvento_indRetif = Convert.ToInt32(eSocialEvtRemun?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtRemun?.IdeEvento?.IndRetif);
            var ideEvento_nrRecibo = eSocialEvtRemun?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtRemun?.IdeEvento?.NrRecibo;
            var ideEvento_indApuracao = eSocialEvtRemun?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtRemun?.IdeEvento?.IndApuracao;
            var ideEvento_perApur = eSocialEvtRemun?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtRemun?.IdeEvento?.PerApur;
            var ideEvento_tpAmb = Convert.ToInt32(eSocialEvtRemun?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtRemun?.IdeEvento?.TpAmb);
            var ideEvento_procEmi = Convert.ToInt32(eSocialEvtRemun?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtRemun?.IdeEvento?.ProcEmi);
            var ideEvento_verProc = eSocialEvtRemun?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtRemun?.IdeEvento?.VerProc;
            var ideEmpregador_tpInsc = Convert.ToInt32(eSocialEvtRemun?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtRemun?.IdeEmpregador?.TpInsc);
            var ideEmpregador_nrInsc = eSocialEvtRemun?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtRemun?.IdeEmpregador?.NrInsc;
            var ideTrabalhador_cpfTrab = eSocialEvtRemun?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtRemun?.IdeTrabalhador?.CpfTrab;
            var ideTrabalhador_nisTrab = eSocialEvtRemun?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtRemun?.IdeTrabalhador?.NisTrab;
            var recibo_nrRecibo = eSocialEvtRemun?.RetornoProcessamentoDownload?.Recibo?.ESocial?.RetornoEvento?.ReciboRetornoEvento?.NrRecibo;
            var recibo_hash = eSocialEvtRemun?.RetornoProcessamentoDownload?.Recibo?.ESocial?.RetornoEvento?.ReciboRetornoEvento?.Hash;
            var signature_DigestValue = eSocialEvtRemun?.RetornoProcessamentoDownload?.Recibo?.ESocial?.SignatureRecibo?.SignedInfoRecibo?.ReferenceRecibo?.DigestValue;
            var signature_SignatureValue = eSocialEvtRemun?.RetornoProcessamentoDownload?.Recibo?.ESocial?.SignatureRecibo?.SignatureValue;
            var recepcao_processamento_dhProcessamento = eSocialEvtRemun?.RetornoProcessamentoDownload?.Recibo?.ESocial?.RetornoEvento?.Processamento?.DhProcessamento;
            var original_numero_lote = eSocialEvtRemun?.RetornoProcessamentoDownload?.Recibo?.ESocial?.RetornoEvento?.Recepcao?.ProtocoloEnvioLote;
            var signature_X509Certificate = eSocialEvtRemun?.RetornoProcessamentoDownload?.Recibo?.ESocial?.SignatureRecibo?.KeyInfoRecibo?.X509DataRecibo?.X509Certificate;

            var versao_layout_evtRemun = Path.GetFileName(eSocialEvtRemun?.RetornoProcessamentoDownload?.Evento?.ESocial?.Namespace);
            var nome_arquivo_importado = Path.GetFileName(arquivo);

            var competencia_procurar_projeto = "";
            if (ideEvento_indApuracao == "2")
            {
                competencia_procurar_projeto = $"13/{ideEvento_perApur}";
            }
            else
            {
                string[] partesCompetencia = ideEvento_perApur.Split('-');
                competencia_procurar_projeto = partesCompetencia[1] + "/" + partesCompetencia[0];
            }

            var cnpjcpf = new string(ideEmpregador_nrInsc.Where(char.IsDigit).ToArray()).Substring(0, 8);

            var repository = new Repository();
            var verificarProjetos = repository.VerificarProjetos(connectionString, competencia_procurar_projeto, cnpjcpf, id_cadastro_envios);

            if (verificarProjetos.Conta_projeto > 0)
            {
                var id_arquivo_ja_cadastrado = 0;
                var data_arquivo_ja_cadastrado = new DateTime();
                var data_arquivo_novo = Convert.ToDateTime(recepcao_processamento_dhProcessamento);
                var sql_verifica_duplicidade = "SELECT id, ideTrabalhador_cpfTrab, recepcao_processamento_dhProcessamento " +
                    "FROM s_1200_evtremun WHERE id_projeto = @id_projeto AND ideTrabalhador_cpfTrab = @ideTrabalhador_cpfTrab";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql_verifica_duplicidade, connection))
                    {
                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                        cmd.Parameters.AddWithValue("@ideTrabalhador_cpfTrab", ideTrabalhador_cpfTrab);

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
                        long id_cad_evtremun = 0;
                        using (MySqlConnection connectionInsert_s_1200_evtremun = new MySqlConnection(connectionString))
                        {
                            connectionInsert_s_1200_evtremun.Open();
                            var sqlInsert_s_1200_evtremun = @"INSERT INTO s_1200_evtremun (id_projeto, id_usuario, evtRemun, 
            ideEvento_indRetif, ideEvento_nrRecibo, ideEvento_indApuracao, ideEvento_perApur, ideEvento_tpAmb, 
            ideEvento_procEmi, ideEvento_verProc, ideEmpregador_tpInsc, ideEmpregador_nrInsc, ideTrabalhador_cpfTrab, 
            ideTrabalhador_nisTrab, recibo_nrRecibo, recibo_hash, recepcao_processamento_dhProcessamento, 
            original_numero_lote, versao_layout_evtRemun, id_cadastro_envios, nome_arquivo_importado) VALUES(@id_projeto, 
            @id_usuario, @evtRemun, @ideEvento_indRetif, @ideEvento_nrRecibo, @ideEvento_indApuracao, @ideEvento_perApur, 
            @ideEvento_tpAmb, @ideEvento_procEmi, @ideEvento_verProc, @ideEmpregador_tpInsc, @ideEmpregador_nrInsc, 
            @ideTrabalhador_cpfTrab, @ideTrabalhador_nisTrab, @recibo_nrRecibo, @recibo_hash, 
            @recepcao_processamento_dhProcessamento, @original_numero_lote, @versao_layout_evtRemun,
            @id_cadastro_envios, @nome_arquivo_importado)";

                            using (MySqlCommand cmdInsert_s_1200_evtremun = new MySqlCommand(sqlInsert_s_1200_evtremun, connectionInsert_s_1200_evtremun))
                            {
                                cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@evtRemun", evtRemun_id ?? "");
                                cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@ideEvento_indRetif", ideEvento_indRetif);
                                cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@ideEvento_nrRecibo", ideEvento_nrRecibo ?? "");
                                cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@ideEvento_indApuracao", ideEvento_indApuracao ?? "");
                                cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@ideEvento_perApur", ideEvento_perApur ?? "");
                                cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@ideEvento_tpAmb", ideEvento_tpAmb);
                                cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@ideEvento_procEmi", ideEvento_procEmi);
                                cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@ideEvento_verProc", ideEvento_verProc ?? "");
                                cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@ideEmpregador_tpInsc", ideEmpregador_tpInsc);
                                cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@ideEmpregador_nrInsc", ideEmpregador_nrInsc ?? "");
                                cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@ideTrabalhador_cpfTrab", ideTrabalhador_cpfTrab ?? "");
                                cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@ideTrabalhador_nisTrab", ideTrabalhador_nisTrab ?? "");
                                cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@recibo_nrRecibo", recibo_nrRecibo ?? "");
                                cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@recibo_hash", recibo_hash ?? "");
                                cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@original_numero_lote", original_numero_lote ?? "");
                                cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@versao_layout_evtRemun", versao_layout_evtRemun ?? "");
                                cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);
                                cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@nome_arquivo_importado", nome_arquivo_importado ?? "");

                                // Convertendo recepcao_processamento_dhProcessamento para o formato "yyyy-MM-dd HH:mm:ss" ou "yyyy-MM-dd HH:mm:sss"
                                int indexOfDot = recepcao_processamento_dhProcessamento.LastIndexOf('.');
                                if (indexOfDot != -1 && indexOfDot + 1 < recepcao_processamento_dhProcessamento.Length)
                                {
                                    string millisecondsPart = recepcao_processamento_dhProcessamento.Substring(indexOfDot + 1);

                                    if (millisecondsPart.Length == 2)
                                    {
                                        DateTime recepcaoProcessamentoDhProcessamento = DateTime.ParseExact(recepcao_processamento_dhProcessamento, "yyyy-MM-ddTHH:mm:ss.ff", CultureInfo.InvariantCulture);
                                        cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@recepcao_processamento_dhProcessamento", recepcaoProcessamentoDhProcessamento.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) ?? "");
                                    }
                                    if (millisecondsPart.Length == 3)
                                    {
                                        DateTime recepcaoProcessamentoDhProcessamento = DateTime.ParseExact(recepcao_processamento_dhProcessamento, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
                                        cmdInsert_s_1200_evtremun.Parameters.AddWithValue("@recepcao_processamento_dhProcessamento", recepcaoProcessamentoDhProcessamento.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) ?? "");
                                    }
                                }

                                try
                                {
                                    cmdInsert_s_1200_evtremun.ExecuteNonQuery();
                                    id_cad_evtremun = cmdInsert_s_1200_evtremun.LastInsertedId;
                                    Console.WriteLine($"EvtRemun({id_cad_evtremun}) inserido na tabela s_1200_evtremun com sucesso!");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Erro ao inserir EvtRemun na tabela s_1200_evtremun: " + ex.Message);
                                }
                            }
                        }

                        var verifica_InfoMV = eSocialEvtRemun?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtRemun?.IdeTrabalhador?.InfoMV;
                        if (verifica_InfoMV != null && verifica_InfoMV.Count > 0)
                        {
                            foreach (var item_infoMV in verifica_InfoMV)
                            {
                                var infoMV_indMV = item_infoMV.IndMV ?? "";
                                long id_infoMV = 0;

                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                {
                                    connection.Open();
                                    var query = "INSERT INTO s_1200_infomv (id_projeto, id_usuario, id_cad_evtremun, infoMV_indMV)" +
                                        "VALUES(@id_projeto, @id_usuario, @id_cad_evtremun, @infoMV_indMV";

                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                    {
                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                        cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                        cmd.Parameters.AddWithValue("@id_cad_evtremun", id_cad_evtremun);
                                        cmd.Parameters.AddWithValue("@infoMV_indMV", infoMV_indMV);

                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                            id_infoMV = cmd.LastInsertedId;
                                            Console.WriteLine("EvtRemun inserido na tabela s_1200_infomv com sucesso!");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Erro ao inserir EvtRemun na tabela s_1200_infomv: " + ex.Message);
                                        }
                                    }
                                }

                                var verifica_RemunOutrEmpr = item_infoMV.RemunOutrEmpr;
                                if (verifica_RemunOutrEmpr != null && verifica_RemunOutrEmpr.Count > 0)
                                {
                                    foreach (var item_remunOutrEmpr in verifica_RemunOutrEmpr)
                                    {
                                        var remunOutrEmpr_tpInsc = Convert.ToInt32(item_remunOutrEmpr.TpInsc);
                                        var remunOutrEmpr_nrInsc = item_remunOutrEmpr.NrInsc ?? "";
                                        var remunOutrEmpr_codCateg = item_remunOutrEmpr.CodCateg ?? "";
                                        var remunOutrEmpr_vlrRemunOE = item_remunOutrEmpr.VlrRemunOE ?? "";

                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            var query = @"INSERT INTO s_1200_infomv_remunoutrempr (id_projeto, id_usuario, id_cad_evtremun, id_infoMV,
                            remunOutrEmpr_tpInsc, remunOutrEmpr_nrInsc, remunOutrEmpr_codCateg, remunOutrEmpr_vlrRemunOE)
                            VALUES(@id_projeto, @id_usuario, @id_cad_evtremun, @id_infoMV,
                            @remunOutrEmpr_tpInsc, @remunOutrEmpr_nrInsc, @remunOutrEmpr_codCateg, @remunOutrEmpr_vlrRemunOE)";

                                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                            {
                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                cmd.Parameters.AddWithValue("@id_cad_evtremun", id_cad_evtremun);
                                                cmd.Parameters.AddWithValue("@id_infoMV", id_infoMV);
                                                cmd.Parameters.AddWithValue("@remunOutrEmpr_tpInsc", remunOutrEmpr_tpInsc);
                                                cmd.Parameters.AddWithValue("@remunOutrEmpr_nrInsc", remunOutrEmpr_nrInsc);
                                                cmd.Parameters.AddWithValue("@remunOutrEmpr_codCateg", remunOutrEmpr_codCateg);
                                                cmd.Parameters.AddWithValue("@remunOutrEmpr_vlrRemunOE", remunOutrEmpr_vlrRemunOE);

                                                try
                                                {
                                                    cmd.ExecuteNonQuery();
                                                    Console.WriteLine("EvtRemun inserido na tabela s_1200_infomv_remunoutrempr com sucesso!");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Erro ao inserir EvtRemun na tabela s_1200_infomv_remunoutrempr: " + ex.Message);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        var verifica_InfoComplem = eSocialEvtRemun?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtRemun?.IdeTrabalhador?.InfoComplem;
                        if (verifica_InfoComplem != null && verifica_InfoComplem.Count > 0)
                        {
                            foreach (var item_infoComplem in verifica_InfoComplem)
                            {
                                var infoComplem_nmTrab = item_infoComplem.NmTrab ?? "";
                                var infoComplem_dtNascto = item_infoComplem.DtNascto ?? "";
                                long id_infoComplem = 0;

                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                {
                                    connection.Open();
                                    var query = @"INSERT INTO s_1200_infocomplem(id_projeto, id_cad_evtremun, infoComplem_nmTrab, infoComplem_dtNascto)
                        VALUES(@id_projeto, @id_cad_evtremun, @infoComplem_nmTrab, @infoComplem_dtNascto)";

                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                    {
                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                        cmd.Parameters.AddWithValue("@id_cad_evtremun", id_cad_evtremun);
                                        cmd.Parameters.AddWithValue("@infoComplem_nmTrab", infoComplem_nmTrab);
                                        cmd.Parameters.AddWithValue("@infoComplem_dtNascto", infoComplem_dtNascto);

                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                            id_infoComplem = cmd.LastInsertedId;
                                            Console.WriteLine("EvtRemun inserido na tabela s_1200_infocomplem com sucesso!");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Erro ao inserir EvtRemun na tabela s_1200_infocomplem: " + ex.Message);
                                        }
                                    }
                                }

                                var verifica_SucessaoVinc = item_infoComplem.SucessaoVinc;
                                if (verifica_SucessaoVinc != null && verifica_SucessaoVinc.Count > 0)
                                {
                                    foreach (var item_sucessaoVinc in verifica_SucessaoVinc)
                                    {
                                        var sucessaoVinc_tpInsc = Convert.ToInt32(item_sucessaoVinc.TpInsc);
                                        var sucessaoVinc_nrInsc = item_sucessaoVinc.NrInsc ?? "";
                                        var sucessaoVinc_matricAnt = item_sucessaoVinc.MatricAnt ?? "";
                                        var sucessaoVinc_dtAdm = item_sucessaoVinc.DtAdm ?? "";
                                        var sucessaoVinc_observacao = item_sucessaoVinc.Observacao ?? "";

                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            var query = @"INSERT INTO s_1200_infocomplem_sucessaovinc
                            (id_projeto, id_cad_evtremun, id_infoComplem, sucessaoVinc_tpInsc, sucessaoVinc_nrInsc, sucessaoVinc_matricAnt, 
                            sucessaoVinc_dtAdm, sucessaoVinc_observacao)
                            VALUES(@id_projeto, @id_cad_evtremun, @id_infoComplem, @sucessaoVinc_tpInsc, @sucessaoVinc_nrInsc, 
                            @sucessaoVinc_matricAnt, @sucessaoVinc_dtAdm, @sucessaoVinc_observacao)";

                                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                            {
                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                cmd.Parameters.AddWithValue("@id_cad_evtremun", id_cad_evtremun);
                                                cmd.Parameters.AddWithValue("@id_infoComplem", id_infoComplem);
                                                cmd.Parameters.AddWithValue("@sucessaoVinc_tpInsc", sucessaoVinc_tpInsc);
                                                cmd.Parameters.AddWithValue("@sucessaoVinc_nrInsc", sucessaoVinc_nrInsc);
                                                cmd.Parameters.AddWithValue("@sucessaoVinc_matricAnt", sucessaoVinc_matricAnt);
                                                cmd.Parameters.AddWithValue("@sucessaoVinc_dtAdm", sucessaoVinc_dtAdm);
                                                cmd.Parameters.AddWithValue("@sucessaoVinc_observacao", sucessaoVinc_observacao);

                                                try
                                                {
                                                    cmd.ExecuteNonQuery();
                                                    Console.WriteLine("EvtRemun inserido na tabela s_1200_infocomplem_sucessaovinc com sucesso!");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Erro ao inserir EvtRemun na tabela s_1200_infocomplem_sucessaovinc: " + ex.Message);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        var verifica_ProcJudTrab = eSocialEvtRemun?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtRemun?.IdeTrabalhador?.ProcJudTrab;
                        if (verifica_ProcJudTrab != null && verifica_ProcJudTrab.Count > 0)
                        {
                            foreach (var item_procJudTrab in verifica_ProcJudTrab)
                            {
                                var procJudTrab_tpTrib = item_procJudTrab.TpTrib ?? "";
                                var procJudTrab_nrProcJud = item_procJudTrab.NrProcJud ?? "";
                                var procJudTrab_codSusp = item_procJudTrab.CodSusp ?? "";

                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                {
                                    connection.Open();
                                    var query = @"INSERT INTO s_1200_procjudtrab (id_projeto, id_cad_evtremun, procJudTrab_tpTrib, procJudTrab_nrProcJud, 
                                    procJudTrab_codSusp)
                                    VALUES (@id_projeto, @id_cad_evtremun, @procJudTrab_tpTrib, @procJudTrab_nrProcJud, @procJudTrab_codSusp)";

                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                    {
                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                        cmd.Parameters.AddWithValue("@id_cad_evtremun", id_cad_evtremun);
                                        cmd.Parameters.AddWithValue("@procJudTrab_tpTrib", procJudTrab_tpTrib);
                                        cmd.Parameters.AddWithValue("@procJudTrab_nrProcJud", procJudTrab_nrProcJud);
                                        cmd.Parameters.AddWithValue("@procJudTrab_codSusp", procJudTrab_codSusp);

                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                            Console.WriteLine("EvtRemun inserido na tabela s_1200_procjudtrab com sucesso!");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Erro ao inserir EvtRemun na tabela s_1200_procjudtrab: " + ex.Message);
                                        }
                                    }
                                }
                            }
                        }

                        var verifica_InfoInterm = eSocialEvtRemun?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtRemun?.IdeTrabalhador?.InfoInterm;
                        if (verifica_InfoInterm != null && verifica_InfoInterm.Count > 0)
                        {
                            foreach (var item_infoInterm in verifica_InfoInterm)
                            {
                                var infoInterm_dia = item_infoInterm.Dia ?? "";

                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                {
                                    connection.Open();
                                    var query = @"INSERT INTO s_1200_infointerm (id_projeto, id_cad_evtremun, infoInterm_dia)
                                            VALUES(@id_projeto, @id_cad_evtremun, @infoInterm_dia)";

                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                    {
                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                        cmd.Parameters.AddWithValue("@id_cad_evtremun", id_cad_evtremun);
                                        cmd.Parameters.AddWithValue("@infoInterm_dia", infoInterm_dia);

                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                            Console.WriteLine("EvtRemun inserido na tabela s_1200_infointerm com sucesso!");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Erro ao inserir EvtRemun na tabela s_1200_infointerm: " + ex.Message);
                                        }
                                    }
                                }
                            }
                        }

                        var verifica_DmDev = eSocialEvtRemun?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtRemun?.DmDev;
                        if (verifica_DmDev != null && verifica_DmDev.Count > 0)
                        {
                            foreach (var item in verifica_DmDev)
                            {
                                var dmDev_ideDmDev = item.IdeDmDev ?? "";
                                var dmDev_icodCateg = Convert.ToInt32(item.CodCateg);
                                long id_cadastra_s_1200_dmdev = 0;

                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                {
                                    connection.Open();
                                    var query = @"INSERT INTO s_1200_dmdev (id_projeto, id_usuario, id_cad_evtremun, dmDev_ideDmDev, dmDev_icodCateg, 
                                    id_cadastro_envios)
                                    VALUES(@id_projeto, @id_usuario, @id_cad_evtremun, @dmDev_ideDmDev, @dmDev_icodCateg, @id_cadastro_envios)";

                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                    {
                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                        cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                        cmd.Parameters.AddWithValue("@id_cad_evtremun", id_cad_evtremun);
                                        cmd.Parameters.AddWithValue("@dmDev_ideDmDev", dmDev_ideDmDev);
                                        cmd.Parameters.AddWithValue("@dmDev_icodCateg", dmDev_icodCateg);
                                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                            id_cadastra_s_1200_dmdev = cmd.LastInsertedId;
                                            Console.WriteLine("EvtRemun inserido na tabela s_1200_infointerm com sucesso!");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Erro ao inserir EvtRemun na tabela s_1200_infointerm: " + ex.Message);
                                        }
                                    }
                                }

                                var verifica_IdeEstabLot = item.InfoPerApur?.IdeEstabLot;
                                if (verifica_IdeEstabLot != null && verifica_IdeEstabLot.Count > 0)
                                {
                                    foreach (var item_ideEstabLot in verifica_IdeEstabLot)
                                    {
                                        var ideEstabLot_tpInsc = Convert.ToInt32(item_ideEstabLot.TpInsc);
                                        var ideEstabLot_nrInsc = item_ideEstabLot.NrInsc ?? "";
                                        var ideEstabLot_codLotacao = item_ideEstabLot.CodLotacao ?? "";
                                        var ideEstabLot_qtdDiasAv = Convert.ToInt32(item_ideEstabLot.QtdDiasAv);
                                        long id_s_1200_dmdev_periodo_ideestablot = 0;

                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            var query = @"INSERT INTO s_1200_dmdev_periodo_ideestablot ( id_projeto, id_usuario, id_cad_evtremun, id_s_1200_dmdev, 
                            ideEstabLot_tpInsc, ideEstabLot_nrInsc, ideEstabLot_codLotacao, ideEstabLot_qtdDiasAv, 
                            id_cadastro_envios) VALUES(@id_projeto, @id_usuario, @id_cad_evtremun, @id_s_1200_dmdev, @ideEstabLot_tpInsc, 
                            @ideEstabLot_nrInsc, @ideEstabLot_codLotacao, @ideEstabLot_qtdDiasAv, @id_cadastro_envios)";

                                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                            {
                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                cmd.Parameters.AddWithValue("@id_cad_evtremun", id_cad_evtremun);
                                                cmd.Parameters.AddWithValue("@id_s_1200_dmdev", id_cadastra_s_1200_dmdev);
                                                cmd.Parameters.AddWithValue("@ideEstabLot_tpInsc", ideEstabLot_tpInsc);
                                                cmd.Parameters.AddWithValue("@ideEstabLot_nrInsc", ideEstabLot_nrInsc);
                                                cmd.Parameters.AddWithValue("@ideEstabLot_codLotacao", ideEstabLot_codLotacao);
                                                cmd.Parameters.AddWithValue("@ideEstabLot_qtdDiasAv", ideEstabLot_qtdDiasAv);
                                                cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                try
                                                {
                                                    cmd.ExecuteNonQuery();
                                                    id_s_1200_dmdev_periodo_ideestablot = cmd.LastInsertedId;
                                                    Console.WriteLine("EvtRemun inserido na tabela s_1200_dmdev_periodo_ideestablot com sucesso!");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Erro ao inserir EvtRemun na tabela s_1200_dmdev_periodo_ideestablot: " + ex.Message);
                                                }
                                            }
                                        }

                                        var verifica_RemunPerApur = item_ideEstabLot.RemunPerApur;
                                        if (verifica_RemunPerApur != null && verifica_RemunPerApur.Count > 0)
                                        {
                                            foreach (var item_remunPerApur in verifica_RemunPerApur)
                                            {
                                                var remunPerApur_matricula = item_remunPerApur.Matricula ?? "";
                                                var remunPerApur_indSimples = item_remunPerApur.IndSimples ?? "";
                                                long id_periodo_matricula = 0;

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var query = @"INSERT INTO s_1200_dmdev_periodo_ideestablot_matricula (
                                    id_projeto, id_cad_evtremun, id_s_1200_dmdev, id_dmdev_periodo_ideestablot, remunPerApur_matricula, 
                                    remunPerApur_indSimples, id_cadastro_envios)VALUES(@id_projeto, @id_cad_evtremun, @id_s_1200_dmdev, 
                                    @id_dmdev_periodo_ideestablot, @remunPerApur_matricula, @remunPerApur_indSimples, @id_cadastro_envios)";

                                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_cad_evtremun", id_cad_evtremun);
                                                        cmd.Parameters.AddWithValue("@id_s_1200_dmdev", id_cadastra_s_1200_dmdev);
                                                        cmd.Parameters.AddWithValue("@id_dmdev_periodo_ideestablot", id_s_1200_dmdev_periodo_ideestablot);
                                                        cmd.Parameters.AddWithValue("@remunPerApur_matricula", remunPerApur_matricula);
                                                        cmd.Parameters.AddWithValue("@remunPerApur_indSimples", remunPerApur_indSimples);
                                                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            id_periodo_matricula = cmd.LastInsertedId;
                                                            Console.WriteLine("EvtRemun inserido na tabela s_1200_dmdev_periodo_ideestablot_matricula com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtRemun na tabela s_1200_dmdev_periodo_ideestablot_matricula: " + ex.Message);
                                                        }
                                                    }
                                                }

                                                long id_cadastra_remuneracao = 0;
                                                var verifica_ItensRemun = item_remunPerApur.ItensRemun;
                                                if (verifica_ItensRemun != null && verifica_ItensRemun.Count > 0)
                                                {
                                                    foreach (var item_itensRemun in verifica_ItensRemun)
                                                    {
                                                        var itensRemun_codRubr = item_itensRemun.CodRubr ?? "";
                                                        var itensRemun_ideTabRubr = item_itensRemun.IdeTabRubr ?? "";
                                                        var itensRemun_vrRubr = Convert.ToDecimal(item_itensRemun.VrRubr);
                                                        var itensfatorRubr = Convert.ToDecimal(item_itensRemun.FatorRubr);
                                                        var itensindApurIR = Convert.ToInt32(item_itensRemun.IndApurIR);
                                                        var itensRemun_qtdRubr = Convert.ToDecimal(item_itensRemun.QtdRubr);
                                                        var infoAgNocivo_grauExp = 0;

                                                        var verifica_InfoAgNocivo = item_remunPerApur.InfoAgNocivo;
                                                        if (verifica_InfoAgNocivo != null && verifica_InfoAgNocivo.Count > 0)
                                                        {
                                                            foreach (var item_infoAgNocivo in verifica_InfoAgNocivo)
                                                            {
                                                                infoAgNocivo_grauExp = Convert.ToInt32(item_infoAgNocivo.GrauExp);
                                                            }
                                                        }

                                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                        {
                                                            connection.Open();
                                                            var query = @"INSERT INTO s_1200_remuneracao (id_projeto, 
                                        id_usuario, id_cad_evtremun, id_dmdev, id_dmdev_periodo_ideestablot, 
                                        id_periodo_ideestablot_matricula, evtRemun, dmDev_ideDmDev, dmDev_icodCateg, 
                                        ideEstabLot_tpInsc, ideEstabLot_nrInsc, ideEstabLot_codLotacao, remunPerApur_matricula, 
                                        itensRemun_codRubr, itensRemun_ideTabRubr, itensRemun_vrRubr, itensfatorRubr, 
                                        itensindApurIR, infoAgNocivo_grauExp, nome_arquivo, itensRemun_qtdRubr, 
                                        id_cadastro_envios) VALUES(@id_projeto, @id_usuario, @id_cad_evtremun, @id_dmdev, 
                                        @id_dmdev_periodo_ideestablot, @id_periodo_ideestablot_matricula, @evtRemun, 
                                        @dmDev_ideDmDev, @dmDev_icodCateg, @ideEstabLot_tpInsc, @ideEstabLot_nrInsc, 
                                        @ideEstabLot_codLotacao, @remunPerApur_matricula, @itensRemun_codRubr, @itensRemun_ideTabRubr, 
                                        @itensRemun_vrRubr, @itensfatorRubr, @itensindApurIR, @infoAgNocivo_grauExp, @nome_arquivo, 
                                        @itensRemun_qtdRubr, @id_cadastro_envios)";

                                                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                                            {
                                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                                cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                                cmd.Parameters.AddWithValue("@id_cad_evtremun", id_cad_evtremun);
                                                                cmd.Parameters.AddWithValue("@id_dmdev", id_cadastra_s_1200_dmdev);
                                                                cmd.Parameters.AddWithValue("@id_dmdev_periodo_ideestablot", id_s_1200_dmdev_periodo_ideestablot);
                                                                cmd.Parameters.AddWithValue("@id_periodo_ideestablot_matricula", id_periodo_matricula);
                                                                cmd.Parameters.AddWithValue("@evtRemun", evtRemun_id);
                                                                cmd.Parameters.AddWithValue("@dmDev_ideDmDev", dmDev_ideDmDev);
                                                                cmd.Parameters.AddWithValue("@dmDev_icodCateg", dmDev_icodCateg);
                                                                cmd.Parameters.AddWithValue("@ideEstabLot_tpInsc", ideEstabLot_tpInsc);
                                                                cmd.Parameters.AddWithValue("@ideEstabLot_nrInsc", ideEstabLot_nrInsc);
                                                                cmd.Parameters.AddWithValue("@ideEstabLot_codLotacao", ideEstabLot_codLotacao);
                                                                cmd.Parameters.AddWithValue("@remunPerApur_matricula", remunPerApur_matricula);
                                                                cmd.Parameters.AddWithValue("@itensRemun_codRubr", itensRemun_codRubr);
                                                                cmd.Parameters.AddWithValue("@itensRemun_ideTabRubr", itensRemun_ideTabRubr);
                                                                cmd.Parameters.AddWithValue("@itensRemun_vrRubr", itensRemun_vrRubr);
                                                                cmd.Parameters.AddWithValue("@itensfatorRubr", itensfatorRubr);
                                                                cmd.Parameters.AddWithValue("@itensindApurIR", itensindApurIR);
                                                                cmd.Parameters.AddWithValue("@infoAgNocivo_grauExp", infoAgNocivo_grauExp);
                                                                cmd.Parameters.AddWithValue("@nome_arquivo", nome_arquivo_importado);
                                                                cmd.Parameters.AddWithValue("@itensRemun_qtdRubr", itensRemun_qtdRubr);
                                                                cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                                try
                                                                {
                                                                    cmd.ExecuteNonQuery();
                                                                    id_cadastra_remuneracao = cmd.LastInsertedId;
                                                                    Console.WriteLine("EvtRemun inserido na tabela s_1200_remuneracao com sucesso!");
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    Console.WriteLine("Erro ao inserir EvtRemun na tabela s_1200_remuneracao: " + ex.Message);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                var verifica_InfoSaudeColet = item_remunPerApur.InfoSaudeColet;
                                                if (verifica_InfoSaudeColet != null && verifica_InfoSaudeColet.Count > 0)
                                                {
                                                    foreach (var item_infoSaudeColet in verifica_InfoSaudeColet)
                                                    {
                                                        var infoSaudeColet_cnpjOper = item_infoSaudeColet.DetOper?.CnpjOper ?? "";
                                                        var infoSaudeColet_regANS = item_infoSaudeColet.DetOper?.RegANS ?? "";
                                                        var infoSaudeColet_vrPgTit = Convert.ToDecimal(item_infoSaudeColet.DetOper?.VrPgTit);

                                                        var verifica_DetPlano = item_infoSaudeColet.DetOper?.DetPlano;
                                                        if (verifica_DetPlano != null && verifica_DetPlano.Count > 0)
                                                        {
                                                            foreach (var item_detPlano in verifica_DetPlano)
                                                            {
                                                                var detPlano_tpDep = Convert.ToInt32(item_detPlano.TpDep);
                                                                var detPlano_nmDep = item_detPlano.NmDep ?? "";
                                                                var detPlano_dtNascto = Convert.ToDateTime(item_detPlano.DtNascto);
                                                                var detPlano_vlrPgDep = Convert.ToDecimal(item_detPlano.VlrPgDep);

                                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                                {
                                                                    connection.Open();
                                                                    var query = @"INSERT INTO s_1200_adicionais (id_projeto, id_remuneracao, infoSaudeColet_cnpjOper, 
                                                            infoSaudeColet_regANS, infoSaudeColet_vrPgTit, infoSaudeColet_tpDep, 
                                                            infoSaudeColet_nmDep, infoSaudeColet_dtNascto, infoSaudeColet_vlrPgDep) 
                                                            VALUES(@id_projeto, @id_remuneracao, @infoSaudeColet_cnpjOper, 
                                                            @infoSaudeColet_regANS, @infoSaudeColet_vrPgTit, @infoSaudeColet_tpDep, 
                                                            @infoSaudeColet_nmDep, @infoSaudeColet_dtNascto, @infoSaudeColet_vlrPgDep)";

                                                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                                                    {
                                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                                        cmd.Parameters.AddWithValue("@id_remuneracao", id_cadastra_remuneracao);
                                                                        cmd.Parameters.AddWithValue("@infoSaudeColet_cnpjOper", infoSaudeColet_cnpjOper);
                                                                        cmd.Parameters.AddWithValue("@infoSaudeColet_regANS", infoSaudeColet_regANS);
                                                                        cmd.Parameters.AddWithValue("@infoSaudeColet_vrPgTit", infoSaudeColet_vrPgTit);
                                                                        cmd.Parameters.AddWithValue("@infoSaudeColet_tpDep", detPlano_tpDep);
                                                                        cmd.Parameters.AddWithValue("@infoSaudeColet_nmDep", detPlano_nmDep);
                                                                        cmd.Parameters.AddWithValue("@infoSaudeColet_dtNascto", detPlano_dtNascto);
                                                                        cmd.Parameters.AddWithValue("@infoSaudeColet_vlrPgDep", detPlano_vlrPgDep);

                                                                        try
                                                                        {
                                                                            cmd.ExecuteNonQuery();
                                                                            id_periodo_matricula = cmd.LastInsertedId;
                                                                            Console.WriteLine("EvtRemun inserido na tabela s_1200_adicionais com sucesso!");
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            Console.WriteLine("Erro ao inserir EvtRemun na tabela s_1200_adicionais: " + ex.Message);
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

                                var verifica_infoPerAnt = item.InfoPerAnt;
                                if (verifica_infoPerAnt != null && verifica_infoPerAnt.Count > 0)
                                {
                                    foreach (var item_infoPerAnt in verifica_infoPerAnt)
                                    {
                                        var verifica_infoPerAnt_IdeADC = item_infoPerAnt.IdeADC;
                                        if (verifica_infoPerAnt_IdeADC != null && verifica_infoPerAnt_IdeADC.Count > 0)
                                        {
                                            foreach (var item_ideADC in verifica_infoPerAnt_IdeADC)
                                            {
                                                var dtAcConv = Convert.ToDateTime(item_ideADC.DtAcConv);
                                                var tpAcConv = item_ideADC.TpAcConv ?? "";
                                                var compAcConv = item_ideADC.CompAcConv ?? "";
                                                var dtEfAcConv = item_ideADC.DtEfAcConv ?? "";
                                                var dsc = item_ideADC.Dsc ?? "";
                                                var remunSuc = item_ideADC.RemunSuc ?? "";
                                                long id_cadastra_s_1200_dmdev_anterior_ideadc = 0;

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var query = "INSERT INTO s_1200_dmdev_anterior_ideadc (id_projeto, id_cad_evtremun, id_s_1200_dmdev, " +
                                                        "dtAcConv, tpAcConv, compAcConv, dtEfAcConv, dsc, remunSuc) " +
                                                        "VALUES(@id_projeto, @id_cad_evtremun, @id_s_1200_dmdev, @dtAcConv, @tpAcConv, @compAcConv, " +
                                                        "@dtEfAcConv, @dsc, @remunSuc)";

                                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_cad_evtremun", id_cad_evtremun);
                                                        cmd.Parameters.AddWithValue("@id_s_1200_dmdev", id_cadastra_s_1200_dmdev);
                                                        cmd.Parameters.AddWithValue("@dtAcConv", dtAcConv);
                                                        cmd.Parameters.AddWithValue("@tpAcConv", tpAcConv);
                                                        cmd.Parameters.AddWithValue("@compAcConv", compAcConv);
                                                        cmd.Parameters.AddWithValue("@dtEfAcConv", dtEfAcConv);
                                                        cmd.Parameters.AddWithValue("@dsc", dsc);
                                                        cmd.Parameters.AddWithValue("@remunSuc", remunSuc);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            id_cadastra_s_1200_dmdev_anterior_ideadc = cmd.LastInsertedId;
                                                            Console.WriteLine("EvtRemun inserido na tabela s_1200_dmdev_anterior_ideadc com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtRemun na tabela s_1200_dmdev_anterior_ideadc: " + ex.Message);
                                                        }
                                                    }
                                                }

                                                var verifica_IdePeriodo = item_ideADC.IdePeriodo;
                                                if (verifica_IdePeriodo != null && verifica_IdePeriodo.Count > 0)
                                                {
                                                    foreach (var item_idePeriodo in verifica_IdePeriodo)
                                                    {
                                                        var ideADC_idePeriodo_perRef = item_idePeriodo.PerRef ?? "";
                                                        long id_s_1200_dmdev_anterior_ideadc_perref = 0;

                                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                        {
                                                            connection.Open();
                                                            var query = "INSERT INTO s_1200_dmdev_anterior_ideadc_perref (id_projeto, id_cad_evtremun, " +
                                                                "id_s_1200_dmdev, id_dmdev_anterior_ideadc, idePeriodo_perRef) " +
                                                                "VALUES(@id_projeto, @id_cad_evtremun, @id_s_1200_dmdev, @id_dmdev_anterior_ideadc, " +
                                                                "@idePeriodo_perRef)";

                                                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                                            {
                                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                                cmd.Parameters.AddWithValue("@id_cad_evtremun", id_cad_evtremun);
                                                                cmd.Parameters.AddWithValue("@id_s_1200_dmdev", id_cadastra_s_1200_dmdev);
                                                                cmd.Parameters.AddWithValue("@id_dmdev_anterior_ideadc", id_cadastra_s_1200_dmdev_anterior_ideadc);
                                                                cmd.Parameters.AddWithValue("@idePeriodo_perRef", ideADC_idePeriodo_perRef);

                                                                try
                                                                {
                                                                    cmd.ExecuteNonQuery();
                                                                    id_s_1200_dmdev_anterior_ideadc_perref = cmd.LastInsertedId;
                                                                    Console.WriteLine("EvtRemun inserido na tabela s_1200_dmdev_anterior_ideadc_perref com sucesso!");
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    Console.WriteLine("Erro ao inserir EvtRemun na tabela s_1200_dmdev_anterior_ideadc_perref: " + ex.Message);
                                                                }
                                                            }
                                                        }

                                                        var verifica_item_ideEstabLot = item_idePeriodo.IdeEstabLot;
                                                        if (verifica_item_ideEstabLot != null && verifica_item_ideEstabLot.Count > 0)
                                                        {
                                                            foreach (var item_ideEstabLot_anterior in verifica_item_ideEstabLot)
                                                            {
                                                                var anterior_ideEstabLot_tpInsc = Convert.ToInt32(item_ideEstabLot_anterior.TpInsc);
                                                                var anterior_ideEstabLot_nrInsc = item_ideEstabLot_anterior.NrInsc ?? "";
                                                                var anterior_ideEstabLot_codLotacao = item_ideEstabLot_anterior.CodLotacao ?? "";
                                                                long id_anterior_ideadc_ideperiodo_ideestablot = 0;

                                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                                {
                                                                    connection.Open();
                                                                    var query = "INSERT INTO s_1200_dmdev_anterior_ideadc_ideperiodo_ideestablot (id_projeto, id_cad_evtremun, " +
                                                                        "id_s_1200_dmdev, id_dmdev_anterior_ideadc, id_anterior_ideadc_perref, anterior_ideEstabLot_tpInsc, " +
                                                                        "anterior_ideEstabLot_nrInsc, anterior_ideEstabLot_codLotacao) " +
                                                                        "VALUES(@id_projeto, @id_cad_evtremun, @id_s_1200_dmdev, @id_dmdev_anterior_ideadc, " +
                                                                        "@id_anterior_ideadc_perref, @anterior_ideEstabLot_tpInsc, @anterior_ideEstabLot_nrInsc, " +
                                                                        "@anterior_ideEstabLot_codLotacao)";

                                                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                                                    {
                                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                                        cmd.Parameters.AddWithValue("@id_cad_evtremun", id_cad_evtremun);
                                                                        cmd.Parameters.AddWithValue("@id_s_1200_dmdev", id_cadastra_s_1200_dmdev);
                                                                        cmd.Parameters.AddWithValue("@id_dmdev_anterior_ideadc", id_cadastra_s_1200_dmdev_anterior_ideadc);
                                                                        cmd.Parameters.AddWithValue("@id_anterior_ideadc_perref", id_s_1200_dmdev_anterior_ideadc_perref);
                                                                        cmd.Parameters.AddWithValue("@anterior_ideEstabLot_tpInsc", anterior_ideEstabLot_tpInsc);
                                                                        cmd.Parameters.AddWithValue("@anterior_ideEstabLot_nrInsc", anterior_ideEstabLot_nrInsc);
                                                                        cmd.Parameters.AddWithValue("@anterior_ideEstabLot_codLotacao", anterior_ideEstabLot_codLotacao);

                                                                        try
                                                                        {
                                                                            cmd.ExecuteNonQuery();
                                                                            id_anterior_ideadc_ideperiodo_ideestablot = cmd.LastInsertedId;
                                                                            Console.WriteLine("EvtRemun inserido na tabela s_1200_dmdev_anterior_ideadc_ideperiodo_ideestablot com sucesso!");
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            Console.WriteLine("Erro ao inserir EvtRemun na tabela s_1200_dmdev_anterior_ideadc_ideperiodo_ideestablot: " + ex.Message);
                                                                        }
                                                                    }
                                                                }

                                                                var verifica_RemunPerAnt = item_ideEstabLot_anterior.RemunPerAnt;
                                                                if (verifica_RemunPerAnt != null && verifica_RemunPerAnt.Count > 0)
                                                                {
                                                                    foreach (var item_remunPerAnt_anterior in verifica_RemunPerAnt)
                                                                    {
                                                                        var remunPerAnt_matricula = item_remunPerAnt_anterior.Matricula ?? "";
                                                                        var remunPerAnt_indSimples = item_remunPerAnt_anterior.IndSimples ?? "";
                                                                        long id_cadastra_anterior_ideestablot_matricula = 0;

                                                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                                        {
                                                                            connection.Open();
                                                                            var query = "INSERT INTO s_1200_dmdev_anterior_ideadc_ideperiodo_ideestablot_matricula " +
                                                                                "(id_projeto, id_cad_evtremun, id_s_1200_dmdev, id_dmdev_anterior_ideadc, " +
                                                                                "id_anterior_ideadc_perref, id_anterior_ideadc_idePeriodo_ideEstabLot, remunPerAnt_matricula, " +
                                                                                "remunPerAnt_indSimples) " +
                                                                                "VALUES(@id_projeto, @id_cad_evtremun, @id_s_1200_dmdev, @id_dmdev_anterior_ideadc, " +
                                                                                "@id_anterior_ideadc_perref, @id_anterior_ideadc_idePeriodo_ideEstabLot, " +
                                                                                "@remunPerAnt_matricula, @remunPerAnt_indSimples)";

                                                                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                                                            {
                                                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                                                cmd.Parameters.AddWithValue("@id_cad_evtremun", id_cad_evtremun);
                                                                                cmd.Parameters.AddWithValue("@id_s_1200_dmdev", id_cadastra_s_1200_dmdev);
                                                                                cmd.Parameters.AddWithValue("@id_dmdev_anterior_ideadc", id_cadastra_s_1200_dmdev_anterior_ideadc);
                                                                                cmd.Parameters.AddWithValue("@id_anterior_ideadc_perref", id_s_1200_dmdev_anterior_ideadc_perref);
                                                                                cmd.Parameters.AddWithValue("@id_anterior_ideadc_idePeriodo_ideEstabLot", id_anterior_ideadc_ideperiodo_ideestablot);
                                                                                cmd.Parameters.AddWithValue("@remunPerAnt_matricula", remunPerAnt_matricula);
                                                                                cmd.Parameters.AddWithValue("@remunPerAnt_indSimples", remunPerAnt_indSimples);

                                                                                try
                                                                                {
                                                                                    cmd.ExecuteNonQuery();
                                                                                    id_cadastra_anterior_ideestablot_matricula = cmd.LastInsertedId;
                                                                                    Console.WriteLine("EvtRemun inserido na tabela s_1200_dmdev_anterior_ideadc_ideperiodo_ideestablot_matricula com sucesso!");
                                                                                }
                                                                                catch (Exception ex)
                                                                                {
                                                                                    Console.WriteLine("Erro ao inserir EvtRemun na tabela s_1200_dmdev_anterior_ideadc_ideperiodo_ideestablot_matricula: " + ex.Message);
                                                                                }
                                                                            }
                                                                        }

                                                                        var verifica_ItensRemun = item_remunPerAnt_anterior.ItensRemun;
                                                                        if (verifica_ItensRemun != null && verifica_ItensRemun.Count > 0)
                                                                        {
                                                                            foreach (var item_intens_remuneracao_anterior in verifica_ItensRemun)
                                                                            {
                                                                                var remuneracao_anterior_codRubr = item_intens_remuneracao_anterior.CodRubr ?? "";
                                                                                var remuneracao_anterior_ideTabRubr = item_intens_remuneracao_anterior.IdeTabRubr ?? "";
                                                                                var remuneracao_anterior_qtdRubr = item_intens_remuneracao_anterior.QtdRubr ?? "";
                                                                                var remuneracao_anterior_fatorRubr = item_intens_remuneracao_anterior.FatorRubr ?? "";
                                                                                var remuneracao_anterior_vrUnit = item_intens_remuneracao_anterior.VrUnit ?? "";
                                                                                var remuneracao_anterior_vrRubr = Convert.ToDecimal(item_intens_remuneracao_anterior.VrRubr);
                                                                                var remuneracao_anterior_indApurIR = Convert.ToInt32(item_intens_remuneracao_anterior.IndApurIR);
                                                                                var infoAgNocivo_grauExp = Convert.ToInt32(item_remunPerAnt_anterior.InfoAgNocivo?.GrauExp);

                                                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                                                {
                                                                                    connection.Open();
                                                                                    var query = "INSERT INTO s_1200_remuneracao_anterior (id_projeto, id_cad_evtremun, " +
                                                                                        "id_s_1200_dmdev, id_dmdev_anterior_ideadc, id_anterior_ideadc_perref, " +
                                                                                        "id_anterior_ideadc_idePeriodo_ideEstabLot, " +
                                                                                        "id_anterior_ideadc_ideperiodo_ideestablot_matricula, remuneracao_anterior_codRubr, " +
                                                                                        "remuneracao_anterior_ideTabRubr, remuneracao_anterior_qtdRubr, " +
                                                                                        "remuneracao_anterior_fatorRubr, remuneracao_anterior_vrUnit, " +
                                                                                        "remuneracao_anterior_vrRubr, remuneracao_anterior_indApurIR, infoAgNocivo_grauExp) " +
                                                                                        "VALUES(@id_projeto, @id_cad_evtremun, @id_s_1200_dmdev, @id_dmdev_anterior_ideadc, " +
                                                                                        "@id_anterior_ideadc_perref, @id_anterior_ideadc_idePeriodo_ideEstabLot, " +
                                                                                        "@id_anterior_ideadc_ideperiodo_ideestablot_matricula, @remuneracao_anterior_codRubr, " +
                                                                                        "@remuneracao_anterior_ideTabRubr, @remuneracao_anterior_qtdRubr, " +
                                                                                        "@remuneracao_anterior_fatorRubr, @remuneracao_anterior_vrUnit, " +
                                                                                        "@remuneracao_anterior_vrRubr, @remuneracao_anterior_indApurIR, " +
                                                                                        "@infoAgNocivo_grauExp)";

                                                                                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                                                                    {
                                                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                                                        cmd.Parameters.AddWithValue("@id_cad_evtremun", id_cad_evtremun);
                                                                                        cmd.Parameters.AddWithValue("@id_s_1200_dmdev", id_cadastra_s_1200_dmdev);
                                                                                        cmd.Parameters.AddWithValue("@id_dmdev_anterior_ideadc", id_cadastra_s_1200_dmdev_anterior_ideadc);
                                                                                        cmd.Parameters.AddWithValue("@id_anterior_ideadc_perref", id_s_1200_dmdev_anterior_ideadc_perref);
                                                                                        cmd.Parameters.AddWithValue("@id_anterior_ideadc_idePeriodo_ideEstabLot", id_anterior_ideadc_ideperiodo_ideestablot);
                                                                                        cmd.Parameters.AddWithValue("@id_anterior_ideadc_ideperiodo_ideestablot_matricula", id_cadastra_anterior_ideestablot_matricula);
                                                                                        cmd.Parameters.AddWithValue("@remuneracao_anterior_codRubr", remuneracao_anterior_codRubr);
                                                                                        cmd.Parameters.AddWithValue("@remuneracao_anterior_ideTabRubr", remuneracao_anterior_ideTabRubr);
                                                                                        cmd.Parameters.AddWithValue("@remuneracao_anterior_qtdRubr", remuneracao_anterior_qtdRubr);
                                                                                        cmd.Parameters.AddWithValue("@remuneracao_anterior_fatorRubr", remuneracao_anterior_fatorRubr);
                                                                                        cmd.Parameters.AddWithValue("@remuneracao_anterior_vrUnit", remuneracao_anterior_vrUnit);
                                                                                        cmd.Parameters.AddWithValue("@remuneracao_anterior_vrRubr", remuneracao_anterior_vrRubr);
                                                                                        cmd.Parameters.AddWithValue("@remuneracao_anterior_indApurIR", remuneracao_anterior_indApurIR);
                                                                                        cmd.Parameters.AddWithValue("@infoAgNocivo_grauExp", infoAgNocivo_grauExp);

                                                                                        try
                                                                                        {
                                                                                            cmd.ExecuteNonQuery();
                                                                                            id_cadastra_anterior_ideestablot_matricula = cmd.LastInsertedId;
                                                                                            Console.WriteLine("EvtRemun inserido na tabela s_1200_remuneracao_anterior com sucesso!");
                                                                                        }
                                                                                        catch (Exception ex)
                                                                                        {
                                                                                            Console.WriteLine("Erro ao inserir EvtRemun na tabela s_1200_remuneracao_anterior: " + ex.Message);
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

                                var verifica_infocomplcont = item.InfoComplCont;
                                if (verifica_infocomplcont != null && verifica_infocomplcont.Count > 0)
                                {
                                    foreach (var item_infoComplCont in verifica_infocomplcont)
                                    {
                                        var infoComplCont_codCBO = item_infoComplCont.CodCBO ?? "";
                                        var infoComplCont_natAtividade = item_infoComplCont.NatAtividade ?? "";
                                        var infoComplCont_qtdDiasTrab = item_infoComplCont.QtdDiasTrab ?? "";

                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            var query = "INSERT INTO s_1200_infocomplcont (id_projeto, id_cad_evtremun, id_dmdev, infoComplCont_codCBO, " +
                                                "infoComplCont_natAtividade, infoComplCont_qtdDiasTrab) " +
                                                "VALUES(@id_projeto, @id_cad_evtremun, @id_dmdev, @infoComplCont_codCBO, @infoComplCont_natAtividade, " +
                                                "@infoComplCont_qtdDiasTrab)";

                                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                            {
                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                cmd.Parameters.AddWithValue("@id_cad_evtremun", id_cad_evtremun);
                                                cmd.Parameters.AddWithValue("@id_dmdev", id_cadastra_s_1200_dmdev);
                                                cmd.Parameters.AddWithValue("@infoComplCont_codCBO", infoComplCont_codCBO);
                                                cmd.Parameters.AddWithValue("@infoComplCont_natAtividade", infoComplCont_natAtividade);
                                                cmd.Parameters.AddWithValue("@infoComplCont_qtdDiasTrab", infoComplCont_qtdDiasTrab);

                                                try
                                                {
                                                    cmd.ExecuteNonQuery();
                                                    Console.WriteLine("EvtRemun inserido na tabela s_1200_infocomplcont com sucesso!");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Erro ao inserir EvtRemun na tabela s_1200_infocomplcont: " + ex.Message);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    repository.InserirAquivosRejeitados(competencia_procurar_projeto, cnpjcpf, connectionString, arquivo, id_cadastro_arquivo, id_cadastro_envios);
                }
            }
            else
            {
                repository.InserirAquivosRejeitados(competencia_procurar_projeto, cnpjcpf, connectionString, arquivo, id_cadastro_arquivo, id_cadastro_envios);
            }
        }
    }
}