using System.Xml;
using System.Xml.Serialization;

namespace TransformarXmlEmCSharpESalvarNoBanco.Models.EvtTabRubrica
{
    [XmlRoot(ElementName = "eSocial", Namespace = "http://www.esocial.gov.br/schema/download/retornoProcessamento/v1_0_0")]
    public class ESocialEvtTabRubrica
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

        [XmlElement(ElementName = "evtTabRubrica")]
        public EvtTabRubrica? EvtTabRubrica { get; set; }

        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature? Signature { get; set; }
    }

    public class EvtTabRubrica
    {
        [XmlAttribute(AttributeName = "Id")]
        public string? Id { get; set; }

        [XmlElement(ElementName = "ideEvento")]
        public IdeEvento? IdeEvento { get; set; }

        [XmlElement(ElementName = "ideEmpregador")]
        public IdeEmpregador? IdeEmpregador { get; set; }

        [XmlElement(ElementName = "infoRubrica")]
        public InfoRubrica? InfoRubrica { get; set; }
    }

    public class IdeEvento
    {
        [XmlElement(ElementName = "tpAmb")]
        public string? TpAmb { get; set; }

        [XmlElement(ElementName = "procEmi")]
        public string? ProcEmi { get; set; }

        [XmlElement(ElementName = "verProc")]
        public string? VerProc { get; set; }
    }

    public class IdeEmpregador
    {
        [XmlElement(ElementName = "tpInsc")]
        public string? TpInsc { get; set; }

        [XmlElement(ElementName = "nrInsc")]
        public string? NrInsc { get; set; }
    }

    public class InfoRubrica
    {
        public InfoRubrica()
        {
            Inclusao = new List<Inclusao>();
            Alteracao = new List<Alteracao>();
            Exclusao = new List<Exclusao>();
        }

        [XmlElement(ElementName = "inclusao")]
        public List<Inclusao> Inclusao { get; set; }

        [XmlElement(ElementName = "alteracao")]
        public List<Alteracao> Alteracao { get; set; }

        [XmlElement(ElementName = "exclusao")]
        public List<Exclusao> Exclusao { get; set; }
    }

    public class Inclusao
    {
        [XmlElement(ElementName = "ideRubrica")]
        public IdeRubrica? IdeRubrica { get; set; }

        [XmlElement(ElementName = "dadosRubrica")]
        public DadosRubrica? DadosRubrica { get; set; }
    }

    public class IdeRubrica
    {
        [XmlElement(ElementName = "codRubr")]
        public string? CodRubr { get; set; }

        [XmlElement(ElementName = "ideTabRubr")]
        public string? IdeTabRubr { get; set; }

        [XmlElement(ElementName = "iniValid")]
        public string? IniValid { get; set; }

        [XmlElement(ElementName = "fimValid")]
        public string? FimValid { get; set; }
    }

    public class DadosRubrica
    {
        public DadosRubrica()
        {
            IdeProcessoCP = new List<IdeProcessoCP>();
            IdeProcessoIRRF = new List<IdeProcessoIRRF>();
            IdeProcessoFGTS = new List<IdeProcessoFGTS>();
            IdeProcessoSIND = new List<IdeProcessoSIND>();
        }

        [XmlElement(ElementName = "dscRubr")]
        public string? DscRubr { get; set; }

        [XmlElement(ElementName = "natRubr")]
        public string? NatRubr { get; set; }

        [XmlElement(ElementName = "tpRubr")]
        public string? TpRubr { get; set; }

        [XmlElement(ElementName = "codIncCP")]
        public string? CodIncCP { get; set; }

        [XmlElement(ElementName = "codIncIRRF")]
        public string? CodIncIRRF { get; set; }

        [XmlElement(ElementName = "codIncFGTS")]
        public string? CodIncFGTS { get; set; }

        [XmlElement(ElementName = "codIncSIND")]
        public string? CodIncSIND { get; set; }

        [XmlElement(ElementName = "observacao")]
        public string? Observacao { get; set; }

        [XmlElement(ElementName = "ideProcessoCP")]
        public List<IdeProcessoCP> IdeProcessoCP { get; set; }

        [XmlElement(ElementName = "ideProcessoIRRF")]
        public List<IdeProcessoIRRF> IdeProcessoIRRF { get; set; }

        [XmlElement(ElementName = "ideProcessoFGTS")]
        public List<IdeProcessoFGTS> IdeProcessoFGTS { get; set; }

        [XmlElement(ElementName = "ideProcessoSIND")]
        public List<IdeProcessoSIND> IdeProcessoSIND { get; set; }
    }

    public class IdeProcessoCP
    {
        [XmlElement(ElementName = "tpProc")]
        public string? TpProc { get; set; }

        [XmlElement(ElementName = "nrProc")]
        public string? NrProc { get; set; }

        [XmlElement(ElementName = "extDecisao")]
        public string? ExtDecisao { get; set; }

        [XmlElement(ElementName = "codSusp")]
        public string? CodSusp { get; set; }
    }

    public class IdeProcessoIRRF
    {
        [XmlElement(ElementName = "nrProc")]
        public string? NrProc { get; set; }

        [XmlElement(ElementName = "codSusp")]
        public string? CodSusp { get; set; }
    }

    public class IdeProcessoFGTS
    {
        [XmlElement(ElementName = "nrProc")]
        public string? NrProc { get; set; }
    }

    public class IdeProcessoSIND
    {
        [XmlElement(ElementName = "tpProc")]
        public string? TpProc { get; set; }
    }

    public class Alteracao
    {
        [XmlElement(ElementName = "ideRubrica")]
        public IdeRubricaAlteracao? IdeRubrica { get; set; }

        [XmlElement(ElementName = "dadosRubrica")]
        public DadosRubricaAlteracao? DadosRubrica { get; set; }
    }

    public class IdeRubricaAlteracao
    {
        [XmlElement(ElementName = "codRubr")]
        public string? CodRubr { get; set; }

        [XmlElement(ElementName = "ideTabRubr")]
        public string? IdeTabRubr { get; set; }

        [XmlElement(ElementName = "iniValid")]
        public string? IniValid { get; set; }

        [XmlElement(ElementName = "novaValidade")]
        public NovaValidade? NovaValidade { get; set; }
    }

    public class NovaValidade
    {
        [XmlElement(ElementName = "iniValid")]
        public string? IniValid { get; set; }

        [XmlElement(ElementName = "fimValid")]
        public string? FimValid { get; set; }
    }

    public class DadosRubricaAlteracao
    {
        public DadosRubricaAlteracao()
        {
            IdeProcessoCP = new List<IdeProcessoCPAlteracao>();
            IdeProcessoIRRF = new List<IdeProcessoIRRFAlteracao>();
            IdeProcessoFGTS = new List<IdeProcessoFGTSAlteracao>();
            IdeProcessoSIND = new List<IdeProcessoSINDAlteracao>();
        }

        [XmlElement(ElementName = "dscRubr")]
        public string? DscRubr { get; set; }

        [XmlElement(ElementName = "natRubr")]
        public string? NatRubr { get; set; }

        [XmlElement(ElementName = "tpRubr")]
        public string? TpRubr { get; set; }

        [XmlElement(ElementName = "codIncCP")]
        public string? CodIncCP { get; set; }

        [XmlElement(ElementName = "codIncIRRF")]
        public string? CodIncIRRF { get; set; }

        [XmlElement(ElementName = "codIncFGTS")]
        public string? CodIncFGTS { get; set; }

        [XmlElement(ElementName = "codIncSIND")]
        public string? CodIncSIND { get; set; }

        [XmlElement(ElementName = "observacao")]
        public string? Observacao { get; set; }

        [XmlElement(ElementName = "ideProcessoCP")]
        public List<IdeProcessoCPAlteracao> IdeProcessoCP { get; set; }

        [XmlElement(ElementName = "ideProcessoIRRF")]
        public List<IdeProcessoIRRFAlteracao> IdeProcessoIRRF { get; set; }

        [XmlElement(ElementName = "ideProcessoFGTS")]
        public List<IdeProcessoFGTSAlteracao> IdeProcessoFGTS { get; set; }

        [XmlElement(ElementName = "ideProcessoSIND")]
        public List<IdeProcessoSINDAlteracao> IdeProcessoSIND { get; set; }
    }

    public class IdeProcessoCPAlteracao
    {
        [XmlElement(ElementName = "tpProc")]
        public string? TpProc { get; set; }

        [XmlElement(ElementName = "nrProc")]
        public string? NrProc { get; set; }

        [XmlElement(ElementName = "extDecisao")]
        public string? ExtDecisao { get; set; }

        [XmlElement(ElementName = "codSusp")]
        public string? CodSusp { get; set; }
    }

    public class IdeProcessoIRRFAlteracao
    {
        [XmlElement(ElementName = "nrProc")]
        public string? NrProc { get; set; }

        [XmlElement(ElementName = "codSusp")]
        public string? CodSusp { get; set; }
    }

    public class IdeProcessoFGTSAlteracao
    {
        [XmlElement(ElementName = "nrProc")]
        public string? NrProc { get; set; }
    }

    public class IdeProcessoSINDAlteracao
    {
        [XmlElement(ElementName = "tpProc")]
        public string? TpProc { get; set; }
    }

    public class Exclusao
    {
        [XmlElement(ElementName = "ideRubrica")]
        public IdeRubricaExclusao? IdeRubrica { get; set; }
    }

    public class IdeRubricaExclusao
    {
        [XmlElement(ElementName = "codRubr")]
        public string? CodRubr { get; set; }

        [XmlElement(ElementName = "ideTabRubr")]
        public string? IdeTabRubr { get; set; }

        [XmlElement(ElementName = "iniValid")]
        public string? IniValid { get; set; }

        [XmlElement(ElementName = "fimValid")]
        public string? FimValid { get; set; }
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
        [XmlElement(ElementName = "cdResposta")]
        public string? CdResposta { get; set; }

        [XmlElement(ElementName = "descResposta")]
        public string? DescResposta { get; set; }

        [XmlElement(ElementName = "versaoAppProcessamento")]
        public string? VersaoAppProcessamento { get; set; }

        [XmlElement(ElementName = "dhProcessamento")]
        public string? DhProcessamento { get; set; }
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