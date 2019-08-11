using System;

namespace COMMO.Common.Objects
{
    public class RuleViolation
    {
        public RuleViolation()
        {
            _creationDate = DateTime.UtcNow;
        }

        private readonly DateTime _creationDate;

		public uint Time => (uint) DateTime.UtcNow.Subtract(_creationDate).TotalSeconds;

		public Player Reporter { get; set; }

        public Player Assignee { get; set; }
        
        public string Message { get; set; }
    }
}