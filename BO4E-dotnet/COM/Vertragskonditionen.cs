using Newtonsoft.Json;
namespace BO4E.COM

{
    /// <summary>Abbildung für Vertragskonditionen. Die Komponente wird sowohl im Vertrag als auch im Tarif verwendet.</summary>
    public class Vertragskonditionen : COM
    {
        /// <summary>Freitext zur Beschreibung der Konditionen, z.B. "Standardkonditionen Gas"</summary>
        [JsonProperty(Required = Required.Default)]
        public string beschreibung;
        /// <summary>Anzahl der vereinbarten Abschläge pro Jahr, z.B. 12</summary>
        [JsonProperty(Required = Required.Default)]
        public string anzahlAbschlaege;
        /// <summary>Über diesen Zeitraum läuft der Vertrag. Details <see cref="Zeitraum" /></summary>
        [JsonProperty(Required = Required.Default)]
        public Zeitraum vertragslaufzeit;
        /// <summary>Innerhalb dieser Frist kann der Vertrag gekündigt werden. Details <see cref="Zeitraum" /></summary>
        [JsonProperty(Required = Required.Default)]
        public Zeitraum kuendigungsfrist;
        /// <summary>Falls der Vertrag nicht gekündigt wird, verlängert er sich automatisch um die hier angegebene Zeit. Details <see cref="Zeitraum" /></summary>
        [JsonProperty(Required = Required.Default)]
        public Zeitraum vertragsverlaengerung;
        /// <summary>In diesen Zyklen werden Abschläge gestellt. Details <see cref="Zeitraum" />. Alternativ kann auch die Anzahl in den Konditionen angeben werden."</summary>
        [JsonProperty(Required = Required.Default)]
        public Zeitraum abschlagszyklus;
    }
}