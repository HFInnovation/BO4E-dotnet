using BO4E.meta;

namespace BO4E.ENUM.EDI
{
    /// <summary>
    /// EDIFACT values of <see cref="Verbrauchsart"/>
    /// </summary>
    /// <author>Hochfrequenz Unternehmensberatung GmbH</author>
    public enum VerbrauchsartEdi
    {
        /// <summary>
        /// Kraft/Licht
        /// </summary>
        /// Hierunter ist Strom zu verstehen, der ausschlie�lich zum Betrieb von
        /// Endverbrauchsger�ten (z.B. Radio, Fernseher, K�hlschrank, Beleuchtung...)
        /// genutzt wird.
        [Mapping(Verbrauchsart.KL)]
        Z64,
        /// <summary>W�rme</summary>
        /// Hierunter ist Strom zu verstehen, der zur W�rmebedarfsdeckung (z.B. 
        /// Standspeicherheizung, Fu�bodenspeicherheizungen, W�rmepumpe....) eingesetzt
        /// wird. Bei Nutzung dieses Qualifiers dient die OBIS ausschlie�lich diesem Zweck. 
        /// Hierunter fallen Marktlokationen, bei den die W�rme in aller Regel mit einer 
        /// separaten Messung erfasst werden
        [Mapping(Verbrauchsart.W)]
        Z65,
        /// <summary>
        /// Kraft/Licht/W�rme
        /// </summary>
        /// Bei gemeinsam gemessenen Marktlokationen wird Strom sowohl f�r Endverbrauchsger�te
        /// als auch zur W�rmebedarfsdeckung eingesetzt. Bei diesem kombinierten Verbrauchsverhalten
        /// ist dieser Qualifier zu nutzen.
        [Mapping(Verbrauchsart.KLW)]
        Z66
    }
}
