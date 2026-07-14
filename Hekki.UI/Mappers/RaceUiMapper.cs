using Hekki.Application.DTOs.Race;
using Hekki.UI.ViewModels;

namespace Hekki.UI.Mappers
{ 
    public static class RaceUiMapper
    {
        public static void ApplyTo(RaceViewModel vm, RaceDataDto dto)
        {
            vm.RaceId = dto.RaceId;
            vm.RaceName = dto.RaceName;
            vm.Location = dto.Location;
            vm.RaceDate = dto.Date;
        }
    }
}
