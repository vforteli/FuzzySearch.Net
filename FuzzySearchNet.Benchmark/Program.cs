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



|                                 Method |     Mean |     Error |    StdDev |   Median |
|--------------------------------------- |---------:|----------:|----------:|---------:|
|         SubstitutionOnlyBufferingShort | 2.582 us | 0.0515 us | 0.1233 us | 2.543 us |
|          SubstitutionOnlyBufferingLong | 2.053 us | 0.0407 us | 0.0743 us | 2.019 us |
| SubstitutionOnlyBufferingLong3distance | 2.819 us | 0.0559 us | 0.1285 us | 2.782 us |

|                                 Method |     Mean |     Error |    StdDev |   Median |
|--------------------------------------- |---------:|----------:|----------:|---------:|
|         SubstitutionOnlyBufferingShort | 2.690 us | 0.0536 us | 0.1539 us | 2.720 us |
|          SubstitutionOnlyBufferingLong | 2.042 us | 0.0405 us | 0.1074 us | 2.001 us |
| SubstitutionOnlyBufferingLong3distance | 2.824 us | 0.0563 us | 0.1412 us | 2.754 us |




*/