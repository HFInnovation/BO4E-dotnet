using System;
using System.Collections.Generic;

using BO4E.COM;
using BO4E.ENUM;
using BO4E.meta;

using Newtonsoft.Json;

using static BO4E.Reporting.CompletenessReport;

namespace BO4E.Reporting
{
    /// <summary>
    /// A completeness report contains information about the completeness of a pure
    /// <see cref="Energiemenge"/>. In this context "pure" means, that the Energiemenge
    /// does only contain one distinct set of (<see cref="Verbrauch.Obiskennzahl"/>, <see cref="Verbrauch.Einheit"/>,
    /// <see cref="Verbrauch.Wertermittlungsverfahren"/>).
    /// </summary>
    public class CompletenessReportLight : Report
    {
        /// <summary>
        /// all information like e.g. <see cref="Coverage"/> is normalised to this reference time frame.
        /// Must only be null if an error occurred and <see cref="ErrorMessage"/> is not null.
        /// </summary>
        [JsonProperty(PropertyName = "referenceTimeFrame", Required = Required.AllowNull, Order = 7)]
        public IEnumerable<ZeitraumCoverage> TimeFrameList { get; set; }

        /// <summary>
        /// <see cref="Energiemenge.lokationsId"/>
        /// </summary>
        [DataCategory(DataCategory.POD)]
        [JsonProperty(PropertyName = "lokationsId", Required = Required.Always, Order = 8)]
        public string LokationsId { get; set; }

        /// <summary>
        /// <see cref="Verbrauch.Obiskennzahl"/>
        /// </summary>
        [JsonProperty(PropertyName = "obiskennzahl", Required = Required.Default, Order = 5)]
        public string Obiskennzahl { get; set; }

        /// <summary>
        /// <see cref="Verbrauch.Einheit"/>
        /// </summary>
        [JsonProperty(PropertyName = "einheit", Required = Required.Default, Order = 6)]
        public Mengeneinheit Einheit { get; set; }

        /// <summary>
        /// <see cref="Verbrauch.Wertermittlungsverfahren"/>
        /// </summary>
        [JsonProperty(PropertyName = "wertermittlungsverfahren", Required = Required.Default, Order = 7)]
        public Wertermittlungsverfahren wertermittlungsverfahren { get; set; }

        /// <summary>
        /// values
        /// </summary>
        [JsonProperty(PropertyName = "values", Required = Required.Default, Order = 5)]
        [DataCategory(DataCategory.METER_READING)]
        public List<BasicVerbrauch> Values { get; set; }

        /// <summary>
        /// gaps are continous values (<see cref="Values"/>) with value null.
        /// </summary>
        [JsonProperty(PropertyName = "gaps", Required = Required.Default, Order = 6)]
        public List<BasicVerbrauch> Gaps { get; set; }

        /// <summary>
        /// optional field for storing error messages
        /// </summary>
        [JsonProperty(PropertyName = "_errorMessage", Required = Required.Default)]
        public string ErrorMessage { get; set; }

    }
}
