namespace ISC.IDRDownloader.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using ISC.IDRDownloader.Validation.Contracts;

    public class ArgumentValidator : IArgumentValidator
    {
        private string[] validArgs = new string[] { "--username", "--password", "--partnerid", "--changessince", "--type", "--savepath" };
        private string[] validTypes = new string[] { "entities", "relationships", "reviewstatus", "all" };

        public StringBuilder Validate(string[] args)
        {
            var errors = new StringBuilder();
            var invalidArgs = new List<string>();

            if (args.Length == 0)
            {
                errors.AppendLine("Please specify the arguments --username and --password");
            }

            foreach (var arg in args)
            {
                var argParts = arg.Split('=');

                if (argParts.Length != 2)
                {
                    errors.AppendLine($"Argument '{argParts[0]}' should be in the format '--argument=value'");
                    continue;
                }

                if (!validArgs.Contains(argParts[0]))
                {
                    invalidArgs.Add(argParts[0]);
                }

                ValidateType(argParts, errors);
                ValidatePartnerId(argParts, errors);
                ValidateChangesSince(argParts, errors);
            }

            if (invalidArgs.Any())
            {
                errors.AppendLine($"Invalid argument(s) '{string.Join(' ', invalidArgs.ToArray())}'. Please use these valid arguments only '{string.Join('-', validArgs)}'");
            }

            return errors;
        }

        private void ValidateType(string[] args, StringBuilder errors)
        {
            if (args[0] == "--type")
            {
                if (!validTypes.Contains(args[1]))
                {
                    errors.AppendLine($"Invalid 'type' argument '{args[1]}'. Please use these valid 'type' arguments only '{string.Join('-', validTypes)}'");
                }
            }
        }

        private void ValidatePartnerId(string[] args, StringBuilder errors)
        {
            if (args[0] == "--partnerid")
            {
                int partnerId;

                if (!int.TryParse(args[1], out partnerId))
                {
                    errors.AppendLine($"'partnerid' should be a number but is '{args[1]}'");
                }
            }
        }

        private void ValidateChangesSince(string[] args, StringBuilder errors)
        {
            if (args[0] == "--changessince")
            {
                DateTime changesSince;

                if (!DateTime.TryParseExact(args[1], "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out changesSince))
                {
                    errors.AppendLine($"'changessince' should be a date but is '{args[1]}'");
                }
            }
        }
    }
}
