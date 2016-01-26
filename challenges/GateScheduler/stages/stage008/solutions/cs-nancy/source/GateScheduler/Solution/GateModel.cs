using System;

namespace GateScheduler.Solution
{
    public class GateModel
    {
        public string Gate { get; set; }

        public GateModel Clone()
        {
            return new GateModel { Gate = Gate};
        }
    }
}
