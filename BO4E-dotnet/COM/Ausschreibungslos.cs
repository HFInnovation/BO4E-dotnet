using System.Collections.Generic;

using BO4E.ENUM;

using Newtonsoft.Json;

using ProtoBuf;

namespace BO4E.COM
{
    /// <summary>Eine Komponente zur Abbildung einzelner Lose einer Ausschreibung.</summary>
    [ProtoContract]
    public class Ausschreibungslos : Com
    {
        /// <summary>Laufende Nummer des Loses</summary>
        [JsonProperty(PropertyName = "losnummer", Required = Required.Always)]
        [ProtoMember(3)]
        public string Losnummer { get; set; }
        /// <summary>Bezeichnung der Ausschreibung</summary>
        [JsonProperty(PropertyName = "bezeichung", Required = Required.Always)]
        [ProtoMember(4)]
        public string Bezeichung { get; set; }
        /// <summary>Bemerkung des Kunden zur Ausschreibung</summary>
        [JsonProperty(PropertyName = "bemerkung", Required = Required.Default)]
        [ProtoMember(5)]
        public string Bemerkung { get; set; }
        /// <summary>Bezeichnung der Preismodelle in Ausschreibungen für die Energielieferung. Details <see cref="ENUM.Preismodell" /></summary>
        [JsonProperty(PropertyName = "preismodell", Required = Required.Always)]
        [ProtoMember(6)]
        public Preismodell Preismodell { get; set; }
        /// <summary>Unterscheidungsmöglichkeiten für die Sparte. Details <see cref="Sparte" /></summary>
        [JsonProperty(PropertyName = "energieart", Required = Required.Always)]
        [ProtoMember(7)]
        public Sparte Energieart { get; set; }
        /// <summary>Aufzählung der Möglichkeiten zur Rechnungslegung in Ausschreibungen. Details <see cref="Rechnungslegung" /></summary>
        [JsonProperty(PropertyName = "wunschRechnungslegung", Required = Required.Always)]
        [ProtoMember(8)]
        public Rechnungslegung WunschRechnungslegung { get; set; }
        /// <summary>Aufzählung der Möglichkeiten zu Vertragsformen in Ausschreibungen. Details <see cref="Vertragsform" /></summary>
        [JsonProperty(PropertyName = "wunschVertragsform", Required = Required.Always)]
        [ProtoMember(9)]
        public Vertragsform WunschVertragsform { get; set; }
        /// <summary>Name des Lizenzpartners</summary>
        [JsonProperty(PropertyName = "betreutDurch", Required = Required.Always)]
        [ProtoMember(10)]
        public string BetreutDurch { get; set; }
        /// <summary>Anzahl der Lieferstellen in dieser Ausschreibung</summary>
        [JsonProperty(PropertyName = "anzahlLieferstellen", Required = Required.Always)]
        [ProtoMember(11)]
        public int AnzahlLieferstellen { get; set; } // does this make sense? lieferstellen.Size()?
        /// <summary>Die ausgeschriebenen Lieferstellen. Details <see cref="Ausschreibungsdetail" /></summary>
        [JsonProperty(PropertyName = "lieferstellen", Required = Required.Always)]
        [ProtoMember(12)]
        public List<Ausschreibungsdetail> Lieferstellen { get; set; }
        /// <summary>Gibt den Gesamtjahresverbrauch (z.B. in kWh) aller in diesem Los enthaltenen Lieferstellen an.</summary> <see cref="Menge"/>
        [JsonProperty(PropertyName = "gesamtmenge", Required = Required.Default)]
        [ProtoMember(13)]
        public Menge Gesamtmenge { get; set; }
        /// <summary>Mindestmenge Toleranzband (kWh, %)</summary> <see cref="Menge"/> 
        [JsonProperty(PropertyName = "wunschMindestmenge", Required = Required.Default)]
        [ProtoMember(14)]
        public Menge WunschMindestmenge { get; set; }
        /// <summary>Maximalmenge Toleranzband (kWh, %)</summary> <see cref="Menge"/> 
        [JsonProperty(PropertyName = "wunschMaximalmenge", Required = Required.Default)]
        [ProtoMember(15)]
        public Menge WunschMaximalmenge { get; set; }
        /// <summary>Angabe, in welchem Intervall die Angebotsabgabe wiederholt werden darf. Angabe nur gesetzt für die 2. Phase bei öffentlich-rechtlichen Ausschreibungen, ansonsten NULL</summary> <see cref="Zeitraum" />
        [JsonProperty(PropertyName = "wiederholungsintervall", Required = Required.Default)]
        [ProtoMember(16)]
        public Zeitraum Wiederholungsintervall { get; set; }
        /// <summary>Zeitraum, für den die in diesem Los enthaltenen Lieferstellen beliefert werden sollen</summary> <see cref="Zeitraum" />
        [JsonProperty(PropertyName = "lieferzeitraum", Required = Required.Default)]
        [ProtoMember(17)]
        public Zeitraum Lieferzeitraum { get; set; }
        /// <summary>Kundenwunsch zur Kündigungsfrist in der Ausschreibung.</summary> <see cref="Zeitraum" />
        [JsonProperty(PropertyName = "wunschKuendingungsfrist", Required = Required.Default)]
        [ProtoMember(18)]
        public Zeitraum WunschKuendingungsfrist { get; set; }
        /// <summary>Kundenwunsch zum Zahlungsziel in der Ausschreibung.</summary> <see cref="Zeitraum" />
        [JsonProperty(PropertyName = "wunschZahlungsziel", Required = Required.Default)]
        [ProtoMember(19)]
        public Zeitraum WunschZahlungsziel { get; set; }
    }
}