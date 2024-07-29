namespace CQRSimple
{
    public interface IDispatcher
    {
        Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand;
        Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>;
    }
}
