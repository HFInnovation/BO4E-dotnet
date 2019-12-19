﻿using BO4E.ENUM;
using Newtonsoft.Json;

namespace BO4E.BO
{
    /// <summary>
    /// Die Variante des Preisblattmodells zur Abbildung von allgemeinen Abgaben.
    /// </summary>
    public class PreisblattUmlagen : Preisblatt
    {
        /// <summary>
        /// Sparte, auf die sich die Umlage bezieht. 
        /// </summary>
        [JsonProperty(Required = Required.Always, Order = -1)]
        public Sparte sparte;
    }
}