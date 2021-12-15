using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.Azure.Cosmos.IntegrationTests
{
    internal static class Conf
    {
        public static IConfiguration Value { get; }
        static Conf()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddUserSecrets(typeof(Conf).Assembly);

            Value = configurationBuilder.Build();
        }
    }
}
