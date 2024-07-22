using MySql.Data.MySqlClient;
using System.Globalization;
using TransformarXmlEmCSharpESalvarNoBanco.Models.EvtPgtos;

namespace TransformarXmlEmCSharpESalvarNoBanco.Repositories.EvtPgtos
{
    public class EvtPgtos_V02_0_Repository
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
                            ideEvento_indRetif, ideEvento_nrRecibo, ideEvento_indApuracao, ideEvento_perApur, ideEvento_tpAmb, 
                            ideEvento_procEmi, ideEvento_verProc, ideEmpregador_tpInsc, ideEmpregador_nrInsc, ideBenef_cpfBenef, 
                            deps_vrDedDep, recibo_nrRecibo, recepcao_processamento_dhProcessamento, 
                            versao_layout_evtPgtos, id_cadastro_envios, nome_arquivo_importado)VALUES(@id_projeto, @id_usuario, 
                            @evtPgtos_id, @ideEvento_indRetif, @ideEvento_nrRecibo, @ideEvento_indApuracao, @ideEvento_perApur, 
                            @ideEvento_tpAmb, @ideEvento_procEmi, @ideEvento_verProc, @ideEmpregador_tpInsc, @ideEmpregador_nrInsc, 
                            @ideBenef_cpfBenef, @deps_vrDedDep, @recibo_nrRecibo, @recepcao_processamento_dhProcessamento, 
                            @versao_layout_evtPgtos, @id_cadastro_envios, @nome_arquivo_importado)";


                            using (MySqlCommand cmdInsert_s_1210_evtpgtos = new MySqlCommand(sqlInsert_s_1210_evtpgtos, connectionInsert_s_1210_evtpgtos))
                            {
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("evtPgtos_id", evtPgtos_id ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("ideEvento_indRetif", ideEvento_indRetif ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("ideEvento_nrRecibo", ideEvento_nrRecibo ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("ideEvento_indApuracao", ideEvento_indApuracao ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("ideEvento_perApur", ideEvento_perApur ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("ideEvento_tpAmb", ideEvento_tpAmb ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("ideEvento_procEmi", ideEvento_procEmi ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("ideEvento_verProc", ideEvento_verProc ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("ideEmpregador_tpInsc", ideEmpregador_tpInsc);
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("ideEmpregador_nrInsc", ideEmpregador_nrInsc ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("ideBenef_cpfBenef", ideBenef_cpfBenef ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("deps_vrDedDep", deps_vrDedDep ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("recibo_nrRecibo", recibo_nrRecibo ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("versao_layout_evtPgtos", versao_layout_evtPgtos ?? "");
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("id_cadastro_envios", id_cadastro_envios);
                                cmdInsert_s_1210_evtpgtos.Parameters.AddWithValue("nome_arquivo_importado", nome_arquivo_importado ?? "");

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
                                var infoPgto_indResBr = item_infoPgto.IndResBr ?? "";
                                long id_evtpgtos_informacoes = 0;

                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                {
                                    connection.Open();
                                    var sql = @"INSERT INTO s_1210_evtpgtos_informacoes (id_projeto, 
                                id_usuario, id_cad_evtpgtos, infoPgto_dtPgto, infoPgto_tpPgto, infoPgto_indResBr, 
                                id_cadastro_envios)VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, @infoPgto_dtPgto, @infoPgto_tpPgto, 
                                @infoPgto_indResBr, @id_cadastro_envios)";

                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                    {
                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                        cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                        cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                        cmd.Parameters.AddWithValue("@infoPgto_dtPgto", infoPgto_dtPgto);
                                        cmd.Parameters.AddWithValue("@infoPgto_tpPgto", infoPgto_tpPgto);
                                        cmd.Parameters.AddWithValue("@infoPgto_indResBr", infoPgto_indResBr);
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

                                var verifica_DetPgtoFl = item_infoPgto.DetPgtoFl;
                                if (verifica_DetPgtoFl != null && verifica_DetPgtoFl.Count > 0)
                                {
                                    foreach (var item_detPgtoFl in verifica_DetPgtoFl)
                                    {
                                        var detPgtoFl_perRef = item_detPgtoFl.PerRef ?? "";
                                        var detPgtoFl_ideDmDev = item_detPgtoFl.IdeDmDev ?? "";
                                        var detPgtoFl_indPgtoTt = item_detPgtoFl.IndPgtoTt ?? "";
                                        var detPgtoFl_vrLiq = item_detPgtoFl.VrLiq ?? "";
                                        var detPgtoFl_nrRecArq = item_detPgtoFl.NrRecArq ?? "";
                                        long id_s_1210_evtpgtos_info_detpgtofl = 0;

                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            var sql = @"INSERT INTO s_1210_evtpgtos_info_detpgtofl (id_projeto, 
                                    id_usuario, id_cad_evtpgtos, id_evtpgtos_informacoes, detPgtoFl_perRef, detPgtoFl_ideDmDev, 
                                    detPgtoFl_indPgtoTt, detPgtoFl_vrLiq, detPgtoFl_nrRecArq, id_cadastro_envios)VALUES(@id_projeto, 
                                    @id_usuario, @id_cad_evtpgtos, @id_evtpgtos_informacoes, @detPgtoFl_perRef, @detPgtoFl_ideDmDev, 
                                    @detPgtoFl_indPgtoTt, @detPgtoFl_vrLiq, @detPgtoFl_nrRecArq, @id_cadastro_envios)";

                                            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                            {
                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                cmd.Parameters.AddWithValue("@id_evtpgtos_informacoes", id_evtpgtos_informacoes);
                                                cmd.Parameters.AddWithValue("@detPgtoFl_perRef", detPgtoFl_perRef);
                                                cmd.Parameters.AddWithValue("@detPgtoFl_ideDmDev", detPgtoFl_ideDmDev);
                                                cmd.Parameters.AddWithValue("@detPgtoFl_indPgtoTt", detPgtoFl_indPgtoTt);
                                                cmd.Parameters.AddWithValue("@detPgtoFl_vrLiq", detPgtoFl_vrLiq);
                                                cmd.Parameters.AddWithValue("@detPgtoFl_nrRecArq", detPgtoFl_nrRecArq);
                                                cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                try
                                                {
                                                    cmd.ExecuteNonQuery();
                                                    id_s_1210_evtpgtos_info_detpgtofl = cmd.LastInsertedId;
                                                    Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_info_detpgtofl com sucesso!");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_info_detpgtofl: " + ex.Message);
                                                }
                                            }
                                        }

                                        var verifica_RetPgtoTot = item_detPgtoFl.RetPgtoTot;
                                        if (verifica_RetPgtoTot != null && verifica_RetPgtoTot.Count > 0)
                                        {
                                            foreach (var item_retPgtoTot in verifica_RetPgtoTot)
                                            {
                                                var retPgtoTot_codRubr = item_retPgtoTot.CodRubr ?? "";
                                                var retPgtoTot_ideTabRubr = item_retPgtoTot.IdeTabRubr ?? "";
                                                var retPgtoTot_qtdRubr = item_retPgtoTot.QtdRubr ?? "";
                                                var retPgtoTot_fatorRubr = item_retPgtoTot.FatorRubr ?? "";
                                                var retPgtoTot_vrUnit = item_retPgtoTot.VrUnit ?? "";
                                                var retPgtoTot_vrRubr = item_retPgtoTot.VrRubr ?? "";
                                                long id_s_1210_evtpgtos_info_detpgtofl_retpgtotot = 0;

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var sql = @"INSERT INTO s_1210_evtpgtos_info_detpgtofl_retpgtotot (id_projeto, 
                                        id_usuario, id_cad_evtpgtos, id_evtpgtos_informacoes, id_evtpgtos_info_detpgtofl, info_detpgtofl_codRubr, 
                                        info_detpgtofl_ideTabRubr, info_detpgtofl_qtdRubr, info_detpgtofl_fatorRubr, info_detpgtofl_vrUnit, 
                                        info_detpgtofl_vrRubr, id_cadastro_envios)VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, 
                                        @id_evtpgtos_informacoes, @id_evtpgtos_info_detpgtofl, @info_detpgtofl_codRubr, @info_detpgtofl_ideTabRubr, 
                                        @info_detpgtofl_qtdRubr, @info_detpgtofl_fatorRubr, @info_detpgtofl_vrUnit, @info_detpgtofl_vrRubr, @id_cadastro_envios)";

                                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                        cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                        cmd.Parameters.AddWithValue("@id_evtpgtos_informacoes", id_evtpgtos_informacoes);
                                                        cmd.Parameters.AddWithValue("@id_evtpgtos_info_detpgtofl", id_s_1210_evtpgtos_info_detpgtofl);
                                                        cmd.Parameters.AddWithValue("@info_detpgtofl_codRubr", retPgtoTot_codRubr);
                                                        cmd.Parameters.AddWithValue("@info_detpgtofl_ideTabRubr", retPgtoTot_ideTabRubr);
                                                        cmd.Parameters.AddWithValue("@info_detpgtofl_qtdRubr", retPgtoTot_qtdRubr);
                                                        cmd.Parameters.AddWithValue("@info_detpgtofl_fatorRubr", retPgtoTot_fatorRubr);
                                                        cmd.Parameters.AddWithValue("@info_detpgtofl_vrUnit", retPgtoTot_vrUnit);
                                                        cmd.Parameters.AddWithValue("@info_detpgtofl_vrRubr", retPgtoTot_vrRubr);
                                                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            id_s_1210_evtpgtos_info_detpgtofl_retpgtotot = cmd.LastInsertedId;
                                                            Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_info_detpgtofl_retpgtotot com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_info_detpgtofl_retpgtotot: " + ex.Message);
                                                        }
                                                    }
                                                }

                                                var verifica_PenAlim = item_retPgtoTot.PenAlim;
                                                if (verifica_PenAlim != null && verifica_PenAlim.Count > 0)
                                                {
                                                    foreach (var item_penAlim in verifica_PenAlim)
                                                    {
                                                        var penAlim_cpfBenef = item_penAlim.CpfBenef ?? "";
                                                        var penAlim_dtNasctoBenef = item_penAlim.DtNasctoBenef ?? "";
                                                        var penAlim_nmBenefic = item_penAlim.NmBenefic ?? "";
                                                        var penAlim_vlrPensao = item_penAlim.VlrPensao ?? "";

                                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                        {
                                                            connection.Open();
                                                            var sql = @"INSERT INTO s_1210_evtpgtos_info_detpgtofl_retpgtotot_penalim (id_projeto, 
                                            id_usuario, id_cad_evtpgtos, id_evtpgtos_informacoes, id_evtpgtos_info_detpgtofl, id_evtpgtos_info_detpgtofl_retpgtotot, 
                                            penAlim_cpfBenef, penAlim_dtNasctoBenef, penAlim_nmBenefic, penAlim_vlrPensao, id_cadastro_envios)VALUES(@id_projeto, 
                                            @id_usuario, @id_cad_evtpgtos, @id_evtpgtos_informacoes, @id_evtpgtos_info_detpgtofl, 
                                            @id_evtpgtos_info_detpgtofl_retpgtotot, @penAlim_cpfBenef, @penAlim_dtNasctoBenef, @penAlim_nmBenefic, 
                                            @penAlim_vlrPensao, @id_cadastro_envios)";

                                                            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                            {
                                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                                cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                                cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                                cmd.Parameters.AddWithValue("@id_evtpgtos_informacoes", id_evtpgtos_informacoes);
                                                                cmd.Parameters.AddWithValue("@id_evtpgtos_info_detpgtofl", id_s_1210_evtpgtos_info_detpgtofl);
                                                                cmd.Parameters.AddWithValue("@id_evtpgtos_info_detpgtofl_retpgtotot", id_s_1210_evtpgtos_info_detpgtofl_retpgtotot);
                                                                cmd.Parameters.AddWithValue("@penAlim_cpfBenef", penAlim_cpfBenef);
                                                                cmd.Parameters.AddWithValue("@penAlim_dtNasctoBenef", penAlim_dtNasctoBenef);
                                                                cmd.Parameters.AddWithValue("@penAlim_nmBenefic", penAlim_nmBenefic);
                                                                cmd.Parameters.AddWithValue("@penAlim_vlrPensao", penAlim_vlrPensao);
                                                                cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                                try
                                                                {
                                                                    cmd.ExecuteNonQuery();
                                                                    Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_info_detpgtofl_retpgtotot_penalim com sucesso!");
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_info_detpgtofl_retpgtotot_penalim: " + ex.Message);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        var verifica_InfoPgtoParc = item_detPgtoFl.InfoPgtoParc;
                                        if (verifica_InfoPgtoParc != null && verifica_InfoPgtoParc.Count > 0)
                                        {
                                            foreach (var item_infoPgtoParc in verifica_InfoPgtoParc)
                                            {
                                                var infoPgtoParc_matricula = item_infoPgtoParc.Matricula ?? "";
                                                var infoPgtoParc_codRubr = item_infoPgtoParc.CodRubr ?? "";
                                                var infoPgtoParc_ideTabRubr = item_infoPgtoParc.IdeTabRubr ?? "";
                                                var infoPgtoParc_qtdRubr = item_infoPgtoParc.QtdRubr ?? "";
                                                var infoPgtoParc_fatorRubr = item_infoPgtoParc.FatorRubr ?? "";
                                                var infoPgtoParc_vrUnit = item_infoPgtoParc.VrUnit ?? "";
                                                var infoPgtoParc_vrRubr = item_infoPgtoParc.VrRubr ?? "";

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var sql = @"INSERT INTO s_1210_evtpgtos_info_detpgtofl_infopgtoparc (id_projeto, id_usuario, id_cad_evtpgtos, 
                                        id_evtpgtos_informacoes, id_evtpgtos_info_detpgtofl, infoPgtoParc_matricula, infoPgtoParc_codRubr, 
                                        infoPgtoParc_ideTabRubr, infoPgtoParc_qtdRubr, infoPgtoParc_fatorRubr, infoPgtoParc_vrUnit, infoPgtoParc_vrRubr)
                                        VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, @id_evtpgtos_informacoes, @id_evtpgtos_info_detpgtofl, 
                                        @infoPgtoParc_matricula, @infoPgtoParc_codRubr, @infoPgtoParc_ideTabRubr, @infoPgtoParc_qtdRubr, 
                                        @infoPgtoParc_fatorRubr, @infoPgtoParc_vrUnit, @infoPgtoParc_vrRubr)";

                                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                        cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                        cmd.Parameters.AddWithValue("@id_evtpgtos_informacoes", id_evtpgtos_informacoes);
                                                        cmd.Parameters.AddWithValue("@id_evtpgtos_info_detpgtofl", id_s_1210_evtpgtos_info_detpgtofl);
                                                        cmd.Parameters.AddWithValue("@infoPgtoParc_matricula", infoPgtoParc_matricula);
                                                        cmd.Parameters.AddWithValue("@infoPgtoParc_codRubr", infoPgtoParc_codRubr);
                                                        cmd.Parameters.AddWithValue("@infoPgtoParc_ideTabRubr", infoPgtoParc_ideTabRubr);
                                                        cmd.Parameters.AddWithValue("@infoPgtoParc_qtdRubr", infoPgtoParc_qtdRubr);
                                                        cmd.Parameters.AddWithValue("@infoPgtoParc_fatorRubr", infoPgtoParc_fatorRubr);
                                                        cmd.Parameters.AddWithValue("@infoPgtoParc_vrUnit", infoPgtoParc_vrUnit);
                                                        cmd.Parameters.AddWithValue("@infoPgtoParc_vrRubr", infoPgtoParc_vrRubr);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_info_detpgtofl_infopgtoparc com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_info_detpgtofl_infopgtoparc: " + ex.Message);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                var verifica_DetPgtoBenPr = item_infoPgto.DetPgtoBenPr;
                                if (verifica_DetPgtoBenPr != null && verifica_DetPgtoBenPr.Count > 0)
                                {
                                    foreach (var item_detPgtoBenPr in verifica_DetPgtoBenPr)
                                    {
                                        var detPgtoBenPr_perRef = item_detPgtoBenPr.PerRef ?? "";
                                        var detPgtoBenPr_ideDmDev = item_detPgtoBenPr.IdeDmDev ?? "";
                                        var detPgtoBenPr_indPgtoTt = item_detPgtoBenPr.IndPgtoTt ?? "";
                                        var detPgtoBenPr_vrLiq = item_detPgtoBenPr.VrLiq ?? "";
                                        long id_s_1210_evtpgtos_info_detpgtobenpr = 0;

                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            var sql = @"INSERT INTO s_1210_evtpgtos_info_detpgtobenpr (id_projeto, id_usuario, id_cad_evtpgtos, 
                                    id_evtpgtos_informacoes, detPgtoBenPr_perRef, detPgtoBenPr_ideDmDev, detPgtoBenPr_indPgtoTt, detPgtoBenPr_vrLiq)
                                    VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, @id_evtpgtos_informacoes, @detPgtoBenPr_perRef, 
                                    @detPgtoBenPr_ideDmDev, @detPgtoBenPr_indPgtoTt, @detPgtoBenPr_vrLiq)";

                                            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                            {
                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                cmd.Parameters.AddWithValue("@id_evtpgtos_informacoes", id_evtpgtos_informacoes);
                                                cmd.Parameters.AddWithValue("@detPgtoBenPr_perRef", detPgtoBenPr_perRef);
                                                cmd.Parameters.AddWithValue("@detPgtoBenPr_ideDmDev", detPgtoBenPr_ideDmDev);
                                                cmd.Parameters.AddWithValue("@detPgtoBenPr_indPgtoTt", detPgtoBenPr_indPgtoTt);
                                                cmd.Parameters.AddWithValue("@detPgtoBenPr_vrLiq", detPgtoBenPr_vrLiq);

                                                try
                                                {
                                                    cmd.ExecuteNonQuery();
                                                    id_s_1210_evtpgtos_info_detpgtobenpr = cmd.LastInsertedId;
                                                    Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_info_detpgtobenpr com sucesso!");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_info_detpgtobenpr: " + ex.Message);
                                                }
                                            }
                                        }

                                        var verifica_RetPgtoTot = item_detPgtoBenPr.RetPgtoTot;
                                        if (verifica_RetPgtoTot != null && verifica_DetPgtoBenPr.Count > 0)
                                        {
                                            foreach (var item_retPgtoTot in verifica_RetPgtoTot)
                                            {
                                                var retPgtoTot_codRubr = item_retPgtoTot.CodRubr ?? "";
                                                var retPgtoTot_ideTabRubr = item_retPgtoTot.IdeTabRubr ?? "";
                                                var retPgtoTot_qtdRubr = item_retPgtoTot.QtdRubr ?? "";
                                                var retPgtoTot_fatorRubr = item_retPgtoTot.FatorRubr ?? "";
                                                var retPgtoTot_vrUnit = item_retPgtoTot.VrUnit ?? "";
                                                var retPgtoTot_vrRubr = item_retPgtoTot.VrRubr ?? "";

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var sql = @"INSERT INTO s_1210_evtpgtos_info_detpgtobenpr_retpgtotot (id_projeto, id_usuario, id_cad_evtpgtos, 
                                        id_evtpgtos_informacoes, id_evtpgtos_info_detpgtobenpr, retPgtoTot_codRubr, retPgtoTot_ideTabRubr, 
                                        retPgtoTot_qtdRubr, retPgtoTot_fatorRubr, retPgtoTot_vrUnit, retPgtoTot_vrRubr)VALUES(@id_projeto, @id_usuario,
                                        @id_cad_evtpgtos, @id_evtpgtos_informacoes, @id_evtpgtos_info_detpgtobenpr, @retPgtoTot_codRubr, 
                                        @retPgtoTot_ideTabRubr, @retPgtoTot_qtdRubr, @retPgtoTot_fatorRubr, @retPgtoTot_vrUnit, @retPgtoTot_vrRubr)";

                                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                        cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                        cmd.Parameters.AddWithValue("@id_evtpgtos_informacoes", id_evtpgtos_informacoes);
                                                        cmd.Parameters.AddWithValue("@id_evtpgtos_info_detpgtobenpr", id_s_1210_evtpgtos_info_detpgtobenpr);
                                                        cmd.Parameters.AddWithValue("@retPgtoTot_codRubr", retPgtoTot_codRubr);
                                                        cmd.Parameters.AddWithValue("@retPgtoTot_ideTabRubr", retPgtoTot_ideTabRubr);
                                                        cmd.Parameters.AddWithValue("@retPgtoTot_qtdRubr", retPgtoTot_qtdRubr);
                                                        cmd.Parameters.AddWithValue("@retPgtoTot_fatorRubr", retPgtoTot_fatorRubr);
                                                        cmd.Parameters.AddWithValue("@retPgtoTot_vrUnit", retPgtoTot_vrUnit);
                                                        cmd.Parameters.AddWithValue("@retPgtoTot_vrRubr", retPgtoTot_vrRubr);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_info_detpgtobenpr_retpgtotot com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_info_detpgtobenpr_retpgtotot: " + ex.Message);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        var verifica_InfoPgtoParc = item_detPgtoBenPr.InfoPgtoParc;
                                        if (verifica_InfoPgtoParc != null && verifica_InfoPgtoParc.Count > 0)
                                        {
                                            foreach (var item_infoPgtoParc in verifica_InfoPgtoParc)
                                            {
                                                var infoPgtoParc_codRubr = item_infoPgtoParc.CodRubr ?? "";
                                                var infoPgtoParc_ideTabRubr = item_infoPgtoParc.IdeTabRubr ?? "";
                                                var infoPgtoParc_qtdRubr = item_infoPgtoParc.QtdRubr ?? "";
                                                var infoPgtoParc_fatorRubr = item_infoPgtoParc.FatorRubr ?? "";
                                                var infoPgtoParc_vrUnit = item_infoPgtoParc.VrUnit ?? "";
                                                var infoPgtoParc_vrRubr = item_infoPgtoParc.VrRubr ?? "";

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var sql = @"INSERT INTO s_1210_evtpgtos_info_detpgtobenpr_infopgtoparc (id_projeto, id_usuario, id_cad_evtpgtos,
                                        id_evtpgtos_informacoes, id_evtpgtos_info_detpgtobenpr, infoPgtoParc_codRubr, infoPgtoParc_ideTabRubr, 
                                        infoPgtoParc_qtdRubr, infoPgtoParc_fatorRubr, infoPgtoParc_vrUnit, infoPgtoParc_vrRubr)VALUES(@id_projeto, 
                                        @id_usuario, @id_cad_evtpgtos, @id_evtpgtos_informacoes, @id_evtpgtos_info_detpgtobenpr, @infoPgtoParc_codRubr,
                                        @infoPgtoParc_ideTabRubr, @infoPgtoParc_qtdRubr, @infoPgtoParc_fatorRubr, @infoPgtoParc_vrUnit, 
                                        @infoPgtoParc_vrRubr)";

                                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                        cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                        cmd.Parameters.AddWithValue("@id_evtpgtos_informacoes", id_evtpgtos_informacoes);
                                                        cmd.Parameters.AddWithValue("@id_evtpgtos_info_detpgtobenpr", id_s_1210_evtpgtos_info_detpgtobenpr);
                                                        cmd.Parameters.AddWithValue("@infoPgtoParc_codRubr", infoPgtoParc_codRubr);
                                                        cmd.Parameters.AddWithValue("@infoPgtoParc_ideTabRubr", infoPgtoParc_ideTabRubr);
                                                        cmd.Parameters.AddWithValue("@infoPgtoParc_qtdRubr", infoPgtoParc_qtdRubr);
                                                        cmd.Parameters.AddWithValue("@infoPgtoParc_fatorRubr", infoPgtoParc_fatorRubr);
                                                        cmd.Parameters.AddWithValue("@infoPgtoParc_vrUnit", infoPgtoParc_vrUnit);
                                                        cmd.Parameters.AddWithValue("@infoPgtoParc_vrRubr", infoPgtoParc_vrRubr);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_info_detpgtobenpr_infopgtoparc com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_info_detpgtobenpr_infopgtoparc: " + ex.Message);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                var verifica_DetPgtoFer = item_infoPgto.DetPgtoFer;
                                if (verifica_DetPgtoFer != null && verifica_DetPgtoFer.Count > 0)
                                {
                                    foreach (var item_detPgtoFer in verifica_DetPgtoFer)
                                    {
                                        var detPgtoFer_codCateg = item_detPgtoFer.CodCateg ?? "";
                                        var detPgtoFer_matricula = item_detPgtoFer.Matricula ?? "";
                                        var detPgtoFer_dtIniGoz = item_detPgtoFer.DtIniGoz ?? "";
                                        var detPgtoFer_qtDias = item_detPgtoFer.QtDias ?? "";
                                        var detPgtoFer_vrLiq = item_detPgtoFer.VrLiq ?? "";
                                        long id_s_1210_evtpgtos_info_detpgtofer = 0;

                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            var sql = @"INSERT INTO s_1210_evtpgtos_info_detpgtofer (id_projeto, 
                                    id_usuario, id_cad_evtpgtos, id_evtpgtos_informacoes, detPgtoFer_codCateg, detPgtoFer_matricula, 
                                    detPgtoFer_dtIniGoz, detPgtoFer_qtDias, detPgtoFer_vrLiq, id_cadastro_envios)VALUES(@id_projeto, 
                                    @id_usuario, @id_cad_evtpgtos, @id_evtpgtos_informacoes, @detPgtoFer_codCateg, @detPgtoFer_matricula, 
                                    @detPgtoFer_dtIniGoz, @detPgtoFer_qtDias, @detPgtoFer_vrLiq, @id_cadastro_envios)";

                                            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                            {
                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                cmd.Parameters.AddWithValue("@id_evtpgtos_informacoes", id_evtpgtos_informacoes);
                                                cmd.Parameters.AddWithValue("@detPgtoFer_codCateg", detPgtoFer_codCateg);
                                                cmd.Parameters.AddWithValue("@detPgtoFer_matricula", detPgtoFer_matricula);
                                                cmd.Parameters.AddWithValue("@detPgtoFer_dtIniGoz", detPgtoFer_dtIniGoz);
                                                cmd.Parameters.AddWithValue("@detPgtoFer_qtDias", detPgtoFer_qtDias);
                                                cmd.Parameters.AddWithValue("@detPgtoFer_vrLiq", detPgtoFer_vrLiq);
                                                cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                try
                                                {
                                                    cmd.ExecuteNonQuery();
                                                    id_s_1210_evtpgtos_info_detpgtofer = cmd.LastInsertedId;
                                                    Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_info_detpgtofer com sucesso!");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_info_detpgtofer: " + ex.Message);
                                                }
                                            }
                                        }

                                        var verifica_DetRubrFer = item_detPgtoFer.DetRubrFer;
                                        if (verifica_DetRubrFer != null && verifica_DetRubrFer.Count > 0)
                                        {
                                            foreach (var item_detRubrFer in verifica_DetRubrFer)
                                            {
                                                var detRubrFer_codRubr = item_detRubrFer.CodRubr ?? "";
                                                var detRubrFer_ideTabRubr = item_detRubrFer.IdeTabRubr ?? "";
                                                var detRubrFer_qtdRubr = item_detRubrFer.QtdRubr ?? "";
                                                var detRubrFer_fatorRubr = item_detRubrFer.FatorRubr ?? "";
                                                var detRubrFer_vrUnit = item_detRubrFer.VrUnit ?? "";
                                                var detRubrFer_vrRubr = item_detRubrFer.VrRubr ?? "";
                                                long id_s_1210_evtpgtos_info_detpgtofer_detrubrfer = 0;

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var sql = @"INSERT INTO s_1210_evtpgtos_info_detpgtofer_detrubrfer (id_projeto, 
                                        id_usuario, id_cad_evtpgtos, id_evtpgtos_informacoes, id_evtpgtos_info_detpgtofer, detRubrFer_codRubr, 
                                        detRubrFer_ideTabRubr, detRubrFer_qtdRubr, detRubrFer_fatorRubr, detRubrFer_vrUnit, 
                                        detRubrFer_vrRubr, id_cadastro_envios)VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, @id_evtpgtos_informacoes, 
                                        @id_evtpgtos_info_detpgtofer, @detRubrFer_codRubr, @detRubrFer_ideTabRubr, @detRubrFer_qtdRubr, @detRubrFer_fatorRubr, 
                                        @detRubrFer_vrUnit, @detRubrFer_vrRubr, @id_cadastro_envios)";

                                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                        cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                        cmd.Parameters.AddWithValue("@id_evtpgtos_informacoes", id_evtpgtos_informacoes);
                                                        cmd.Parameters.AddWithValue("@id_evtpgtos_info_detpgtofer", id_s_1210_evtpgtos_info_detpgtofer);
                                                        cmd.Parameters.AddWithValue("@detRubrFer_codRubr", detRubrFer_codRubr);
                                                        cmd.Parameters.AddWithValue("@detRubrFer_ideTabRubr", detRubrFer_ideTabRubr);
                                                        cmd.Parameters.AddWithValue("@detRubrFer_qtdRubr", detRubrFer_qtdRubr);
                                                        cmd.Parameters.AddWithValue("@detRubrFer_fatorRubr", detRubrFer_fatorRubr);
                                                        cmd.Parameters.AddWithValue("@detRubrFer_vrUnit", detRubrFer_vrUnit);
                                                        cmd.Parameters.AddWithValue("@detRubrFer_vrRubr", detRubrFer_vrRubr);
                                                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            id_s_1210_evtpgtos_info_detpgtofer_detrubrfer = cmd.LastInsertedId;
                                                            Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_info_detpgtofer_detrubrfer com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_info_detpgtofer_detrubrfer: " + ex.Message);
                                                        }
                                                    }
                                                }

                                                var verifica_PenAlim = item_detRubrFer.PenAlim;
                                                if (verifica_PenAlim != null && verifica_PenAlim.Count > 0)
                                                {
                                                    foreach (var item_penAlim in verifica_PenAlim)
                                                    {
                                                        var penAlim_cpfBenef = item_penAlim.CpfBenef ?? "";
                                                        var penAlim_dtNasctoBenef = item_penAlim.DtNasctoBenef ?? "";
                                                        var penAlim_nmBenefic = item_penAlim.NmBenefic ?? "";
                                                        var penAlim_vlrPensao = item_penAlim.VlrPensao ?? "";

                                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                        {
                                                            connection.Open();
                                                            var sql = @"INSERT INTO s_1210_evtpgtos_info_detpgtofer_detrubrfer_penalim (id_projeto, id_usuario, 
                                            id_cad_evtpgtos, id_evtpgtos_informacoes, id_evtpgtos_info_detpgtofer, id_evtpgtos_info_detpgtofer_detrubrfer,
                                            penAlim_cpfBenef, penAlim_dtNasctoBenef, penAlim_nmBenefic, penAlim_vlrPensao)VALUES(@id_projeto, 
                                            @id_usuario, @id_cad_evtpgtos, @id_evtpgtos_informacoes, @id_evtpgtos_info_detpgtofer, 
                                            @id_evtpgtos_info_detpgtofer_detrubrfer, @penAlim_cpfBenef, @penAlim_dtNasctoBenef, @penAlim_nmBenefic, 
                                            @penAlim_vlrPensao)";

                                                            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                            {
                                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                                cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                                cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                                cmd.Parameters.AddWithValue("@id_evtpgtos_informacoes", id_evtpgtos_informacoes);
                                                                cmd.Parameters.AddWithValue("@id_evtpgtos_info_detpgtofer", id_s_1210_evtpgtos_info_detpgtofer);
                                                                cmd.Parameters.AddWithValue("@id_evtpgtos_info_detpgtofer_detrubrfer", id_s_1210_evtpgtos_info_detpgtofer_detrubrfer);
                                                                cmd.Parameters.AddWithValue("@penAlim_cpfBenef", penAlim_cpfBenef);
                                                                cmd.Parameters.AddWithValue("@penAlim_dtNasctoBenef", penAlim_dtNasctoBenef);
                                                                cmd.Parameters.AddWithValue("@penAlim_nmBenefic", penAlim_nmBenefic);
                                                                cmd.Parameters.AddWithValue("@penAlim_vlrPensao", penAlim_vlrPensao);

                                                                try
                                                                {
                                                                    cmd.ExecuteNonQuery();
                                                                    Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_info_detpgtofer_detrubrfer_penalim com sucesso!");
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_info_detpgtofer_detrubrfer_penalim: " + ex.Message);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                var verifica_DetPgtoAnt = item_infoPgto.DetPgtoAnt;
                                if (verifica_DetPgtoAnt != null && verifica_DetPgtoAnt.Count > 0)
                                {
                                    foreach (var item_detPgtoAnt in verifica_DetPgtoAnt)
                                    {
                                        var detPgtoAnt_codCateg = item_detPgtoAnt.CodCateg ?? "";
                                        long id_s_1210_evtpgtos_info_detpgtoant = 0;

                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            var sql = @"INSERT INTO s_1210_evtpgtos_info_detpgtoant (id_projeto, id_usuario, id_cad_evtpgtos, 
                                    id_evtpgtos_informacoes, detPgtoAnt_codCateg)VALUES(@id_projeto, @id_usuario, @id_cad_evtpgtos, 
                                    @id_evtpgtos_informacoes, @detPgtoAnt_codCateg)";

                                            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                            {
                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                cmd.Parameters.AddWithValue("@id_evtpgtos_informacoes", id_evtpgtos_informacoes);
                                                cmd.Parameters.AddWithValue("@detPgtoAnt_codCateg", detPgtoAnt_codCateg);

                                                try
                                                {
                                                    cmd.ExecuteNonQuery();
                                                    id_s_1210_evtpgtos_info_detpgtoant = cmd.LastInsertedId;
                                                    Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_info_detpgtoant com sucesso!");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_info_detpgtoant: " + ex.Message);
                                                }
                                            }
                                        }

                                        var verifica_InfoPgtoAnt = item_detPgtoAnt.InfoPgtoAnt;
                                        if (verifica_InfoPgtoAnt != null && verifica_InfoPgtoAnt.Count > 0)
                                        {
                                            foreach (var item_infoPgtoAnt in verifica_InfoPgtoAnt)
                                            {
                                                var infoPgtoAnt_tpBcIRRF = item_infoPgtoAnt.TpBcIRRF ?? "";
                                                var infoPgtoAnt_vrBcIRRF = item_infoPgtoAnt.VrBcIRRF ?? "";

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var sql = @"INSERT INTO s_1210_evtpgtos_info_detpgtoant_infopgtoant (id_projeto, id_usuario, id_cad_evtpgtos, 
                                        id_evtpgtos_informacoes, id_evtpgtos_info_detpgtoant, infoPgtoAnt_tpBcIRRF, infoPgtoAnt_vrBcIRRF)VALUES(@id_projeto,
                                        @id_usuario, @id_cad_evtpgtos, @id_evtpgtos_informacoes, @id_evtpgtos_info_detpgtoant, @infoPgtoAnt_tpBcIRRF, 
                                        @infoPgtoAnt_vrBcIRRF)";

                                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                        cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                        cmd.Parameters.AddWithValue("@id_evtpgtos_informacoes", id_evtpgtos_informacoes);
                                                        cmd.Parameters.AddWithValue("@id_evtpgtos_info_detpgtoant", id_s_1210_evtpgtos_info_detpgtoant);
                                                        cmd.Parameters.AddWithValue("@infoPgtoAnt_tpBcIRRF", infoPgtoAnt_tpBcIRRF);
                                                        cmd.Parameters.AddWithValue("@infoPgtoAnt_vrBcIRRF", infoPgtoAnt_vrBcIRRF);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_info_detpgtoant_infopgtoant com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_info_detpgtoant_infopgtoant: " + ex.Message);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                var verifica_IdePgtoExt = item_infoPgto.IdePgtoExt;
                                if (verifica_IdePgtoExt != null && verifica_IdePgtoExt.Count > 0)
                                {
                                    foreach (var item_idePgtoExt in verifica_IdePgtoExt)
                                    {
                                        var idePgtoExt_codPais = item_idePgtoExt.IdePais?.CodPais ?? "";
                                        var idePgtoExt_indNIF = item_idePgtoExt.IdePais?.IndNIF ?? "";
                                        var idePgtoExt_nifBenef = item_idePgtoExt.IdePais?.NifBenef ?? "";
                                        var idePgtoExt_dscLograd = item_idePgtoExt.EndExt?.DscLograd ?? "";
                                        var idePgtoExt_nrLograd = item_idePgtoExt.EndExt?.NrLograd ?? "";
                                        var idePgtoExt_complem = item_idePgtoExt.EndExt?.Complem ?? "";
                                        var idePgtoExt_bairro = item_idePgtoExt.EndExt?.Bairro ?? "";
                                        var idePgtoExt_nmCid = item_idePgtoExt.EndExt?.NmCid ?? "";
                                        var idePgtoExt_codPostal = item_idePgtoExt.EndExt?.CodPostal ?? "";

                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            var sql = @"INSERT INTO s_1210_evtpgtos_info_idepgtoext (id_projeto, id_usuario, id_cad_evtpgtos, 
                                    id_evtpgtos_informacoes, idePgtoExt_codPais, idePgtoExt_indNIF, idePgtoExt_nifBenef, idePgtoExt_dscLograd, 
                                    idePgtoExt_nrLograd, idePgtoExt_complem, idePgtoExt_bairro, idePgtoExt_nmCid, idePgtoExt_codPostal)VALUES(
                                    @id_projeto, @id_usuario, @id_cad_evtpgtos, @id_evtpgtos_informacoes, @idePgtoExt_codPais, @idePgtoExt_indNIF, 
                                    @idePgtoExt_nifBenef, @idePgtoExt_dscLograd, @idePgtoExt_nrLograd, @idePgtoExt_complem, @idePgtoExt_bairro, 
                                    @idePgtoExt_nmCid, @idePgtoExt_codPostal)";

                                            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                            {
                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                cmd.Parameters.AddWithValue("@id_usuario", verificarProjetos.Id_usuario);
                                                cmd.Parameters.AddWithValue("@id_cad_evtpgtos", id_cad_evtpgtos);
                                                cmd.Parameters.AddWithValue("@id_evtpgtos_informacoes", id_evtpgtos_informacoes);
                                                cmd.Parameters.AddWithValue("@idePgtoExt_codPais", idePgtoExt_codPais);
                                                cmd.Parameters.AddWithValue("@idePgtoExt_indNIF", idePgtoExt_indNIF);
                                                cmd.Parameters.AddWithValue("@idePgtoExt_nifBenef", idePgtoExt_nifBenef);
                                                cmd.Parameters.AddWithValue("@idePgtoExt_dscLograd", idePgtoExt_dscLograd);
                                                cmd.Parameters.AddWithValue("@idePgtoExt_nrLograd", idePgtoExt_nrLograd);
                                                cmd.Parameters.AddWithValue("@idePgtoExt_complem", idePgtoExt_complem);
                                                cmd.Parameters.AddWithValue("@idePgtoExt_bairro", idePgtoExt_bairro);
                                                cmd.Parameters.AddWithValue("@idePgtoExt_nmCid", idePgtoExt_nmCid);
                                                cmd.Parameters.AddWithValue("@idePgtoExt_codPostal", idePgtoExt_codPostal);

                                                try
                                                {
                                                    cmd.ExecuteNonQuery();
                                                    Console.WriteLine("EvtPgtos inserido na tabela s_1210_evtpgtos_info_idepgtoext com sucesso!");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Erro ao inserir EvtPgtos na tabela s_1210_evtpgtos_info_idepgtoext: " + ex.Message);
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