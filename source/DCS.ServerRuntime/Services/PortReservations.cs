using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DCS.Core;
using DCS.ServerRuntime.Framework;
using log4net;

namespace DCS.ServerRuntime.Services
{
    [RegisterComponent(RegisterWith.SingleInstance)]
    public class PortReservations
    {
        private readonly AppSettings _settings;
        private readonly ILog _log;
        private readonly List<int> _reservations = new List<int>();

        public PortReservations(AppSettings settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        public bool TryReserve(out PortAssignment assignment)
        {
            lock (_reservations)
            {
                int port = 0;

                var availablePorts = new Queue<int>(_settings.Env.SolutionPorts
                    .Except(_reservations)
                    .Shuffle());
                while (port == 0 && availablePorts.Count > 0)
                {
                    int dequeued = availablePorts.Dequeue();
                    if (CheckPort(dequeued))
                    {
                        port = dequeued;
                    }
                }

                if (port == 0)
                {
                    assignment = null;
                    _log.InfoFormat("Failed to reserve a port");
                    return false;
                }

                _reservations.Add(port);
                assignment = new PortAssignment(port, this);
                return true;
            }
        }

        private bool CheckPort(int? port)
        {
            var listener = new HttpListener();
            try
            {
                listener.Prefixes.Add("http://+:{0}/".FormatFrom(port));
                listener.Start();
                return true;
            }
            catch (Exception ex)
            {
                _log.WarnFormat("Failed to access HTTP port {0}: {1}", port, ex.Summary());
                return false;
            }
            finally
            {
                try
                {
                    listener.Close();
                }
                catch (Exception ex)
                {
                    _log.WarnFormat("Failed to close HTTP port {0}: {1}", port, ex.Summary());
                }
            }
        }

        public void Free(int port)
        {
            lock (_reservations)
            {
                _reservations.Remove(port);
            }
        }

        public IReadOnlyCollection<int> Reservations
        {
            get { return _reservations.Select(_ => _).ToArray(); }
        }
    }
}