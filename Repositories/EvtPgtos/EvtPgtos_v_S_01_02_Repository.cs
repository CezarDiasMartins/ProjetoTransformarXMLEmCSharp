using MySql.Data.MySqlClient;
using System.Globalization;
using TransformarXmlEmCSharpESalvarNoBanco.Models.EvtPgtos;

namespace TransformarXmlEmCSharpESalvarNoBanco.Repositories.EvtPgtos
{
    public class EvtPgtos_v_S_01_02_Repository
    {
        public void InsertEvtPgtos(string connectionString, ESocialEvtPgtos eSocialEvtPgtos, string arquivo, int id_cadastro_envios, int id_cadastro_arquivo)
        {
            var evtPgtos_id = eSocialEvtPgtos?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtPgtos?.Id;
            var ideEvento_indRetif = eSocialEvtPgtos?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtPgtos?.IdeEvento?.IndRetif;
            var ideEvento_nrRecibo = eSocialEvtPgtos?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtPgtos?.IdeEvento?.NrRecibo;
            var ideEvento_indApuracao = eSocialEvtPgtos?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtPgtos?.IdeEvento?.IndApuracao;
            var ideEvento_perApur = eSocialEvtPgtos?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtPgtos?.IdeEvento?.PerApur;
            var ideEvento_indGuia = eSocialEvtPgtos?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtPgtos?.IdeEvento?.IndGuia;
            var ideEvento_tpAmb = eSocialEvtPgtos?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtPgtos?.IdeEvento?.TpAmb;
            var ideEvento_procEmi = eSocialEvtPgtos?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtPgtos?.IdeEvento?.ProcEmi;
            var ideEvento_verProc = eSocialEvtPgtos?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtPgtos?.IdeEvento?.VerProc;
            var ideEmpregador_tpInsc = Convert.ToInt32(eSocialEvtPgtos?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtPgtos?.IdeEmpregador?.TpInsc);
            var ideEmpregador_nrInsc = eSocialEvtPgtos?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtPgtos?.IdeEmpregador?.NrInsc;
            var ideBenef_cpfBenef = eSocialEvtPgtos?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtPgtos?.IdeBenef?.CpfBenef;
            var deps_vrDedDep = eSocialEvtPgtos?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtPgtos?.IdeBenef?.Deps?.VrDedDep;
            var recibo_nrRecibo = eSocialEvtPgtos?.RetornoProcessamentoDownload?.Recibo?.ESocial?.RetornoEvento?.ReciboRetornoEvento?.NrRecibo;
            var recepcao_processamento_dhProcessamento = eSocialEvtPgtos?.RetornoProcessamentoDownload?.Recibo?.ESocial?.RetornoEvento?.Processamento?.DhProcessamento;

            var versao_layout_evtPgtos = Path.GetFileName(eSocialEvtPgtos?.RetornoProcessamentoDownload?.Evento?.ESocial?.Namespace);
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
            ideBenef_cpfBenef = new string(ideBenef_cpfBenef.Where(char.IsDigit).ToArray());

            var repository = new Repository();
            var verificarProjetos = repository.VerificarProjetos(connectionString, competencia_procurar_projeto, cnpjcpf, id_cadastro_envios);

            if (verificarProjetos.Conta_projeto > 0)
            {
                var id_arquivo_ja_cadastrado = 0;
                var data_arquivo_ja_cadastrado = new DateTime();
                var data_arquivo_novo = Convert.ToDateTime(recepcao_processamento_dhProcessamento);
                var sql_verifica_duplicidade = "SELECT id, ideBenef_cpfBenef, recepcao_processamento_dhProcessamento " +
                    "FROM s_1210_evtpgtos WHERE id_projeto = @id_projeto AND ideBenef_cpfBenef = @ideBenef_cpfBenef";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql_verifica_duplicidade, connection))
                    {
                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                        cmd.Parameters.AddWithValue("@ideBenef_cpfBenef", ideBenef_cpfBenef);

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
                        long id_cad_evtpgtos = 0;
                        using (MySqlConnection connectionInsert_s_1210_evtpgtos = new MySqlConnection(connectionString))
                        {
                            connectionInsert_s_1210_evtpgtos.Open();
                            var sqlInsert_s_1210_evtpgtos = @"INSERT INTO s_1210_evtpgtos (id_projeto, id_usuario, evtPgtos_id, 
       ideEvento_indRetif, ideEvento_nrRecibo, ideEvento_indApuracao, ideEvento_perApur, ideEvento_indGuia, ideEvento_tpAmb, 
       ideEvento_procEmi, ideEvento_verProc, ideEmpregador_tpInsc, ideEmpregador_nrInsc, ideBenef_cpfBenef, 
       deps_vrDedDep, recibo_nrRecibo, recepcao_processamento_dhProcessamento, versao_layout_evtPgtos, 
       id_cadastro_envios, nome_arquivo_importado) VALUES(@id_projeto, @id_usuario, @evtPgtos_id, @ideEvento_indRetif, 
       @ideEvento_nrRecibo, @ideEvento_indApuracao, @ideEvento_perApur, @ideEvento_indGuia, @ideEvento_tpAmb, @ideEvento_procEmi, 
       @ideEvento_verProc, @ideEmpregador_tpInsc, @ideEmpregador_nrInsc, @ideBenef_cpfBenef, @deps_vrDedDep, 
       @recibo_nrRecibo, @recepcao_processamento_dhProcessamento, @versao_layout_evtPgtos, @id_cadastro_envios, @nome_arquivo_importado)";


                            using (MySqlCommand cmdInsert_s_1210_evtpgtos = new MySqlCommand(sqlInsert_s_1210_evtpgtos, connectionInsert_s_1210_evtpgtos))
                            {
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@evtPgtos_id", evtPgtos_id ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@ideEvento_indRetif", ideEvento_indRetif ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@ideEvento_nrRecibo", ideEvento_nrRecibo ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@ideEvento_indApuracao", ideEvento_indApuracao ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@ideEvento_perApur", ideEvento_perApur ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@ideEvento_indGuia", ideEvento_indGuia ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@ideEvento_tpAmb", ideEvento_tpAmb ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@ideEvento_procEmi", ideEvento_procEmi ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@ideEvento_verProc", ideEvento_verProc ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@ideEmpregador_tpInsc", ideEmpregador_tpInsc);
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@ideEmpregador_nrInsc", ideEmpregador_nrInsc ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@ideBenef_cpfBenef", ideBenef_cpfBenef ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@deps_vrDedDep", deps_vrDedDep ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@recibo_nrRecibo", recibo_nrRecibo ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@versao_layout_evtPgtos", versao_layout_evtPgtos ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@nome_arquivo_importado", nome_arquivo_importado ?? "");

                                // Convertendo recepcao_processamento_dhProcessamento para o formato "yyyy-MM-dd HH:mm:ss" ou "yyyy-MM-dd HH:mm:sss"
                                int indexOfDot = recepcao_processamento_dhProcessamento.LastIndexOf('.');
                                if (indexOfDot != -1 && indexOfDot + 1 < recepcao_processamento_dhProcessamento.Length)
                                {
                                    string millisecondsPart = recepcao_processamento_dhProcessamento.Substring(indexOfDot + 1);

                                    if (millisecondsPart.Length == 2)
                                    {
                                        DateTime recepcaoProcessamentoDhProcessamento = DateTime.ParseExact(recepcao_processamento_dhProcessamento, "yyyy-MM-ddTHH:mm:ss.ff", CultureInfo.InvariantCulture);
                                        cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@recepcao_processamento_dhProcessamento", recepcaoProcessamentoDhProcessamento.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) ?? "");
                                    }
                                    if (millisecondsPart.Length == 3)
                                    {
                                        DateTime recepcaoProcessamentoDhProcessamento = DateTime.ParseExact(recepcao_processamento_dhProcessamento, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
                                        cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@recepcao_processamento_dhProcessamento", recepcaoProcessamentoDhProcessamento.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) ?? "");
                                    }
                                }

                                try
                                {
                                    cmdInsert_s_1210_evtpgtos.ExecuteNonQuery();
                                    id_cad_evtpgtos = cmdInsert_s_1210_evtpgtos.LastInsertedId;
                                    Console.WriteLine($"EvtPgtos({id_cad_evtpgtos}) inserido na tabela s_1210_evtpgtos com sucesso!");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos: " + ex.Message);
                                }
                            }
                        }

                        var verifica_InfoPgto = eSocialEvtPgtos?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtPgtos?.IdeBenef?.InfoPgto;
                        if (verifica_InfoPgto != null && verifica_InfoPgto.Count > 0)
                        {
                            foreach (var item_infoPgto in verifica_InfoPgto)
                            {
                                var infoPgto_dtPgto = Convert.ToDateTime(item_infoPgto.DtPgto);
                                var infoPgto_tpPgto = Convert.ToInt32(item_infoPgto.TpPgto);
                                var infoPgto_perRef = item_infoPgto.PerRef ?? "";
                                var infoPgto_ideDmDev = item_infoPgto.IdeDmDev ?? "";
                                var infoPgto_vrLiq = item_infoPgto.VrLiq ?? "";
                                var infoPgto_paisResidExt = item_infoPgto.PaisResidExt ?? "";
                                long id_evtpgtos_informacoes = 0;

                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                {
                                    connection.Open();
                                    var sql = @"INSERT INTO s_1210_evtpgtos_informacoes (id_projeto, 
                id_usuario, id_cad_evtpgtos, infoPgto_dtPgto, infoPgto_tpPgto, infoPgto_perRef, infoPgto_ideDmDev, 
                infoPgto_vrLiq, infoPgto_paisResidExt, id_cadastro_envios)VALUES(@id_projeto, @id_usuario,
                @id_cad_evtpgtos, @infoPgto_dtPgto, @infoPgto_tpPgto, @infoPgto_perRef, @infoPgto_ideDmDev,
                @infoPgto_vrLiq, @infoPgto_paisResidExt, @id_cadastro_envios)";

                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                    {
                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                        cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                        cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                        cmd.Parameters.AddWithValue("@infoPgto_dtPgto", infoPgto_dtPgto);
                                        cmd.Parameters.AddWithValue("@infoPgto_tpPgto", infoPgto_tpPgto);
                                        cmd.Parameters.AddWithValue("@infoPgto_perRef", infoPgto_perRef);
                                        cmd.Parameters.AddWithValue("@infoPgto_ideDmDev", infoPgto_ideDmDev);
                                        cmd.Parameters.AddWithValue("@infoPgto_vrLiq", infoPgto_vrLiq);
                                        cmd.Parameters.AddWithValue("@infoPgto_paisResidExt", infoPgto_paisResidExt);
                                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                            id_evtpgtos_informacoes = cmd.LastInsertedId;
                                            Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_informacoes com sucesso!");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_informacoes: " + ex.Message);
                                        }
                                    }
                                }

                                var verifica_InfoPgtoExt = item_infoPgto.InfoPgtoExt;
                                if (verifica_InfoPgtoExt != null && verifica_InfoPgtoExt.Count > 0)
                                {
                                    foreach (var item_infoPgtoExt in verifica_InfoPgtoExt)
                                    {
                                        var idePgtoExt_indNIF = item_infoPgtoExt.IndNIF ?? "";
                                        var idePgtoExt_nifBenef = item_infoPgtoExt.NifBenef ?? "";
                                        var idePgtoExt_frmTribut = item_infoPgtoExt.FrmTribut ?? "";
                                        long id_idepgtoext = 0;

                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            var sql = @"INSERT INTO s_1210_evtpgtos_info_idepgtoext (id_projeto, id_usuario, 
                                            id_cad_evtpgtos, id_evtpgtos_informacoes, idePgtoExt_indNIF, idePgtoExt_nifBenef, idePgtoExt_frmTribut)
                                            VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, @id_evtpgtos_informacoes, @idePgtoExt_indNIF, 
                                            @idePgtoExt_nifBenef, @idePgtoExt_frmTribut)";

                                            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                            {
                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                cmd.Parameters.AddWithValue("@id_evtpgtos_informacoes", id_evtpgtos_informacoes);
                                                cmd.Parameters.AddWithValue("@idePgtoExt_indNIF", idePgtoExt_indNIF);
                                                cmd.Parameters.AddWithValue("@idePgtoExt_nifBenef", idePgtoExt_nifBenef);
                                                cmd.Parameters.AddWithValue("@idePgtoExt_frmTribut", idePgtoExt_frmTribut);

                                                try
                                                {
                                                    cmd.ExecuteNonQuery();
                                                    id_idepgtoext = cmd.LastInsertedId;
                                                    Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_info_idepgtoext com sucesso!");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_info_idepgtoext: " + ex.Message);
                                                }
                                            }
                                        }

                                        var verifica_EndExt = item_infoPgtoExt.EndExt;
                                        if (verifica_EndExt != null && verifica_EndExt.Count > 0)
                                        {
                                            foreach (var item_endExtt in verifica_EndExt)
                                            {
                                                var endExt_endDscLograd = item_endExtt.EndDscLograd ?? "";
                                                var endExt_endNrLograd = item_endExtt.EndNrLograd ?? "";
                                                var endExt_endComplem = item_endExtt.EndComplem ?? "";
                                                var endExt_endBairro = item_endExtt.EndBairro ?? "";
                                                var endExt_endCidade = item_endExtt.EndCidade ?? "";
                                                var endExt_endEstado = item_endExtt.EndEstado ?? "";
                                                var endExt_endCodPostal = item_endExtt.EndCodPostal ?? "";
                                                var endExt_telef = item_endExtt.Telef ?? "";

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var sql = @"INSERT INTO s_1210_evtpgtos_info_idepgtoext_endext (id_projeto, 
                                        id_usuario, id_cad_evtpgtos, id_evtpgtos_informacoes, id_evtpgtos_informacoes_idepgtoext, endExt_endDscLograd, 
                                        endExt_endNrLograd, endExt_endComplem, endExt_endBairro, endExt_endCidade, endExt_endEstado, endExt_endCodPostal, 
                                        endExt_telef)VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, @id_evtpgtos_informacoes, 
                                        @id_evtpgtos_informacoes_idepgtoext, @endExt_endDscLograd, @endExt_endNrLograd, @endExt_endComplem, 
                                        @endExt_endBairro, @endExt_endCidade, @endExt_endEstado, @endExt_endCodPostal, @endExt_telef)";

                                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                        cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                        cmd.Parameters.AddWithValue("@id_evtpgtos_informacoes", id_evtpgtos_informacoes);
                                                        cmd.Parameters.AddWithValue("@id_evtpgtos_informacoes_idepgtoext", id_idepgtoext);
                                                        cmd.Parameters.AddWithValue("@endExt_endDscLograd", endExt_endDscLograd);
                                                        cmd.Parameters.AddWithValue("@endExt_endNrLograd", endExt_endNrLograd);
                                                        cmd.Parameters.AddWithValue("@endExt_endComplem", endExt_endComplem);
                                                        cmd.Parameters.AddWithValue("@endExt_endBairro", endExt_endBairro);
                                                        cmd.Parameters.AddWithValue("@endExt_endCidade", endExt_endCidade);
                                                        cmd.Parameters.AddWithValue("@endExt_endEstado", endExt_endEstado);
                                                        cmd.Parameters.AddWithValue("@endExt_endCodPostal", endExt_endCodPostal);
                                                        cmd.Parameters.AddWithValue("@endExt_telef", endExt_telef);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            id_idepgtoext = cmd.LastInsertedId;
                                                            Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_info_idepgtoext_endext com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_info_idepgtoext_endext: " + ex.Message);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        var verifica_InfoIRComplem = eSocialEvtPgtos?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtPgtos?.IdeBenef?.InfoIRComplem;
                        if (verifica_InfoIRComplem != null && verifica_InfoIRComplem.Count > 0)
                        {
                            foreach (var item_infoIRComplem in verifica_InfoIRComplem)
                            {
                                var infoIRComplem_dtLaudo = item_infoIRComplem.DtLaudo ?? "";
                                long id_infoircomplem = 0;

                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                {
                                    connection.Open();
                                    var sql = @"INSERT INTO s_1210_evtpgtos_infoircomplem (id_projeto, 
                id_usuario, id_cad_evtpgtos, infoIRComplem_dtLaudo, id_cadastro_envios)VALUES(@id_projeto, @id_usuario, 
                @id_cad_evtpgtos, @infoIRComplem_dtLaudo, @id_cadastro_envios)";

                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                    {
                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                        cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                        cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                        cmd.Parameters.AddWithValue("@infoIRComplem_dtLaudo", infoIRComplem_dtLaudo);
                                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                            id_infoircomplem = cmd.LastInsertedId;
                                            Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_infoircomplem com sucesso!");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_infoircomplem: " + ex.Message);
                                        }
                                    }
                                }

                                var verifica_InfoDep = item_infoIRComplem.InfoDep;
                                if (verifica_InfoDep != null && verifica_InfoDep.Count > 0)
                                {
                                    foreach (var item_infoDep in verifica_InfoDep)
                                    {
                                        var infoDep_cpfDep = item_infoDep.CpfDep ?? "";
                                        var infoDep_dtNascto = item_infoDep.DtNascto ?? "";
                                        var infoDep_nome = item_infoDep.Nome ?? "";
                                        var infoDep_depIRRF = item_infoDep.DepIRRF ?? "";
                                        var infoDep_tpDep = item_infoDep.TpDep ?? "";
                                        var infoDep_descrDep = item_infoDep.DescrDep ?? "";

                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            var sql = @"INSERT INTO s_1210_evtpgtos_infoircomplem_infodep 
                    (id_projeto, id_usuario, id_cad_evtpgtos, id_infoircomplem, infoDep_cpfDep, infoDep_dtNascto, infoDep_nome, infoDep_depIRRF, infoDep_tpDep, infoDep_descrDep, id_cadastro_envios)
                    VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, @id_infoircomplem, @infoDep_cpfDep, @infoDep_dtNascto, 
                    @infoDep_nome, @infoDep_depIRRF, @infoDep_tpDep, @infoDep_descrDep, @id_cadastro_envios)";

                                            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                            {
                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                cmd.Parameters.AddWithValue("@id_infoircomplem", id_infoircomplem);
                                                cmd.Parameters.AddWithValue("@infoDep_cpfDep", infoDep_cpfDep);
                                                cmd.Parameters.AddWithValue("@infoDep_dtNascto", infoDep_dtNascto);
                                                cmd.Parameters.AddWithValue("@infoDep_nome", infoDep_nome);
                                                cmd.Parameters.AddWithValue("@infoDep_depIRRF", infoDep_depIRRF);
                                                cmd.Parameters.AddWithValue("@infoDep_tpDep", infoDep_tpDep);
                                                cmd.Parameters.AddWithValue("@infoDep_descrDep", infoDep_descrDep);
                                                cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                try
                                                {
                                                    cmd.ExecuteNonQuery();
                                                    id_infoircomplem = cmd.LastInsertedId;
                                                    Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_infoircomplem_infodep com sucesso!");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_infoircomplem_infodep: " + ex.Message);
                                                }
                                            }
                                        }
                                    }
                                }

                                var verifica_InfoIRCR = item_infoIRComplem.InfoIRCR;
                                if (verifica_InfoIRCR != null && verifica_InfoIRCR.Count > 0)
                                {
                                    foreach (var item_infoIRCR in verifica_InfoIRCR)
                                    {
                                        var infoIRCR_tpCR = item_infoIRCR.TpCR ?? "";
                                        long id_infoIRCR = 0;

                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            var sql = @"INSERT INTO s_1210_evtpgtos_infoircomplem_infoircr 
                                        (id_projeto, id_usuario, id_cad_evtpgtos, id_infoircomplem, infoIRCR_tpCR, 
                                        id_cadastro_envios)VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, 
                                        @id_infoircomplem, @infoIRCR_tpCR, @id_cadastro_envios)";

                                            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                            {
                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                cmd.Parameters.AddWithValue("@id_infoircomplem", id_infoircomplem);
                                                cmd.Parameters.AddWithValue("@infoIRCR_tpCR", infoIRCR_tpCR);
                                                cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                try
                                                {
                                                    cmd.ExecuteNonQuery();
                                                    id_infoIRCR = cmd.LastInsertedId;
                                                    Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_infoircomplem_infoircr com sucesso!");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_infoircomplem_infoircr: " + ex.Message);
                                                }
                                            }
                                        }

                                        var verifica_DedDepen = item_infoIRCR.DedDepen;
                                        if (verifica_DedDepen != null && verifica_DedDepen.Count > 0)
                                        {
                                            foreach (var item_dedDepen in verifica_DedDepen)
                                            {
                                                var dedDepen_tpRend = item_dedDepen.TpRend ?? "";
                                                var dedDepen_cpfDep = item_dedDepen.CpfDep ?? "";
                                                var dedDepen_vlrDedDep = item_dedDepen.VlrDedDep ?? "";

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var sql = @"INSERT INTO s_1210_evtpgtos_infoircomplem_infoircr_deddepen
                        (id_projeto, id_usuario, id_cad_evtpgtos, id_infoircomplem, id_infoircr, dedDepen_tpRend, 
                        dedDepen_cpfDep, dedDepen_vlrDedDep, id_cadastro_envios)
                        VALUES (@id_projeto, @id_usuario, @id_cad_evtpgtos, @id_infoircomplem, @id_infoircr, 
                        @dedDepen_tpRend, @dedDepen_cpfDep, @dedDepen_vlrDedDep, @id_cadastro_envios)";

                                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                        cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                        cmd.Parameters.AddWithValue("@id_infoircomplem", id_infoircomplem);
                                                        cmd.Parameters.AddWithValue("@id_infoircr", id_infoIRCR);
                                                        cmd.Parameters.AddWithValue("@dedDepen_tpRend", dedDepen_tpRend);
                                                        cmd.Parameters.AddWithValue("@dedDepen_cpfDep", dedDepen_cpfDep);
                                                        cmd.Parameters.AddWithValue("@dedDepen_vlrDedDep", dedDepen_vlrDedDep);
                                                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_infoircomplem_infoircr com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_infoircomplem_infoircr: " + ex.Message);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        var verifica_PenAlim = item_infoIRCR.PenAlim;
                                        if (verifica_PenAlim != null && verifica_PenAlim.Count > 0)
                                        {
                                            foreach (var item_penAlim in verifica_PenAlim)
                                            {
                                                var penAlim_tpRend = item_penAlim.TpRend ?? "";
                                                var penAlim_cpfDep = item_penAlim.CpfDep ?? "";
                                                var penAlim_vlrDedPenAlim = item_penAlim.VlrDedPenAlim ?? "";

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var sql = @"INSERT INTO s_1210_evtpgtos_infoircomplem_infoircr_penalim
                                            (id_projeto, id_usuario, id_cad_evtpgtos, id_infoircomplem, id_infoircr, 
                                            penAlim_tpRend, penAlim_cpfDep, penAlim_vlrDedPenAlim, id_cadastro_envios)
                                            VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, @id_infoircomplem, 
                                            @id_infoircr, @penAlim_tpRend, @penAlim_cpfDep, @penAlim_vlrDedPenAlim, 
                                            @id_cadastro_envios)";

                                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                        cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                        cmd.Parameters.AddWithValue("@id_infoircomplem", id_infoircomplem);
                                                        cmd.Parameters.AddWithValue("@id_infoircr", id_infoIRCR);
                                                        cmd.Parameters.AddWithValue("@penAlim_tpRend", penAlim_tpRend);
                                                        cmd.Parameters.AddWithValue("@penAlim_cpfDep", penAlim_cpfDep);
                                                        cmd.Parameters.AddWithValue("@penAlim_vlrDedPenAlim", penAlim_vlrDedPenAlim);
                                                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_infoircomplem_infoircr_penalim com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_infoircomplem_infoircr_penalim: " + ex.Message);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        var verifica_PrevidCompl = item_infoIRCR.PrevidCompl;
                                        if (verifica_PrevidCompl != null && verifica_PrevidCompl.Count > 0)
                                        {
                                            foreach (var item_previdCompl in verifica_PrevidCompl)
                                            {
                                                var previdCompl_tpPrev = item_previdCompl.TpPrev ?? "";
                                                var previdCompl_cnpjEntidPC = item_previdCompl.CnpjEntidPC ?? "";
                                                var previdCompl_vlrDedPC = item_previdCompl.VlrDedPC ?? "";
                                                var previdCompl_vlrPatrocFunp = item_previdCompl.VlrPatrocFunp ?? "";

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var sql = @"INSERT INTO s_1210_evtpgtos_infoircomplem_infoircr_previdcompl 
                                                (id_projeto, id_usuario, id_cad_evtpgtos, id_infoircomplem, id_infoircr, 
                                                previdCompl_tpPrev, previdCompl_cnpjEntidPC, previdCompl_vlrDedPC, 
                                                previdCompl_vlrPatrocFunp, id_cadastro_envios)
                                                VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, @id_infoircomplem, 
                                                @id_infoircr, @previdCompl_tpPrev, @previdCompl_cnpjEntidPC, 
                                                @previdCompl_vlrDedPC, @previdCompl_vlrPatrocFunp, @id_cadastro_envios)";

                                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                        cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                        cmd.Parameters.AddWithValue("@id_infoircomplem", id_infoircomplem);
                                                        cmd.Parameters.AddWithValue("@id_infoircr", id_infoIRCR);
                                                        cmd.Parameters.AddWithValue("@previdCompl_tpPrev", previdCompl_tpPrev);
                                                        cmd.Parameters.AddWithValue("@previdCompl_cnpjEntidPC", previdCompl_cnpjEntidPC);
                                                        cmd.Parameters.AddWithValue("@previdCompl_vlrDedPC", previdCompl_vlrDedPC);
                                                        cmd.Parameters.AddWithValue("@previdCompl_vlrPatrocFunp", previdCompl_vlrPatrocFunp);
                                                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_infoircomplem_infoircr_previdcompl com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_infoircomplem_infoircr_previdcompl: " + ex.Message);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        var verifica_InfoProcRet = item_infoIRCR.InfoProcRet;
                                        if (verifica_InfoProcRet != null && verifica_InfoProcRet.Count > 0)
                                        {
                                            foreach (var item_infoProcRet in verifica_InfoProcRet)
                                            {
                                                var infoProcRet_tpProcRet = item_infoProcRet.TpProcRet ?? "";
                                                var infoProcRet_nrProcRet = item_infoProcRet.NrProcRet ?? "";
                                                var infoProcRet_codSusp = item_infoProcRet.CodSusp ?? "";
                                                long id_infoProcRet = 0;

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var sql = @"INSERT INTO s_1210_evtpgtos_infoircomplem_infoircr_infoprocret
                        (id_projeto, id_usuario, id_cad_evtpgtos, id_infoircomplem, id_infoircr, infoProcRet_tpProcRet, 
                        infoProcRet_nrProcRet, infoProcRet_codSusp, id_cadastro_envios)
                        VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, @id_infoircomplem, @id_infoircr, 
                        @infoProcRet_tpProcRet, @infoProcRet_nrProcRet, @infoProcRet_codSusp, @id_cadastro_envios)";

                                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                        cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                        cmd.Parameters.AddWithValue("@id_infoircomplem", id_infoircomplem);
                                                        cmd.Parameters.AddWithValue("@id_infoircr", id_infoIRCR);
                                                        cmd.Parameters.AddWithValue("@infoProcRet_tpProcRet", infoProcRet_tpProcRet);
                                                        cmd.Parameters.AddWithValue("@infoProcRet_nrProcRet", infoProcRet_nrProcRet);
                                                        cmd.Parameters.AddWithValue("@infoProcRet_codSusp", infoProcRet_codSusp);
                                                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            id_infoProcRet = cmd.LastInsertedId;
                                                            Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_infoircomplem_infoircr_infoprocret com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_infoircomplem_infoircr_infoprocret: " + ex.Message);
                                                        }
                                                    }
                                                }

                                                var verifica_InfoValores = item_infoProcRet.InfoValores;
                                                if (verifica_InfoValores != null && verifica_InfoValores.Count > 0)
                                                {
                                                    foreach (var item_infoValores in verifica_InfoValores)
                                                    {
                                                        var infoValores_indApuracao = item_infoValores.IndApuracao ?? "";
                                                        var infoValores_vlrNRetido = item_infoValores.VlrNRetido ?? "";
                                                        var infoValores_vlrDepJud = item_infoValores.VlrDepJud ?? "";
                                                        var infoValores_vlrCmpAnoCal = item_infoValores.VlrCmpAnoCal ?? "";
                                                        var infoValores_vlrCmpAnoAnt = item_infoValores.VlrCmpAnoAnt ?? "";
                                                        var infoValores_vlrRendSusp = item_infoValores.VlrRendSusp ?? "";
                                                        long id_infoValores = 0;

                                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                        {
                                                            connection.Open();
                                                            var sql = @"INSERT INTO s_1210_evtpgtos_infoircomplem_infoircr_infoprocret_infovalores
                            (id_projeto, id_usuario, id_cad_evtpgtos, id_infoircomplem, id_infoircr, id_infoProcRet, 
                            infoValores_indApuracao, infoValores_vlrNRetido, infoValores_vlrDepJud, infoValores_vlrCmpAnoCal, 
                            infoValores_vlrCmpAnoAnt, infoValores_vlrRendSusp, id_cadastro_envios)
                            VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, @id_infoircomplem, @id_infoircr, 
                            @id_infoProcRet, @infoValores_indApuracao, @infoValores_vlrNRetido, @infoValores_vlrDepJud, 
                            @infoValores_vlrCmpAnoCal, @infoValores_vlrCmpAnoAnt, @infoValores_vlrRendSusp, 
                            @id_cadastro_envios)";

                                                            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                            {
                                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                                cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                                cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                                cmd.Parameters.AddWithValue("@id_infoircomplem", id_infoircomplem);
                                                                cmd.Parameters.AddWithValue("@id_infoircr", id_infoIRCR);
                                                                cmd.Parameters.AddWithValue("@id_infoProcRet", id_infoProcRet);
                                                                cmd.Parameters.AddWithValue("@infoValores_indApuracao", infoValores_indApuracao);
                                                                cmd.Parameters.AddWithValue("@infoValores_vlrNRetido", infoValores_vlrNRetido);
                                                                cmd.Parameters.AddWithValue("@infoValores_vlrDepJud", infoValores_vlrDepJud);
                                                                cmd.Parameters.AddWithValue("@infoValores_vlrCmpAnoCal", infoValores_vlrCmpAnoCal);
                                                                cmd.Parameters.AddWithValue("@infoValores_vlrCmpAnoAnt", infoValores_vlrCmpAnoAnt);
                                                                cmd.Parameters.AddWithValue("@infoValores_vlrRendSusp", infoValores_vlrRendSusp);
                                                                cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                                try
                                                                {
                                                                    cmd.ExecuteNonQuery();
                                                                    id_infoValores = cmd.LastInsertedId;
                                                                    Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_infoircomplem_infoircr_infoprocret_infovalores com sucesso!");
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_infoircomplem_infoircr_infoprocret_infovalores: " + ex.Message);
                                                                }
                                                            }
                                                        }

                                                        var verifica_DedSusp = item_infoValores.DedSusp;
                                                        if (verifica_DedSusp != null && verifica_DedSusp.Count > 0)
                                                        {
                                                            foreach (var item_dedSusp in verifica_DedSusp)
                                                            {
                                                                var dedSusp_indTpDeducao = item_dedSusp.IndTpDeducao;
                                                                var dedSusp_vlrDedSusp = item_dedSusp.VlrDedSusp;
                                                                var dedSusp_cnpjEntidPC = item_dedSusp.CnpjEntidPC;
                                                                var dedSusp_vlrPatrocFunp = item_dedSusp.VlrPatrocFunp;
                                                                long id_dedSusp = 0;

                                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                                {
                                                                    connection.Open();
                                                                    var sql = @"INSERT INTO s_1210_evtpgtos_infoircomplem_info_info_info_dedsusp
                                (id_projeto, id_usuario, id_cad_evtpgtos, id_infoircomplem, id_infoircr, id_infoProcRet, 
                                id_infoValores, dedSusp_indTpDeducao, dedSusp_vlrDedSusp, dedSusp_cnpjEntidPC, 
                                dedSusp_vlrPatrocFunp, id_cadastro_envios)
                                VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, @id_infoircomplem, @id_infoircr, 
                                @id_infoProcRet, @id_infoValores, @dedSusp_indTpDeducao, @dedSusp_vlrDedSusp, 
                                @dedSusp_cnpjEntidPC, @dedSusp_vlrPatrocFunp, @id_cadastro_envios)";

                                                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                                    {
                                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                                        cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                                        cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                                        cmd.Parameters.AddWithValue("@id_infoircomplem", id_infoircomplem);
                                                                        cmd.Parameters.AddWithValue("@id_infoircr", id_infoIRCR);
                                                                        cmd.Parameters.AddWithValue("@id_infoProcRet", id_infoProcRet);
                                                                        cmd.Parameters.AddWithValue("@id_infoValores", id_infoValores);
                                                                        cmd.Parameters.AddWithValue("@dedSusp_indTpDeducao", dedSusp_indTpDeducao);
                                                                        cmd.Parameters.AddWithValue("@dedSusp_vlrDedSusp", dedSusp_vlrDedSusp);
                                                                        cmd.Parameters.AddWithValue("@dedSusp_cnpjEntidPC", dedSusp_cnpjEntidPC);
                                                                        cmd.Parameters.AddWithValue("@dedSusp_vlrPatrocFunp", dedSusp_vlrPatrocFunp);
                                                                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                                        try
                                                                        {
                                                                            cmd.ExecuteNonQuery();
                                                                            id_dedSusp = cmd.LastInsertedId;
                                                                            Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_infoircomplem_info_info_info_dedsusp com sucesso!");
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_infoircomplem_info_info_info_dedsusp: " + ex.Message);
                                                                        }
                                                                    }
                                                                }

                                                                var verifica_BenefPen = item_dedSusp.BenefPen;
                                                                if (verifica_BenefPen != null && verifica_BenefPen.Count > 0)
                                                                {
                                                                    foreach (var item_benefPen in verifica_BenefPen)
                                                                    {
                                                                        var benefPen_cpfDep = item_benefPen.CpfDep ?? "";
                                                                        var benefPen_vlrDepenSusp = item_benefPen.VlrDepenSusp ?? "";

                                                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                                        {
                                                                            connection.Open();
                                                                            var sql = @"INSERT INTO s_1210_evtpgtos_infoircomplem_info_info_info_dedsusp_benefpen
                                    (id_projeto, id_usuario, id_cad_evtpgtos, id_infoircomplem, id_infoircr, id_infoProcRet, 
                                    id_infoValores, id_dedSusp, benefPen_cpfDep, benefPen_vlrDepenSusp, id_cadastro_envios)
                                    VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, @id_infoircomplem, @id_infoircr, 
                                    @id_infoProcRet, @id_infoValores, @id_dedSusp, @benefPen_cpfDep, @benefPen_vlrDepenSusp,
                                    @id_cadastro_envios)";

                                                                            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                                            {
                                                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                                                cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                                                cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                                                cmd.Parameters.AddWithValue("@id_infoircomplem", id_infoircomplem);
                                                                                cmd.Parameters.AddWithValue("@id_infoircr", id_infoIRCR);
                                                                                cmd.Parameters.AddWithValue("@id_infoProcRet", id_infoProcRet);
                                                                                cmd.Parameters.AddWithValue("@id_infoValores", id_infoValores);
                                                                                cmd.Parameters.AddWithValue("@id_dedSusp", id_dedSusp);
                                                                                cmd.Parameters.AddWithValue("@benefPen_cpfDep", benefPen_cpfDep);
                                                                                cmd.Parameters.AddWithValue("@benefPen_vlrDepenSusp", benefPen_vlrDepenSusp);
                                                                                cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                                                try
                                                                                {
                                                                                    cmd.ExecuteNonQuery();
                                                                                    Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_infoircomplem_info_info_info_dedsusp_benefpen com sucesso!");
                                                                                }
                                                                                catch (Exception ex)
                                                                                {
                                                                                    Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_infoircomplem_info_info_info_dedsusp_benefpen: " + ex.Message);
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

                                var verifica_PlanSaude = item_infoIRComplem.PlanSaude;
                                if (verifica_PlanSaude != null && verifica_PlanSaude.Count > 0)
                                {
                                    foreach (var item_planSaude in verifica_PlanSaude)
                                    {
                                        var planSaude_cnpjOper = item_planSaude.CnpjOper ?? "";
                                        var planSaude_regANS = item_planSaude.RegANS ?? "";
                                        var planSaude_vlrSaudeTit = item_planSaude.VlrSaudeTit ?? "";
                                        long id_planSaude = 0;

                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            var sql = @"INSERT INTO s_1210_evtpgtos_infoircomplem_plansaude (id_projeto, id_usuario, id_cad_evtpgtos, 
                                    id_infoircomplem, planSaude_cnpjOper, planSaude_regANS, planSaude_vlrSaudeTit, id_cadastro_envios)
                                    VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, @id_infoircomplem, @planSaude_cnpjOper, @planSaude_regANS, 
                                    @planSaude_vlrSaudeTit, @id_cadastro_envios)";

                                            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                            {
                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                cmd.Parameters.AddWithValue("@id_infoircomplem", id_infoircomplem);
                                                cmd.Parameters.AddWithValue("@planSaude_cnpjOper", planSaude_cnpjOper);
                                                cmd.Parameters.AddWithValue("@planSaude_regANS", planSaude_regANS);
                                                cmd.Parameters.AddWithValue("@planSaude_vlrSaudeTit", planSaude_vlrSaudeTit);
                                                cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                try
                                                {
                                                    cmd.ExecuteNonQuery();
                                                    id_planSaude = cmd.LastInsertedId;
                                                    Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_infoircomplem_plansaude com sucesso!");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_infoircomplem_plansaude: " + ex.Message);
                                                }
                                            }
                                        }

                                        var verifica_InfoDepSau = item_planSaude.InfoDepSau;
                                        if (verifica_InfoDepSau != null && verifica_InfoDepSau.Count > 0)
                                        {
                                            foreach (var item_infoDepSau in verifica_InfoDepSau)
                                            {
                                                var infoDepSau_cpfDep = item_infoDepSau.CpfDep ?? "";
                                                var infoDepSau_vlrSaudeDep = item_infoDepSau.VlrSaudeDep ?? "";

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var sql = @"INSERT INTO s_1210_evtpgtos_infoircomplem_plansaude_infodepsau(id_projeto, id_usuario, 
                                        id_cad_evtpgtos, id_infoircomplem, id_planSaude, infoDepSau_cpfDep, infoDepSau_vlrSaudeDep, id_cadastro_envios)
                                        VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, @id_infoircomplem, @id_planSaude, @infoDepSau_cpfDep, 
                                        @infoDepSau_vlrSaudeDep, @id_cadastro_envios)";

                                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                        cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                        cmd.Parameters.AddWithValue("@id_infoircomplem", id_infoircomplem);
                                                        cmd.Parameters.AddWithValue("@id_planSaude", id_planSaude);
                                                        cmd.Parameters.AddWithValue("@infoDepSau_cpfDep", infoDepSau_cpfDep);
                                                        cmd.Parameters.AddWithValue("@infoDepSau_vlrSaudeDep", infoDepSau_vlrSaudeDep);
                                                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_infoircomplem_plansaude_infodepsau com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_infoircomplem_plansaude_infodepsau: " + ex.Message);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                var verifica_InfoReembMed = item_infoIRComplem.InfoReembMed;
                                if (verifica_InfoReembMed != null && verifica_InfoReembMed.Count > 0)
                                {
                                    foreach (var item_infoReembMed in verifica_InfoReembMed)
                                    {
                                        var infoReembMed_indOrgReemb = item_infoReembMed.IndOrgReemb ?? "";
                                        var infoReembMed_cnpjOper = item_infoReembMed.CnpjOper ?? "";
                                        var infoReembMed_regANS = item_infoReembMed.RegANS ?? "";
                                        long id_infoReembMed = 0;

                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            var sql = @"INSERT INTO s_1210_evtpgtos_infoircomplem_inforeembmed(id_projeto, id_usuario, id_cad_evtpgtos, 
                                    id_infoircomplem, infoReembMed_indOrgReemb, infoReembMed_cnpjOper, infoReembMed_regANS, id_cadastro_envios)
                                    VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, @id_infoircomplem, @infoReembMed_indOrgReemb, 
                                    @infoReembMed_cnpjOper, @infoReembMed_regANS, @id_cadastro_envios)";

                                            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                            {
                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                cmd.Parameters.AddWithValue("@id_infoircomplem", id_infoircomplem);
                                                cmd.Parameters.AddWithValue("@infoReembMed_indOrgReemb", infoReembMed_indOrgReemb);
                                                cmd.Parameters.AddWithValue("@infoReembMed_cnpjOper", infoReembMed_cnpjOper);
                                                cmd.Parameters.AddWithValue("@infoReembMed_regANS", infoReembMed_regANS);
                                                cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                try
                                                {
                                                    cmd.ExecuteNonQuery();
                                                    id_infoReembMed = cmd.LastInsertedId;
                                                    Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_infoircomplem_inforeembmed com sucesso!");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_infoircomplem_inforeembmed: " + ex.Message);
                                                }
                                            }
                                        }

                                        var verifica_DetReembTit = item_infoReembMed.DetReembTit;
                                        if (verifica_DetReembTit != null && verifica_DetReembTit.Count > 0)
                                        {
                                            foreach (var item_detReembTit in verifica_DetReembTit)
                                            {
                                                var detReembTit_tpInsc = item_detReembTit.TpInsc ?? "";
                                                var detReembTit_nrInsc = item_detReembTit.NrInsc ?? "";
                                                var detReembTit_vlrReemb = item_detReembTit.VlrReemb ?? "";
                                                var detReembTit_vlrReembAnt = item_detReembTit.VlrReembAnt ?? "";

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var sql = @"INSERT INTO s_1210_evtpgtos_infoircomplem_inforeembmed_detreembtit(id_projeto, id_usuario, 
                                        id_cad_evtpgtos, id_infoircomplem, id_inforeembmed, detReembTit_tpInsc, detReembTit_nrInsc, 
                                        detReembTit_vlrReemb, detReembTit_vlrReembAnt, id_cadastro_envios) VALUES(@id_projeto, @id_usuario, 
                                        @id_cad_evtpgtos, @id_infoircomplem, @id_inforeembmed, @detReembTit_tpInsc, @detReembTit_nrInsc, 
                                        @detReembTit_vlrReemb, @detReembTit_vlrReembAnt, @id_cadastro_envios)";

                                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                        cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                        cmd.Parameters.AddWithValue("@id_infoircomplem", id_infoircomplem);
                                                        cmd.Parameters.AddWithValue("@id_inforeembmed", id_infoReembMed);
                                                        cmd.Parameters.AddWithValue("@detReembTit_tpInsc", detReembTit_tpInsc);
                                                        cmd.Parameters.AddWithValue("@detReembTit_nrInsc", detReembTit_nrInsc);
                                                        cmd.Parameters.AddWithValue("@detReembTit_vlrReemb", detReembTit_vlrReemb);
                                                        cmd.Parameters.AddWithValue("@detReembTit_vlrReembAnt", detReembTit_vlrReembAnt);
                                                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_infoircomplem_inforeembmed_detreembtit com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_infoircomplem_inforeembmed_detreembtit: " + ex.Message);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        var verifica_InfoReembDep = item_infoReembMed.InfoReembDep;
                                        if (verifica_InfoReembDep != null && verifica_InfoReembDep.Count > 0)
                                        {
                                            foreach (var item_infoReembDep in verifica_InfoReembDep)
                                            {
                                                var infoReembDep_cpfBenef = item_infoReembDep.CpfBenef ?? "";
                                                var infoReembDep_detReembDep = "";
                                                long id_infoReembDep = 0;

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var sql = @"INSERT INTO s_1210_evtpgtos_infoircomplem_inforeembmed_inforeembdep(id_projeto, id_usuario, 
                                        id_cad_evtpgtos, id_infoircomplem, id_infoReembMed, infoReembDep_cpfBenef, infoReembDep_detReembDep, 
                                        id_cadastro_envios) VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, @id_infoircomplem, @id_infoReembMed, 
                                        @infoReembDep_cpfBenef, @infoReembDep_detReembDep, @id_cadastro_envios)";

                                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                        cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                        cmd.Parameters.AddWithValue("@id_infoircomplem", id_infoircomplem);
                                                        cmd.Parameters.AddWithValue("@id_inforeembmed", id_infoReembMed);
                                                        cmd.Parameters.AddWithValue("@infoReembDep_cpfBenef", infoReembDep_cpfBenef);
                                                        cmd.Parameters.AddWithValue("@infoReembDep_detReembDep", infoReembDep_detReembDep);
                                                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            id_infoReembDep = cmd.LastInsertedId;
                                                            Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_infoircomplem_inforeembmed_inforeembdep com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_infoircomplem_inforeembmed_inforeembdep: " + ex.Message);
                                                        }
                                                    }
                                                }

                                                var verifica_item_infoReembDep_DetReembTit = item_infoReembDep.DetReembTit;
                                                if (verifica_item_infoReembDep_DetReembTit != null && verifica_item_infoReembDep_DetReembTit.Count > 0)
                                                {
                                                    foreach (var item_detReembTit_dep in verifica_item_infoReembDep_DetReembTit)
                                                    {
                                                        var detReembDep_tpInsc = item_detReembTit_dep.TpInsc ?? "";
                                                        var detReembDep_nrInsc = item_detReembTit_dep.NrInsc ?? "";
                                                        var detReembDep_vlrReemb = item_detReembTit_dep.VlrReemb ?? "";
                                                        var detReembDep_vlrReembAnt = item_detReembTit_dep.VlrReembAnt ?? "";

                                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                        {
                                                            connection.Open();
                                                            var sql = @"INSERT INTO s_1210_evtpgtos_infoircomplem_inforeembmed_inforeembdep_reembdep
                            (id_projeto, id_usuario, id_cad_evtpgtos, id_infoircomplem, id_inforeembmed, id_inforeembdep,
                            detReembDep_tpInsc, detReembDep_nrInsc, detReembDep_vlrReemb, detReembDep_vlrReembAnt, id_cadastro_envios)
                            VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, @id_infoircomplem, @id_inforeembmed, @id_inforeembdep,
                            @detReembDep_tpInsc, @detReembDep_nrInsc, @detReembDep_vlrReemb, @detReembDep_vlrReembAnt, @id_cadastro_envios)";

                                                            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                            {
                                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                                cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                                cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                                cmd.Parameters.AddWithValue("@id_infoircomplem", id_infoircomplem);
                                                                cmd.Parameters.AddWithValue("@id_inforeembmed", id_infoReembMed);
                                                                cmd.Parameters.AddWithValue("@id_inforeembdep", id_infoReembDep);
                                                                cmd.Parameters.AddWithValue("@detReembDep_tpInsc", detReembDep_tpInsc);
                                                                cmd.Parameters.AddWithValue("@detReembDep_nrInsc", detReembDep_nrInsc);
                                                                cmd.Parameters.AddWithValue("@detReembDep_vlrReemb", detReembDep_vlrReemb);
                                                                cmd.Parameters.AddWithValue("@detReembDep_vlrReembAnt", detReembDep_vlrReembAnt);
                                                                cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                                try
                                                                {
                                                                    cmd.ExecuteNonQuery();
                                                                    Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_infoircomplem_inforeembmed_inforeembdep_reembdep com sucesso!");
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_infoircomplem_inforeembmed_inforeembdep_reembdep: " + ex.Message);
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