namespace CG.Luxa4Slack.Console
{
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;

  using Mono.Options;

  public class CommandLineOptions
  {
    public CommandLineOptions(IEnumerable<string> arguments, IEnumerable<string> clientTypes)
    {
      this.ShowUnreadMessages = true;
      this.ShowUnreadMentions = true;

      OptionSet optionSet =
        new OptionSet().Add("t=|token=", "Slack token (use --requesttoken option)", x => this.Token = x)
          .Add("noUnreadMessages", "Disable unread messages notifications", x => this.ShowUnreadMessages = x == null)
          .Add("noUnreadMentions", "Disable unread mentions notifications", x => this.ShowUnreadMentions = x == null)
          .Add("d|debug", "Show debug informations", x => this.Debug = x != null)
          .Add("requesttoken", "Request a Slack token", x => this.RequestToken = x != null)
          .Add("client=", $"Client to use (Available: {string.Join(", ", clientTypes)})", x => this.Client = x);

      var unparsedArguments = optionSet.Parse(arguments);
      var hasUnparsedArguments = unparsedArguments.Any();
      var hasMissingRequiredArgument = (string.IsNullOrEmpty(this.Token) || string.IsNullOrEmpty(this.Client)) && this.RequestToken == false;
      var hasConflictArguments = string.IsNullOrEmpty(this.Token) == false && this.RequestToken;
      var hasArgument = arguments.Any();

      this.HasError = hasArgument == false || hasUnparsedArguments || hasMissingRequiredArgument || hasConflictArguments;
      if (hasUnparsedArguments)
      {
        this.ErrorMessage = $"Argument(s) {string.Join(", ", unparsedArguments)} are unknown";
      }

      if (hasMissingRequiredArgument)
      {
        this.ErrorMessage = "Missing mandatory argument. At least --token and --client arguments or --requesttoken argument should be provided";
      }

      if (hasConflictArguments)
      {
        this.ErrorMessage = "--token argument is incompatible with --requesttoken argument";
      }

      using (TextWriter textWriter = new StringWriter())
      {
        optionSet.WriteOptionDescriptions(textWriter);
        this.Help = textWriter.ToString();
      }
    }

    public string Token { get; private set; }

    public bool ShowUnreadMessages { get; private set; }

    public bool ShowUnreadMentions { get; private set; }

    public bool Debug { get; private set; }

    public bool RequestToken { get; private set; }

    public bool HasError { get; private set; }

    public string ErrorMessage { get; private set; }

    public string Help { get; private set; }

    public string Client { get; private set; }
  }
}
