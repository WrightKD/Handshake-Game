using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HandshakeGame.Models
{
    public class Position
    {
        public float Longitude;
        public float Latitude;

        public Position(float longitude, float latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }
    }
}
