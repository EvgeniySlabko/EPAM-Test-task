using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Interface for all validators.
    /// </summary>
    /// <typeparam name="T">The type of the variable to be validated.</typeparam>
    public interface IValidator<T>
    {
        /// <summary>
        /// Validation method.
        /// </summary>
        /// <param name="value">Given value or validation.</param>
        /// <returns>True if the value is valid otherwise false.</returns>
        public Tuple<bool, string> Validate(T value);
    }
}
