using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyraPad
{
    /// <summary>
    /// throw this exception whenever the pad lacks sufficient information to build the UI.
    /// </summary>
    public class CannotBuildException : Exception
    {
        public CannotBuildErrorType ErrorType { get; }

        public CannotBuildException(CannotBuildErrorType errorType, string error) : base(error)
        {
            if (String.IsNullOrWhiteSpace(error))
                throw new ArgumentException("Eror message cannot be null or empty!", nameof(error));

            ErrorType = errorType;
        }
    }

    public enum CannotBuildErrorType
    {
        FONT_MISSING,
        XML_SYNTAX_ERROR
    }
}
