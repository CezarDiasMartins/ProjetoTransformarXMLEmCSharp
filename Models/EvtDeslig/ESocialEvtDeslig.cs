using System.Xml;
using System.Xml.Serialization;

namespace TransformarXmlEmCSharpESalvarNoBanco.Models.EvtDeslig
{
    [XmlRoot(ElementName = "eSocial", Namespace = "http://www.esocial.gov.br/schema/download/retornoProcessamento/v1_0_0")]
    public class ESocialEvtDeslig
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

        [XmlElement(ElementName = "evtDeslig")]
        public EvtDeslig? EvtDeslig { get; set; }

        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature? Signature { get; set; }
    }

    public class EvtDeslig
    {
        [XmlAttribute(AttributeName = "Id")]
        public string? Id { get; set; }

        [XmlElement(ElementName = "ideEvento")]
        public IdeEvento? IdeEvento { get; set; }

        [XmlElement(ElementName = "ideEmpregador")]
        public IdeEmpregador? IdeEmpregador { get; set; }

        [XmlElement(ElementName = "ideVinculo")]
        public IdeVinculo? IdeVinculo { get; set; }

        [XmlElement(ElementName = "infoDeslig")]
        public InfoDeslig? InfoDeslig { get; set; }
    }

    public class IdeEvento
    {
        [XmlElement(ElementName = "indRetif")]
        public string? IndRetif { get; set; }

        [XmlElement(ElementName = "nrRecibo")]
        public string? NrRecibo { get; set; }

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

    public class IdeVinculo
    {
        [XmlElement(ElementName = "cpfTrab")]
        public string? CpfTrab { get; set; }

        [XmlElement(ElementName = "nisTrab")]
        public string? NisTrab { get; set; }

        [XmlElement(ElementName = "matricula")]
        public string? Matricula { get; set; }
    }

    public class InfoDeslig
    {
        public InfoDeslig()
        {
            RemunAposDeslig = new List<RemunAposDeslig>();
            InfoInterm = new List<ItemInfoInterm>();
            Observacoes = new List<Observacoes>();
            ConsigFGTS = new List<ConsigFGTS>();
        }

        [XmlElement(ElementName = "mtvDeslig")]
        public string? MtvDeslig { get; set; }

        [XmlElement(ElementName = "dtDeslig")]
        public string? DtDeslig { get; set; }

        [XmlElement(ElementName = "indPagtoAPI")]
        public string? IndPagtoAPI { get; set; }

        [XmlElement(ElementName = "dtProjFimAPI")]
        public string? DtProjFimAPI { get; set; }

        [XmlElement(ElementName = "pensAlim")]
        public string? PensAlim { get; set; }

        [XmlElement(ElementName = "percAliment")]
        public string? PercAliment { get; set; }

        [XmlElement(ElementName = "vrAlim")]
        public string? VrAlim { get; set; }

        [XmlElement(ElementName = "nrCertObito")]
        public string? NrCertObito { get; set; }

        [XmlElement(ElementName = "nrProcTrab")]
        public string? NrProcTrab { get; set; }

        [XmlElement(ElementName = "indPDV")]
        public string? IndPDV { get; set; }

        [XmlElement(ElementName = "indCumprParc")]
        public string? IndCumprParc { get; set; }

        [XmlElement(ElementName = "sucessaoVinc")]
        public SucessaoVinc? SucessaoVinc { get; set; }

        [XmlElement(ElementName = "transfTit")]
        public TransfTit? TransfTit { get; set; }

        [XmlElement(ElementName = "mudancaCPF")]
        public MudancaCPF? MudancaCPF { get; set; }

        [XmlElement(ElementName = "quarentena")]
        public Quarentena? Quarentena { get; set; }

        [XmlElement(ElementName = "remunAposDeslig")]
        public List<RemunAposDeslig> RemunAposDeslig { get; set; }

        [XmlElement(ElementName = "verbasResc")]
        public VerbasResc? VerbasResc { get; set; }

        [XmlElement(ElementName = "infoInterm")]
        public List<ItemInfoInterm> InfoInterm { get; set; }

        [XmlElement(ElementName = "observacoes")]
        public List<Observacoes> Observacoes { get; set; }

        [XmlElement(ElementName = "consigFGTS")]
        public List<ConsigFGTS> ConsigFGTS { get; set; }
    }

    public class SucessaoVinc
    {
        [XmlElement(ElementName = "tpInscSuc")]
        public string? TpInscSuc { get; set; }

        [XmlElement(ElementName = "cnpjSucessora")]
        public string? CnpjSucessora { get; set; }
    }

    public class TransfTit
    {
        [XmlElement(ElementName = "cpfSubstituto")]
        public string? CpfSubstituto { get; set; }

        [XmlElement(ElementName = "dtNascto")]
        public string? DtNascto { get; set; }
    }

    public class MudancaCPF
    {
        [XmlElement(ElementName = "novoCPF")]
        public string? NovoCPF { get; set; }
    }

    public class Quarentena
    {
        [XmlElement(ElementName = "dtFimQuar")]
        public string? DtFimQuar { get; set; }
    }

    public class RemunAposDeslig
    {
        [XmlElement(ElementName = "indRemun")]
        public string? IndRemun { get; set; }

        [XmlElement(ElementName = "dtFimRemun")]
        public string? DtFimRemun { get; set; }
    }

    public class VerbasResc
    {
        public VerbasResc()
        {
            DmDev = new List<DmDev>();
            ProcJudTrab = new List<ProcJudTrab>();
            InfoMV = new List<InfoMV>();
            ProcCS = new List<ProcCS>();
        }

        [XmlElement(ElementName = "dmDev")]
        public List<DmDev> DmDev { get; set; }

        [XmlElement(ElementName = "procJudTrab")]
        public List<ProcJudTrab> ProcJudTrab { get; set; }

        [XmlElement(ElementName = "infoMV")]
        public List<InfoMV> InfoMV { get; set; }

        [XmlElement(ElementName = "procCS")]
        public List<ProcCS> ProcCS { get; set; }
    }

    public class ItemInfoInterm
    {
        [XmlElement(ElementName = "dia")]
        public string? Dia { get; set; }
    }

    public class Observacoes
    {
        [XmlElement(ElementName = "observacao")]
        public string? Observacao { get; set; }
    }

    public class ConsigFGTS
    {
        [XmlElement(ElementName = "insConsig")]
        public string? InsConsig { get; set; }

        [XmlElement(ElementName = "nrContr")]
        public string? NrContr { get; set; }
    }

    public class DmDev
    {
        public DmDev()
        {
            InfoPerAnt = new List<InfoPerAnt>();
            InfoRRA = new List<InfoRRA>();
        }

        [XmlElement(ElementName = "ideDmDev")]
        public string? IdeDmDev { get; set; }

        [XmlElement(ElementName = "indRRA")]
        public string? IndRRA { get; set; }

        [XmlElement(ElementName = "infoPerApur")]
        public InfoPerApur? InfoPerApur { get; set; }

        [XmlElement(ElementName = "infoPerAnt")]
        public List<InfoPerAnt> InfoPerAnt { get; set; }

        [XmlElement(ElementName = "infoRRA")]
        public List<InfoRRA> InfoRRA { get; set; }
    }

    public class InfoPerApur
    {
        public InfoPerApur()
        {
            IdeEstabLot = new List<IdeEstabLot>();
        }

        [XmlElement(ElementName = "ideEstabLot")]
        public List<IdeEstabLot> IdeEstabLot { get; set; }
    }

    public class IdeEstabLot
    {
        public IdeEstabLot()
        {
            DetVerbas = new List<DetVerbas>();
        }

        [XmlElement(ElementName = "tpInsc")]
        public string? TpInsc { get; set; }

        [XmlElement(ElementName = "nrInsc")]
        public string? NrInsc { get; set; }

        [XmlElement(ElementName = "codLotacao")]
        public string? CodLotacao { get; set; }

        [XmlElement(ElementName = "infoAgNocivo")]
        public InfoAgNocivo? InfoAgNocivo { get; set; }
        
        [XmlElement(ElementName = "infoSimples")]
        public InfoSimples? InfoSimples { get; set; }

        [XmlElement(ElementName = "detVerbas")]
        public List<DetVerbas> DetVerbas { get; set; }
        
        [XmlElement(ElementName = "infoSaudeColet")]
        public List<InfoSaudeColet> InfoSaudeColet { get; set; }
    }

    public class InfoAgNocivo
    {
        [XmlElement(ElementName = "grauExp")]
        public string? GrauExp { get; set; }
    }

    public class InfoSimples
    {
        [XmlElement(ElementName = "indSimples")]
        public string? IndSimples { get; set; }
    }

    public class DetVerbas
    {
        [XmlElement(ElementName = "codRubr")]
        public string? CodRubr { get; set; }

        [XmlElement(ElementName = "ideTabRubr")]
        public string? IdeTabRubr { get; set; }

        [XmlElement(ElementName = "qtdRubr")]
        public string? QtdRubr { get; set; }

        [XmlElement(ElementName = "fatorRubr")]
        public string? FatorRubr { get; set; }

        [XmlElement(ElementName = "vrRubr")]
        public string? VrRubr { get; set; }

        [XmlElement(ElementName = "indApurIR")]
        public string? IndApurIR { get; set; }

        [XmlElement(ElementName = "vrUnit")]
        public string? VrUnit { get; set; }
    }

    public class InfoSaudeColet
    {
        public InfoSaudeColet()
        {
            DetOper = new List<DetOper>();
        }

        [XmlElement(ElementName = "detOper")]
        public List<DetOper> DetOper { get; set; }
    }

    public class DetOper
    {
        public DetOper()
        {
            DetPlano = new List<DetPlano>();
        }

        [XmlElement(ElementName = "cnpjOper")]
        public string? CnpjOper { get; set; }

        [XmlElement(ElementName = "regANS")]
        public string? RegANS { get; set; }

        [XmlElement(ElementName = "vrPgTit")]
        public string? VrPgTit { get; set; }

        [XmlElement(ElementName = "detPlano")]
        public List<DetPlano> DetPlano { get; set; }
    }

    public class DetPlano
    {
        [XmlElement(ElementName = "tpDep")]
        public string? TpDep { get; set; }

        [XmlElement(ElementName = "cpfDep")]
        public string? CpfDep { get; set; }

        [XmlElement(ElementName = "nmDep")]
        public string? NmDep { get; set; }

        [XmlElement(ElementName = "dtNascto")]
        public string? DtNascto { get; set; }

        [XmlElement(ElementName = "vlrPgDep")]
        public string? VlrPgDep { get; set; }
    }

    public class InfoPerAnt
    {
        public InfoPerAnt()
        {
            IdeADC = new List<IdeADC>();
        }

        [XmlElement(ElementName = "ideADC")]
        public List<IdeADC> IdeADC { get; set; }
    }

    public class IdeADC
    {
        public IdeADC()
        {
            IdePeriodo = new List<IdePeriodo>();
        }

        [XmlElement(ElementName = "dtAcConv")]
        public string? DtAcConv { get; set; }

        [XmlElement(ElementName = "tpAcConv")]
        public string? TpAcConv { get; set; }

        [XmlElement(ElementName = "compAcConv")]
        public string? CompAcConv { get; set; }

        [XmlElement(ElementName = "dtEfAcConv")]
        public string? DtEfAcConv { get; set; }

        [XmlElement(ElementName = "dsc")]
        public string? Dsc { get; set; }

        [XmlElement(ElementName = "idePeriodo")]
        public List<IdePeriodo> IdePeriodo { get; set; }
    }

    public class IdePeriodo
    {
        public IdePeriodo()
        {
            IdeEstabLot = new List<IdePeriodo_IdeEstabLot>();
        }

        [XmlElement(ElementName = "perRef")]
        public string? PerRef { get; set; }

        [XmlElement(ElementName = "ideEstabLot")]
        public List<IdePeriodo_IdeEstabLot> IdeEstabLot { get; set; }
    }

    public class IdePeriodo_IdeEstabLot
    {
        public IdePeriodo_IdeEstabLot()
        {
            DetVerbas = new List<IdeEstabLot_DetVerbas>();
        }

        [XmlElement(ElementName = "tpInsc")]
        public string? TpInsc { get; set; }

        [XmlElement(ElementName = "nrInsc")]
        public string? NrInsc { get; set; }

        [XmlElement(ElementName = "codLotacao")]
        public string? CodLotacao { get; set; }

        [XmlElement(ElementName = "infoAgNocivo")]
        public IdeEstabLot_InfoAgNocivo? InfoAgNocivo { get; set; }

        [XmlElement(ElementName = "infoSimples")]
        public IdeEstabLot_InfoSimples? InfoSimples { get; set; }

        [XmlElement(ElementName = "detVerbas")]
        public List<IdeEstabLot_DetVerbas> DetVerbas { get; set; }
    }

    public class IdeEstabLot_InfoAgNocivo
    {
        [XmlElement(ElementName = "grauExp")]
        public string? GrauExp { get; set; }
    }

    public class IdeEstabLot_InfoSimples
    {
        [XmlElement(ElementName = "indSimples")]
        public string? IndSimples { get; set; }
    }

    public class IdeEstabLot_DetVerbas
    {
        [XmlElement(ElementName = "codRubr")]
        public string? CodRubr { get; set; }

        [XmlElement(ElementName = "ideTabRubr")]
        public string? IdeTabRubr { get; set; }

        [XmlElement(ElementName = "qtdRubr")]
        public string? QtdRubr { get; set; }

        [XmlElement(ElementName = "fatorRubr")]
        public string? FatorRubr { get; set; }

        [XmlElement(ElementName = "vrRubr")]
        public string? VrRubr { get; set; }

        [XmlElement(ElementName = "indApurIR")]
        public string? IndApurIR { get; set; }

        [XmlElement(ElementName = "vrUnit")]
        public string? VrUnit { get; set; }
    }

    public class ProcJudTrab
    {
        [XmlElement(ElementName = "tpTrib")]
        public string? TpTrib { get; set; }

        [XmlElement(ElementName = "nrProcJud")]
        public string? NrProcJud { get; set; }

        [XmlElement(ElementName = "codSusp")]
        public string? CodSusp { get; set; }
    }

    public class InfoMV
    {
        public InfoMV()
        {
            RemunOutrEmpr = new List<RemunOutrEmpr>();
        }

        [XmlElement(ElementName = "indMV")]
        public string? IndMV { get; set; }

        [XmlElement(ElementName = "remunOutrEmpr")]
        public List<RemunOutrEmpr> RemunOutrEmpr { get; set; }
    }

    public class ProcCS
    {
        [XmlElement(ElementName = "nrProcJud")]
        public string? NrProcJud { get; set; }
    }

    public class RemunOutrEmpr
    {
        [XmlElement(ElementName = "tpInsc")]
        public string? TpInsc { get; set; }

        [XmlElement(ElementName = "nrInsc")]
        public string? NrInsc { get; set; }

        [XmlElement(ElementName = "codCateg")]
        public string? CodCateg { get; set; }

        [XmlElement(ElementName = "vlrRemunOE")]
        public string? VlrRemunOE { get; set; }
    }

    public class InfoRRA
    {
        public InfoRRA()
        {
            DespProcJud = new List<DespProcJud>();
            IdeAdv = new List<IdeAdv>();
        }

        [XmlElement(ElementName = "tpProcRRA")]
        public string? TpProcRRA { get; set; }

        [XmlElement(ElementName = "nrProcRRA")]
        public string? NrProcRRA { get; set; }

        [XmlElement(ElementName = "descRRA")]
        public string? DescRRA { get; set; }

        [XmlElement(ElementName = "qtdMesesRRA")]
        public string? QtdMesesRRA { get; set; }

        [XmlElement(ElementName = "despProcJud")]
        public List<DespProcJud> DespProcJud { get; set; }

        [XmlElement(ElementName = "ideAdv")]
        public List<IdeAdv> IdeAdv { get; set; }
    }

    public class DespProcJud
    {
        [XmlElement(ElementName = "vlrDespCustas")]
        public string? VlrDespCustas { get; set; }

        [XmlElement(ElementName = "vlrDespAdvogados")]
        public string? VlrDespAdvogados { get; set; }
    }

    public class IdeAdv
    {
        [XmlElement(ElementName = "tpInsc")]
        public string? TpInsc { get; set; }

        [XmlElement(ElementName = "nrInsc")]
        public string? NrInsc { get; set; }

        [XmlElement(ElementName = "vlrAdv")]
        public string? VlrAdv { get; set; }
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