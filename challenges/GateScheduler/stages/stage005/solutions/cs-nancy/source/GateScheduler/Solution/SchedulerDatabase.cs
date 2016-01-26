using System.Collections.Generic;
using System.Linq;

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
            _gates.Sort((a, b) => a.Gate.CompareTo(b.Gate));
        }

        public void AddFlight(FlightModel flight)
        {
            _flights.Add(flight);
        }
        
        public void SetFlightGate(FlightModel flight, string gate)
        {
            flight.Gate = gate;
        }

        public IEnumerable<FlightModel> GetFlightsForGate(string gate)
        {
            return _flights.Where(f => f.Gate == gate);
        }

        public GateModel GetGateOrNullForFlight(FlightModel flight)
        {
            return _gates.Single(g => flight.Gate == g.Gate);
        }
    }
}
