﻿# QsFmt: Q# Formatter

QsFmt is a source code formatter for Q#.
It's in the very early stages of development and is currently experimental.
It will very likely eat your code when it tries to format it!

You can use the command-line tool by running `dotnet run -p App` from this folder.
This will only print the formatted code to the console, and won't overwrite your files, so it's safe to use.

## Limitations

* The current formatting capabilities are very simple, with only basic indentation and whitespace normalization.
* Many types of syntax are not yet supported and will remain unchanged by the formatter.
* There are several bugs related to other types of syntax, especially callable declaration syntax like attributes, type parameters, and specializations.
  These may be swallowed by the formatter, resulting in incorrect output.

## Design

QsFmt uses a [concrete syntax tree](https://en.wikipedia.org/wiki/Parse_tree), which is lossless: a Q# program parsed into a CST can be converted back into a string without any loss of information.
QsFmt's syntax tree is modeled on the Q# compiler's abstract syntax tree, but with additional information on every node for tokens like semicolons and curly braces that are unnecessary in an AST.
Whitespace and comment tokens, known as *trivia tokens*, are attached as a prefix to a non-trivia token.
For example, in the expression `x + y`, `x` has prefix `""`, `+` has prefix `" "`, and `y` has prefix `" "`.

QsFmt uses [ANTLR](https://www.antlr.org/) to parse Q# programs.
It uses a grammar based on the [grammar in the Q# language specification](https://github.com/microsoft/qsharp-language/tree/main/Specifications/Language/5_Grammar).
ANTLR's parse tree is then converted into Q# Formatter's concrete syntax tree.

Formatting rules are a mapping from one CST to another CST.
Then the formatting pipeline is:

1. Parse a Q# program into a CST.
2. Apply formatting rules to the CST in order.
3. Unparse the CST into a Q# program.

This allows formatting transformations (indentation, brace position, etc.) to be written separately, and in many cases may be independent from each other.
(However, there may be dependencies where one transformation must run before another.)
This will hopefully make the formatting transformations more modular and simpler to write.
