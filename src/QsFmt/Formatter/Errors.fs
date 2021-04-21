﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

module Microsoft.Quantum.QsFmt.Formatter.Errors

open Antlr4.Runtime

type SyntaxError =
    {
        /// The line number.
        line: int

        /// The column number.
        column: int

        /// The error message.
        message: string
    }

    override error.ToString() =
        sprintf "Line %d, column %d: %s" error.line error.column error.message

[<Sealed>]
type ListErrorListener() =
    let mutable errors = []

    member _.SyntaxErrors = errors

    interface IToken IAntlrErrorListener with
        member _.SyntaxError(_, _, _, line, column, message, _) =
            let error =
                {
                    line = line
                    column = column
                    message = message
                }

            errors <- error :: errors
