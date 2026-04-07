namespace Hekki.UI
{
    public record RegulationChoiceItem
    (
        string Title,
        RegulationChoiceKind Kind,
        int? RegulationId
    );

    public enum RegulationChoiceKind
    {
        Create,
        Existing
    }
}
