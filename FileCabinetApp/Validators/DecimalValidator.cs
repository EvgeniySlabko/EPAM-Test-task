﻿using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Vaidator for decimal type.
    /// </summary>
    public class DecimalValidator : IValidator<decimal>
    {
        private readonly decimal minValue;
        private readonly decimal maxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalValidator"/> class.
        /// String validator constructor.
        /// </summary>
        /// <param name="maxValue">Maximum value.</param>
        /// <param name="minValue">Minimal value length.</param>
        public DecimalValidator(decimal minValue, decimal maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <returns>Gets delegate for Validate mrthod.</returns>
        public Func<decimal, Tuple<bool, string>> GetDelegate()
        {
            return this.Validate;
        }

        /// <summary>
        /// Given value.
        /// </summary>
        /// <param name="inputValue">Given decimal value.</param>
        /// <returns>Result tupple. bool value - validation result. string value - message string.</returns>
        public Tuple<bool, string> Validate(decimal inputValue)
        {
            bool valid;
            string message;
            if (inputValue < this.maxValue && inputValue > this.minValue)
            {
                message = "Succesful";
                valid = true;
            }
            else
            {
                message = "Invalind value";
                valid = false;
            }

            return new Tuple<bool, string>(valid, message);
        }
    }
}