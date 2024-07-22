using System.Xml;
using System.Xml.Serialization;

namespace TransformarXmlEmCSharpESalvarNoBanco.Models.EvtRemun
{
    [XmlRoot(ElementName = "eSocial", Namespace = "http://www.esocial.gov.br/schema/download/retornoProcessamento/v1_0_0")]
    public class ESocialEvtRemun
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

        [XmlElement(ElementName = "evtRemun")]
        public EvtRemun? EvtRemun { get; set; }

        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature? Signature { get; set; }
    }

    public class EvtRemun
    {
        public EvtRemun()
        {
            DmDev = new List<DmDev>();
        }

        [XmlAttribute(AttributeName = "Id")]
        public string? Id { get; set; }

        [XmlElement(ElementName = "ideEvento")]
        public IdeEvento? IdeEvento { get; set; }

        [XmlElement(ElementName = "ideEmpregador")]
        public IdeEmpregador? IdeEmpregador { get; set; }

        [XmlElement(ElementName = "ideTrabalhador")]
        public IdeTrabalhador? IdeTrabalhador { get; set; }

        [XmlElement(ElementName = "dmDev")]
        public List<DmDev> DmDev { get; set; }
    }

    public class IdeEvento
    {
        [XmlElement(ElementName = "indRetif")]
        public string? IndRetif { get; set; }

        [XmlElement(ElementName = "nrRecibo")]
        public string? NrRecibo { get; set; }

        [XmlElement(ElementName = "indApuracao")]
        public string? IndApuracao { get; set; }

        [XmlElement(ElementName = "perApur")]
        public string? PerApur { get; set; }

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

    public class IdeTrabalhador
    {
        public IdeTrabalhador()
        {
            InfoMV = new List<InfoMV>();
            InfoComplem = new List<InfoComplem>();
            ProcJudTrab = new List<ProcJudTrab>();
            InfoInterm = new List<InfoInterm>();
        }

        [XmlElement(ElementName = "cpfTrab")]
        public string? CpfTrab { get; set; }

        [XmlElement(ElementName = "nisTrab")]
        public string? NisTrab { get; set; }

        [XmlElement(ElementName = "infoMV")]
        public List<InfoMV> InfoMV { get; set; }

        [XmlElement(ElementName = "infoComplem")]
        public List<InfoComplem> InfoComplem { get; set; }
        
        [XmlElement(ElementName = "procJudTrab")]
        public List<ProcJudTrab> ProcJudTrab { get; set; }
        
        [XmlElement(ElementName = "infoInterm")]
        public List<InfoInterm> InfoInterm { get; set; }
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

    public class InfoComplem
    {
        public InfoComplem()
        {
            SucessaoVinc = new List<SucessaoVinc>();
        }

        [XmlElement(ElementName = "nmTrab")]
        public string? NmTrab { get; set; }

        [XmlElement(ElementName = "dtNascto")]
        public string? DtNascto { get; set; }

        [XmlElement(ElementName = "sucessaoVinc")]
        public List<SucessaoVinc> SucessaoVinc { get; set; }
    }

    public class SucessaoVinc
    {
        [XmlElement(ElementName = "tpInsc")]
        public string? TpInsc { get; set; }

        [XmlElement(ElementName = "nrInsc")]
        public string? NrInsc { get; set; }

        [XmlElement(ElementName = "matricAnt")]
        public string? MatricAnt { get; set; }

        [XmlElement(ElementName = "dtAdm")]
        public string? DtAdm { get; set; }
        
        [XmlElement(ElementName = "observacao")]
        public string? Observacao { get; set; }
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

    public class InfoInterm
    {
        [XmlElement(ElementName = "dia")]
        public string? Dia { get; set; }
    }
    /// <summary>
    /// ////////////////////
    /// </summary>
    public class DmDev
    {
        public DmDev()
        {
            InfoPerAnt = new List<InfoPerAnt>();
            InfoComplCont = new List<InfoComplCont>();
        }

        [XmlElement(ElementName = "ideDmDev")]
        public string? IdeDmDev { get; set; }

        [XmlElement(ElementName = "codCateg")]
        public string? CodCateg { get; set; }

        [XmlElement(ElementName = "infoPerApur")]
        public InfoPerApur? InfoPerApur { get; set; }

        [XmlElement(ElementName = "infoPerAnt")]
        public List<InfoPerAnt> InfoPerAnt { get; set; }

        [XmlElement(ElementName = "infoComplCont")]
        public List<InfoComplCont> InfoComplCont { get; set; }
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
            RemunPerApur = new List<RemunPerApur>();
        }

        [XmlElement(ElementName = "tpInsc")]
        public string? TpInsc { get; set; }

        [XmlElement(ElementName = "nrInsc")]
        public string? NrInsc { get; set; }

        [XmlElement(ElementName = "codLotacao")]
        public string? CodLotacao { get; set; }

        [XmlElement(ElementName = "qtdDiasAv")]
        public string? QtdDiasAv { get; set; }

        [XmlElement(ElementName = "remunPerApur")]
        public List<RemunPerApur> RemunPerApur { get; set; }
    }

    public class RemunPerApur
    {
        public RemunPerApur()
        {
            InfoAgNocivo = new List<InfoAgNocivo>();
            ItensRemun = new List<ItensRemun>();
            InfoSaudeColet = new List<InfoSaudeColet>();
        }

        [XmlElement(ElementName = "matricula")]
        public string? Matricula { get; set; }

        [XmlElement(ElementName = "indSimples")]
        public string? IndSimples { get; set; }

        [XmlElement(ElementName = "infoAgNocivo")]
        public List<InfoAgNocivo> InfoAgNocivo { get; set; }

        [XmlElement(ElementName = "itensRemun")]
        public List<ItensRemun> ItensRemun { get; set; }

        [XmlElement(ElementName = "infoSaudeColet")]
        public List<InfoSaudeColet> InfoSaudeColet { get; set; }
    }

    public class InfoAgNocivo
    {
        [XmlElement(ElementName = "grauExp")]
        public string? GrauExp { get; set; }
    }

    public class ItensRemun
    {
        [XmlElement(ElementName = "codRubr")]
        public string? CodRubr { get; set; }

        [XmlElement(ElementName = "ideTabRubr")]
        public string? IdeTabRubr { get; set; }

        [XmlElement(ElementName = "qtdRubr")]
        public string? QtdRubr { get; set; }

        [XmlElement(ElementName = "fatorRubr")]
        public string? FatorRubr { get; set; }

        [XmlElement(ElementName = "vrUnit")]
        public string? VrUnit { get; set; }

        [XmlElement(ElementName = "vrRubr")]
        public string? VrRubr { get; set; }

        [XmlElement(ElementName = "indApurIR")]
        public string? IndApurIR { get; set; }
    }

    public class InfoSaudeColet
    {
        [XmlElement(ElementName = "detOper")]
        public DetOper? DetOper { get; set; }
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
        
        [XmlElement(ElementName = "remunSuc")]
        public string? RemunSuc { get; set; }

        [XmlElement(ElementName = "idePeriodo")]
        public List<IdePeriodo> IdePeriodo { get; set; }
    }

    public class IdePeriodo
    {
        public IdePeriodo()
        {
            IdeEstabLot = new List<IdeEstabLotIdePeriodo>();
        }

        [XmlElement(ElementName = "perRef")]
        public string? PerRef { get; set; }

        [XmlElement(ElementName = "ideEstabLot")]
        public List<IdeEstabLotIdePeriodo> IdeEstabLot { get; set; }
    }

    public class IdeEstabLotIdePeriodo
    {
        public IdeEstabLotIdePeriodo()
        {
            RemunPerAnt = new List<RemunPerAnt>();
        }

        [XmlElement(ElementName = "tpInsc")]
        public string? TpInsc { get; set; }

        [XmlElement(ElementName = "nrInsc")]
        public string? NrInsc { get; set; }

        [XmlElement(ElementName = "codLotacao")]
        public string? CodLotacao { get; set; }

        [XmlElement(ElementName = "remunPerAnt")]
        public List<RemunPerAnt> RemunPerAnt { get; set; }
    }

    public class RemunPerAnt
    {
        public RemunPerAnt()
        {
            ItensRemun = new List<ItensRemunRemunPerAnt>();
        }

        [XmlElement(ElementName = "matricula")]
        public string? Matricula { get; set; }

        [XmlElement(ElementName = "indSimples")]
        public string? IndSimples { get; set; }

        [XmlElement(ElementName = "infoAgNocivo")]
        public InfoAgNocivoRemunPerAnt? InfoAgNocivo { get; set; }

        [XmlElement(ElementName = "itensRemun")]
        public List<ItensRemunRemunPerAnt> ItensRemun { get; set; }
    }

    public class InfoAgNocivoRemunPerAnt
    {
        [XmlElement(ElementName = "grauExp")]
        public string? GrauExp { get; set; }
    }

    public class ItensRemunRemunPerAnt
    {
        [XmlElement(ElementName = "codRubr")]
        public string? CodRubr { get; set; }

        [XmlElement(ElementName = "ideTabRubr")]
        public string? IdeTabRubr { get; set; }

        [XmlElement(ElementName = "qtdRubr")]
        public string? QtdRubr { get; set; }

        [XmlElement(ElementName = "fatorRubr")]
        public string? FatorRubr { get; set; }

        [XmlElement(ElementName = "vrUnit")]
        public string? VrUnit { get; set; }

        [XmlElement(ElementName = "vrRubr")]
        public string? VrRubr { get; set; }

        [XmlElement(ElementName = "indApurIR")]
        public string? IndApurIR { get; set; }
    }

    public class InfoComplCont
    {
        [XmlElement(ElementName = "codCBO")]
        public string? CodCBO { get; set; }

        [XmlElement(ElementName = "natAtividade")]
        public string? NatAtividade { get; set; }

        [XmlElement(ElementName = "qtdDiasTrab")]
        public string? QtdDiasTrab { get; set; }
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