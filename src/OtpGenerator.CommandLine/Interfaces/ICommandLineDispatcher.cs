using OtpGenerator.Logic.Interfaces;

namespace OtpGenerator.CommandLine.Interfaces
{
    public interface ICommandLineDispatcher
    {
        void RegisterAddCommandHandler(IOperationResultHandler resultHandler);
        void RegisterDeleteCommandHandler(IOperationResultHandler resultHandler);
        void RegisterListCommandHandler(IOperationResultHandler resultHandler);
        void RegisterGenerateCommandHandler(IOperationResultHandler resultHandler);
        void RegisterImportCommandHandler(IOperationResultHandler resultHandler);
        void RegisterExportCommandHandler(IOperationResultHandler resultHandler);
        void RegisterLiveViewCommandHandler(IOperationResultHandler resultHandler);
        void RegisterVerboseContextOptionHandler(IOperationResultHandler resultHandler);
        void RegisterShowSecretsContextOptionHandler(IOperationResultHandler resultHandler);
        void RegisterDummyContextOptionHandler(IOperationResultHandler resultHandler);
        void Dispatch(ICommandLineParser parser);
    }
}
