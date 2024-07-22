using System.Xml.Serialization;

namespace TransformarXmlEmCSharpESalvarNoBanco.Models
{
    [XmlRoot(ElementName = "eSocial", Namespace = "http://www.esocial.gov.br/schema/download/retornoProcessamento/v1_0_0")]
    public class Evt
{
        [XmlIgnore]
        public string Namespace { get; set; }

        [XmlElement(ElementName = "retornoProcessamentoDownload")]
        public RetornoProcessamentoDownload RetornoProcessamentoDownload { get; set; }
    }

    public class RetornoProcessamentoDownload
    {
        [XmlElement(ElementName = "evento")]
        public Evento Evento { get; set; }

        [XmlElement(ElementName = "recibo")]
        public Recibo Recibo { get; set; }
    }

    public class Evento
    {
        [XmlElement(ElementName = "eSocial")]
        public ESocialEvento ESocialEvento { get; set; }
    }

    public class ESocialEvento
    {
        [XmlIgnore]
        public string Namespace { get; set; }
    }

    public class Recibo
    {
        [XmlElement(ElementName = "eSocial")]
        public ESocialRecibo ESocialRecibo { get; set; }
    }

    public class ESocialRecibo
    {
        [XmlIgnore]
        public string Namespace { get; set; }
    }
}