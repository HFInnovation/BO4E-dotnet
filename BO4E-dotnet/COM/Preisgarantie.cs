using BO4E.ENUM;
using Newtonsoft.Json;

namespace BO4E.COM
{
    /// <summary>
    /// Definition für eine Preisgarantie mit der Möglichkeit verschiedener Ausprägungen.
    /// </summary>
    public class Preisgarantie : COM
    {
        /// <summary>Freitext zur Beschreibung der Preisgarantie</summary>
        [JsonProperty(Required = Required.Default)]
        public string beschreibung;
        /// <summary>Festlegung, auf welche Preisbestandteile die Garantie gewährt wird. Details <see cref="Preisgarantietyp" /></summary>
        [JsonProperty(Required = Required.Always)]
        public Preisgarantietyp preisgarantietyp;
        /// <summary>Zeitraum, bis zu dem die Preisgarantie gilt, z.B. bis zu einem absolutem / fixem Datum oder als Laufzeit in Monaten. Details <see cref="Zeitraum" /></summary>
        [JsonProperty(Required = Required.Always)]
        public Zeitraum zeitlicheGueltigkeit;
    }
}