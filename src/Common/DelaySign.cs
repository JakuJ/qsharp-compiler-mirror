﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if SIGNED
using System.Reflection;

// Attributes for delay-signing
[assembly: AssemblyKeyFile("..\\..\\..\\build\\267DevDivSNKey2048.snk")]
[assembly: AssemblyDelaySign(true)]
#endif
