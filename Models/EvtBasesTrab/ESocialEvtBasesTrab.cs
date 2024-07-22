using System.Xml;
using System.Xml.Serialization;

namespace TransformarXmlEmCSharpESalvarNoBanco.Models.EvtBasesTrab
{
    [XmlRoot(ElementName = "eSocial", Namespace = "http://www.esocial.gov.br/schema/download/retornoProcessamento/v1_0_0")]
    public class ESocialEvtBasesTrab
    {
        [XmlElement(ElementName = "retornoProcessamentoDownload")]
        public RetornoProcessamentoDownload? RetornoProcessamentoDownload { get; set; }
    }

    public class RetornoProcessamentoDownload
    {
        [XmlElement(ElementName = "evento")]
        public Evento? Evento { get; set; }

        [XmlElement(ElementName = "recibo")]
        public Recibo? Recibo { get; set; }
    }

    public class Evento
    {
        [XmlIgnore]
        public ESocialEvento ESocial { get; set; }

        [XmlAnyElement]
        public XmlElement ESocialElement
        {
            get => SerializeESocialToXmlElement();
            set => ESocial = DeserializeXmlElementToESocial(value);
        }

        private XmlElement SerializeESocialToXmlElement()
        {
            if (ESocial == null) return null;

            XmlDocument doc = new XmlDocument();
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ESocialEvento));
                serializer.Serialize(stream, ESocial);
                stream.Position = 0;
                doc.Load(stream);
            }
            return doc.DocumentElement;
        }

        private ESocialEvento DeserializeXmlElementToESocial(XmlElement element)
        {
            if (element == null) return null;

            string namespaceUri = element.NamespaceURI;
            XmlSerializer serializer = new XmlSerializer(typeof(ESocialEvento), new XmlRootAttribute { ElementName = "eSocial", Namespace = namespaceUri });

            using (StringReader reader = new StringReader(element.OuterXml))
            {
                ESocialEvento eSocial = (ESocialEvento)serializer.Deserialize(reader);
                eSocial.Namespace = namespaceUri;
                return eSocial;
            }
        }
    }

    public class ESocialEvento
    {
        [XmlIgnore]
        public string? Namespace { get; set; }

        [XmlElement(ElementName = "evtBasesTrab")]
        public EvtBasesTrab? EvtBasesTrab { get; set; }

        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature? Signature { get; set; }
    }

    public class EvtBasesTrab
    {
        public EvtBasesTrab()
        {
            InfoCp = new List<InfoCp>();
        }

        [XmlAttribute(AttributeName = "Id")]
        public string? Id { get; set; }

        [XmlElement(ElementName = "ideEvento")]
        public IdeEvento? IdeEvento { get; set; }

        [XmlElement(ElementName = "ideEmpregador")]
        public IdeEmpregador? IdeEmpregador { get; set; }

        [XmlElement(ElementName = "ideTrabalhador")]
        public IdeTrabalhador? IdeTrabalhador { get; set; }

        [XmlElement(ElementName = "infoCpCalc")]
        public InfoCpCalc? InfoCpCalc { get; set; }

        [XmlElement(ElementName = "infoCp")]
        public List<InfoCp> InfoCp { get; set; }
    }

    public class IdeEvento
    {
        [XmlElement(ElementName = "nrRecArqBase")]
        public string? NrRecArqBase { get; set; }

        [XmlElement(ElementName = "indApuracao")]
        public string? IndApuracao { get; set; }

        [XmlElement(ElementName = "perApur")]
        public string? PerApur { get; set; }
    }

    public class IdeEmpregador
    {
        [XmlElement(ElementName = "tpInsc")]
        public string? TpInsc { get; set; }

        [XmlElement(ElementName = "nrInsc")]
        public string? NrInsc { get; set; }
    }

    public class IdeTrabalhador
    {
        [XmlElement(ElementName = "cpfTrab")]
        public string? CpfTrab { get; set; }
    }

    public class InfoCpCalc
    {
        [XmlElement(ElementName = "tpCR")]
        public string? TpCR { get; set; }

        [XmlElement(ElementName = "vrCpSeg")]
        public string? VrCpSeg { get; set; }

        [XmlElement(ElementName = "vrDescSeg")]
        public string? VrDescSeg { get; set; }
    }

    public class InfoCp
    {
        public InfoCp()
        {
            IdeEstabLot = new List<IdeEstabLot>();
        }

        [XmlElement(ElementName = "ideEstabLot")]
        public List<IdeEstabLot> IdeEstabLot { get; set; }
        
        [XmlElement(ElementName = "classTrib")]
        public string? ClassTrib { get; set; }
    }

    public class IdeEstabLot
    {
        public IdeEstabLot()
        {
            InfoCategIncid = new List<InfoCategIncid>();
        }

        [XmlElement(ElementName = "tpInsc")]
        public string? TpInsc { get; set; }

        [XmlElement(ElementName = "nrInsc")]
        public string? NrInsc { get; set; }

        [XmlElement(ElementName = "codLotacao")]
        public string? CodLotacao { get; set; }

        [XmlElement(ElementName = "infoCategIncid")]
        public List<InfoCategIncid> InfoCategIncid { get; set; }
    }

    public class InfoCategIncid
    {
        public InfoCategIncid()
        {
            InfoBaseCS = new List<InfoBaseCS>();
            InfoPerRef = new List<InfoPerRef>();
        }

        [XmlElement(ElementName = "matricula")]
        public string? Matricula { get; set; }

        [XmlElement(ElementName = "codCateg")]
        public string? CodCateg { get; set; }

        [XmlElement(ElementName = "infoBaseCS")]
        public List<InfoBaseCS> InfoBaseCS { get; set; }

        [XmlElement(ElementName = "infoPerRef")]
        public List<InfoPerRef> InfoPerRef { get; set; }
    }

    public class InfoBaseCS
    {
        [XmlElement(ElementName = "ind13")]
        public string? Ind13 { get; set; }

        [XmlElement(ElementName = "tpValor")]
        public string? TpValor { get; set; }

        [XmlElement(ElementName = "valor")]
        public string? Valor { get; set; }
    }

    public class InfoPerRef
    {
        public InfoPerRef()
        {
            DetInfoPerRef = new List<DetInfoPerRef>();
        }

        [XmlElement(ElementName = "perRef")]
        public string? PerRef { get; set; }

        [XmlElement(ElementName = "detInfoPerRef")]
        public List<DetInfoPerRef> DetInfoPerRef { get; set; }
    }

    public class DetInfoPerRef
    {
        [XmlElement(ElementName = "ind13")]
        public string? Ind13 { get; set; }

        [XmlElement(ElementName = "tpValor")]
        public string? TpValor { get; set; }

        [XmlElement(ElementName = "vrPerRef")]
        public string? VrPerRef { get; set; }

        [XmlElement(ElementName = "tpVrPerRef")]
        public string? TpVrPerRef { get; set; }
    }

    public class Signature
    {
        [XmlIgnore]
        public string? Namespace { get; set; }

        [XmlElement(ElementName = "SignedInfo")]
        public SignedInfo? SignedInfo { get; set; }

        [XmlElement(ElementName = "SignatureValue")]
        public string? SignatureValue { get; set; }

        [XmlElement(ElementName = "KeyInfo")]
        public KeyInfo? KeyInfo { get; set; }
    }

    public class SignedInfo
    {
        [XmlIgnore]
        public string? Namespace { get; set; }

        [XmlElement(ElementName = "CanonicalizationMethod")]
        public CanonicalizationMethod? CanonicalizationMethod { get; set; }

        [XmlElement(ElementName = "SignatureMethod")]
        public SignatureMethod? SignatureMethod { get; set; }

        [XmlElement(ElementName = "Reference")]
        public Reference? Reference { get; set; }
    }

    public class CanonicalizationMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string? Algorithm { get; set; }
    }

    public class SignatureMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string? Algorithm { get; set; }
    }

    public class Reference
    {
        [XmlAttribute(AttributeName = "URI")]
        public string? URI { get; set; }

        [XmlElement(ElementName = "Transforms")]
        public Transforms? Transforms { get; set; }

        [XmlElement(ElementName = "DigestMethod")]
        public DigestMethod? DigestMethod { get; set; }

        [XmlElement(ElementName = "DigestValue")]
        public string? DigestValue { get; set; }
    }

    public class Transforms
    {
        public Transforms()
        {
            Transform = new List<Transform>();
        }

        [XmlElement(ElementName = "Transform")]
        public List<Transform> Transform { get; set; }
    }

    public class Transform
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string? Algorithm { get; set; }
    }

    public class DigestMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string? Algorithm { get; set; }
    }

    public class KeyInfo
    {
        [XmlElement(ElementName = "X509Data")]
        public X509Data? X509Data { get; set; }
    }

    public class X509Data
    {
        [XmlElement(ElementName = "X509Certificate")]
        public string? X509Certificate { get; set; }
    }

    public class Recibo
    {
        [XmlIgnore]
        public ESocialRecibo ESocial { get; set; }

        [XmlAnyElement]
        public XmlElement ESocialElement
        {
            get => SerializeESocialToXmlElement();
            set => ESocial = DeserializeXmlElementToESocial(value);
        }

        private XmlElement SerializeESocialToXmlElement()
        {
            if (ESocial == null) return null;

            XmlDocument doc = new XmlDocument();
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ESocialRecibo));
                serializer.Serialize(stream, ESocial);
                stream.Position = 0;
                doc.Load(stream);
            }
            return doc.DocumentElement;
        }

        private ESocialRecibo DeserializeXmlElementToESocial(XmlElement element)
        {
            if (element == null) return null;

            string namespaceUri = element.NamespaceURI;
            XmlSerializer serializer = new XmlSerializer(typeof(ESocialRecibo), new XmlRootAttribute { ElementName = "eSocial", Namespace = namespaceUri });

            using (StringReader reader = new StringReader(element.OuterXml))
            {
                ESocialRecibo eSocial = (ESocialRecibo)serializer.Deserialize(reader);
                eSocial.Namespace = namespaceUri;
                return eSocial;
            }
        }
    }

    public class ESocialRecibo
    {
        [XmlIgnore]
        public string? Namespace { get; set; }

        [XmlElement(ElementName = "retornoEvento")]
        public RetornoEvento? RetornoEvento { get; set; }

        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureRecibo? SignatureRecibo { get; set; }
    }

    public class RetornoEvento
    {
        [XmlAttribute(AttributeName = "Id")]
        public string? Id { get; set; }

        [XmlElement(ElementName = "ideEmpregador")]
        public IdeEmpregadorlRecibo? IdeEmpregadorlRecibo { get; set; }

        [XmlElement(ElementName = "recepcao")]
        public Recepcao? Recepcao { get; set; }

        [XmlElement(ElementName = "processamento")]
        public Processamento? Processamento { get; set; }

        [XmlElement(ElementName = "recibo")]
        public ReciboRetornoEvento? ReciboRetornoEvento { get; set; }
    }

    public class IdeEmpregadorlRecibo
    {
        [XmlElement(ElementName = "tpInsc")]
        public string? TpInsc { get; set; }

        [XmlElement(ElementName = "nrInsc")]
        public string? NrInsc { get; set; }
    }

    public class Recepcao
    {
        [XmlElement(ElementName = "tpAmb")]
        public string? TpAmb { get; set; }

        [XmlElement(ElementName = "dhRecepcao")]
        public string? DhRecepcao { get; set; }

        [XmlElement(ElementName = "versaoAppRecepcao")]
        public string? VersaoAppRecepcao { get; set; }

        [XmlElement(ElementName = "protocoloEnvioLote")]
        public string? ProtocoloEnvioLote { get; set; }
    }

    public class Processamento
    {
        public Processamento()
        {
            Ocorrencias = new List<Ocorrencia>();
        }

        [XmlElement(ElementName = "cdResposta")]
        public string? CdResposta { get; set; }

        [XmlElement(ElementName = "descResposta")]
        public string? DescResposta { get; set; }

        [XmlElement(ElementName = "versaoAppProcessamento")]
        public string? VersaoAppProcessamento { get; set; }

        [XmlElement(ElementName = "dhProcessamento")]
        public string? DhProcessamento { get; set; }

        [XmlElement(ElementName = "ocorrencias")]
        public List<Ocorrencia> Ocorrencias { get; set; }
    }

    public class Ocorrencia
    {
        [XmlElement(ElementName = "tipo")]
        public string? Tipo { get; set; }

        [XmlElement(ElementName = "codigo")]
        public string? Codigo { get; set; }

        [XmlElement(ElementName = "descricao")]
        public string? Descricao { get; set; }
    }

    public class ReciboRetornoEvento
    {
        [XmlElement(ElementName = "nrRecibo")]
        public string? NrRecibo { get; set; }

        [XmlElement(ElementName = "hash")]
        public string? Hash { get; set; }
    }

    public class SignatureRecibo
    {
        [XmlIgnore]
        public string? Namespace { get; set; }

        [XmlElement(ElementName = "SignedInfo")]
        public SignedInfoRecibo? SignedInfoRecibo { get; set; }

        [XmlElement(ElementName = "SignatureValue")]
        public string? SignatureValue { get; set; }

        [XmlElement(ElementName = "KeyInfo")]
        public KeyInfoRecibo? KeyInfoRecibo { get; set; }
    }

    public class SignedInfoRecibo
    {
        [XmlElement(ElementName = "CanonicalizationMethod")]
        public CanonicalizationMethodRecibo? CanonicalizationMethodRecibo { get; set; }

        [XmlElement(ElementName = "SignatureMethod")]
        public SignatureMethodRecibo? SignatureMethodRecibo { get; set; }

        [XmlElement(ElementName = "Reference")]
        public ReferenceRecibo? ReferenceRecibo { get; set; }
    }

    public class CanonicalizationMethodRecibo
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string? Algorithm { get; set; }
    }

    public class SignatureMethodRecibo
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string? Algorithm { get; set; }
    }

    public class ReferenceRecibo
    {
        [XmlAttribute(AttributeName = "URI")]
        public string? URI { get; set; }

        [XmlElement(ElementName = "Transforms")]
        public TransformsRecibo? Transforms { get; set; }

        [XmlElement(ElementName = "DigestMethod")]
        public DigestMethodRecibo? DigestMethodRecibo { get; set; }

        [XmlElement(ElementName = "DigestValue")]
        public string? DigestValue { get; set; }
    }

    public class TransformsRecibo
    {
        public TransformsRecibo()
        {
            Transform = new List<TransformRecibo>();
        }

        [XmlElement(ElementName = "Transform")]
        public List<TransformRecibo> Transform { get; set; }
    }

    public class TransformRecibo
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string? Algorithm { get; set; }
    }

    public class DigestMethodRecibo
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string? Algorithm { get; set; }
    }

    public class KeyInfoRecibo
    {
        [XmlElement(ElementName = "X509Data")]
        public X509DataRecibo? X509DataRecibo { get; set; }
    }

    public class X509DataRecibo
    {
        [XmlElement(ElementName = "X509Certificate")]
        public string? X509Certificate { get; set; }
    }
}
