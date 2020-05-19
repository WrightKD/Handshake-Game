using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HandshakeGame.Models
{
    public class Mapbox
    {
        private readonly ILogger<Mapbox> _logger;
        private readonly IConfiguration _configuration;

        public string MapboxAccessToken { get; set; }

        public Mapbox(ILogger<Mapbox> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            MapboxAccessToken = _configuration["Mapbox:AccessToken"];
            _logger.LogInformation("User accessed MapboxAccessToken");
        }
    }
}
