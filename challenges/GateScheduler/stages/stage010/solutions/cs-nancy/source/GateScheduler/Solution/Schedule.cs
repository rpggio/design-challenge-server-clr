using System;
using System.Collections.Generic;
using System.Linq;

namespace GateScheduler.Solution
{
    public class Schedule
    {
        private readonly List<GateModel> _gates = new List<GateModel>();
        private readonly List<FlightModel> _flights = new List<FlightModel>();

        public IReadOnlyCollection<GateModel> Gates
        {
            get { return _gates; }
        }

        public IReadOnlyCollection<FlightModel> Flights
        {
            get { return _flights; }
        }

        public IReadOnlyCollection<FlightModel> UnscheduledFlights
        {
            get { return _flights.Where(f => f.Gate == null).ToList(); }
        } 

        public void ClearAll()
        {
            _flights.Clear();
            _gates.Clear();
        }

        public void AddGates(IEnumerable<GateModel> gates)
        {
            _gates.AddRange(gates.Select(g => g.Clone()));
            _gates.Sort((a, b) => a.Gate.CompareTo(b.Gate));
        }

        public void AddFlights(IEnumerable<FlightModel> flights)
        {
            foreach (var flight in flights)
            {
                if (flight.Gate != null)
                {
                    throw new Exception("Cannot add flight already scheduled for a gate.");
                }
                _flights.Add(flight.Clone());
            }
        }

        public IEnumerable<FlightModel> GetFlightsForGate(string gate)
        {
            return _flights.Where(f => f.Gate == gate);
        }

        public GateModel GetGateForFlight(FlightModel flight)
        {
            return _gates.Single(g => flight.Gate == g.Gate);
        }

        public Schedule Clone()
        {
            var clone = new Schedule();
            clone._gates.AddRange(Gates.Select(g => g.Clone()));
            clone._flights.AddRange(Flights.Select(f => f.Clone()));
            return clone;
        }

        public bool ScheduleUnassignedFlights()
        {
            var schedule = Clone();

            if (schedule.UnscheduledFlights.Count == 0)
            {
                return true;
            }

            var working = schedule.Clone();
            var unscheduled = new Queue<FlightModel>(
                working.UnscheduledFlights.OrderBy(f => f.Arrives));
            working.ScheduleFlights(unscheduled);
            var best = working.Clone();

            int iterations = working.Flights.Count - 1;
            while (working.UnscheduledFlights.Count > 0 && iterations-- > 0)
            {
                unscheduled.Enqueue(unscheduled.Dequeue());
                working.ScheduleFlights(unscheduled);
                if (working.UnscheduledFlights.Count < best.UnscheduledFlights.Count)
                {
                    best = working.Clone();
                }
            }

            UpdateFrom(best);

            return UnscheduledFlights.Count == 0;
        }

        private bool ScheduleFlights(IEnumerable<FlightModel> flights)
        {
            var flightList = flights.ToList();
            bool allScheduled = true;
            foreach (var flight in flightList)
            {
                flight.Gate = null;
            }
            foreach (var flight in flightList)
            {
                var gate = FindOpenGateFor(flight);
                if (gate == null)
                {
                    allScheduled = false;
                }
                else
                {
                    flight.Gate = gate.Gate;
                }
            }
            return allScheduled;
        }

        private void UpdateFrom(Schedule schedule)
        {
            ClearAll();
            _gates.AddRange(schedule.Gates.Select(g => g.Clone()));
            _flights.AddRange(schedule.Flights.Select(f => f.Clone()));
        }

        private GateModel FindOpenGateFor(FlightModel flight)
        {
            ICollection<FlightModel> priorGateFlights = new FlightModel[0];
            foreach (var gate in Gates.OrderBy(g => g.Gate))
            {
                var gateFlights = GetFlightsForGate(gate.Gate).ToList();

                try
                {
                    if (priorGateFlights.Any(f => !flight.CanLoadAtGateNextTo(f)))
                    {
                        continue;
                    }

                    if (gateFlights.Any(f => !f.CanLoadAtSameGateAs(flight)))
                    {
                        continue;
                    }

                    return gate;
                }
                finally
                {
                    priorGateFlights = gateFlights;
                }
            }
            return null;
        }
    }
}