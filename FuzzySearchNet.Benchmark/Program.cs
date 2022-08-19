using BenchmarkDotNet.Running;
using FuzzySearchNet.Benchmark;

var summary = BenchmarkRunner.Run<BenchmarkFuzzySearch>();