using BO4E.ENUM;
using BO4E.meta;

using Newtonsoft.Json;

namespace BO4E.BO
{
    // Ordering is defined in Geschaeftspartner
    /// <summary>
    /// Objekt zur Kommunikation von Marktteilnehmern jeglicher Art.
    /// </summary>
    //[ProtoContract]
    public class Marktteilnehmer : Geschaeftspartner
    {
        /// <summary>
        /// empty constructor
        /// </summary>
        public Marktteilnehmer()
        {
            Gewerbekennzeichnung = true;
        }

        /// <summary>Gibt im Klartext die Bezeichnung der Marktrolle an.</summary>
        /// <example>LF</example>
        [JsonProperty(Required = Required.Always, Order = 19, PropertyName = "marktrolle")]

        [System.Text.Json.Serialization.JsonPropertyName("marktrolle")]
        //[ProtoMember(19)]
        public Marktrolle Marktrolle { get; set; }

        /// <summary>Gibt die Codenummer der Marktrolle an.</summary>
        /// <example>"9903100000006"</example>
        [BoKey(true)]
        [JsonProperty(Required = Required.Always, Order = 20, PropertyName = "rollencodenummer")]

        [System.Text.Json.Serialization.JsonPropertyName("rollencodenummer")]
        //[ProtoMember(20)]
        public string Rollencodenummer { get; set; }

        /// <summary>Gibt den Typ des Codes an.</summary>
        /// <example>BDEW (instead of 293, 500 etc.)</example>
        [JsonProperty(Required = Required.Always, Order = 21, PropertyName = "rollencodetyp")]

        [System.Text.Json.Serialization.JsonPropertyName("rollencodetyp")]
        //[ProtoMember(21)]
        public Rollencodetyp Rollencodetyp { get; set; }

        /// <summary>
        /// Die 1:1-Kommunikationsadresse des Marktteilnehmers. Diese wird in der
        /// Marktkommunikation verwendet.
        /// </summary>
        [JsonProperty(Required = Required.Default, Order = 22, PropertyName = "makoadresse")] // relaxed from always to default to make COM.Marktrolle obsolete.

        [System.Text.Json.Serialization.JsonPropertyName("makoadresse")]
        //[ProtoMember(22)]
        public string Makoadresse { get; set; }

        /// <summary>
        /// Ansprechpartner as in EDIFACT NAD+MS, that includes e.g. the email address of a natural person.
        /// </summary>
        [NonOfficial(NonOfficialCategory.CUSTOMER_REQUIREMENTS)]
        [JsonProperty(Required = Required.Default, Order = 23, PropertyName = "ansprechpartner")]

        [System.Text.Json.Serialization.JsonPropertyName("ansprechpartner")]
        //[ProtoMember(23)]
        public Ansprechpartner Ansprechpartner { get; set; }
    }
}