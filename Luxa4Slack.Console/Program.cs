namespace CG.Luxa4Slack.Console
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Reflection;

  using NLog;

  using CG.Luxa4Slack.Client;

  public class Program
  {
    private static readonly ILogger logger = LogManager.GetLogger("Luxa4Slack.Console");
    private static Luxa4Slack luxa4Slack;

    public static void Main(string[] args)
    {
      logger.Info("Start");

      var clientTypes = DiscoverAvailableClientTypes();

      CommandLineOptions options = ParseCommandLine(args, clientTypes.Select(x => x.Name));
      if (options.HasError)
      {
        Console.ReadLine();
        return;
      }

      if (options.Debug)
      {
        EnableDebugLog();
      }

      if (options.RequestToken)
      {
        logger.Warn("Please visit following uri to retrieve your Slack token");
        logger.Warn(OAuthHelper.GetAuthorizationUri());
        return;
      }

      Type selectedClientType = clientTypes.FirstOrDefault(x => x.Name == options.Client);
      if (selectedClientType == null)
      {
        logger.Error("Client '{0}' not found. Fallback to {1}", options.Client, typeof(DebugClient).Name);
        selectedClientType = typeof(DebugClient);
      }

      luxa4Slack = new Luxa4Slack(
        options.Token,
        options.ShowUnreadMessages,
        options.ShowUnreadMentions,
        (ILuxaforClient)Activator.CreateInstance(selectedClientType));

      try
      {
        luxa4Slack.Initialize();
        luxa4Slack.LuxaforFailure += OnLuxaforFailure;

        Console.ReadLine();
      }
      catch (Exception ex)
      {
        logger.Error(ex);
      }
      finally
      {
        luxa4Slack.Dispose();
      }
    }

    private static IEnumerable<Type> DiscoverAvailableClientTypes()
    {
      var assembliesName =
        Directory.GetFiles(
          Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
          "*.dll",
          SearchOption.TopDirectoryOnly).Select(x => new AssemblyName(Path.GetFileNameWithoutExtension(x)));

      var types =
        assembliesName.SelectMany(
          x => Assembly.Load(x).GetExportedTypes().Where(y => y.GetInterfaces().Any(z => z == typeof(ILuxaforClient))));

      var duplicates = types.GroupBy(x => x.Name).Where(x => x.Count() > 1).SelectMany(x => x);
      if (duplicates.Any())
      {
        throw new Exception($"Client types must have different names ({string.Join(", ", duplicates)})");
      }

      return types;
    }

    private static CommandLineOptions ParseCommandLine(string[] arguments, IEnumerable<string> clientTypes)
    {
      CommandLineOptions options = new CommandLineOptions(arguments, clientTypes);
      if (options.HasError)
      {
        var assemblyName = typeof(Program).GetTypeInfo().Assembly.GetName();
        logger.Info("{0} - {1}{2}", assemblyName.Name, assemblyName.Version, Environment.NewLine);
        if (string.IsNullOrEmpty(options.ErrorMessage) == false)
        {
          logger.Error("Error: {0}{1}", options.ErrorMessage, Environment.NewLine);
        }

        logger.Info("Options: {0}{1}", Environment.NewLine, options.Help);
      }

      return options;
    }

    private static void OnLuxaforFailure()
    {
      logger.Error("Luxafor communication issue. Please unplug/replug the Luxafor and restart the application");
    }

    private static void EnableDebugLog()
    {
      foreach (var rule in LogManager.Configuration.LoggingRules)
      {
        rule.EnableLoggingForLevels(LogLevel.Trace, LogLevel.Fatal);
      }

      LogManager.ReconfigExistingLoggers();
    }
  }
}
