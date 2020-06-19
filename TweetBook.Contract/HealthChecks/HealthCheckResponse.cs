using System;
using System.Collections.Generic;
using System.Text;

namespace TweetBook.Contract.HealthChecks
{
    public class HealthCheckResponse
    {
        public string Status { get; set; }
        public TimeSpan Duartion { get; set; }
        public IEnumerable<HealthCheck> Checks { get; set; }
    }

   
}
