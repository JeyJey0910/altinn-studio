// <auto-generated/>
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace App.IntegrationTests.Mocks.Apps.tdd.custom_validation
{
    public class Skjema
    {
        [Range(Int32.MinValue, Int32.MaxValue)]
        [XmlAttribute("skjemanummer")]
        [BindNever]
        public decimal skjemanummer { get; set; } = 1472;
        [Range(Int32.MinValue, Int32.MaxValue)]
        [XmlAttribute("spesifikasjonsnummer")]
        [BindNever]
        public decimal spesifikasjonsnummer { get; set; } = 9812;
        [XmlAttribute("blankettnummer")]
        [BindNever]
        public string blankettnummer { get; set; } = "AFP-01";
        [XmlAttribute("tittel")]
        [BindNever]
        public string tittel { get; set; } = "Arbeidsgiverskjema AFP";
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("gruppeid")]
        [BindNever]
        public decimal gruppeid { get; set; } = 8818;
        [XmlAttribute("etatid")]
        public string etatid { get; set; }
        [XmlElement("OpplysningerOmArbeidstakeren-grp-8819")]
        public OpplysningerOmArbeidstakerengrp8819 OpplysningerOmArbeidstakerengrp8819 { get; set; }
        [XmlElement("StillingsbrokGrad-grp-8821")]
        public StillingsbrokGradgrp8821 StillingsbrokGradgrp8821 { get; set; }
        [XmlElement("Permisjonsopplysninger-grp-8822")]
        public Permisjonsopplysningergrp8822 Permisjonsopplysningergrp8822 { get; set; }
        [XmlElement("Foretak-grp-8820")]
        public Foretakgrp8820 Foretakgrp8820 { get; set; }
    }
    public class OpplysningerOmArbeidstakerengrp8819
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("gruppeid")]
        [BindNever]
        public decimal gruppeid { get; set; } = 8819;
        [XmlElement("Skjemainstans-grp-8854")]
        public Skjemainstansgrp8854 Skjemainstansgrp8854 { get; set; }
        [XmlElement("OpplysningerOmArbeidstakeren-grp-8855")]
        public OpplysningerOmArbeidstakerengrp8855 OpplysningerOmArbeidstakerengrp8855 { get; set; }
        [XmlElement("Arbeidsforhold-grp-8856")]
        public Arbeidsforholdgrp8856 Arbeidsforholdgrp8856 { get; set; }
    }
    public class Skjemainstansgrp8854
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("gruppeid")]
        [BindNever]
        public decimal gruppeid { get; set; } = 8854;
        [XmlElement("Journalnummer-datadef-33316")]
        public Journalnummerdatadef33316 Journalnummerdatadef33316 { get; set; }
        [XmlElement("IdentifikasjonsnummerKrav-datadef-33317")]
        public IdentifikasjonsnummerKravdatadef33317 IdentifikasjonsnummerKravdatadef33317 { get; set; }
        [XmlElement("TestRepeating-grp-123")]
        public List<Journalnummerdatadef33316> TestRepeatinggrp123 { get; set; }
    }
    public class Journalnummerdatadef33316
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33316;
        [Range(Int32.MinValue, Int32.MaxValue)]
        [XmlText()]
        public decimal value { get; set; }
    }
    public class IdentifikasjonsnummerKravdatadef33317
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33317;
        [MinLength(1)]
        [MaxLength(20)]
        [XmlText()]
        public string value { get; set; }
    }
    public class OpplysningerOmArbeidstakerengrp8855
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("gruppeid")]
        [BindNever]
        public decimal gruppeid { get; set; } = 8855;
        [XmlElement("AnsattNavn-datadef-1223")]
        public AnsattNavndatadef1223 AnsattNavndatadef1223 { get; set; }
        [XmlElement("AnsattFodselsnummer-datadef-1224")]
        public AnsattFodselsnummerdatadef1224 AnsattFodselsnummerdatadef1224 { get; set; }
        [XmlElement("OppgavegiverTelefonnummer-datadef-27335")]
        public OppgavegiverTelefonnummerdatadef27335 OppgavegiverTelefonnummerdatadef27335 { get; set; }
        [XmlElement("OppgavegiverEPost-datadef-27334")]
        public OppgavegiverEPostdatadef27334 OppgavegiverEPostdatadef27334 { get; set; }
        [XmlElement("AnsattSoknadAFPDato-datadef-33282")]
        public AnsattSoknadAFPDatodatadef33282 AnsattSoknadAFPDatodatadef33282 { get; set; }
    }
    public class AnsattNavndatadef1223
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 1223;
        [MinLength(1)]
        [MaxLength(35)]
        [XmlText()]
        public string value { get; set; }
    }
    public class AnsattFodselsnummerdatadef1224
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 1224;
        [XmlText()]
        public string value { get; set; }
    }
    public class OppgavegiverTelefonnummerdatadef27335
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 27335;
        [MinLength(1)]
        [MaxLength(11, ErrorMessage = "ERROR: Max length is 11")]
        [XmlText()]
        public string value { get; set; }
    }
    public class OppgavegiverEPostdatadef27334
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 27334;
        [MinLength(1)]
        [MaxLength(50)]
        [XmlText()]
        public string value { get; set; }
    }
    public class AnsattSoknadAFPDatodatadef33282
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33282;
        [XmlText()]
        public DateTime value { get; set; }
    }
    public class Arbeidsforholdgrp8856
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("gruppeid")]
        [BindNever]
        public decimal gruppeid { get; set; } = 8856;
        [XmlElement("AnsattTiltredelseDato-datadef-1387")]
        public AnsattTiltredelseDatodatadef1387 AnsattTiltredelseDatodatadef1387 { get; set; }
        [XmlElement("AnsattSammenhengendeAnsattAnsettelse-datadef-33267")]
        public AnsattSammenhengendeAnsattAnsettelsedatadef33267 AnsattSammenhengendeAnsattAnsettelsedatadef33267 { get; set; }
        [XmlElement("AnsattArbeidsforholdPabegynt-datadef-33268")]
        public AnsattArbeidsforholdPabegyntdatadef33268 AnsattArbeidsforholdPabegyntdatadef33268 { get; set; }
        [XmlElement("AnsattArbeidsforholdOpphort-datadef-33269")]
        public AnsattArbeidsforholdOpphortdatadef33269 AnsattArbeidsforholdOpphortdatadef33269 { get; set; }
        [XmlElement("AnsattArbeidsforholdOpphortDato-datadef-33261")]
        public AnsattArbeidsforholdOpphortDatodatadef33261 AnsattArbeidsforholdOpphortDatodatadef33261 { get; set; }
        [XmlElement("AnsattLonnUtbetaltDato-datadef-33262")]
        public AnsattLonnUtbetaltDatodatadef33262 AnsattLonnUtbetaltDatodatadef33262 { get; set; }
        [XmlElement("AnsattArbeidsforholdOpphortDato-datadef-33270")]
        public AnsattArbeidsforholdOpphortDatodatadef33270 AnsattArbeidsforholdOpphortDatodatadef33270 { get; set; }
        [XmlElement("AnsattArbeidsforholdOpphortArsak-datadef-33271")]
        public AnsattArbeidsforholdOpphortArsakdatadef33271 AnsattArbeidsforholdOpphortArsakdatadef33271 { get; set; }
        [XmlElement("AnsattLonnSpesifisertManed-datadef-33263")]
        public AnsattLonnSpesifisertManeddatadef33263 AnsattLonnSpesifisertManeddatadef33263 { get; set; }
        [XmlElement("AnsattLonnBelopPeriode-datadef-33264")]
        public AnsattLonnBelopPeriodedatadef33264 AnsattLonnBelopPeriodedatadef33264 { get; set; }
    }
    public class AnsattTiltredelseDatodatadef1387
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 1387;
        [XmlText()]
        public DateTime value { get; set; }
    }
    public class AnsattSammenhengendeAnsattAnsettelsedatadef33267
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33267;
        [MinLength(1)]
        [MaxLength(3)]
        [XmlText()]
        public string value { get; set; }
    }
    public class AnsattArbeidsforholdPabegyntdatadef33268
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33268;
        [XmlText()]
        public DateTime value { get; set; }
    }
    public class AnsattArbeidsforholdOpphortdatadef33269
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33269;
        [MinLength(1)]
        [MaxLength(3)]
        [XmlText()]
        public string value { get; set; }
    }
    public class AnsattArbeidsforholdOpphortDatodatadef33261
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33261;
        [XmlText()]
        public DateTime value { get; set; }
    }
    public class AnsattLonnUtbetaltDatodatadef33262
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33262;
        [XmlText()]
        public DateTime value { get; set; }
    }
    public class AnsattArbeidsforholdOpphortDatodatadef33270
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33270;
        [XmlText()]
        public DateTime value { get; set; }
    }
    public class AnsattArbeidsforholdOpphortArsakdatadef33271
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33271;
        [MinLength(1)]
        [MaxLength(10)]
        [XmlText()]
        public string value { get; set; }
    }
    public class AnsattLonnSpesifisertManeddatadef33263
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33263;
        [Range(Int32.MinValue, Int32.MaxValue)]
        [XmlText()]
        public decimal value { get; set; }
    }
    public class AnsattLonnBelopPeriodedatadef33264
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33264;
        [Range(Int32.MinValue, Int32.MaxValue)]
        [XmlText()]
        public decimal value { get; set; }
    }
    public class StillingsbrokGradgrp8821
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("gruppeid")]
        [BindNever]
        public decimal gruppeid { get; set; } = 8821;
        [XmlElement("NavarendeStillingsbrokGrad-grp-8857")]
        public NavarendeStillingsbrokGradgrp8857 NavarendeStillingsbrokGradgrp8857 { get; set; }
        [XmlElement("ForhenvarendeStillingsbrokGrad-grp-8858")]
        public ForhenvarendeStillingsbrokGradgrp8858 ForhenvarendeStillingsbrokGradgrp8858 { get; set; }
    }
    public class NavarendeStillingsbrokGradgrp8857
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("gruppeid")]
        [BindNever]
        public decimal gruppeid { get; set; } = 8857;
        [XmlElement("AnsattHeltidDeltidSesong-datadef-33273")]
        public AnsattHeltidDeltidSesongdatadef33273 AnsattHeltidDeltidSesongdatadef33273 { get; set; }
        [XmlElement("AnsattStillingsprosent-datadef-31808")]
        public AnsattStillingsprosentdatadef31808 AnsattStillingsprosentdatadef31808 { get; set; }
        [XmlElement("NarBegynteArbeidstakerenINavarendeStillingsbrokGrad-grp-8860")]
        public NarBegynteArbeidstakerenINavarendeStillingsbrokGradgrp8860 NarBegynteArbeidstakerenINavarendeStillingsbrokGradgrp8860 { get; set; }
    }
    public class AnsattHeltidDeltidSesongdatadef33273
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33273;
        [MinLength(1)]
        [MaxLength(20)]
        [XmlText()]
        public string value { get; set; }
    }
    public class AnsattStillingsprosentdatadef31808
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 31808;
        [Range(1, Int32.MaxValue)]
        [XmlText()]
        public decimal value { get; set; }
    }
    public class NarBegynteArbeidstakerenINavarendeStillingsbrokGradgrp8860
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("gruppeid")]
        [BindNever]
        public decimal gruppeid { get; set; } = 8860;
        [XmlElement("AnsattPabegyntStillingDato-datadef-33272")]
        public AnsattPabegyntStillingDatodatadef33272 AnsattPabegyntStillingDatodatadef33272 { get; set; }
    }
    public class AnsattPabegyntStillingDatodatadef33272
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33272;
        [XmlText()]
        public DateTime value { get; set; }
    }
    public class ForhenvarendeStillingsbrokGradgrp8858
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("gruppeid")]
        [BindNever]
        public decimal gruppeid { get; set; } = 8858;
        [XmlElement("AnsattHeltidDeltidSesongArbeidetIkkeIForetaketTidligere-datadef-33274")]
        public AnsattHeltidDeltidSesongArbeidetIkkeIForetaketTidligeredatadef33274 AnsattHeltidDeltidSesongArbeidetIkkeIForetaketTidligeredatadef33274 { get; set; }
        [XmlElement("AnsattStillingsbrokTidligereProsent-datadef-33277")]
        public AnsattStillingsbrokTidligereProsentdatadef33277 AnsattStillingsbrokTidligereProsentdatadef33277 { get; set; }
        [XmlElement("TidsromForDenneStillingsbrokenGraden-grp-8859")]
        public TidsromForDenneStillingsbrokenGradengrp8859 TidsromForDenneStillingsbrokenGradengrp8859 { get; set; }
    }
    public class AnsattHeltidDeltidSesongArbeidetIkkeIForetaketTidligeredatadef33274
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33274;
        [MinLength(1)]
        [MaxLength(30)]
        [XmlText()]
        public string value { get; set; }
    }
    public class AnsattStillingsbrokTidligereProsentdatadef33277
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33277;
        [Range(1, Int32.MaxValue)]
        [XmlText()]
        public decimal value { get; set; }
    }
    public class TidsromForDenneStillingsbrokenGradengrp8859
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("gruppeid")]
        [BindNever]
        public decimal gruppeid { get; set; } = 8859;
        [XmlElement("AnsattStillingTidligereFra-datadef-33275")]
        public AnsattStillingTidligereFradatadef33275 AnsattStillingTidligereFradatadef33275 { get; set; }
        [XmlElement("AnsattStillingTidligereTil-datadef-33276")]
        public AnsattStillingTidligereTildatadef33276 AnsattStillingTidligereTildatadef33276 { get; set; }
    }
    public class AnsattStillingTidligereFradatadef33275
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33275;
        [XmlText()]
        public DateTime value { get; set; }
    }
    public class AnsattStillingTidligereTildatadef33276
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33276;
        [XmlText()]
        public DateTime value { get; set; }
    }
    public class Permisjonsopplysningergrp8822
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("gruppeid")]
        [BindNever]
        public decimal gruppeid { get; set; } = 8822;
        [XmlElement("AnsattSykemeldt-datadef-33265")]
        public AnsattSykemeldtdatadef33265 AnsattSykemeldtdatadef33265 { get; set; }
        [XmlElement("AnsattPermittert-datadef-33278")]
        public AnsattPermittertdatadef33278 AnsattPermittertdatadef33278 { get; set; }
        [XmlElement("AnsattPermittertPermiteringsgrad-datadef-33279")]
        public AnsattPermittertPermiteringsgraddatadef33279 AnsattPermittertPermiteringsgraddatadef33279 { get; set; }
        [XmlElement("AnsattPermittertDato-datadef-33283")]
        public AnsattPermittertDatodatadef33283 AnsattPermittertDatodatadef33283 { get; set; }
        [XmlElement("AnsattPermisjon-datadef-33280")]
        public AnsattPermisjondatadef33280 AnsattPermisjondatadef33280 { get; set; }
        [XmlElement("AnsattPermisjonType-datadef-33281")]
        public AnsattPermisjonTypedatadef33281 AnsattPermisjonTypedatadef33281 { get; set; }
        [XmlElement("AnsattYtelserMottattUtenArbeidsplikt-datadef-33293")]
        public AnsattYtelserMottattUtenArbeidspliktdatadef33293 AnsattYtelserMottattUtenArbeidspliktdatadef33293 { get; set; }
        [XmlElement("AnsattEierandel20EllerMer-datadef-33294")]
        public AnsattEierandel20EllerMerdatadef33294 AnsattEierandel20EllerMerdatadef33294 { get; set; }
        [XmlElement("AnsattIForetaketHovedbeskjeftigelse-datadef-33284")]
        public AnsattIForetaketHovedbeskjeftigelsedatadef33284 AnsattIForetaketHovedbeskjeftigelsedatadef33284 { get; set; }
        [XmlElement("AnsattStillingsbrokUnder20-datadef-33285")]
        public AnsattStillingsbrokUnder20datadef33285 AnsattStillingsbrokUnder20datadef33285 { get; set; }
    }
    public class AnsattSykemeldtdatadef33265
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33265;
        [MinLength(1)]
        [MaxLength(3)]
        [XmlText()]
        public string value { get; set; }
    }
    public class AnsattPermittertdatadef33278
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33278;
        [MinLength(1)]
        [MaxLength(3)]
        [XmlText()]
        public string value { get; set; }
    }
    public class AnsattPermittertPermiteringsgraddatadef33279
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33279;
        [Range(1, Int32.MaxValue)]
        [XmlText()]
        public decimal value { get; set; }
    }
    public class AnsattPermittertDatodatadef33283
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33283;
        [XmlText()]
        public DateTime value { get; set; }
    }
    public class AnsattPermisjondatadef33280
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33280;
        [MinLength(1)]
        [MaxLength(3)]
        [XmlText()]
        public string value { get; set; }
    }
    public class AnsattPermisjonTypedatadef33281
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33281;
        [MinLength(1)]
        [MaxLength(100)]
        [XmlText()]
        public string value { get; set; }
    }
    public class AnsattYtelserMottattUtenArbeidspliktdatadef33293
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33293;
        [MinLength(1)]
        [MaxLength(3)]
        [XmlText()]
        public string value { get; set; }
    }
    public class AnsattEierandel20EllerMerdatadef33294
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33294;
        [MinLength(1)]
        [MaxLength(2)]
        [XmlText()]
        public string value { get; set; }
    }
    public class AnsattIForetaketHovedbeskjeftigelsedatadef33284
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33284;
        [MinLength(1)]
        [MaxLength(2)]
        [XmlText()]
        public string value { get; set; }
    }
    public class AnsattStillingsbrokUnder20datadef33285
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33285;
        [MinLength(1)]
        [MaxLength(3)]
        [XmlText()]
        public string value { get; set; }
    }
    public class Foretakgrp8820
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("gruppeid")]
        [BindNever]
        public decimal gruppeid { get; set; } = 8820;
        [XmlElement("InnsenderNavn-datadef-3443")]
        public InnsenderNavndatadef3443 InnsenderNavndatadef3443 { get; set; }
        [XmlElement("EnhetNavnEndring-datadef-33286")]
        public EnhetNavnEndringdatadef33286 EnhetNavnEndringdatadef33286 { get; set; }
        [XmlElement("InnsenderAdresse-datadef-3445")]
        public InnsenderAdressedatadef3445 InnsenderAdressedatadef3445 { get; set; }
        [XmlElement("InnsenderPostnummer-datadef-11339")]
        public InnsenderPostnummerdatadef11339 InnsenderPostnummerdatadef11339 { get; set; }
        [XmlElement("InnsenderPoststed-datadef-11340")]
        public InnsenderPoststeddatadef11340 InnsenderPoststeddatadef11340 { get; set; }
        [XmlElement("EnhetTelefonnummer-datadef-755")]
        public EnhetTelefonnummerdatadef755 EnhetTelefonnummerdatadef755 { get; set; }
        [XmlElement("EnhetTelefaksnummer-datadef-1816")]
        public EnhetTelefaksnummerdatadef1816 EnhetTelefaksnummerdatadef1816 { get; set; }
        [XmlElement("EnhetEPost-datadef-21591")]
        public EnhetEPostdatadef21591 EnhetEPostdatadef21591 { get; set; }
        [XmlElement("ForetakOrganisasjonsnummer-datadef-33552")]
        public ForetakOrganisasjonsnummerdatadef33552 ForetakOrganisasjonsnummerdatadef33552 { get; set; }
        [XmlElement("EnhetOrganisasjonsnummerNy-datadef-22684")]
        public EnhetOrganisasjonsnummerNydatadef22684 EnhetOrganisasjonsnummerNydatadef22684 { get; set; }
        [XmlElement("EnhetOrganisasjonsnummerEndringArsak-datadef-33287")]
        public EnhetOrganisasjonsnummerEndringArsakdatadef33287 EnhetOrganisasjonsnummerEndringArsakdatadef33287 { get; set; }
        [XmlElement("BedriftOrganisasjonsnummer-datadef-19")]
        public BedriftOrganisasjonsnummerdatadef19 BedriftOrganisasjonsnummerdatadef19 { get; set; }
        [XmlElement("BedriftOrganisasjonsnummerNy-datadef-33292")]
        public BedriftOrganisasjonsnummerNydatadef33292 BedriftOrganisasjonsnummerNydatadef33292 { get; set; }
        [XmlElement("BedriftOrganisasjonsnummerEndringArsak-datadef-33288")]
        public BedriftOrganisasjonsnummerEndringArsakdatadef33288 BedriftOrganisasjonsnummerEndringArsakdatadef33288 { get; set; }
        [XmlElement("AnsattAnsettelsePgaFusjonFisjonOverdragelse-datadef-33289")]
        public AnsattAnsettelsePgaFusjonFisjonOverdragelsedatadef33289 AnsattAnsettelsePgaFusjonFisjonOverdragelsedatadef33289 { get; set; }
        [XmlElement("EnhetNavnEndring-datadef-31")]
        public EnhetNavnEndringdatadef31 EnhetNavnEndringdatadef31 { get; set; }
        [XmlElement("Merknad-datadef-33290")]
        public Merknaddatadef33290 Merknaddatadef33290 { get; set; }
    }
    public class InnsenderNavndatadef3443
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 3443;
        [MinLength(1)]
        [MaxLength(175)]
        [XmlText()]
        public string value { get; set; }
    }
    public class EnhetNavnEndringdatadef33286
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33286;
        [MinLength(1)]
        [MaxLength(175)]
        [XmlText()]
        public string value { get; set; }
    }
    public class InnsenderAdressedatadef3445
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 3445;
        [MinLength(1)]
        [MaxLength(105)]
        [XmlText()]
        public string value { get; set; }
    }
    public class InnsenderPostnummerdatadef11339
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 11339;
        [RegularExpression(@"[0-9]{4}")]
        [XmlText()]
        public string value { get; set; }
    }
    public class InnsenderPoststeddatadef11340
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 11340;
        [MinLength(1)]
        [MaxLength(35)]
        [XmlText()]
        public string value { get; set; }
    }
    public class EnhetTelefonnummerdatadef755
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 755;
        [MinLength(1)]
        [MaxLength(13)]
        [XmlText()]
        public string value { get; set; }
    }
    public class EnhetTelefaksnummerdatadef1816
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 1816;
        [MinLength(1)]
        [MaxLength(13)]
        [XmlText()]
        public string value { get; set; }
    }
    public class EnhetEPostdatadef21591
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 21591;
        [MinLength(1)]
        [MaxLength(100)]
        [XmlText()]
        public string value { get; set; }
    }
    public class ForetakOrganisasjonsnummerdatadef33552
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33552;
        [XmlText()]
        public string value { get; set; }
    }
    public class EnhetOrganisasjonsnummerNydatadef22684
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 22684;
        [XmlText()]
        public string value { get; set; }
    }
    public class EnhetOrganisasjonsnummerEndringArsakdatadef33287
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33287;
        [MinLength(1)]
        [MaxLength(500)]
        [XmlText()]
        public string value { get; set; }
    }
    public class BedriftOrganisasjonsnummerdatadef19
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 19;
        [XmlText()]
        public string value { get; set; }
    }
    public class BedriftOrganisasjonsnummerNydatadef33292
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33292;
        [XmlText()]
        public string value { get; set; }
    }
    public class BedriftOrganisasjonsnummerEndringArsakdatadef33288
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33288;
        [MinLength(1)]
        [MaxLength(500)]
        [XmlText()]
        public string value { get; set; }
    }
    public class AnsattAnsettelsePgaFusjonFisjonOverdragelsedatadef33289
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33289;
        [MinLength(1)]
        [MaxLength(3)]
        [XmlText()]
        public string value { get; set; }
    }
    public class EnhetNavnEndringdatadef31
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 31;
        [MinLength(1)]
        [MaxLength(175)]
        [XmlText()]
        public string value { get; set; }
    }
    public class Merknaddatadef33290
    {
        [Range(1, Int32.MaxValue)]
        [XmlAttribute("orid")]
        [BindNever]
        public decimal orid { get; set; } = 33290;
        [MinLength(1)]
        [MaxLength(500)]
        [XmlText()]
        public string value { get; set; }
    }
}
