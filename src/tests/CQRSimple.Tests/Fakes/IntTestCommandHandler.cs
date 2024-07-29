namespace CQRSimple.Tests.Fakes
{
    internal class IntTestCommandHandler : ICommandHandler<IntTestCommand>
    {
        public Task HandleAsync(IntTestCommand command)
        {
            return Task.CompletedTask;
        }
    }
}
