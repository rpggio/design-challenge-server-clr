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
        private readonly Dictionary<string, string> _arrivalSchedule = new Dictionary<string, string>();

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
            _arrivalSchedule.Clear();
        }

        public void AddGates(IEnumerable<GateModel> gates)
        {
            _gates.AddRange(gates);
            _gates.Sort((a, b) => a.Gate.CompareTo(b.Gate));
        }

        public void AddFlights(IEnumerable<FlightModel> flights)
        {
            _flights.AddRange(flights);
        }

        
        public void SetFlightGate(string flight, string gate)
        {
            _arrivalSchedule[flight] = gate;
        }

        public IEnumerable<FlightModel> GetFlightsForGate(string gate)
        {
            var flightNumbers = _arrivalSchedule
                .Where(flightGate => flightGate.Value == gate)
                .Select(flightGate => flightGate.Key)
                .ToList();
            return _flights.Where(f => flightNumbers.Contains(f.Flight));
        }

        public GateModel GetGateOrNullForFlight(string flight)
        {
            string gateNumber;
            if (_arrivalSchedule.TryGetValue(flight, out gateNumber))
            {
                return _gates.Single(g => g.Gate == gateNumber);
            };
            return null;
        }
    }
}
