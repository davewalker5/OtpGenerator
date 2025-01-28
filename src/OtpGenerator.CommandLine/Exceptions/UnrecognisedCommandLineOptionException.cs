﻿using System.Diagnostics.CodeAnalysis;

namespace OtpGenerator.CommandLine.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class UnrecognisedCommandLineOptionException : Exception
    {
        public UnrecognisedCommandLineOptionException()
        {
        }

        public UnrecognisedCommandLineOptionException(string message) : base(message)
        {
        }

        public UnrecognisedCommandLineOptionException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
