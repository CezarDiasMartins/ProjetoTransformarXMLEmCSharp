using MySql.Data.MySqlClient;
using System.Globalization;
using TransformarXmlEmCSharpESalvarNoBanco.Models.EvtBasesTrab;

namespace TransformarXmlEmCSharpESalvarNoBanco.Repositories.EvtBasesTrab
{
    public class EvtBasesTrabRepository
    {
        public void InsertEvtBasesTrab(string connectionString, ESocialEvtBasesTrab eSocialEvtBasesTrab, string arquivo, int id_cadastro_envios)
        {
            var url = $"XmlImportados/{id_cadastro_envios}/{arquivo}";
            string[] partes = url.Split('/');
            string caminho_arquivo = partes[partes.Length - 1];

            var evtBasesTrab_id = eSocialEvtBasesTrab?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtBasesTrab?.Id;
            var processamento_recibo_nrRecibo = eSocialEvtBasesTrab?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtBasesTrab?.IdeEvento?.NrRecArqBase;
            var ideEmpregador_nrInsc = eSocialEvtBasesTrab?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtBasesTrab?.IdeEmpregador?.NrInsc;
            var dadosrecepcaolote_protocoloenvio = "nao aplicado ao retorno"; //eSocialEvtBasesTrab?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtBasesTrab?.IdeEvento.NrRecArqBase; - COMENTÁRIO DO PHP
            var ideEvento_perApur = eSocialEvtBasesTrab?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtBasesTrab?.IdeEvento?.PerApur;
            var ideEvento_indApuracao = eSocialEvtBasesTrab?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtBasesTrab?.IdeEvento?.IndApuracao;
            var ideTrabalhador_cpfTrab = eSocialEvtBasesTrab?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtBasesTrab?.IdeTrabalhador?.CpfTrab;
            var infoCpCalc_tpCR = eSocialEvtBasesTrab?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtBasesTrab?.InfoCpCalc?.TpCR;
            var infoCpCalc_vrCpSeg = Convert.ToDecimal(eSocialEvtBasesTrab?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtBasesTrab?.InfoCpCalc?.VrCpSeg);
            var infoCpCalc_vrDescSeg = Convert.ToDecimal(eSocialEvtBasesTrab?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtBasesTrab?.InfoCpCalc?.VrDescSeg);
            //var processamento_dhProcessamento = (string)body[0].ConsultarLoteEventosResponse.ConsultarLoteEventosResult.ESocial.RetornoProcessamentoLoteEventos.RetornoEventos.Evento.RetornoEvento.ESocial.RetornoEvento.Processamento.DhProcessamento; - COMENTÁRIO DO PHP
            var processamento_dhProcessamento = eSocialEvtBasesTrab?.RetornoProcessamentoDownload?.Recibo?.ESocial?.RetornoEvento?.Processamento?.DhProcessamento;

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
                var id_s_1200_evtremun = 0; // Padrão zero pois não sabemos qual remuração se refere ainda - COMENTÁRIO DO PHP
                long id_cadastra_5001 = 0;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var sql = @"INSERT INTO s_5001_original_cadastro (id_projeto, id_cad_evtremun, 
        cnpj_estabelecimento, ideEvento_perApur, ideTrabalhador_cpfTrab, processamento_recibo_nrRecibo, 
        dadosrecepcaolote_protocoloenvio, processamento_dhProcessamento, infoCpCalc_tpCR, infoCpCalc_vrCpSeg, 
        infoCpCalc_vrDescSeg, caminho_arquivo, evtBasesTrab_id, id_cadastro_envios)VALUES(@id_projeto, @id_cad_evtremun, 
        @cnpj_estabelecimento, @ideEvento_perApur, @ideTrabalhador_cpfTrab, @processamento_recibo_nrRecibo, 
        @dadosrecepcaolote_protocoloenvio, @processamento_dhProcessamento, @infoCpCalc_tpCR, @infoCpCalc_vrCpSeg, 
        @infoCpCalc_vrDescSeg, @caminho_arquivo, @evtBasesTrab_id, @id_cadastro_envios)";

                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                        cmd.Parameters.AddWithValue("@id_cad_evtremun", id_s_1200_evtremun);
                        cmd.Parameters.AddWithValue("@cnpj_estabelecimento", cnpjcpf ?? "");
                        cmd.Parameters.AddWithValue("@ideEvento_perApur", ideEvento_perApur ?? "");
                        cmd.Parameters.AddWithValue("@ideTrabalhador_cpfTrab", ideTrabalhador_cpfTrab ?? "");
                        cmd.Parameters.AddWithValue("@processamento_recibo_nrRecibo", processamento_recibo_nrRecibo ?? "");
                        cmd.Parameters.AddWithValue("@dadosrecepcaolote_protocoloenvio", dadosrecepcaolote_protocoloenvio ?? "");
                        cmd.Parameters.AddWithValue("@infoCpCalc_tpCR", infoCpCalc_tpCR ?? "");
                        cmd.Parameters.AddWithValue("@infoCpCalc_vrCpSeg", infoCpCalc_vrCpSeg);
                        cmd.Parameters.AddWithValue("@infoCpCalc_vrDescSeg", infoCpCalc_vrDescSeg);
                        cmd.Parameters.AddWithValue("@caminho_arquivo", caminho_arquivo ?? "");
                        cmd.Parameters.AddWithValue("@evtBasesTrab_id", evtBasesTrab_id ?? "");
                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                        if (processamento_dhProcessamento != null)
                        {
                            // Convertendo recepcao_processamento_dhProcessamento para o formato "yyyy-MM-dd HH:mm:ss" ou "yyyy-MM-dd HH:mm:sss"
                            int indexOfDot = processamento_dhProcessamento.LastIndexOf('.');
                            if (indexOfDot != -1 && indexOfDot + 1 < processamento_dhProcessamento.Length)
                            {
                                string millisecondsPart = processamento_dhProcessamento.Substring(indexOfDot + 1);

                                if (millisecondsPart.Length == 2)
                                {
                                    DateTime recepcaoProcessamentoDhProcessamento = DateTime.ParseExact(processamento_dhProcessamento, "yyyy-MM-ddTHH:mm:ss.ff", CultureInfo.InvariantCulture);
                                    cmd.Parameters.AddWithValue("@processamento_dhProcessamento", recepcaoProcessamentoDhProcessamento.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) ?? "");
                                }
                                if (millisecondsPart.Length == 3)
                                {
                                    DateTime recepcaoProcessamentoDhProcessamento = DateTime.ParseExact(processamento_dhProcessamento, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
                                    cmd.Parameters.AddWithValue("@processamento_dhProcessamento", recepcaoProcessamentoDhProcessamento.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) ?? "");
                                }
                            }
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@processamento_dhProcessamento", "");
                        }

                        try
                        {
                            cmd.ExecuteNonQuery();
                            id_cadastra_5001 = cmd.LastInsertedId;
                            Console.WriteLine($"EvtBasesTrab({id_cadastra_5001}) inserido na tabela s_5001_original_cadastro com sucesso!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Erro ao inserir EvtBasesTrab na tabela s_5001_original_cadastro: " + ex.Message);
                        }
                    }
                }

                var verifica_infoCp = eSocialEvtBasesTrab?.RetornoProcessamentoDownload?.Evento?.ESocial?.EvtBasesTrab?.InfoCp;
                if (verifica_infoCp != null && verifica_infoCp.Count > 0)
                {
                    foreach (var item_infoCp in verifica_infoCp)
                    {
                        var infoCp_classTrib = item_infoCp.ClassTrib ?? "";
                        long id_cadastra_5001_infoCp = 0;

                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                        {
                            connection.Open();
                            var sql = @"INSERT INTO s_5001_original_infocp (id_projeto, id_original_cadastro, infocp, 
                                                id_cadastro_envios)VALUES(@id_projeto, @id_original_cadastro, @infocp, @id_cadastro_envios)";

                            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                            {
                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                cmd.Parameters.AddWithValue("@id_original_cadastro", id_cadastra_5001);
                                cmd.Parameters.AddWithValue("@infocp", infoCp_classTrib);
                                cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                try
                                {
                                    cmd.ExecuteNonQuery();
                                    id_cadastra_5001_infoCp = cmd.LastInsertedId;
                                    Console.WriteLine($"EvtBasesTrab({id_cadastra_5001_infoCp}) inserido na tabela s_5001_original_infocp com sucesso!");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Erro ao inserir EvtBasesTrab na tabela s_5001_original_infocp: " + ex.Message);
                                }
                            }
                        }

                        var verifica_infoCp_ideEstabLot = item_infoCp.IdeEstabLot;
                        if (verifica_infoCp_ideEstabLot != null && verifica_infoCp_ideEstabLot.Count > 0)
                        {
                            foreach (var item_ideEstabLot in verifica_infoCp_ideEstabLot)
                            {
                                var ideEstabLot_tpInsc = item_ideEstabLot.TpInsc ?? "";
                                var ideEstabLot_nrInsc = item_ideEstabLot.NrInsc ?? "";
                                var ideEstabLot_codLotacao = item_ideEstabLot.CodLotacao ?? "";
                                long id_cadastra_5001_ideEstabLot = 0;

                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                {
                                    connection.Open();
                                    var sql = @"INSERT INTO s_5001_original_ideestablot (id_projeto, 
                        id_original_cadastro, id_original_infocp, ideEstabLot_tpInsc, ideEstabLot_nrInsc, 
                        ideEstabLot_codLotacao, id_cadastro_envios)VALUES(@id_projeto, @id_original_cadastro, @id_original_infocp, 
                        @ideEstabLot_tpInsc, @ideEstabLot_nrInsc, @ideEstabLot_codLotacao, @id_cadastro_envios)";

                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                    {
                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                        cmd.Parameters.AddWithValue("@id_original_cadastro", id_cadastra_5001);
                                        cmd.Parameters.AddWithValue("@id_original_infocp", id_cadastra_5001_infoCp);
                                        cmd.Parameters.AddWithValue("@ideEstabLot_tpInsc", ideEstabLot_tpInsc);
                                        cmd.Parameters.AddWithValue("@ideEstabLot_nrInsc", ideEstabLot_nrInsc);
                                        cmd.Parameters.AddWithValue("@ideEstabLot_codLotacao", ideEstabLot_codLotacao);
                                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                            id_cadastra_5001_ideEstabLot = cmd.LastInsertedId;
                                            Console.WriteLine($"EvtBasesTrab({id_cadastra_5001_ideEstabLot}) inserido na tabela s_5001_original_ideestablot com sucesso!");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Erro ao inserir EvtBasesTrab na tabela s_5001_original_ideestablot: " + ex.Message);
                                        }
                                    }
                                }

                                var verifica_ideEstabLot_infoCategIncid = item_ideEstabLot.InfoCategIncid;
                                if (verifica_ideEstabLot_infoCategIncid != null && verifica_ideEstabLot_infoCategIncid.Count > 0)
                                {
                                    foreach (var item_infoCategIncid in verifica_ideEstabLot_infoCategIncid)
                                    {
                                        var infoCategIncid_matricula = item_infoCategIncid.Matricula ?? "";
                                        var infoCategIncid_codCateg = item_infoCategIncid.CodCateg ?? "";
                                        long id_cadastra_5001_infoCategIncid = 0;

                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            var sql = @"INSERT INTO s_5001_original_infocategincid (id_projeto, 
                                id_original_cadastro, id_original_infocp, id_original_ideestablot, infoCategIncid_matricula, 
                                infoCategIncid_codCateg, id_cadastro_envios)VALUES(@id_projeto, @id_original_cadastro, @id_original_infocp, 
                                @id_original_ideestablot, @infoCategIncid_matricula, @infoCategIncid_codCateg, @id_cadastro_envios)";

                                            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                            {
                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                cmd.Parameters.AddWithValue("@id_original_cadastro", id_cadastra_5001);
                                                cmd.Parameters.AddWithValue("@id_original_infocp", id_cadastra_5001_infoCp);
                                                cmd.Parameters.AddWithValue("@id_original_ideestablot", id_cadastra_5001_ideEstabLot);
                                                cmd.Parameters.AddWithValue("@infoCategIncid_matricula", infoCategIncid_matricula);
                                                cmd.Parameters.AddWithValue("@infoCategIncid_codCateg", infoCategIncid_codCateg);
                                                cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                try
                                                {
                                                    cmd.ExecuteNonQuery();
                                                    id_cadastra_5001_infoCategIncid = cmd.LastInsertedId;
                                                    Console.WriteLine($"EvtBasesTrab({id_cadastra_5001_infoCategIncid}) inserido na tabela s_5001_original_infocategincid com sucesso!");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Erro ao inserir EvtBasesTrab na tabela s_5001_original_infocategincid: " + ex.Message);
                                                }
                                            }
                                        }

                                        var verifica_infoCategIncid_infoBaseCS = item_infoCategIncid.InfoBaseCS;
                                        if (verifica_infoCategIncid_infoBaseCS != null && verifica_infoCategIncid_infoBaseCS.Count > 0)
                                        {
                                            foreach (var item_bases in verifica_infoCategIncid_infoBaseCS)
                                            {
                                                var infoBaseCS_ind13 = item_bases.Ind13 ?? "";
                                                var infoBaseCS_tpValor = item_bases.TpValor ?? "";
                                                var infoBaseCS_valor = Convert.ToDecimal(item_bases.Valor);
                                                long id_cadastra_5001_infobasecs = 0;

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var sql = @"INSERT INTO s_5001_original_infobasecs (id_projeto, 
                                        id_original_cadastro, id_original_infocp, id_original_ideestablot, id_original_infocategincid, 
                                        infoBaseCS_ind13, infoBaseCS_tpValor, infoBaseCS_valor, id_cadastro_envios)VALUES(@id_projeto, @id_original_cadastro, 
                                        @id_original_infocp, @id_original_ideestablot, @id_original_infocategincid, @infoBaseCS_ind13, 
                                        @infoBaseCS_tpValor, @infoBaseCS_valor, @id_cadastro_envios)";

                                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_original_cadastro", id_cadastra_5001);
                                                        cmd.Parameters.AddWithValue("@id_original_infocp", id_cadastra_5001_infoCp);
                                                        cmd.Parameters.AddWithValue("@id_original_ideestablot", id_cadastra_5001_ideEstabLot);
                                                        cmd.Parameters.AddWithValue("@id_original_infocategincid", id_cadastra_5001_infoCategIncid);
                                                        cmd.Parameters.AddWithValue("@infoBaseCS_ind13", infoBaseCS_ind13);
                                                        cmd.Parameters.AddWithValue("@infoBaseCS_tpValor", infoBaseCS_tpValor);
                                                        cmd.Parameters.AddWithValue("@infoBaseCS_valor", infoBaseCS_valor);
                                                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            id_cadastra_5001_infobasecs = cmd.LastInsertedId;
                                                            Console.WriteLine($"EvtBasesTrab({id_cadastra_5001_infobasecs}) inserido na tabela s_5001_original_infobasecs com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtBasesTrab na tabela s_5001_original_infobasecs: " + ex.Message);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        var verifica_infoCategIncid_infoPerRef = item_infoCategIncid.InfoPerRef;
                                        if (verifica_infoCategIncid_infoPerRef != null && verifica_infoCategIncid_infoPerRef.Count > 0)
                                        {
                                            foreach (var item_infoPerRef in verifica_infoCategIncid_infoPerRef)
                                            {
                                                var infoPerRef_perRef = item_infoPerRef.PerRef ?? "";
                                                long id_cadastra_5001_infoperref = 0;

                                                using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                {
                                                    connection.Open();
                                                    var sql = @"INSERT INTO s_5001_original_infoperref (id_projeto, 
                                        id_original_cadastro, id_original_infocp, id_original_ideestablot, id_original_infocategincid, 
                                        infoPerRef_perRef, id_cadastro_envios)VALUES(@id_projeto, @id_original_cadastro, @id_original_infocp, 
                                        @id_original_ideestablot, @id_original_infocategincid, @infoPerRef_perRef, @id_cadastro_envios)";

                                                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                    {
                                                        cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                        cmd.Parameters.AddWithValue("@id_original_cadastro", id_cadastra_5001);
                                                        cmd.Parameters.AddWithValue("@id_original_infocp", id_cadastra_5001_infoCp);
                                                        cmd.Parameters.AddWithValue("@id_original_ideestablot", id_cadastra_5001_ideEstabLot);
                                                        cmd.Parameters.AddWithValue("@id_original_infocategincid", id_cadastra_5001_infoCategIncid);
                                                        cmd.Parameters.AddWithValue("@infoPerRef_perRef", infoPerRef_perRef);
                                                        cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                        try
                                                        {
                                                            cmd.ExecuteNonQuery();
                                                            id_cadastra_5001_infoperref = cmd.LastInsertedId;
                                                            Console.WriteLine($"EvtBasesTrab({id_cadastra_5001_infoperref}) inserido na tabela s_5001_original_infoperref com sucesso!");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Erro ao inserir EvtBasesTrab na tabela s_5001_original_infoperref: " + ex.Message);
                                                        }
                                                    }
                                                }

                                                var verifica_infoPerRef_detInfoPerRef = item_infoPerRef.DetInfoPerRef;
                                                if (verifica_infoPerRef_detInfoPerRef != null && verifica_infoPerRef_detInfoPerRef.Count > 0)
                                                {
                                                    foreach (var item_detInfoPerRef in verifica_infoPerRef_detInfoPerRef)
                                                    {
                                                        var detInfoPerRef_ind13 = item_detInfoPerRef.Ind13 ?? "";
                                                        var detInfoPerRef_tpVrPerRef = item_detInfoPerRef.TpVrPerRef ?? "";
                                                        var detInfoPerRef_vrPerRef = Convert.ToDecimal(item_detInfoPerRef.VrPerRef);
                                                        long id_cadastra_5001_detinfoperref = 0;

                                                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                                                        {
                                                            connection.Open();
                                                            var sql = @"INSERT INTO s_5001_original_detinfoperref (id_projeto, id_original_cadastro, 
                                                    id_original_infocp, id_original_ideestablot, id_original_infocategincid, id_original_infoperref, detInfoPerRef_ind13, 
                                                    detInfoPerRef_tpVrPerRef, detInfoPerRef_vrPerRef, id_cadastro_envios)VALUES(@id_projeto, @id_original_cadastro, @id_original_infocp, 
                                                    @id_original_ideestablot, @id_original_infocategincid, @id_original_infoperref, @detInfoPerRef_ind13, 
                                                    @detInfoPerRef_tpVrPerRef, @detInfoPerRef_vrPerRef, @id_cadastro_envios)";

                                                            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                                                            {
                                                                cmd.Parameters.AddWithValue("@id_projeto", verificarProjetos.Id_projeto);
                                                                cmd.Parameters.AddWithValue("@id_original_cadastro", id_cadastra_5001);
                                                                cmd.Parameters.AddWithValue("@id_original_infocp", id_cadastra_5001_infoCp);
                                                                cmd.Parameters.AddWithValue("@id_original_ideestablot", id_cadastra_5001_ideEstabLot);
                                                                cmd.Parameters.AddWithValue("@id_original_infocategincid", id_cadastra_5001_infoCategIncid);
                                                                cmd.Parameters.AddWithValue("@id_original_infoperref", id_cadastra_5001_infoperref);
                                                                cmd.Parameters.AddWithValue("@detInfoPerRef_ind13", detInfoPerRef_ind13);
                                                                cmd.Parameters.AddWithValue("@detInfoPerRef_tpVrPerRef", detInfoPerRef_tpVrPerRef);
                                                                cmd.Parameters.AddWithValue("@detInfoPerRef_vrPerRef", detInfoPerRef_vrPerRef);
                                                                cmd.Parameters.AddWithValue("@id_cadastro_envios", id_cadastro_envios);

                                                                try
                                                                {
                                                                    cmd.ExecuteNonQuery();
                                                                    id_cadastra_5001_detinfoperref = cmd.LastInsertedId;
                                                                    Console.WriteLine($"EvtBasesTrab({id_cadastra_5001_detinfoperref}) inserido na tabela s_5001_original_detinfoperref com sucesso!");
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    Console.WriteLine("Erro ao inserir EvtBasesTrab na tabela s_5001_original_detinfoperref: " + ex.Message);
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
    }
}