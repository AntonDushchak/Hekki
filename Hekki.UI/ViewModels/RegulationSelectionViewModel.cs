using Hekki.Application.Abstrations;
using Hekki.Domain.Models;
using System.Collections.ObjectModel;

namespace Hekki.UI.ViewModels
{
    public class RegulationSelectionViewModel
    {
        private readonly IRegulationRepository _regulationRepository;
        public ObservableCollection<Regulation> Regulations { get; } = [];

        public RegulationSelectionViewModel(IRegulationRepository regulationRepository)
        {
            _regulationRepository = regulationRepository;
        }

        public async Task InitializeAsync()
        {
            await LoadRegulationsAsync();
        }

        public async Task LoadRegulationsAsync()
        {
            var regulations = await _regulationRepository.GetAllAsync();
            Regulations.Clear();
            foreach (var regulation in regulations)
            {
                Regulations.Add(regulation);
            }
        }
    }
}
