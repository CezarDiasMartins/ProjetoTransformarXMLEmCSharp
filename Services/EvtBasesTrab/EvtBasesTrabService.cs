using System.Xml.Serialization;
using System.Xml;
using TransformarXmlEmCSharpESalvarNoBanco.Models.EvtBasesTrab;

namespace TransformarXmlEmCSharpESalvarNoBanco.Services.EvtBasesTrab
{
    public class EvtBasesTrabService
    {
        public ESocialEvtBasesTrab DesserializarEvtDeslig(string arquivo, string addNamespaceEvento, string addNamespaceRecibo)
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
            if (addNamespaceRecibo != null)
                namespaceManager.AddNamespace("reciboNs", addNamespaceRecibo);

            // Encontrar os nós de evento e recibo
            var eventoNode = xmlDoc.SelectSingleNode("//ns:retornoProcessamentoDownload/ns:evento/eventoNs:eSocial", namespaceManager);
            XmlNode reciboNode = null;
            if (addNamespaceRecibo != null)
                reciboNode = xmlDoc.SelectSingleNode("//ns:retornoProcessamentoDownload/ns:recibo/reciboNs:eSocial", namespaceManager);

            // Capturar os namespaces dos nós encontrados
            string eventoNamespace = eventoNode?.NamespaceURI;
            string reciboNamespace = null;
            if (reciboNode != null)
                reciboNamespace = reciboNode?.NamespaceURI;

            // Desserializar o XML para o objeto
            ESocialEvtBasesTrab resultado;
            using (StringReader stringReader = new StringReader(xmlContent))
            {
                XmlSerializer serializador = new XmlSerializer(typeof(ESocialEvtBasesTrab));
                resultado = (ESocialEvtBasesTrab)serializador.Deserialize(stringReader);
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

            Console.WriteLine("XML de EvtBasesTrab desserializado em classes C#!");
            return resultado;
        }
    }
}
