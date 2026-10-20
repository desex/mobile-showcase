using System;

namespace UState
{
    public sealed class EmptyHistoryException : InvalidOperationException
    {
        public EmptyHistoryException(string stateMachineName) 
            : base($"State machine \"{stateMachineName}\" has an empty history.")
        {
        }
    }
}