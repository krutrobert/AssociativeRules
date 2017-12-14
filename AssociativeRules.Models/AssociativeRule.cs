using System.Collections.Generic;

namespace AssociativeRules.Models
{
    public struct AssociativeRule
    {
        public List<string> IfSet { get; set; }
        public List<string> ThenSet { get; set; }
        public double Confidence { get; set; }
    }
}