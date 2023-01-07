using GraphLight.Algorithm;
using GraphLight.Graph;

namespace GraphLight.Prefomance
{
    public class NetworkSimplexPerfomanceTest : BaseSimplexPerfomanceTest
    {
        protected override void PositioningSimplexTest()
        {
        }

        protected override void RankingSimplexTest()
        {
            var graph = TestData.GraphToTest;
            graph.Acyclic();
            var alg = graph.RankNetworkSimplex();
            alg.Execute();
        }
    }
}