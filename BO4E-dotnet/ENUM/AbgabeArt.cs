namespace BO4E.ENUM
{
    /// <summary>Schwachlastf�higkeit Marktlokation</summary>
    public enum AbgabeArt
    {
        /// <summary>KAS: f�r alle konzessionsvertraglichen Sonderregelungen, die nicht in die Systematik der KAV eingegliedert sind</summary>
        KAS,
        /// <summary>SA: Sondervertragskunden  1 kV, Preis nach � 2 (3) (f�r Strom 0,11 ct/kWh und f�r Gas 0,03 ct/kWh)</summary>
        SA,
        /// <summary>SAS: Kennzeichnung, dass ein abweichender Preis f�r Sondervertragskunden vorliegt</summary>
        SAS,
        /// <summary>TA: Tarifkunden, f�r Strom � 2. (2) 1b HT bzw.ET(hohe KA) und f�r Gas � 2 (2) 2b</summary>
        TA,
        /// <summary>TAS: Kennzeichnung, dass ein abweichender Preis f�r Tarifkunden vorliegt</summary>
        TAS,
        /// <summary>TK: f�r Gas nach KAV � 2 (2) 2a bei ausschlie�licher Nutzung zum Kochen und Warmwassererzeugung</summary>
        TK,
        /// <summary>TKS: Kennzeichnung, wenn nach KAV � 2 (2) 2a ein anderen Preis zu verwenden ist</summary>
        TKS,
        /// <summary>TS: f�r Strom mit Schwachlast � 2. (2) 1a NT(niedrige KA, 0,61 ct/kWh)</summary>
        TS,
        /// <summary>TSS: Kennzeichnung, dass ein abweichender Preis f�r Schwachlast angewendet wird</summary>
        TSS,
    }
}