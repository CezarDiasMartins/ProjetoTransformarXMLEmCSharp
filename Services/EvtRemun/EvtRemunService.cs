using System.Xml.Serialization;
using System.Xml;
using TransformarXmlEmCSharpESalvarNoBanco.Models.EvtRemun;

namespace TransformarXmlEmCSharpESalvarNoBanco.Services.EvtRemun
{
    public class EvtRemunService
    {
        public ESocialEvtRemun DesserializarEvtRemun(string arquivo, string addNamespaceEvento, string addNamespaceRecibo)
        {
            string xmlContent;
            using (StreamReader stream = new StreamReader(arquivo))
            {
                xmlContent = stream.ReadToEnd();
            }

            // Carregar o XML em um XmlDocument para obter o namespace
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            // Ajustar o namespace dinamicamente
            var namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
            namespaceManager.AddNamespace("ns", "http://www.esocial.gov.br/schema/download/retornoProcessamento/v1_0_0");
            namespaceManager.AddNamespace("eventoNs", addNamespaceEvento);
            namespaceManager.AddNamespace("reciboNs", addNamespaceRecibo);

            // Encontrar os nós de evento e recibo
            var eventoNode = xmlDoc.SelectSingleNode("//ns:retornoProcessamentoDownload/ns:evento/eventoNs:eSocial", namespaceManager);
            var reciboNode = xmlDoc.SelectSingleNode("//ns:retornoProcessamentoDownload/ns:recibo/reciboNs:eSocial", namespaceManager);

            // Capturar os namespaces dos nós encontrados
            string eventoNamespace = eventoNode?.NamespaceURI;
            string reciboNamespace = reciboNode?.NamespaceURI;

            // Desserializar o XML para o objeto
            ESocialEvtRemun resultado;
            using (StringReader stringReader = new StringReader(xmlContent))
            {
                XmlSerializer serializador = new XmlSerializer(typeof(ESocialEvtRemun));
                resultado = (ESocialEvtRemun)serializador.Deserialize(stringReader);
            }

            // Ajustar os namespaces dinamicamente
            if (resultado?.RetornoProcessamentoDownload?.Evento?.ESocial != null)
            {
                resultado.RetornoProcessamentoDownload.Evento.ESocial.Namespace = eventoNamespace;
            }

            if (resultado?.RetornoProcessamentoDownload?.Recibo?.ESocial != null)
            {
                resultado.RetornoProcessamentoDownload.Recibo.ESocial.Namespace = reciboNamespace;
            }

            Console.WriteLine("XML de EvtRemun desserializado em classes C#!");
            return resultado;
        }
    }
}