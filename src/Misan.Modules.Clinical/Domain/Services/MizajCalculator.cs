using System;

namespace Misan.Modules.Clinical.Domain.Services;

public enum MizajType
{
    Sanguine,    // Damvi (Hot & Wet)
    Choleric,    // Safravi (Hot & Dry)
    Phlegmatic,  // Balghami (Cold & Wet)
    Melancholic  // Saudavi (Cold & Dry)
}

public class MizajCalculator
{
    public MizajType Calculate(bool isWarm, bool isMoist)
    {
        if (isWarm && isMoist) return MizajType.Sanguine;
        if (isWarm && !isMoist) return MizajType.Choleric;
        if (!isWarm && isMoist) return MizajType.Phlegmatic;
        return MizajType.Melancholic;
    }

    // Advanced calculation could take pulse, urine color, etc.
    // For MVP, simplistic Hot/Cold/Dry/Wet matrix.
}
