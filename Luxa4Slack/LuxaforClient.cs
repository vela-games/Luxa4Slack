﻿namespace CG.Luxa4Slack
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;

  using LuxaforSharp;

  using NLog;

  internal class LuxaforClient : IDisposable
  {
    private const int Timeout = 200;

    private readonly ILogger logger = LogManager.GetLogger("Luxafor");

    public enum Colors
    {
      None,
      White,
      Red,
      Green,
      Yellow,
      Blue,
      Cyan
    }

    private readonly Dictionary<Colors, Color> colorsMapping = new Dictionary<Colors, Color>
                                                        {
                                                          { Colors.None, new Color(0, 0, 0) },
                                                          { Colors.White, new Color(255, 255, 255) },
                                                          { Colors.Red, new Color(255, 0, 0) },
                                                          { Colors.Green, new Color(0, 255, 0) },
                                                          { Colors.Yellow, new Color(255, 255, 0) },
                                                          { Colors.Blue, new Color(0, 0, 255) },
                                                          { Colors.Cyan, new Color(0, 255, 255) },
                                                        };

    private IDevice device;

    public bool Initialize()
    {
      IDeviceList list = new DeviceList();
      list.Scan();
      this.logger.Debug("Found {0} devices", list.Count());

      this.device = list.FirstOrDefault();
      this.logger.Debug("Selected device: {0}", (this.device as Device)?.DevicePath ?? "None");

      return this.device != null;
    }

    public void Dispose()
    {
      this.device?.Dispose();
    }

    public bool Set(Colors color)
    {
      if (this.device == null)
      {
        throw new InvalidOperationException("Not initialized");
      }
      else
      {
        this.logger.Debug("Set color: {0}", color);

        return this.device.AllLeds.SetColor(this.colorsMapping[color]).Wait(Timeout);
      }
    }

    public bool Reset()
    {
      return this.Set(Colors.None);
    }

    public bool Test()
    {
      this.logger.Debug("Test device");

      bool result = this.Set(Colors.White);
      Thread.Sleep(200);

      return result && this.Set(Colors.None);
    }
  }
}
