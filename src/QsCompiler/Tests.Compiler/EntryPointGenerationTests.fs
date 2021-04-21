﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Quantum.QsCompiler.Testing

open System.IO
open Xunit
open Xunit.Abstractions
open Microsoft.Quantum.QsCompiler
open Microsoft.Quantum.QsCompiler.BondSchemas.EntryPoint
open System.Text

type EntryPointGenerationTests(output: ITestOutputHelper) =

    let testCasesDirectory = Path.Combine("TestCases", "QirEntryPointTests")

    [<Theory>]
    [<InlineData("TestUnitReceivesBool")>]
    member this.SerializeToJson(testFileName: string) =
        let expectedCpp = Path.Join(testCasesDirectory, (testFileName + ".cpp")) |> File.ReadAllText
        let entryPointOperationJson = Path.Join(testCasesDirectory, (testFileName + ".json")) |> File.ReadAllText
        let entryPointOperationMs = new MemoryStream(Encoding.UTF8.GetBytes(entryPointOperationJson))
        let entryPointOperation = Protocols.DeserializeFromJson(entryPointOperationMs)
        let cppMs = new MemoryStream()
        QirDriverGeneration.GenerateQirDriverCpp(entryPointOperation, cppMs)
        let cppReader = new StreamReader(cppMs, Encoding.UTF8)
        let generatedCpp = cppReader.ReadToEnd()
        Assert.Equal(expectedCpp, generatedCpp)
