namespace MarkethuntOTC.ApplicationServices;

public interface IAgentCommandService
{
    void OnCommandReceived(object sender, IEnumerable<string> args);
}