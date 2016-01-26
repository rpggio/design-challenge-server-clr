using System.Collections.Generic;

namespace GateScheduler.Solution
{
    /// <summary>
    /// Stores data as in-memory collections, since persistence is not currently
    /// needed beyond app lifetime.
    /// </summary>
    public class SchedulerDatabase
    {
        private readonly List<FlightModel> _flights = new List<FlightModel>();
        private readonly List<GateModel> _gates = new List<GateModel>();

        public IReadOnlyCollection<FlightModel> Flights
        {
            get { return _flights; }
        }

        public IReadOnlyCollection<GateModel> Gates
        {
            get { return _gates; }
        }

        public void ClearAll()
        {
            _flights.Clear();
            _gates.Clear();
        }

        public void AddGates(IEnumerable<GateModel> gates)
        {
            _gates.AddRange(gates);
        }

        public void AddFlights(IEnumerable<FlightModel> flights)
        {
            _flights.AddRange(flights);
        }
    }
}
