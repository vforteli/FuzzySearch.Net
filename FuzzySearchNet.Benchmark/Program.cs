using BenchmarkDotNet.Running;
using FuzzySearchNet.Benchmark;

var summary = BenchmarkRunner.Run<BenchmarkFuzzySearch>();



/*
 *



|           Method |     Mean |     Error |    StdDev |
|----------------- |---------:|----------:|----------:|
| SubstitutionOnly | 4.564 us | 0.0905 us | 0.2551 us |






|           Method |     Mean |    Error |   StdDev |
|----------------- |---------:|---------:|---------:|
| SubstitutionOnly | 18.98 us | 0.373 us | 0.510 us |




















*/