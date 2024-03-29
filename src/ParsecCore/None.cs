﻿namespace ParsecCore
{
    /// <summary>
    /// Helper struct that represents an empty value
    /// Used primarily for type compatibility where we require a type argument
    /// but also logically the type argument should be empty
    /// </summary>
    public struct None
    {
        public static readonly None Instance = new();
    }
}
