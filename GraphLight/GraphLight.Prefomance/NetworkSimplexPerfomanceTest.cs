using GraphLight.Graph;
using GraphLight.Layout;

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
            var alg = new RankNetworkSimplex<VertexAttrs, EdgeAttrs>(graph);
            alg.Execute();
        }
    }
}