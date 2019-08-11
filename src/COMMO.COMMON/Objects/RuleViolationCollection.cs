using System.Collections.Generic;
using System.Linq;

namespace COMMO.Common.Objects
{
    public class RuleViolationCollection
    {
        private readonly List<RuleViolation> _ruleViolations = new List<RuleViolation>();

		public void AddRuleViolation(RuleViolation ruleViolation) => _ruleViolations.Add(ruleViolation);

		public void RemoveRuleViolation(RuleViolation ruleViolation) => _ruleViolations.Remove(ruleViolation);

		public RuleViolation GetRuleViolationByReporter(Player reporter)
        {
            return GetRuleViolations()
                .Where(r => r.Reporter == reporter)
                .FirstOrDefault();
        }
        
        public RuleViolation GetRuleViolationByAssignee(Player assignee)
        {
            return GetRuleViolations()
                .Where(r => r.Assignee == assignee)
                .FirstOrDefault();
        }

		public IEnumerable<RuleViolation> GetRuleViolations() => _ruleViolations;
	}
}