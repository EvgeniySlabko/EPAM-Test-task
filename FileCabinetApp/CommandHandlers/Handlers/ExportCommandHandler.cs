using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// handler for export command.
    /// </summary>
    public class ExportCommandHandler : CommandHandlerBase
    {
        private const string Command = "export";
        private Dictionary<string, FileType> fileType = new Dictionary<string, FileType>
        {
            { "csv", FileType.Сsv },
            { "xml", FileType.Xml },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        public ExportCommandHandler()
            : base(Command)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.CheckCommand(commandRequest) && this.ParseParameters(commandRequest.Parameters, out FileType type, out string path))
            {
                Export(type, path);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void Export(FileType type, string filePath)
        {
            FileCabinetServiceSnapshot snapshot = Program.fileCabinetService.MakeSnapshot();
            Action<StreamWriter> saveToMethod;
            switch (type)
            {
                case FileType.Xml:
                    saveToMethod = snapshot.SaveToXml;
                    break;

                case FileType.Сsv:
                    saveToMethod = snapshot.SaveToCsv;
                    break;

                default:
                    Console.WriteLine(Program.Rm.GetString("InvalidArgumentsMessage", CultureInfo.CurrentCulture));
                    return;
            }

            try
            {
                using var writer = new StreamWriter(filePath);
                snapshot = Program.fileCabinetService.MakeSnapshot();
                saveToMethod(writer);
                Console.WriteLine(Program.Rm.GetString("SuccessfulWriteToFileMessage", CultureInfo.CurrentCulture));
            }
            catch (IOException)
            {
                Console.WriteLine(Program.Rm.GetString("ErrorWriteToFileMessage", CultureInfo.CurrentCulture));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private bool ParseParameters(string parameters, out FileType type, out string path)
        {
            type = default;
            path = default;
            var splitedParameters = parameters.Split(' ');

            if (splitedParameters.Length != 2 || (File.Exists(splitedParameters[1]) && !ConsoleHelper.RewriteFileDialog(splitedParameters[1])))
            {
                return false;
            }

            path = splitedParameters[1];
            if (this.fileType.ContainsKey(splitedParameters[0]))
            {
                type = this.fileType[splitedParameters[0]];
            }
            else
            {
                return false;
            }

            return true;
        }

    }
}
