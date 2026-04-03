using System;
using System.Collections.Generic;
using System.Text;

namespace Hekki.Domain
{
    public class Race
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Guid DefaultReglementId { get; set; }
    }

    public class Pilot
    {
        public int Id { get; set; }
        public int RaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Number { get; set; }
        public string? Team { get; set; }
        public bool IsActive { get; set; }
    }

    public class Reglement
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Json { get; set; } = string.Empty;
        public int Version { get; set; }
    }

    public class Penalty
    {
        public int HeatId { get; set; }
        public int PilotId { get; set; }
        public string Type { get; set; } = string.Empty;
        public double Value { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class Heat
    {
        public int Id { get; set; }
        public int RaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? RoleLabel { get; set; }
        public int ReglementId { get; set; }
        public HeatStatus Status { get; set; }
    }

    public class HeatPilot
    {
        public int HeatId { get; set; }
        public int PilotId { get; set; }
        public int SeedOrder { get; set; }
        public int GridPosition { get; set; }
        public int KartNumber { get; set; }
    }

    public class HeatPilotResult
    {
        public int HeatId { get; set; }
        public int PilotId { get; set; }
        public int? FinishPosition { get; set; }
        public long? TotalTimeMs { get; set; }
        public long? BestLapMs { get; set; }
        public int? Laps { get; set; }
        public ResultStatus Status { get; set; }
    }

    public enum HeatStatus
    {
        Draft,
        Locked
    }

    public enum ResultStatus
    {
        OK,
        DNF,
        DNS,
        DQ
    }
}
