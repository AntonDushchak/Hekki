using System;
using System.Collections.Generic;
using System.Text;

namespace Hekki.Domain.Models
{
    public class Race
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int DefaultReglementId { get; set; }
    }
}
