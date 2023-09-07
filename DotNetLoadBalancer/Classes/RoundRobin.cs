using System;
using RoundRobin;

namespace DotNetLoadBalancer.Classes
{
	public static class RoundRobin
	{
        public static string GetRoundRobinEntry(List<string> endpoints)
        {
            var roundRobinList = new RoundRobinList<string>(endpoints);

            return roundRobinList.Next();
        }
    }
}

