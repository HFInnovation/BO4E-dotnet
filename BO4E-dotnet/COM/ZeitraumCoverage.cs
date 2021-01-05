using System;
using System.Runtime.Serialization;

using BO4E.ENUM;
using BO4E.meta;
using BO4E.meta.LenientConverters;

using Newtonsoft.Json;

using ProtoBuf;

namespace BO4E.COM
{
    /// <summary>Diese Komponente wird zur Abbildung von Zeiträumen in Form von Dauern oder der Angabe von Start und Ende verwendet.</summary>
    [ProtoContract]
    public class ZeitraumCoverage : Zeitraum
    {
        /// <summary>
        /// ratio of time with data present compared to <see cref="ReferenceTimeFrame"/>.
        /// 1.0 means 100% coverage.
        /// </summary>
        [JsonProperty(PropertyName = "coverage", Required = Required.AllowNull)]
        public decimal? Coverage { get; set; }

    }
}