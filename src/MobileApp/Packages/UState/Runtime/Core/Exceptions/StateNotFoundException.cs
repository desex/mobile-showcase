using System;

namespace UState
{
    public sealed class StateNotFoundException : InvalidOperationException
    {
        public StateNotFoundException(string stateName) 
            : base($"State \"{stateName}\" not found")
        {
        }
    }
}