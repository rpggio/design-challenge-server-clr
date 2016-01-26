using System;

namespace DCS.ServerRuntime.Services
{
    public class PortAssignment : IDisposable
    {
        private readonly int _number;
        private readonly PortReservations _reservations;

        public int Number
        {
            get { return _number; }
        }

        public PortAssignment(int number, PortReservations reservations)
        {
            _number = number;
            _reservations = reservations;
        }

        public void Dispose()
        {
            _reservations.Free(_number);
        }

        public override string ToString()
        {
            return _number.ToString();
        }
    }
}