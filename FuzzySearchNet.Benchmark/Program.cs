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








|                                 Method |      Mean |     Error |    StdDev |
|--------------------------------------- |----------:|----------:|----------:|
|         SubstitutionOnlyBufferingShort |  3.704 us | 0.0740 us | 0.0909 us |
|          SubstitutionOnlyBufferingLong |  2.818 us | 0.0292 us | 0.0244 us |
| SubstitutionOnlyBufferingLong3distance |  4.396 us | 0.0501 us | 0.0492 us |



|                                 Method |     Mean |     Error |    StdDev |
|--------------------------------------- |---------:|----------:|----------:|
|         SubstitutionOnlyBufferingShort | 2.628 us | 0.0389 us | 0.0570 us |
|          SubstitutionOnlyBufferingLong | 2.031 us | 0.0404 us | 0.0580 us |
| SubstitutionOnlyBufferingLong3distance | 2.808 us | 0.0548 us | 0.0988 us |


|                                 Method |     Mean |     Error |    StdDev |   Median |
|--------------------------------------- |---------:|----------:|----------:|---------:|
|         SubstitutionOnlyBufferingShort | 2.434 us | 0.0264 us | 0.0220 us | 2.438 us |
|          SubstitutionOnlyBufferingLong | 1.843 us | 0.0366 us | 0.0740 us | 1.810 us |
| SubstitutionOnlyBufferingLong3distance | 2.471 us | 0.0494 us | 0.1105 us | 2.468 us |







*/