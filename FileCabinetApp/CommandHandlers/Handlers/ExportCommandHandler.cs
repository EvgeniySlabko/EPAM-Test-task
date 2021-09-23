using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// handler for export command.
    /// </summary>
    public class ExportCommandHandler : FileCabinetServiceCommandHandlerBase
    {
        private const string Command = "export";

        private readonly Dictionary<string, FileType> fileType = new ()
        {
            { "csv", FileType.Сsv },
            { "xml", FileType.Xml },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        public ExportCommandHandler(IFileCabinetService service)
            : base(Command, service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.CheckCommand(commandRequest))
            {
                var result = this.ParseParameters(commandRequest.Parameters, out FileType type, out string path);
                if (result.Item1)
                {
                    this.Export(type, path);
                }
                else
                {
                    Console.WriteLine(result.Item2);
                }
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private Tuple<bool, string> ParseParameters(string parameters, out FileType type, out string path)
        {
            type = default;
            path = default;
            var splitedParameters = parameters.Split(' ');
            if (splitedParameters.Length != 2)
            {
                return new (false, "Invalid parameters");
            }

            if (File.Exists(splitedParameters[1]) && !ConsoleHelper.RewriteFileDialog(splitedParameters[1]))
            {
                return new (false, "Canceling file saving");
            }

            path = splitedParameters[1];

            if (!this.fileType.TryGetValue(splitedParameters[0], out type))
            {
                return new (false, $"Undefined file type - {splitedParameters[0]}");
            }

            return new (true, string.Empty);
        }

        private void Export(FileType type, string filePath)
        {
            var snapshot = this.Service.MakeSnapshot();
            Action<StreamWriter> saveTo;

            switch (type)
            {
                case FileType.Сsv:
                    saveTo = snapshot.SaveToCsv;
                    break;

                case FileType.Xml:
                    saveTo = snapshot.SaveToXml;
                    break;

                default:
                    Console.WriteLine(StringManager.Rm.GetString("InvalidArgumentsMessage", CultureInfo.CurrentCulture));
                    return;
            }

            try
            {
                using var writer = new StreamWriter(filePath);
                saveTo(writer);
                Console.WriteLine(StringManager.Rm.GetString("SuccessfulWriteToFileMessage", CultureInfo.CurrentCulture));
            }
            catch (IOException)
            {
                Console.WriteLine(StringManager.Rm.GetString("ErrorWriteToFileMessage", CultureInfo.CurrentCulture));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
