using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RecorPredicate = System.Predicate<FileCabinetRecord>;

namespace FileCabinetApp
{
    /// <summary>
    /// Parser.
    /// </summary>
    public class Parser
    {
        private const string Or = "or";
        private const string And = "and";

        private const string Firstname = "firstname";
        private const string LastName = "lastname";
        private const string Dateofbirth = "dateofbirth";
        private const string IdentificationNumber = "identificationNumber";
        private const string Points = "points";
        private const string Id = "id";
        private const string Letter = "letter";
        private new const string Equals = "=";

        private readonly Dictionary<string, Func<object, Action<FileCabinetRecord>>> actionMapper = new ()
        {
            { Parser.Firstname, o => r => r.FirstName = (string)o },
            { Parser.LastName, o => r => r.LastName = (string)o },
            { Parser.Dateofbirth, o => r => r.DateOfBirth = (DateTime)o },
            { Parser.IdentificationNumber, o => r => r.IdentificationNumber = (decimal)o },
            { Parser.Points, o => r => r.PointsForFourTests = (short)o },
            { Parser.Id, o => r => r.Id = (int)o },
            { Parser.Letter, o => r => r.IdentificationLetter = (char)o },
        };

        private readonly Dictionary<string, Func<object, Predicate<FileCabinetRecord>>> predicateMapper = new ()
        {
            { Parser.Firstname, o => r => r.FirstName.Equals((string)o) },
            { Parser.LastName, o => r => r.LastName.Equals((string)o) },
            { Parser.Dateofbirth, o => r => r.DateOfBirth.Equals((DateTime)o) },
            { Parser.IdentificationNumber, o => r => r.IdentificationNumber.Equals((decimal)o) },
            { Parser.Points, o => r => r.PointsForFourTests.Equals((short)o) },
            { Parser.Id, o => r => r.Id.Equals((int)o) },
            { Parser.Letter, o => r => r.IdentificationLetter.Equals((char)o) },
        };

        private readonly Dictionary<string, Func<string, object>> convertMapper = new ()
        {
            { Parser.Firstname, new (Converter.TryConvertToObject<string>) },
            { Parser.LastName, new (Converter.TryConvertToObject<string>) },
            { Parser.Dateofbirth, new (Converter.TryConvertToObject<DateTime>) },
            { Parser.IdentificationNumber, new (Converter.TryConvertToObject<decimal>) },
            { Parser.Points, new (Converter.TryConvertToObject<short>) },
            { Parser.Id, new (Converter.TryConvertToObject<int>) },
            { Parser.Letter, new (Converter.TryConvertToObject<char>) },
        };

        private readonly Dictionary<string, Func<RecorPredicate, RecorPredicate, RecorPredicate>> predicateCompositor = new ()
        {
            { Parser.Or, (p1, p2) => r => p1(r) || p2(r) },
            { Parser.And, (p1, p2) => r => p1(r) && p2(r) },
        };

        /// <summary>
        /// Parser for where Command.
        /// </summary>
        /// <param name="parameters">Given string for parsing.</param>
        /// <param name="predicate">Result set of predicates.</param>
        /// <returns>Result of parsing.</returns>
        public Tuple<bool, string> WhereParser(string parameters, out Predicate<FileCabinetRecord> predicate)
        {
            predicate = null;
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            string[] separators = { Or,  And, };
            var pattern = $@"({And})|({Or})";
            var splitSecond = Regex.Split(parameters, pattern);

            RecorPredicate complexPredicat = null;
            RecorPredicate currentPredicate = null;
            bool logicalOperation = false;
            string currentLogicOperation = null;
            foreach (var s in splitSecond)
            {
                if (logicalOperation)
                {
                    currentLogicOperation = s;
                    logicalOperation = false;
                }
                else
                {
                    var splitThird = Regex.Split(s, $@"(=)").Select(s => s.Trim(' ')).ToArray();
                    if (splitThird.Length != 3 || !splitThird[1].Equals(Equals))
                    {
                        return new (false, $"Invalid expression. Failed to interpret {s}");
                    }

                    splitThird[2] = splitThird[2].Trim('\'');
                    if (!this.convertMapper.TryGetValue(splitThird[0], out Func<string, object> converter))
                    {
                        return new (false, $"Invalid expression. Failed to interpret {splitThird[1]}");
                    }

                    currentPredicate = this.predicateMapper[splitThird[0]](converter(splitThird[2]));
                    complexPredicat = (currentLogicOperation is null) ? currentPredicate : this.predicateCompositor[currentLogicOperation](complexPredicat, currentPredicate);
                    logicalOperation = true;
                }
            }

            predicate = complexPredicat;

            return new (true, "Successfully");
        }

        /// <summary>
        /// Parser for set Command.
        /// </summary>
        /// <param name="parameters">Given string for parsing.</param>
        /// <param name="setAction">Result of parsing. A set of actions on a record.</param>
        /// <returns>Result of parsing.</returns>
        public Tuple<bool, string> SetParser(string parameters, out Action<FileCabinetRecord> setAction)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            setAction = null;

            var splitSecond = parameters.Split(',', StringSplitOptions.TrimEntries);

            Action<FileCabinetRecord> complexAction = null;
            foreach (var actionString in splitSecond)
            {
                var splitThird = Regex.Split(actionString, $@"(=)");
                var splitFour = splitThird.Select(s => s.Trim(' ')).ToArray();
                if (splitFour.Length != 3 || !splitFour[1].Equals(Equals))
                {
                    return new (false, $"Invalid expression. Failed to interpret {actionString}");
                }

                if (!this.convertMapper.TryGetValue(splitFour[0], out Func<string, object> convertor))
                {
                    return new (false, $"Invalid expression. Failed to interpret {splitFour[1]}");
                }

                if (!this.actionMapper.TryGetValue(splitFour[0], out Func<object, Action<FileCabinetRecord>> actionMaker))
                {
                    return new (false, $"Invalid expression. Failed to interpret {splitFour[1]}");
                }

                var action = actionMaker(convertor(splitFour[2].Trim('\'')));
                complexAction = complexAction is null ? action : ActionCompositor(action, complexAction);
            }

            setAction = complexAction;
            return new (true, "Successfully");
        }

        private static Action<FileCabinetRecord> ActionCompositor(Action<FileCabinetRecord> action1, Action<FileCabinetRecord> action2)
        {
            return r =>
            {
                action1(r);
                action2(r);
            };
        }
    }
}
