using MySql.Data.MySqlClient;
using System.Globalization;
using TransformarXmlEmCSharpESalvarNoBanco.Models.EvtPgtos;

namespace TransformarXmlEmCSharpESalvarNoBanco.Repositories.EvtPgtos
{
    public class EvtPgtos_v_S_01_01_Repository
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