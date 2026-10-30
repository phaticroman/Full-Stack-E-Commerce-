using MaleFashion.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Infrastructure.Data
{
    public class ServerTime : IServerTime
    {
        public DateTime DateTime
        {
            get
            {
                return DateTime.UtcNow;
            }
        }
    }

}
