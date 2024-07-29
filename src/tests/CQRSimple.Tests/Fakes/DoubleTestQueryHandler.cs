namespace CQRSimple.Tests.Fakes
{
    internal class DoubleTestQueryHandler : IQueryHandler<DoubleTestQuery, List<double>>
    {
        public Task<List<double>> HandleAsync(DoubleTestQuery query)
        {
            return Task.FromResult(new List<double> { query.Prop });
        }
    }
}
