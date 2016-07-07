namespace CG.Luxa4Slack.Client
{
  using NLog;

  public class DebugClient : ILuxaforClient
  {
    private readonly ILogger logger = LogManager.GetLogger("DebugClient");

    public void Dispose()
    {
      this.logger.Info("Dispose");
    }

    public bool Initialize()
    {
      this.logger.Info("Initialize");
      return true;
    }

    public bool Test()
    {
      this.logger.Info("Test");
      return true;
    }

    public bool Set(Colors color)
    {
      this.logger.Info("Set " + color);
      return true;
    }

    public bool Reset()
    {
      this.logger.Info("Reset");
      return true;
    }
  }
}
