using BO4E.meta;

namespace BO4E.ENUM
{
    /// <summary>
    /// Stromverbrauchsart/W�rmenutzung Marktlokation
    /// EDIFACT values of <see cref="Waermenutzung"/>
    /// </summary>
    /// <author>Hochfrequenz Unternehmensberatung GmbH</author>
    public enum WaermenutzungEdi
    {
        /// <summary>Z56: Speicherheizung</summary>
        [Mapping(Waermenutzung.Speicherheizung)] 
        Z56,
        /// <summary>Z57: W�rmepumpe</summary>
        [Mapping(Waermenutzung.Waermepumpe)]
        Z57,
        ///<summary>Z61: Direktheizung</summary>
        [Mapping(Waermenutzung.Direktheizung)]
        Z61
    }
}