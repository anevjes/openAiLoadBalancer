using System;
using RoundRobin;

namespace DotNetLoadBalancer.Classes
{
	public static class RoundRobin
	{
		public static string GetRoundRobinEntry()
		{
            var roundRobinList = new RoundRobinList<string>(
                new List<string>{
                    "cog-77j27pzzhwq2s.openai.azure.com",
                    "cog-iy5f7cvijqgfg.openai.azure.com"
                }
            );

            return roundRobinList.Next();
        }
	}
}

