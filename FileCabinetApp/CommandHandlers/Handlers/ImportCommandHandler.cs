using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// handler for import command.
    /// </summary>
    public class ImportCommandHandler : CommandHandlerBase
    {
        private const string Command = "import";
        private Dictionary<string, FileType> fileType = new Dictionary<string, FileType>
        {
            { "csv", FileType.Сsv },
            { "xml", FileType.Xml },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        public ImportCommandHandler()
            : base(Command)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (this.CheckCommand(commandRequest) && this.ParseParameters(commandRequest.Parameters, out FileType type, out string path))
            {
                Import(type, path);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void Import(FileType type, string path)
        {
            var snapshot = new FileCabinetServiceSnapshot();
            Action<FileStream> loader;

            switch (type)
            {
                case FileType.Сsv:
                    loader = snapshot.LoadFromCsv;
                    break;

                case FileType.Xml:
                    loader = snapshot.LoadFromXml;
                    break;

                default:
                    Console.WriteLine(Program.Rm.GetString("InvalidArgumentsMessage", CultureInfo.CurrentCulture));
                    return;
            }

            try
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                loader(stream);
                Program.fileCabinetService.Restore(snapshot);
            }
            catch (IOException)
            {
                Console.WriteLine(Program.Rm.GetString("СouldТotOpenFile", CultureInfo.CurrentCulture));
                return;
            }
            catch (ArgumentException)
            {
                Console.WriteLine(Program.Rm.GetString("СouldТotOpenFile", CultureInfo.CurrentCulture));
                return;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine(Program.Rm.GetString("RecordsWereImport", CultureInfo.CurrentCulture), path);
        }

        private bool ParseParameters(string parameters, out FileType type, out string path)
        {
            type = default;
            path = default;
            var splitedParameters = parameters.Split(' ');

            if (splitedParameters.Length != 2)
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
