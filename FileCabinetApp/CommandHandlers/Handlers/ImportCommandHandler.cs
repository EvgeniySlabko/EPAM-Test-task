using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// handler for import command.
    /// </summary>
    public class ImportCommandHandler : FileCabinetServiceCommandHandlerBase
    {
        private const string Command = "import";
        private readonly Dictionary<string, FileType> fileType = new ()
        {
            { "csv", FileType.Сsv },
            { "xml", FileType.Xml },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        public ImportCommandHandler(IFileCabinetService service)
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
                    this.Import(type, path);
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

        private void Import(FileType type, string path)
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
                    Console.WriteLine(StringManager.Rm.GetString("InvalidArgumentsMessage", CultureInfo.CurrentCulture));
                    return;
            }

            try
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                loader(stream);
                this.Service.Restore(snapshot);
            }
            catch (IOException)
            {
                Console.WriteLine(StringManager.Rm.GetString("СouldТotOpenFile", CultureInfo.CurrentCulture));
                return;
            }
            catch (ArgumentException)
            {
                Console.WriteLine(StringManager.Rm.GetString("СouldТotOpenFile", CultureInfo.CurrentCulture));
                return;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine(StringManager.Rm.GetString("RecordsWereImport", CultureInfo.CurrentCulture), path);
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

            path = splitedParameters[1];
            if (!this.fileType.TryGetValue(splitedParameters[0], out type))
            {
                return new (false, $"Undefined file type - {splitedParameters[0]}");
            }

            return new (true, string.Empty);
        }
    }
}
