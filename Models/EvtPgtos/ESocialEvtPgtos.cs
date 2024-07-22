using System.Xml;
using System.Xml.Serialization;

namespace TransformarXmlEmCSharpESalvarNoBanco.Models.EvtPgtos
{
    [XmlRoot(ElementName = "eSocial", Namespace = "http://www.esocial.gov.br/schema/download/retornoProcessamento/v1_0_0")]
    public class ESocialEvtPgtos
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

        [XmlElement(ElementName = "evtPgtos")]
        public EvtPgtos? EvtPgtos { get; set; }

        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature? Signature { get; set; }
    }

    public class EvtPgtos
    {
        [XmlAttribute(AttributeName = "Id")]
        public string? Id { get; set; }

        [XmlElement(ElementName = "ideEvento")]
        public IdeEvento? IdeEvento { get; set; }

        [XmlElement(ElementName = "ideEmpregador")]
        public IdeEmpregador? IdeEmpregador { get; set; }

        [XmlElement(ElementName = "ideBenef")]
        public IdeBenef? IdeBenef { get; set; }
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

        [XmlElement(ElementName = "indGuia")]
        public string? IndGuia { get; set; }

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

    public class IdeBenef
    {
        public IdeBenef()
        {
            InfoPgto = new List<InfoPgto>();
            InfoIRComplem = new List<InfoIRComplem>();
        }

        [XmlElement(ElementName = "cpfBenef")]
        public string? CpfBenef { get; set; }

        [XmlElement(ElementName = "deps")]
        public Deps? Deps { get; set; }

        [XmlElement(ElementName = "infoPgto")]
        public List<InfoPgto> InfoPgto { get; set; }

        [XmlElement(ElementName = "infoIRComplem")]
        public List<InfoIRComplem> InfoIRComplem { get; set; }
    }

    public class Deps
    {
        [XmlElement(ElementName = "vrDedDep")]
        public string? VrDedDep { get; set; }
    }

    public class InfoPgto
    {
        public InfoPgto()
        {
            InfoPgtoExt = new List<InfoPgtoExt>();
            DetPgtoFl = new List<DetPgtoFl>();
            DetPgtoBenPr = new List<DetPgtoBenPr>();
            DetPgtoFer = new List<DetPgtoFer>();
            DetPgtoAnt = new List<DetPgtoAnt>();
            IdePgtoExt = new List<IdePgtoExt>();
        }

        [XmlElement(ElementName = "dtPgto")]
        public string? DtPgto { get; set; }

        [XmlElement(ElementName = "tpPgto")]
        public string? TpPgto { get; set; }

        [XmlElement(ElementName = "indResBr")]
        public string? IndResBr { get; set; }

        [XmlElement(ElementName = "perRef")]
        public string? PerRef { get; set; }

        [XmlElement(ElementName = "ideDmDev")]
        public string? IdeDmDev { get; set; }

        [XmlElement(ElementName = "vrLiq")]
        public string? VrLiq { get; set; }

        [XmlElement(ElementName = "paisResidExt")]
        public string? PaisResidExt { get; set; }

        [XmlElement(ElementName = "infoPgtoExt")]
        public List<InfoPgtoExt> InfoPgtoExt { get; set; }

        [XmlElement(ElementName = "detPgtoFl")]
        public List<DetPgtoFl> DetPgtoFl { get; set; }

        [XmlElement(ElementName = "detPgtoBenPr")]
        public List<DetPgtoBenPr> DetPgtoBenPr { get; set; }

        [XmlElement(ElementName = "detPgtoFer")]
        public List<DetPgtoFer> DetPgtoFer { get; set; }

        [XmlElement(ElementName = "detPgtoAnt")]
        public List<DetPgtoAnt> DetPgtoAnt { get; set; }
        
        [XmlElement(ElementName = "idePgtoExt")]
        public List<IdePgtoExt> IdePgtoExt { get; set; }
    }

    public class IdePgtoExt
    {
        [XmlElement(ElementName = "idePais")]
        public IdePais? IdePais { get; set; }

        [XmlElement(ElementName = "endExt")]
        public EndExtIdePgtoExt? EndExt { get; set; }
    }

    public class IdePais
    {
        [XmlElement(ElementName = "codPais")]
        public string? CodPais { get; set; }

        [XmlElement(ElementName = "indNIF")]
        public string? IndNIF { get; set; }

        [XmlElement(ElementName = "nifBenef")]
        public string? NifBenef { get; set; }
    }

    public class EndExtIdePgtoExt
    {
        [XmlElement(ElementName = "idePais")]
        public string? IdePais { get; set; }

        [XmlElement(ElementName = "dscLograd")]
        public string? DscLograd { get; set; }

        [XmlElement(ElementName = "nrLograd")]
        public string? NrLograd { get; set; }

        [XmlElement(ElementName = "complem")]
        public string? Complem { get; set; }

        [XmlElement(ElementName = "bairro")]
        public string? Bairro { get; set; }

        [XmlElement(ElementName = "nmCid")]
        public string? NmCid { get; set; }

        [XmlElement(ElementName = "codPostal")]
        public string? CodPostal { get; set; }
    }

    public class DetPgtoAnt
    {
        public DetPgtoAnt()
        {
            InfoPgtoAnt = new List<InfoPgtoAnt>();
        }

        [XmlElement(ElementName = "codCateg")]
        public string? CodCateg { get; set; }

        [XmlElement(ElementName = "infoPgtoAnt")]
        public List<InfoPgtoAnt> InfoPgtoAnt { get; set; }
    }

    public class InfoPgtoAnt
    {
        [XmlElement(ElementName = "tpBcIRRF")]
        public string? TpBcIRRF { get; set; }

        [XmlElement(ElementName = "vrBcIRRF")]
        public string? VrBcIRRF { get; set; }
    }

    public class DetPgtoFer
    {
        public DetPgtoFer()
        {
            DetRubrFer = new List<DetRubrFer>();
        }

        [XmlElement(ElementName = "codCateg")]
        public string? CodCateg { get; set; }

        [XmlElement(ElementName = "matricula")]
        public string? Matricula { get; set; }

        [XmlElement(ElementName = "dtIniGoz")]
        public string? DtIniGoz { get; set; }

        [XmlElement(ElementName = "qtDias")]
        public string? QtDias { get; set; }

        [XmlElement(ElementName = "vrLiq")]
        public string? VrLiq { get; set; }

        [XmlElement(ElementName = "detRubrFer")]
        public List<DetRubrFer> DetRubrFer { get; set; }
    }

    public class DetRubrFer
    {
        public DetRubrFer()
        {
            PenAlim = new List<PenAlimDetRubrFer>();
        }

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

        [XmlElement(ElementName = "penAlim")]
        public List<PenAlimDetRubrFer> PenAlim { get; set; }
    }

    public class PenAlimDetRubrFer
    {
        [XmlElement(ElementName = "cpfBenef")]
        public string? CpfBenef { get; set; }

        [XmlElement(ElementName = "dtNasctoBenef")]
        public string? DtNasctoBenef { get; set; }

        [XmlElement(ElementName = "nmBenefic")]
        public string? NmBenefic { get; set; }

        [XmlElement(ElementName = "vlrPensao")]
        public string? VlrPensao { get; set; }
    }

    public class DetPgtoBenPr
    {
        public DetPgtoBenPr()
        {
            RetPgtoTot = new List<RetPgtoTotDetPgtoBenPr>();
            InfoPgtoParc = new List<InfoPgtoParcDetPgtoBenPr>();
        }

        [XmlElement(ElementName = "perRef")]
        public string? PerRef { get; set; }

        [XmlElement(ElementName = "ideDmDev")]
        public string? IdeDmDev { get; set; }

        [XmlElement(ElementName = "indPgtoTt")]
        public string? IndPgtoTt { get; set; }

        [XmlElement(ElementName = "vrLiq")]
        public string? VrLiq { get; set; }

        [XmlElement(ElementName = "retPgtoTot")]
        public List<RetPgtoTotDetPgtoBenPr> RetPgtoTot { get; set; }

        [XmlElement(ElementName = "infoPgtoParc")]
        public List<InfoPgtoParcDetPgtoBenPr> InfoPgtoParc { get; set; }
    }

    public class RetPgtoTotDetPgtoBenPr
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
    }

    public class InfoPgtoParcDetPgtoBenPr
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
    }

    public class InfoPgtoExt
    {
        public InfoPgtoExt()
        {
            EndExt = new List<EndExt>();
        }

        [XmlElement(ElementName = "indNIF")]
        public string? IndNIF { get; set; }

        [XmlElement(ElementName = "nifBenef")]
        public string? NifBenef { get; set; }

        [XmlElement(ElementName = "frmTribut")]
        public string? FrmTribut { get; set; }

        [XmlElement(ElementName = "endExt")]
        public List<EndExt> EndExt { get; set; }
    }

    public class EndExt
    {
        [XmlElement(ElementName = "endDscLograd")]
        public string? EndDscLograd { get; set; }

        [XmlElement(ElementName = "endNrLograd")]
        public string? EndNrLograd { get; set; }

        [XmlElement(ElementName = "endComplem")]
        public string? EndComplem { get; set; }

        [XmlElement(ElementName = "endBairro")]
        public string? EndBairro { get; set; }

        [XmlElement(ElementName = "endCidade")]
        public string? EndCidade { get; set; }

        [XmlElement(ElementName = "endEstado")]
        public string? EndEstado { get; set; }

        [XmlElement(ElementName = "endCodPostal")]
        public string? EndCodPostal { get; set; }
        
        [XmlElement(ElementName = "telef")]
        public string? Telef { get; set; }
    }

    public class DetPgtoFl
    {
        public DetPgtoFl()
        {
            RetPgtoTot = new List<RetPgtoTot>();
            InfoPgtoParc = new List<InfoPgtoParc>();
        }

        [XmlElement(ElementName = "perRef")]
        public string? PerRef { get; set; }

        [XmlElement(ElementName = "ideDmDev")]
        public string? IdeDmDev { get; set; }

        [XmlElement(ElementName = "indPgtoTt")]
        public string? IndPgtoTt { get; set; }
        
        [XmlElement(ElementName = "vrLiq")]
        public string? VrLiq { get; set; }
        
        [XmlElement(ElementName = "nrRecArq")]
        public string? NrRecArq { get; set; }

        [XmlElement(ElementName = "retPgtoTot")]
        public List<RetPgtoTot> RetPgtoTot { get; set; }

        [XmlElement(ElementName = "infoPgtoParc")]
        public List<InfoPgtoParc> InfoPgtoParc { get; set; }
    }

    public class InfoPgtoParc
    {
        [XmlElement(ElementName = "matricula")]
        public string? Matricula { get; set; }

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
    }

    public class RetPgtoTot
    {
        public RetPgtoTot()
        {
            InfoDep = new List<InfoDep>();
            InfoIRCR = new List<InfoIRCR>();
            PlanSaude = new List<PlanSaude>();
            InfoReembMed = new List<InfoReembMed>();
            PenAlim = new List<PenAlimRetPgtoTot>();
        }

        [XmlElement(ElementName = "dtLaudo")]
        public string? DtLaudo { get; set; }

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

        [XmlElement(ElementName = "infoDep")]
        public List<InfoDep> InfoDep { get; set; }

        [XmlElement(ElementName = "infoIRCR")]
        public List<InfoIRCR> InfoIRCR { get; set; }

        [XmlElement(ElementName = "planSaude")]
        public List<PlanSaude> PlanSaude { get; set; }

        [XmlElement(ElementName = "infoReembMed")]
        public List<InfoReembMed> InfoReembMed { get; set; }

        [XmlElement(ElementName = "penAlim")]
        public List<PenAlimRetPgtoTot> PenAlim { get; set; }
    }

    public class InfoIRComplem
    {
        public InfoIRComplem()
        {
            InfoDep = new List<InfoDep>();
            InfoIRCR = new List<InfoIRCR>();
            PlanSaude = new List<PlanSaude>();
            InfoReembMed = new List<InfoReembMed>();
        }

        [XmlElement(ElementName = "dtLaudo")]
        public string? DtLaudo { get; set; }

        [XmlElement(ElementName = "infoDep")]
        public List<InfoDep> InfoDep { get; set; }

        [XmlElement(ElementName = "infoIRCR")]
        public List<InfoIRCR> InfoIRCR { get; set; }

        [XmlElement(ElementName = "planSaude")]
        public List<PlanSaude> PlanSaude { get; set; }
        
        [XmlElement(ElementName = "infoReembMed")]
        public List<InfoReembMed> InfoReembMed { get; set; }
    }

    public class InfoDep
    {
        [XmlElement(ElementName = "cpfDep")]
        public string? CpfDep { get; set; }

        [XmlElement(ElementName = "dtNascto")]
        public string? DtNascto { get; set; }

        [XmlElement(ElementName = "nome")]
        public string? Nome { get; set; }

        [XmlElement(ElementName = "depIRRF")]
        public string? DepIRRF { get; set; }

        [XmlElement(ElementName = "tpDep")]
        public string? TpDep { get; set; }

        [XmlElement(ElementName = "descrDep")]
        public string? DescrDep { get; set; }
    }

    public class InfoIRCR
    {
        public InfoIRCR()
        {
            DedDepen = new List<DedDepen>();
            PenAlim = new List<PenAlim>();
            PrevidCompl = new List<PrevidCompl>();
            InfoProcRet = new List<InfoProcRet>();
        }

        [XmlElement(ElementName = "tpCR")]
        public string? TpCR { get; set; }

        [XmlElement(ElementName = "dedDepen")]
        public List<DedDepen> DedDepen { get; set; }
        
        [XmlElement(ElementName = "penAlim")]
        public List<PenAlim> PenAlim { get; set; }
        
        [XmlElement(ElementName = "previdCompl")]
        public List<PrevidCompl> PrevidCompl { get; set; }
        
        [XmlElement(ElementName = "infoProcRet")]
        public List<InfoProcRet> InfoProcRet { get; set; }
    }

    public class DedDepen
    {
        [XmlElement(ElementName = "tpRend")]
        public string? TpRend { get; set; }

        [XmlElement(ElementName = "cpfDep")]
        public string? CpfDep { get; set; }

        [XmlElement(ElementName = "vlrDedDep")]
        public string? VlrDedDep { get; set; }
    }

    public class PenAlim
    {
        [XmlElement(ElementName = "tpRend")]
        public string? TpRend { get; set; }

        [XmlElement(ElementName = "cpfDep")]
        public string? CpfDep { get; set; }

        [XmlElement(ElementName = "vlrDedPenAlim")]
        public string? VlrDedPenAlim { get; set; }
    }

    public class PrevidCompl
    {
        [XmlElement(ElementName = "tpPrev")]
        public string? TpPrev { get; set; }

        [XmlElement(ElementName = "cnpjEntidPC")]
        public string? CnpjEntidPC { get; set; }

        [XmlElement(ElementName = "vlrDedPC")]
        public string? VlrDedPC { get; set; }

        [XmlElement(ElementName = "vlrPatrocFunp")]
        public string? VlrPatrocFunp { get; set; }
    }

    public class InfoProcRet
    {
        public InfoProcRet()
        {
            InfoValores = new List<InfoValores>();
        }

        [XmlElement(ElementName = "tpProcRet")]
        public string? TpProcRet { get; set; }

        [XmlElement(ElementName = "nrProcRet")]
        public string? NrProcRet { get; set; }

        [XmlElement(ElementName = "codSusp")]
        public string? CodSusp { get; set; }

        [XmlElement(ElementName = "InfoValores")]
        public List<InfoValores> InfoValores { get; set; }
    }

    public class InfoValores
    {
        public InfoValores()
        {
            DedSusp = new List<DedSusp>();
        }

        [XmlElement(ElementName = "indApuracao")]
        public string? IndApuracao { get; set; }

        [XmlElement(ElementName = "vlrNRetido")]
        public string? VlrNRetido { get; set; }

        [XmlElement(ElementName = "vlrDepJud")]
        public string? VlrDepJud { get; set; }

        [XmlElement(ElementName = "vlrCmpAnoCal")]
        public string? VlrCmpAnoCal { get; set; }

        [XmlElement(ElementName = "vlrCmpAnoAnt")]
        public string? VlrCmpAnoAnt { get; set; }

        [XmlElement(ElementName = "vlrRendSusp")]
        public string? VlrRendSusp { get; set; }

        [XmlElement(ElementName = "dedSusp")]
        public List<DedSusp> DedSusp { get; set; }
    }

    public class DedSusp
    {
        public DedSusp()
        {
            BenefPen = new List<BenefPen>();
        }

        [XmlElement(ElementName = "indTpDeducao")]
        public string? IndTpDeducao { get; set; }

        [XmlElement(ElementName = "vlrDedSusp")]
        public string? VlrDedSusp { get; set; }

        [XmlElement(ElementName = "cnpjEntidPC")]
        public string? CnpjEntidPC { get; set; }

        [XmlElement(ElementName = "vlrPatrocFunp")]
        public string? VlrPatrocFunp { get; set; }

        [XmlElement(ElementName = "benefPen")]
        public List<BenefPen> BenefPen { get; set; }
    }

    public class BenefPen
    {
        [XmlElement(ElementName = "cpfDep")]
        public string? CpfDep { get; set; }

        [XmlElement(ElementName = "vlrDepenSusp")]
        public string? VlrDepenSusp { get; set; }
    }

    public class PlanSaude
    {
        public PlanSaude()
        {
            InfoDepSau = new List<InfoDepSau>();
        }

        [XmlElement(ElementName = "cnpjOper")]
        public string? CnpjOper { get; set; }

        [XmlElement(ElementName = "regANS")]
        public string? RegANS { get; set; }
        
        [XmlElement(ElementName = "vlrSaudeTit")]
        public string? VlrSaudeTit { get; set; }

        [XmlElement(ElementName = "infoDepSau")]
        public List<InfoDepSau> InfoDepSau { get; set; }
    }

    public class InfoDepSau
    {
        [XmlElement(ElementName = "cpfDep")]
        public string? CpfDep { get; set; }

        [XmlElement(ElementName = "vlrSaudeDep")]
        public string? VlrSaudeDep { get; set; }
    }

    public class InfoReembMed
    {
        public InfoReembMed()
        {
            DetReembTit = new List<DetReembTit>();
            InfoReembDep = new List<InfoReembDep>();
        }

        [XmlElement(ElementName = "indOrgReemb")]
        public string? IndOrgReemb { get; set; }

        [XmlElement(ElementName = "cnpjOper")]
        public string? CnpjOper { get; set; }

        [XmlElement(ElementName = "regANS")]
        public string? RegANS { get; set; }

        [XmlElement(ElementName = "detReembTit")]
        public List<DetReembTit> DetReembTit { get; set; }
        
        [XmlElement(ElementName = "infoReembDep")]
        public List<InfoReembDep> InfoReembDep { get; set; }
    }

    public class DetReembTit
    {
        [XmlElement(ElementName = "tpInsc")]
        public string? TpInsc { get; set; }

        [XmlElement(ElementName = "nrInsc")]
        public string? NrInsc { get; set; }

        [XmlElement(ElementName = "vlrReemb")]
        public string? VlrReemb { get; set; }

        [XmlElement(ElementName = "vlrReembAnt")]
        public string? VlrReembAnt { get; set; }
    }

    public class InfoReembDep
    {
        public InfoReembDep()
        {
            DetReembTit = new List<DetReembTitInfoReembDep>();
        }

        [XmlElement(ElementName = "cpfBenef")]
        public string? CpfBenef { get; set; }

        [XmlElement(ElementName = "detReembTit")]
        public List<DetReembTitInfoReembDep> DetReembTit { get; set; }
    }

    public class DetReembTitInfoReembDep
    {
        [XmlElement(ElementName = "tpInsc")]
        public string? TpInsc { get; set; }

        [XmlElement(ElementName = "nrInsc")]
        public string? NrInsc { get; set; }

        [XmlElement(ElementName = "vlrReemb")]
        public string? VlrReemb { get; set; }

        [XmlElement(ElementName = "vlrReembAnt")]
        public string? VlrReembAnt { get; set; }
    }

    public class PenAlimRetPgtoTot
    {
        [XmlElement(ElementName = "cpfBenef")]
        public string? CpfBenef { get; set; }

        [XmlElement(ElementName = "dtNasctoBenef")]
        public string? DtNasctoBenef { get; set; }

        [XmlElement(ElementName = "nmBenefic")]
        public string? NmBenefic { get; set; }

        [XmlElement(ElementName = "vlrPensao")]
        public string? VlrPensao { get; set; }
    }

    [XmlRoot(ElementName = "Signature")]
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