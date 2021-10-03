using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileCabinetApp
{
    /// <summary>
    /// Parser fir commands.
    /// </summary>
    public static class CommandParser
    {
        private const string Set = "set";
        private const string Where = "where";
        private const string Values = "values";

        private new const string Equals = "=";
        private const string And = "and";
        private const string Or = "or";

        private const string Firstname = "firstname";
        private const string LastName = "lastname";
        private const string Dateofbirth = "dateofbirth";
        private const string IdentificationNumber = "identificationnumber";
        private const string Points = "points";
        private const string Id = "id";
        private const string Letter = "letter";

        private static readonly Dictionary<string, Func<object, Action<FileCabinetRecord>>> ActionMapper = new ()
        {
            { CommandParser.Firstname, o => r => r.FirstName = (string)o },
            { CommandParser.LastName, o => r => r.LastName = (string)o },
            { CommandParser.Dateofbirth, o => r => r.DateOfBirth = (DateTime)o },
            { CommandParser.IdentificationNumber, o => r => r.IdentificationNumber = (decimal)o },
            { CommandParser.Points, o => r => r.PointsForFourTests = (short)o },
            { CommandParser.Id, o => r => r.Id = (int)o },
            { CommandParser.Letter, o => r => r.IdentificationLetter = (char)o },
        };

        private static readonly Dictionary<string, Action<FileCabinetRecord, object>> SetterMapper = new ()
        {
            { CommandParser.Firstname, (r, o) => r.FirstName = (string)o },
            { CommandParser.LastName, (r, o) => r.LastName = (string)o },
            { CommandParser.Dateofbirth, (r, o) => r.DateOfBirth = (DateTime)o },
            { CommandParser.IdentificationNumber, (r, o) => r.IdentificationNumber = (decimal)o },
            { CommandParser.Points, (r, o) => r.PointsForFourTests = (short)o },
            { CommandParser.Letter, (r, o) => r.IdentificationLetter = (char)o },
            { CommandParser.Id, (r, o) => r.Id = (int)o },
        };

        private static readonly Dictionary<string, Func<object, Predicate<FileCabinetRecord>>> PredicateMapper = new ()
        {
            { CommandParser.Firstname, o => r => r.FirstName.Equals((string)o) },
            { CommandParser.LastName, o => r => r.LastName.Equals((string)o) },
            { CommandParser.Dateofbirth, o => r => r.DateOfBirth.Equals((DateTime)o) },
            { CommandParser.IdentificationNumber, o => r => r.IdentificationNumber.Equals((decimal)o) },
            { CommandParser.Points, o => r => r.PointsForFourTests.Equals((short)o) },
            { CommandParser.Id, o => r => r.Id.Equals((int)o) },
            { CommandParser.Letter, o => r => r.IdentificationLetter.Equals((char)o) },
        };

        private static readonly Dictionary<string, Func<string, object>> ConvertMapper = new ()
        {
            { CommandParser.Firstname, new (Converter.TryConvertToObject<string>) },
            { CommandParser.LastName, new (Converter.TryConvertToObject<string>) },
            { CommandParser.Dateofbirth, new (Converter.TryConvertToObject<DateTime>) },
            { CommandParser.IdentificationNumber, new (Converter.TryConvertToObject<decimal>) },
            { CommandParser.Points, new (Converter.TryConvertToObject<short>) },
            { CommandParser.Id, new (Converter.TryConvertToObject<int>) },
            { CommandParser.Letter, new (Converter.TryConvertToObject<char>) },
        };

        private static readonly Dictionary<string, Func<FileCabinetRecord, string>> GetterMapper = new ()
        {
            { CommandParser.Id, r => r.Id.ToString(CultureInfo.CurrentCulture) },
            { CommandParser.Firstname, r => new string(r.FirstName) },
            { CommandParser.LastName, r => new string(r.LastName) },
            { CommandParser.Dateofbirth, r => new string(r.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.CurrentCulture)) },
            { CommandParser.IdentificationNumber, r => r.IdentificationNumber.ToString(CultureInfo.CurrentCulture) },
            { CommandParser.Points, r => r.PointsForFourTests.ToString(CultureInfo.CurrentCulture) },
            { CommandParser.Letter, r => r.IdentificationLetter.ToString(CultureInfo.CurrentCulture) },
        };

        private static readonly Dictionary<string, Func<Predicate<FileCabinetRecord>, Predicate<FileCabinetRecord>, Predicate<FileCabinetRecord>>> PredicateCompositor = new ()
        {
            { CommandParser.Or, (p1, p2) => r => p1(r) || p2(r) },
            { CommandParser.And, (p1, p2) => r => p1(r) && p2(r) },
        };

        /// <summary>
        /// Gets GlobalParametersGetter.
        /// </summary>
        /// <value>Getter for all record fields.</value>
        public static Func<FileCabinetRecord, List<string>> GlobalParametersGetter
        {
            get
            {
                var getters = new List<Func<FileCabinetRecord, string>>();
                foreach (var getter in GetterMapper)
                {
                    getters.Add(getter.Value);
                }

                return r => getters.Select(g => g(r)).ToList();
            }
        }

        /// <summary>
        /// Get all headers.
        /// </summary>
        /// <returns>All headers.</returns>
        public static string[] GetAllHeaders()
        {
            var headers = new List<string>
            {
                Id,
                Firstname,
                LastName,
                Dateofbirth,
                IdentificationNumber,
                Points,
                Letter,
            };

            return headers.ToArray();
        }

        /// <summary>
        /// Parser for where Command.
        /// </summary>
        /// <param name="parameters">Given string for parsing.</param>
        /// <param name="query">Query.</param>
        /// <returns>Result of parsing.</returns>
        public static Tuple<bool, string> WhereParser(string parameters, out Query query)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            query = new Query();
            var separatedParameters = parameters.Split(' ', 2);
            if (separatedParameters.Length < 1 || !separatedParameters[0].Equals(Where))
            {
                return new (false, $"The request must start with the keyword '{Where}'");
            }

            if (separatedParameters.Length == 1 || string.IsNullOrWhiteSpace(separatedParameters[1]))
            {
                query.Hash = string.Empty.GetHashCode();
                query.Predicate = r => true;
                return new (true, string.Empty);
            }

            var pattern = $@"({And})|({Or})";
            var splitSecond = Regex.Split(separatedParameters[1], pattern);

            Predicate<FileCabinetRecord> complexPredicat = null;
            Predicate<FileCabinetRecord> currentPredicate = null;
            bool logicalOperation = false;
            string currentLogicOperation = null;
            query.Hash = 0;
            foreach (var s in splitSecond)
            {
                if (logicalOperation)
                {
                    currentLogicOperation = s;
                    logicalOperation = false;
                    query.Hash += currentLogicOperation.GetHashCode();
                }
                else
                {
                    var splitThird = Regex.Split(s, $@"({Equals})").Select(s => s.Trim(' ')).Where(s => !string.IsNullOrEmpty(s)).ToArray();
                    if (splitThird.Length != 3 || !splitThird[1].Equals(Equals))
                    {
                        return new (false, $"Invalid expression. Failed to interpret '{s}'");
                    }

                    splitThird[2] = splitThird[2].Trim('\'');
                    if (!ConvertMapper.TryGetValue(splitThird[0], out Func<string, object> converter))
                    {
                        return new (false, $"Invalid expression. Failed to interpret {string.Join(string.Empty, splitThird)}. Word {splitThird[0]} is not reserved. Reserved word list:\n - {string.Join("\n - ", PredicateMapper.Keys)}");
                    }

                    object convertedValue;
                    try
                    {
                        convertedValue = converter(splitThird[2]);
                    }
                    catch (ArgumentException ex)
                    {
                        return new (false, $"Conversion error for '{string.Join(string.Empty, splitThird)}'. {ex.Message}");
                    }

                    currentPredicate = PredicateMapper[splitThird[0]](convertedValue);
                    complexPredicat = (currentLogicOperation is null) ? currentPredicate : PredicateCompositor[currentLogicOperation](complexPredicat, currentPredicate);
                    logicalOperation = true;
                    query.Hash += string.Join(string.Empty, splitThird).GetHashCode();
                }
            }

            query.Predicate = complexPredicat;

            return new (true, string.Empty);
        }

        /// <summary>
        /// Parser for insert command.
        /// </summary>
        /// <param name="parameters">Given string for parsing.</param>
        /// <param name="record">Result record.</param>
        /// <returns>Result of parsing.</returns>
        public static Tuple<bool, string> InsertParser(string parameters, out FileCabinetRecord record)
        {
            record = GetDefaultRecord();

            var separatedByString = Regex.Split(parameters, $@"({Values})").Where(s => !string.IsNullOrEmpty(s)).Select(s => s.Trim(' ')).ToArray();
            if (separatedByString.Length != 3)
            {
                return new (false, "Invalid arguments");
            }

            if (!separatedByString[1].Equals(Values))
            {
                return new (false, $"Not found keyword {Values}");
            }

            char[] brackets = { '(', ')', ' ' };
            var separatedByBrackets = separatedByString.Select(s => s.Trim(brackets)).ToArray();
            var fullySeparated = separatedByBrackets.Select(s => s.Split(',')).ToList();

            char[] separators = { '\'', ' ' };
            var arguments = fullySeparated[0].Select(arg => arg.Trim(separators)).ToArray();
            var values = fullySeparated[2].Select(arg => arg.Trim(separators)).ToArray();
            if (values.Length != arguments.Length)
            {
                return new (false, "Wrong number of arguments and values.");
            }

            for (int i = 0; i < arguments.Length; i++)
            {
                if (!ConvertMapper.TryGetValue(arguments[i], out Func<string, object> converter))
                {
                    return new (false, $"Invalid keyword {arguments[i]}. Word {arguments[i]} is not reserved. Reserved word list:\n - {string.Join("\n - ", PredicateMapper.Keys)}");
                }

                object convertedValue;
                try
                {
                    convertedValue = converter(values[i]);
                }
                catch (ArgumentException ex)
                {
                    return new (false, $"Conversion error for '{arguments[i]} - {values[i]}'. {ex.Message}");
                }

                SetterMapper[arguments[i]](record, convertedValue);
            }

            return new (true, " ");
        }

        /// <summary>
        /// Parser for set command.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        /// <param name="setAction">Action.</param>
        /// <returns>Rsult of parsing.</returns>
        public static Tuple<bool, string> SetParser(string parameters, out Action<FileCabinetRecord> setAction)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            setAction = null;
            var separatedParameters = parameters.Split(' ', 2);
            if (separatedParameters.Length < 1 || !separatedParameters[0].Equals(Set))
            {
                return new (false, $"The request must start with the keyword {Set}");
            }

            var splitSecond = separatedParameters[1].Split(',', StringSplitOptions.TrimEntries);

            Action<FileCabinetRecord> complexAction = null;
            foreach (var actionString in splitSecond)
            {
                var splitThird = Regex.Split(actionString, $@"({Equals})");
                var splitFour = splitThird.Select(s => s.Trim(' ')).ToArray();
                if (splitFour.Length != 3 || !splitFour[1].Equals(Equals))
                {
                    return new (false, $"Invalid expression. Failed to interpret {actionString}");
                }

                if (!ConvertMapper.TryGetValue(splitFour[0], out Func<string, object> convertor))
                {
                    return new (false, $"Invalid keyword {splitFour[0]}. Word {splitFour[0]} is not reserved. Reserved word list:\n - {string.Join("\n - ", SetterMapper.Keys)}");
                }

                var actionMaker = ActionMapper[splitFour[0]];

                object convertedValue;
                try
                {
                    convertedValue = convertor(splitFour[2].Trim('\''));
                }
                catch (ArgumentException ex)
                {
                    return new (false, $"Conversion error for '{actionString}'. {ex.Message}");
                }

                var action = actionMaker(convertedValue);
                complexAction += action;
            }

            setAction = complexAction;
            return new (true, string.Empty);
        }

        /// <summary>
        /// Parser for select command.
        /// </summary>
        /// <param name="parameters">Given string for parsing.</param>
        /// <param name="parametersGetter">A delegate that applies to a record and returns a collection of parameters for that record.</param>
        /// <returns>Result of parsing.</returns>
        public static Tuple<bool, string> SelectParser(string parameters, out Func<FileCabinetRecord, List<string>> parametersGetter)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            parametersGetter = null;
            var splitedparameters = parameters.Split(',').Select(s => s.Trim(' ')).ToArray();

            var getters = new List<Func<FileCabinetRecord, string>>();
            foreach (var arg in splitedparameters)
            {
                var result = GetterMapper.TryGetValue(arg.ToLower(CultureInfo.CurrentCulture), out Func<FileCabinetRecord, string> getter);
                if (!result)
                {
                    return new (false, $"Invalid keyword {arg}. Word {arg} is not reserved. Reserved word list:\n - {string.Join("\n - ", PredicateMapper.Keys)}");
                }

                getters.Add(getter);
            }

            parametersGetter = r => getters.Select(g => g(r)).ToList();
            return new (true, string.Empty);
        }

        private static FileCabinetRecord GetDefaultRecord()
        {
            return new FileCabinetRecord()
            {
                DateOfBirth = default,
                FirstName = string.Empty,
                Id = default,
                IdentificationLetter = default,
                IdentificationNumber = default,
                LastName = string.Empty,
                PointsForFourTests = default,
            };
        }
    }
}
