namespace CG.Luxa4Slack.Client
{
  using System;

  public enum Colors
  {
    None,
    White,
    Red,
    Green,
    Blue
  }

  public interface ILuxaforClient : IDisposable
  {
    bool Initialize();
    bool Test();

    bool Set(Colors color);

    bool Reset();
  }
}
