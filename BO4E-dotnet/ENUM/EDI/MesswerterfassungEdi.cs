using BO4E.meta;

namespace BO4E.ENUM
{

    /// <summary>
    /// Messwerterfassung
    /// EDIFACT values of <see cref="Messwerterfassung"/>
    /// </summary>
    /// <author>Hochfrequenz Unternehmensberatung GmbH</author>
    public enum MesswerterfassungEdi
    {
        /// <summary>AMR: fernauslesbare Z�hler</summary>
        [Mapping(Messwerterfassung.FERNAUSLESBARE)]
        AMR,
        /// <summary>MMR: manuell ausgelesene Z�hler</summary>
        [Mapping(Messwerterfassung.MANUELL_AUSGELESENE)]
        MMR,       
    }
}