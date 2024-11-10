﻿using UserService.Exceptions;

namespace UserService.Abstractions
{
    public interface IValidationResult
    {
        public static readonly ApplicationError ValidationError = new("ValidationError", "A validation problem occurred.");

        ApplicationError[] Errors { get; }
    }
}
