namespace Hekki.UI.ViewModels
{
    public class RaceViewModel
    {
        public int RegulationId { get; }

        public RaceViewModel(int regulationId)
        {
            RegulationId = regulationId;
        }
    }
}