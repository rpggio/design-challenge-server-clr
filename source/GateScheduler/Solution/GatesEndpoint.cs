using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;

namespace GateScheduler.Solution
{
    /// <summary>
    /// Nancy web server module for handling requests to /gates endpoint.
    /// </summary>
    public class GatesModule : NancyModule
    {
        private readonly SchedulerDatabase _db;

        /// <summary>
        /// Create Gates Module. The constructor is called by the IoC container
        /// instead of by our own code.
        /// </summary>
        public GatesModule(SchedulerDatabase db)
            : base("/gates")
        {
            _db = db;

            // This tells the web server to use the provided delegate (Lambda)
            // to serve a GET request at this endpoint.
            Get["/"] = _ => _db.Gates
                // This creates a new dynamic object instead of serializing
                // the gate model object. This is an easy way to map your internal 
                // model objects to the API model. 
                .Select(g => new
                {
                    g.Gate
                });

            // This delegate will be used to serve a POST request.
            Post["/"] = _ =>
            {
                // As long as the Gate model object has properties
                // matching the API model, you can take the shortcut 
                // of deserializing the request directly to the model.
                // The JSON serializer is configured to convert
                // TitleCase to camelCase.
                var gates = this.Bind<IEnumerable<GateModel>>();
                _db.AddGates(gates.ToArray());
                return null;
            };
        }
    }
}
