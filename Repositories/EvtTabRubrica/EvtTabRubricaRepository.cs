using MySql.Data.MySqlClient;
using System.Globalization;
using TransformarXmlEmCSharpESalvarNoBanco.Models.EvtTabRubrica;

namespace TransformarXmlEmCSharpESalvarNoBanco.Repositories.EvtTabRubrica
{
    public class EvtTabRubricaRepository
    {
        public void InsertEvtTabRubrica(string connectionString, ESocialEvtTabRubrica eSocialEvtTabRubrica, string arquivo, int id_cadastro_envios, int id_usuario)
        {
            var evtTabRubrica_id = eSocialEvtTabRubrica?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtTabRubrica?.Id;
            var ideEvento_tpAmb = Convert.ToInt32(eSocialEvtTabRubrica?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtTabRubrica?.IdeEvento?.TpAmb);
            var ideEvento_procEmi = Convert.ToInt32(eSocialEvtTabRubrica?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtTabRubrica?.IdeEvento?.ProcEmi);
            var ideEvento_verProc = eSocialEvtTabRubrica?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtTabRubrica?.IdeEvento?.VerProc;
            var ideEmpregador_tpInsc = Convert.ToInt32(eSocialEvtTabRubrica?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtTabRubrica?.IdeEmpregador?.TpInsc);
            var ideEmpregador_nrInsc = eSocialEvtTabRubrica?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtTabRubrica?.IdeEmpregador?.NrInsc;
            var recepcao_processamento_dhProcessamento = eSocialEvtTabRubrica?.RetornoProcessamentoDownload?.Recibo?.ESocial?.RetornoEvento?.Processamento?.DhProcessamento;
            var recibo_nrRecibo = eSocialEvtTabRubrica?.RetornoProcessamentoDownload?.Recibo?.ESocial?.RetornoEvento?.ReciboRetornoEvento?.NrRecibo;
            var recibo_hash = eSocialEvtTabRubrica?.RetornoProcessamentoDownload?.Recibo?.ESocial?.RetornoEvento?.ReciboRetornoEvento?.Hash;
            var nome_arquivo_importado = Path.GetFileName(arquivo);
            var versao_layout_evtTabRubrica = Path.GetFileName(eSocialEvtTabRubrica?.RetornoProcessamentoDownload?.Evento?.ESocial?.Namespace);
            
            var inclusao_ideRubrica_codRubr = "";
            var inclusao_ideRubrica_ideTabRubr = "";
            var inclusao_ideRubrica_iniValid = "";
            var inclusao_ideRubrica_fimValid = "";
            var inclusao_dadosRubrica_dscRubr = "";
            var inclusao_dadosRubrica_natRubr = 0;
            var inclusao_dadosRubrica_tpRubr = 0;
            var inclusao_dadosRubrica_codIncCP = "";
            var inclusao_dadosRubrica_codIncIRRF = "";
            var inclusao_dadosRubrica_codIncFGTS = "";
            var inclusao_dadosRubrica_codIncSIND = "";
            var inclusao_dadosRubrica_observacao = "";

            var inclusao_ideProcessoCP_tpProc = "";
            var inclusao_ideProcessoCP_nrProc = "";
            var inclusao_ideProcessoCP_extDecisao = "";
            var inclusao_ideProcessoCP_codSusp = "";

            var inclusao_ideProcessoIRRF_nrProc = "";
            var inclusao_ideProcessoIRRF_codSusp = "";

            var inclusao_ideProcessoFGTS_nrProc = "";

            var inclusao_ideProcessoSIND_nrProc = "";

            var inclusao_dadosRubrica_verifica = eSocialEvtTabRubrica?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtTabRubrica?.InfoRubrica?.Inclusao;
            if (inclusao_dadosRubrica_verifica != null && inclusao_dadosRubrica_verifica.Count > 0)
            {
                foreach (var item_inclusao in inclusao_dadosRubrica_verifica)
                {
                    inclusao_ideRubrica_codRubr = item_inclusao.IdeRubrica?.CodRubr;
                    inclusao_ideRubrica_ideTabRubr = item_inclusao.IdeRubrica?.IdeTabRubr ?? "";
                    inclusao_ideRubrica_iniValid = item_inclusao.IdeRubrica?.IniValid ?? "";
                    inclusao_ideRubrica_fimValid = item_inclusao.IdeRubrica?.FimValid ?? "";
                    inclusao_dadosRubrica_dscRubr = item_inclusao.DadosRubrica?.DscRubr ?? "";
                    inclusao_dadosRubrica_natRubr = Convert.ToInt32(item_inclusao.DadosRubrica?.NatRubr);
                    inclusao_dadosRubrica_tpRubr = Convert.ToInt32(item_inclusao.DadosRubrica?.TpRubr);
                    inclusao_dadosRubrica_codIncCP = item_inclusao.DadosRubrica?.CodIncCP ?? "";
                    inclusao_dadosRubrica_codIncIRRF = item_inclusao.DadosRubrica?.CodIncIRRF ?? "";
                    inclusao_dadosRubrica_codIncFGTS = item_inclusao.DadosRubrica?.CodIncFGTS ?? "";
                    inclusao_dadosRubrica_codIncSIND = item_inclusao.DadosRubrica?.CodIncSIND ?? "";
                    inclusao_dadosRubrica_observacao = item_inclusao.DadosRubrica?.Observacao ?? "";

                    var inclusao_dadosRubrica_ideProcessoCP_verifica = item_inclusao.DadosRubrica?.IdeProcessoCP;
                    if (inclusao_dadosRubrica_ideProcessoCP_verifica != null && inclusao_dadosRubrica_ideProcessoCP_verifica.Count > 0)
                    {
                        foreach (var item_ideProcessoCP in inclusao_dadosRubrica_ideProcessoCP_verifica)
                        {
                            inclusao_ideProcessoCP_tpProc = item_ideProcessoCP.TpProc ?? "";
                            inclusao_ideProcessoCP_nrProc = item_ideProcessoCP.NrProc ?? "";
                            inclusao_ideProcessoCP_extDecisao = item_ideProcessoCP.ExtDecisao ?? "";
                            inclusao_ideProcessoCP_codSusp = item_ideProcessoCP.CodSusp ?? "";
                        }
                    }

                    var alteracao_dadosRubrica_ideProcessoIRRF_verifica = item_inclusao.DadosRubrica?.IdeProcessoIRRF;
                    if (alteracao_dadosRubrica_ideProcessoIRRF_verifica != null && alteracao_dadosRubrica_ideProcessoIRRF_verifica.Count > 0)
                    {
                        foreach (var item_ideProcessoIRRF in alteracao_dadosRubrica_ideProcessoIRRF_verifica)
                        {
                            inclusao_ideProcessoIRRF_nrProc = item_ideProcessoIRRF.NrProc ?? "";
                            inclusao_ideProcessoIRRF_codSusp = item_ideProcessoIRRF.CodSusp ?? "";
                        }
                    }

                    var alteracao_dadosRubrica_ideProcessoFGTS_verifica = item_inclusao.DadosRubrica?.IdeProcessoFGTS;
                    if (alteracao_dadosRubrica_ideProcessoFGTS_verifica != null && alteracao_dadosRubrica_ideProcessoFGTS_verifica.Count > 0)
                    {
                        foreach (var item_ideProcessoFGTS in alteracao_dadosRubrica_ideProcessoFGTS_verifica)
                        {
                            inclusao_ideProcessoFGTS_nrProc = item_ideProcessoFGTS.NrProc ?? "";
                        }
                    }

                    var alteracao_dadosRubrica_ideProcessoSIND_verifica = item_inclusao.DadosRubrica?.IdeProcessoSIND;
                    if (alteracao_dadosRubrica_ideProcessoSIND_verifica != null && alteracao_dadosRubrica_ideProcessoSIND_verifica.Count > 0)
                    {
                        foreach (var item_ideProcessoSIND in alteracao_dadosRubrica_ideProcessoSIND_verifica)
                        {
                            inclusao_ideProcessoSIND_nrProc = item_ideProcessoSIND.TpProc ?? "";
                        }
                    }
                }
            }
            
            var alteracao_ideRubrica_codRubr = "";
            var alteracao_ideRubrica_ideTabRubr = "";
            var alteracao_ideRubrica_iniValid = "";
            var alteracao_ideRubrica_novaValidade_iniValid = "";
            var alteracao_ideRubrica_novaValidade_fimValid = "";
            var alteracao_dadosRubrica_dscRubr = "";
            var alteracao_dadosRubrica_natRubr = 0;
            var alteracao_dadosRubrica_tpRubr = 0;
            var alteracao_dadosRubrica_codIncCP = "";
            var alteracao_dadosRubrica_codIncIRRF = "";
            var alteracao_dadosRubrica_codIncFGTS = "";
            var alteracao_dadosRubrica_codIncSIND = "";
            var alteracao_dadosRubrica_observacao = "";

            var alteracao_ideProcessoCP_tpProc = "";
            var alteracao_ideProcessoCP_nrProc = "";
            var alteracao_ideProcessoCP_extDecisao = "";
            var alteracao_ideProcessoCP_codSusp = "";

            var alteracao_ideProcessoIRRF_nrProc = "";
            var alteracao_ideProcessoIRRF_codSusp = "";

            var alteracao_ideProcessoFGTS_nrProc = "";

            var alteracao_ideProcessoSIND_nrProc = "";

            var infoRubrica_alteracao_verifica = eSocialEvtTabRubrica?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtTabRubrica?.InfoRubrica?.Alteracao;
            if (infoRubrica_alteracao_verifica != null && infoRubrica_alteracao_verifica.Count > 0)
            {
                foreach (var item_alteracao in infoRubrica_alteracao_verifica)
                {
                    alteracao_ideRubrica_codRubr = item_alteracao.IdeRubrica?.CodRubr;
                    alteracao_ideRubrica_ideTabRubr = item_alteracao.IdeRubrica?.IdeTabRubr ?? "";
                    alteracao_ideRubrica_iniValid = item_alteracao.IdeRubrica?.IniValid ?? "";
                    alteracao_ideRubrica_novaValidade_iniValid = item_alteracao.IdeRubrica?.NovaValidade?.IniValid ?? "";
                    alteracao_ideRubrica_novaValidade_fimValid = item_alteracao.IdeRubrica?.NovaValidade?.FimValid ?? "";
                    alteracao_dadosRubrica_dscRubr = item_alteracao.DadosRubrica?.DscRubr ?? "";
                    alteracao_dadosRubrica_natRubr = Convert.ToInt32(item_alteracao.DadosRubrica?.NatRubr);
                    alteracao_dadosRubrica_tpRubr = Convert.ToInt32(item_alteracao.DadosRubrica?.TpRubr);
                    alteracao_dadosRubrica_codIncCP = item_alteracao.DadosRubrica?.CodIncCP ?? "";
                    alteracao_dadosRubrica_codIncIRRF = item_alteracao.DadosRubrica?.CodIncIRRF ?? "";
                    alteracao_dadosRubrica_codIncFGTS = item_alteracao.DadosRubrica?.CodIncFGTS ?? "";
                    alteracao_dadosRubrica_codIncSIND = item_alteracao.DadosRubrica?.CodIncSIND ?? "";
                    alteracao_dadosRubrica_observacao = item_alteracao.DadosRubrica?.Observacao ?? "";

                    var alteracao_dadosRubrica_observacao_verifica = item_alteracao.DadosRubrica?.IdeProcessoCP;
                    if (alteracao_dadosRubrica_observacao_verifica != null && alteracao_dadosRubrica_observacao_verifica.Count > 0)
                    {
                        foreach (var item_ideProcessoCP in alteracao_dadosRubrica_observacao_verifica)
                        {
                            alteracao_ideProcessoCP_tpProc = item_ideProcessoCP.TpProc ?? "";
                            alteracao_ideProcessoCP_nrProc = item_ideProcessoCP.NrProc ?? "";
                            alteracao_ideProcessoCP_extDecisao = item_ideProcessoCP.ExtDecisao ?? "";
                            alteracao_ideProcessoCP_codSusp = item_ideProcessoCP.CodSusp ?? "";
                        }
                    }

                    var alteracao_ideProcessoIRRF_nrProc_verifica = item_alteracao.DadosRubrica?.IdeProcessoIRRF;
                    if (alteracao_ideProcessoIRRF_nrProc_verifica != null && alteracao_ideProcessoIRRF_nrProc_verifica.Count > 0)
                    {
                        foreach (var item_ideProcessoIRRF in alteracao_ideProcessoIRRF_nrProc_verifica)
                        {
                            alteracao_ideProcessoIRRF_nrProc = item_ideProcessoIRRF.NrProc ?? "";
                            alteracao_ideProcessoIRRF_codSusp = item_ideProcessoIRRF.CodSusp ?? "";
                        }
                    }

                    var alteracao_ideProcessoFGTS_nrProc_verifica = item_alteracao.DadosRubrica?.IdeProcessoFGTS;
                    if (alteracao_ideProcessoFGTS_nrProc_verifica != null && alteracao_ideProcessoFGTS_nrProc_verifica.Count > 0)
                    {
                        foreach (var item_ideProcessoFGTS in alteracao_ideProcessoFGTS_nrProc_verifica)
                        {
                            alteracao_ideProcessoFGTS_nrProc = item_ideProcessoFGTS.NrProc ?? "";
                        }
                    }

                    var alteracao_ideProcessoSIND_nrProc_verifica = item_alteracao.DadosRubrica?.IdeProcessoSIND;
                    if (alteracao_ideProcessoSIND_nrProc_verifica != null && alteracao_ideProcessoSIND_nrProc_verifica.Count > 0)
                    {
                        foreach (var item_ideProcessoSIND in alteracao_ideProcessoSIND_nrProc_verifica)
                        {
                            alteracao_ideProcessoSIND_nrProc = item_ideProcessoSIND.TpProc ?? "";
                        }
                    }
                }
            }
            
            var exclusao_ideRubrica_codRubr = "";
            var exclusao_ideRubrica_ideTabRubr = "";
            var exclusao_ideRubrica_iniValid = "";
            var exclusao_ideRubrica_fimValid = "";
            var infoRubrica_exclusao_verifica = eSocialEvtTabRubrica?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtTabRubrica?.InfoRubrica?.Exclusao;
            if (infoRubrica_exclusao_verifica != null && infoRubrica_exclusao_verifica.Count > 0)
            {
                foreach (var item_exclusao in infoRubrica_exclusao_verifica)
                {
                    exclusao_ideRubrica_codRubr = item_exclusao.IdeRubrica?.CodRubr;
                    exclusao_ideRubrica_ideTabRubr = item_exclusao.IdeRubrica?.IdeTabRubr ?? "";
                    exclusao_ideRubrica_iniValid = item_exclusao.IdeRubrica?.IniValid ?? "";
                    exclusao_ideRubrica_fimValid = item_exclusao.IdeRubrica?.FimValid ?? "";
                }
            }

            var filtro_tipo_rubrica = " AND inclusao_ideRubrica_codRubr = '0' ";
            if (inclusao_ideRubrica_codRubr != "0")
                filtro_tipo_rubrica = " AND inclusao_ideRubrica_codRubr = '" + inclusao_ideRubrica_codRubr + "' ";
            else if (alteracao_ideRubrica_codRubr != "0")
                filtro_tipo_rubrica = " AND alteracao_ideRubrica_codRubr = '" + alteracao_ideRubrica_codRubr + "' ";
            else if (exclusao_ideRubrica_codRubr != "0")
                filtro_tipo_rubrica = " AND exclusao_ideRubrica_codRubr = '" + exclusao_ideRubrica_codRubr + "' ";

            var dados_cod_rubrica_id = 0;
            var data_arquivo_ja_cadastrado = new DateTime();
            var data_arquivo_novo = Convert.ToDateTime(recepcao_processamento_dhProcessamento);
            var sql_verifica_cod_rubrica = @$"SELECT id, id_cadastro_envios, inclusao_ideRubrica_codRubr, alteracao_ideRubrica_codRubr, 
                                            exclusao_ideRubrica_codRubr, recepcao_processamento_dhProcessamento FROM s_1010_evttabrubrica 
                                            WHERE id_cadastro_envios = '{id_cadastro_envios}' {filtro_tipo_rubrica}";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql_verifica_cod_rubrica, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        var conta_cod_rubrica = 0;

                        while (reader.Read())
                        {
                            if (conta_cod_rubrica == 0)
                            {
                                data_arquivo_ja_cadastrado = reader.GetDateTime("recepcao_processamento_dhProcessamento");
                                dados_cod_rubrica_id = reader.GetInt32(reader.GetOrdinal("id"));
                            }
                            conta_cod_rubrica++;
                        }
                    }
                }
            }

            if (data_arquivo_novo > data_arquivo_ja_cadastrado)
            {
                // EXCLUIR O ARQUIVO ANTIGO
                var repository = new Repository();
                var tabela = repository.BuscararNomeTabela(sql_verifica_cod_rubrica);
                var exclusao = repository.ExcluirRegistroAntigo(connectionString, tabela, dados_cod_rubrica_id);

                if (exclusao == true)
                {
                    // INSERIR O ARQUIVO NOVO
                    long id_evtdeslig = 0;
                    using (MySqlConnection connectionInsert_s_1010_evttabrubricag = new MySqlConnection(connectionString))
                    {
                        connectionInsert_s_1010_evttabrubricag.Open();
                        var sqlInsert_s_1010_evttabrubrica = @"INSERT INTO s_1010_evttabrubrica (id_cadastro_envios, id_usuario, 
                        evtTabRubrica_id, ideEvento_tpAmb, ideEvento_procEmi, ideEvento_verProc, ideEmpregador_tpInsc, 
                        ideEmpregador_nrInsc, inclusao_ideRubrica_codRubr, inclusao_ideRubrica_ideTabRubr, 
                        inclusao_ideRubrica_iniValid, inclusao_ideRubrica_fimValid, inclusao_dadosRubrica_dscRubr, 
                        inclusao_dadosRubrica_natRubr, inclusao_dadosRubrica_tpRubr, inclusao_dadosRubrica_codIncCP, 
                        inclusao_dadosRubrica_codIncIRRF, inclusao_dadosRubrica_codIncFGTS, inclusao_dadosRubrica_codIncSIND, 
                        inclusao_dadosRubrica_observacao, inclusao_ideProcessoCP_tpProc, inclusao_ideProcessoCP_nrProc, 
                        inclusao_ideProcessoCP_extDecisao, inclusao_ideProcessoCP_codSusp, inclusao_ideProcessoIRRF_nrProc, 
                        inclusao_ideProcessoIRRF_codSusp, inclusao_ideProcessoFGTS_nrProc, inclusao_ideProcessoSIND_nrProc, 
                        alteracao_ideRubrica_codRubr, alteracao_ideRubrica_ideTabRubr, alteracao_ideRubrica_iniValid, 
                        alteracao_ideRubrica_novaValidade_iniValid, alteracao_ideRubrica_novaValidade_fimValid, 
                        alteracao_dadosRubrica_dscRubr, alteracao_dadosRubrica_natRubr, alteracao_dadosRubrica_tpRubr, 
                        alteracao_dadosRubrica_codIncCP, alteracao_dadosRubrica_codIncIRRF, alteracao_dadosRubrica_codIncFGTS, 
                        alteracao_dadosRubrica_codIncSIND, alteracao_dadosRubrica_observacao, alteracao_ideProcessoCP_tpProc, 
                        alteracao_ideProcessoCP_nrProc, alteracao_ideProcessoCP_extDecisao, alteracao_ideProcessoCP_codSusp, 
                        alteracao_ideProcessoIRRF_nrProc, alteracao_ideProcessoIRRF_codSusp, alteracao_ideProcessoFGTS_nrProc, 
                        alteracao_ideProcessoSIND_nrProc, exclusao_ideRubrica_codRubr, exclusao_ideRubrica_ideTabRubr, 
                        exclusao_ideRubrica_iniValid, exclusao_ideRubrica_fimValid, recibo_nrRecibo, recepcao_processamento_dhProcessamento, 
                        recibo_hash, nome_arquivo_importado)
                        VALUES(
                        @id_cadastro_envios, @id_usuario, @evtTabRubrica_id, @ideEvento_tpAmb, @ideEvento_procEmi, 
                        @ideEvento_verProc, @ideEmpregador_tpInsc, @ideEmpregador_nrInsc, @inclusao_ideRubrica_codRubr, 
                        @inclusao_ideRubrica_ideTabRubr, @inclusao_ideRubrica_iniValid, @inclusao_ideRubrica_fimValid, 
                        @inclusao_dadosRubrica_dscRubr, @inclusao_dadosRubrica_natRubr, @inclusao_dadosRubrica_tpRubr, 
                        @inclusao_dadosRubrica_codIncCP, @inclusao_dadosRubrica_codIncIRRF, @inclusao_dadosRubrica_codIncFGTS, 
                        @inclusao_dadosRubrica_codIncSIND, @inclusao_dadosRubrica_observacao, @inclusao_ideProcessoCP_tpProc, 
                        @inclusao_ideProcessoCP_nrProc, @inclusao_ideProcessoCP_extDecisao, @inclusao_ideProcessoCP_codSusp, 
                        @inclusao_ideProcessoIRRF_nrProc, @inclusao_ideProcessoIRRF_codSusp, @inclusao_ideProcessoFGTS_nrProc, 
                        @inclusao_ideProcessoSIND_nrProc, @alteracao_ideRubrica_codRubr, @alteracao_ideRubrica_ideTabRubr, 
                        @alteracao_ideRubrica_iniValid, @alteracao_ideRubrica_novaValidade_iniValid, 
                        @alteracao_ideRubrica_novaValidade_fimValid, @alteracao_dadosRubrica_dscRubr, 
                        @alteracao_dadosRubrica_natRubr, @alteracao_dadosRubrica_tpRubr, @alteracao_dadosRubrica_codIncCP, 
                        @alteracao_dadosRubrica_codIncIRRF, @alteracao_dadosRubrica_codIncFGTS, @alteracao_dadosRubrica_codIncSIND, 
                        @alteracao_dadosRubrica_observacao, @alteracao_ideProcessoCP_tpProc, @alteracao_ideProcessoCP_nrProc, 
                        @alteracao_ideProcessoCP_extDecisao, @alteracao_ideProcessoCP_codSusp, @alteracao_ideProcessoIRRF_nrProc, 
                        @alteracao_ideProcessoIRRF_codSusp, @alteracao_ideProcessoFGTS_nrProc, @alteracao_ideProcessoSIND_nrProc, 
                        @exclusao_ideRubrica_codRubr, @exclusao_ideRubrica_ideTabRubr, @exclusao_ideRubrica_iniValid, 
                        @exclusao_ideRubrica_fimValid, @recibo_nrRecibo, @recepcao_processamento_dhProcessamento, @recibo_hash, @nome_arquivo_importado)";


                        using (MySqlCommand cmdInsert_s_1010_evttabrubrica = new MySqlCommand(sqlInsert_s_1010_evttabrubrica, connectionInsert_s_1010_evttabrubricag))
                        {

                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@id_usuario", id_usuario);
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@evtTabRubrica_id", evtTabRubrica_id ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@ideEvento_tpAmb", ideEvento_tpAmb);
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@ideEvento_procEmi", ideEvento_procEmi);
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@ideEvento_verProc", ideEvento_verProc ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@ideEmpregador_tpInsc", ideEmpregador_tpInsc);
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@ideEmpregador_nrInsc", ideEmpregador_nrInsc ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@inclusao_ideRubrica_codRubr", inclusao_ideRubrica_codRubr);
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@inclusao_ideRubrica_ideTabRubr", inclusao_ideRubrica_ideTabRubr ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@inclusao_ideRubrica_iniValid", inclusao_ideRubrica_iniValid ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@inclusao_ideRubrica_fimValid", inclusao_ideRubrica_fimValid ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@inclusao_dadosRubrica_dscRubr", inclusao_dadosRubrica_dscRubr ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@inclusao_dadosRubrica_natRubr", inclusao_dadosRubrica_natRubr);
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@inclusao_dadosRubrica_tpRubr", inclusao_dadosRubrica_tpRubr);
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@inclusao_dadosRubrica_codIncCP", inclusao_dadosRubrica_codIncCP ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@inclusao_dadosRubrica_codIncIRRF", inclusao_dadosRubrica_codIncIRRF ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@inclusao_dadosRubrica_codIncFGTS", inclusao_dadosRubrica_codIncFGTS ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@inclusao_dadosRubrica_codIncSIND", inclusao_dadosRubrica_codIncSIND ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@inclusao_dadosRubrica_observacao", inclusao_dadosRubrica_observacao ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@inclusao_ideProcessoCP_tpProc", inclusao_ideProcessoCP_tpProc ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@inclusao_ideProcessoCP_nrProc", inclusao_ideProcessoCP_nrProc ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@inclusao_ideProcessoCP_extDecisao", inclusao_ideProcessoCP_extDecisao ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@inclusao_ideProcessoCP_codSusp", inclusao_ideProcessoCP_codSusp ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@inclusao_ideProcessoIRRF_nrProc", inclusao_ideProcessoIRRF_nrProc ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@inclusao_ideProcessoIRRF_codSusp", inclusao_ideProcessoIRRF_codSusp ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@inclusao_ideProcessoFGTS_nrProc", inclusao_ideProcessoFGTS_nrProc ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@inclusao_ideProcessoSIND_nrProc", inclusao_ideProcessoSIND_nrProc ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_ideRubrica_codRubr", alteracao_ideRubrica_codRubr);
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_ideRubrica_ideTabRubr", alteracao_ideRubrica_ideTabRubr ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_ideRubrica_iniValid", alteracao_ideRubrica_iniValid ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_ideRubrica_novaValidade_iniValid", alteracao_ideRubrica_novaValidade_iniValid ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_ideRubrica_novaValidade_fimValid", alteracao_ideRubrica_novaValidade_fimValid ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_dadosRubrica_dscRubr", alteracao_dadosRubrica_dscRubr ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_dadosRubrica_natRubr", alteracao_dadosRubrica_natRubr);
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_dadosRubrica_tpRubr", alteracao_dadosRubrica_tpRubr);
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_dadosRubrica_codIncCP", alteracao_dadosRubrica_codIncCP ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_dadosRubrica_codIncIRRF", alteracao_dadosRubrica_codIncIRRF ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_dadosRubrica_codIncFGTS", alteracao_dadosRubrica_codIncFGTS ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_dadosRubrica_codIncSIND", alteracao_dadosRubrica_codIncSIND ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_dadosRubrica_observacao", alteracao_dadosRubrica_observacao ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_ideProcessoCP_tpProc", alteracao_ideProcessoCP_tpProc ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_ideProcessoCP_nrProc", alteracao_ideProcessoCP_nrProc ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_ideProcessoCP_extDecisao", alteracao_ideProcessoCP_extDecisao ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_ideProcessoCP_codSusp", alteracao_ideProcessoCP_codSusp ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_ideProcessoIRRF_nrProc", alteracao_ideProcessoIRRF_nrProc ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_ideProcessoIRRF_codSusp", alteracao_ideProcessoIRRF_codSusp ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_ideProcessoFGTS_nrProc", alteracao_ideProcessoFGTS_nrProc ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@alteracao_ideProcessoSIND_nrProc", alteracao_ideProcessoSIND_nrProc ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@exclusao_ideRubrica_codRubr", exclusao_ideRubrica_codRubr);
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@exclusao_ideRubrica_ideTabRubr", exclusao_ideRubrica_ideTabRubr ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@exclusao_ideRubrica_iniValid", exclusao_ideRubrica_iniValid ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@exclusao_ideRubrica_fimValid", exclusao_ideRubrica_fimValid ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@recibo_nrRecibo", recibo_nrRecibo ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@recibo_hash", recibo_hash ?? "");
                            cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@nome_arquivo_importado", nome_arquivo_importado ?? "");

                            // Convertendo recepcao_processamento_dhProcessamento para o formato "yyyy-MM-dd HH:mm:ss" ou "yyyy-MM-dd HH:mm:sss"
                            int indexOfDot = recepcao_processamento_dhProcessamento.LastIndexOf('.');
                            if (indexOfDot != -1 && indexOfDot + 1 < recepcao_processamento_dhProcessamento.Length)
                            {
                                string millisecondsPart = recepcao_processamento_dhProcessamento.Substring(indexOfDot + 1);

                                if (millisecondsPart.Length == 2)
                                {
                                    DateTime recepcaoProcessamentoDhProcessamento = DateTime.ParseExact(recepcao_processamento_dhProcessamento, "yyyy-MM-ddTHH:mm:ss.ff", CultureInfo.InvariantCulture);
                                    cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@recepcao_processamento_dhProcessamento", recepcaoProcessamentoDhProcessamento.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) ?? "");
                                }
                                if (millisecondsPart.Length == 3)
                                {
                                    DateTime recepcaoProcessamentoDhProcessamento = DateTime.ParseExact(recepcao_processamento_dhProcessamento, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
                                    cmdInsert_s_1010_evttabrubrica.Parameters.AddWithValue("@recepcao_processamento_dhProcessamento", recepcaoProcessamentoDhProcessamento.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) ?? "");
                                }
                            }

                            try
                            {
                                cmdInsert_s_1010_evttabrubrica.ExecuteNonQuery();
                                var id_evttabrubrica = cmdInsert_s_1010_evttabrubrica.LastInsertedId;
                                Console.WriteLine($"EvtTabRubrica({id_evttabrubrica}) inserido na tabela s_1010_evttabrubrica com sucesso!");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Erro ao inserir EvtTabRubrica na tabela s_1010_evttabrubrica: " + ex.Message);
                            }
                        }
                    }
                }
            }
        }
    }
}