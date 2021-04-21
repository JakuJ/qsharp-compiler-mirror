﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.Quantum.QsCompiler.Diagnostics;
using Microsoft.Quantum.QsCompiler.QIR;
using Microsoft.Quantum.QsCompiler.SyntaxTree;
using Microsoft.Quantum.QsCompiler.Transformations.Monomorphization.Validation;

namespace Microsoft.Quantum.QsCompiler.BuiltInRewriteSteps
{
    internal class QirGeneration : IRewriteStep
    {
        private readonly string outputFile;

        private readonly List<IRewriteStep.Diagnostic> diagnostics;

        public QirGeneration(string outputFileName)
        {
            this.outputFile = outputFileName;
            this.diagnostics = new List<IRewriteStep.Diagnostic>();
            this.AssemblyConstants = new Dictionary<string, string?>();
        }

        /// <inheritdoc/>
        public string Name => "QIR Generation";

        /// <inheritdoc/>
        public int Priority => -10; // currently not used

        /// <inheritdoc/>
        public IDictionary<string, string?> AssemblyConstants { get; }

        /// <inheritdoc/>
        public IEnumerable<IRewriteStep.Diagnostic> GeneratedDiagnostics => this.diagnostics;

        /// <inheritdoc/>
        public bool ImplementsPreconditionVerification => true;

        /// <inheritdoc/>
        public bool ImplementsTransformation => true;

        /// <inheritdoc/>
        public bool ImplementsPostconditionVerification => false;

        /// <inheritdoc/>
        public bool PreconditionVerification(QsCompilation compilation)
        {
            try
            {
                ValidateMonomorphization.Apply(compilation);
                return true;
            }
            catch
            {
                this.diagnostics.Add(new IRewriteStep.Diagnostic
                {
                    Severity = DiagnosticSeverity.Error,
                    Stage = IRewriteStep.Stage.PreconditionVerification,
                    Message = DiagnosticItem.Message(ErrorCode.SyntaxTreeNotMonomorphized, Array.Empty<string>()),
                    Source = Assembly.GetExecutingAssembly().Location
                });
                return false;
            }
        }

        /// <inheritdoc/>
        public bool Transformation(QsCompilation compilation, out QsCompilation transformed)
        {
            var generator = new Generator(compilation);
            generator.Apply();
            generator.Emit(this.outputFile);
            transformed = compilation;
            return true;
        }

        /// <inheritdoc/>
        public bool PostconditionVerification(QsCompilation compilation) =>
            throw new NotImplementedException();
    }
}
