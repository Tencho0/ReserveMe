using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Exceptions
{
    public sealed class ReservationCapacityExceededException : Exception
    {
        public int AvailableSeats { get; }

        public ReservationCapacityExceededException(string message, int availableSeats)
            : base(message)
        {
            AvailableSeats = availableSeats;
        }
    }
}
