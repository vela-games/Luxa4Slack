namespace CG.Luxa4Slack.Client.CommandLine
{
  using NLog;

  using CG.Luxa4Slack.Client;

  public class CommandLineClient : ILuxaforClient
  {
    private readonly ILogger logger = LogManager.GetLogger("CommandLine");

    public void Dispose()
    {
      this.logger.Warn("Dispose");
    }

    public bool Initialize()
    {
      this.logger.Warn("Initialize");
      return true;
    }

    public bool Test()
    {
      this.logger.Warn("Test");
      return true;
    }

    public bool Set(Colors color)
    {
      this.logger.Warn("Set " + color);
      return true;
    }

    public bool Reset()
    {
      this.logger.Warn("Reset");
      return true;
    }
  }
}
