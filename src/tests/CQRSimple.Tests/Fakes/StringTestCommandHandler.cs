namespace CQRSimple.Tests.Fakes
{
    internal class StringTestCommandHandler : ICommandHandler<StringTestCommand>
    {
        public Task HandleAsync(StringTestCommand command)
        {
            return Task.CompletedTask;
        }
    }
}
