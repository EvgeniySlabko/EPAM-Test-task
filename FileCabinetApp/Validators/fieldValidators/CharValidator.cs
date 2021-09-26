using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Vaidator for char type.
    /// </summary>
    public class CharValidator : IValidator<char>
    {
        private readonly List<Predicate<char>> predicateList;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharValidator"/> class.
        /// </summary>
        /// <param name="predicate">Predicate for validate char character.</param>
        public CharValidator(Predicate<char> predicate)
        {
            this.predicateList = new ();
            this.predicateList.Add(predicate);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharValidator"/> class.
        /// </summary>
        /// <param name="predicateList">List of predicates for validate char character.</param>
        public CharValidator(List<Predicate<char>> predicateList)
        {
            this.predicateList = predicateList;
        }

        /// <inheritdoc/>
        public Tuple<bool, string> Validate(char inputValue)
        {
            bool valid = true;
            string message = string.Empty;

            foreach (var predicate in this.predicateList)
            {
                if (!predicate(inputValue))
                {
                    valid = false;
                    message = "Invaling char value";
                }
            }

            return new Tuple<bool, string>(valid, message);
        }
    }
}
