// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using SnmpSharpNet.Tests;
using SnmpSharpNet.Tests.AuthenticationTests;

var summary = BenchmarkRunner.Run<AuthenticationBenchmarks>();