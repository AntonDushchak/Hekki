namespace Hekki.UI.Enums
{
    public record RegulationChoiceItem
    (
        string Title,
        RegulationChoiceKind Kind,
        int? RegulationId
    );
}
