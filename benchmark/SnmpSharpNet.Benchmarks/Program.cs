// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using SnmpSharpNet.Tests;

var summary = BenchmarkRunner.Run<Counter64Benchmarks>();