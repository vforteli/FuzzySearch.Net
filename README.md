# FuzzySearchNet

[![Build](https://github.com/vforteli/FuzzySearch.Net/actions/workflows/build.yml/badge.svg)](https://github.com/vforteli/FuzzySearch.Net/actions/workflows/build.yml)

## Fuzzy search strings using levenshtein distance.   
This package is inspired by fuzzysearch for python (https://github.com/taleinat/fuzzysearch) and to some extent follows the same conventions.

# Installation
Build from source or download NuGet package: https://www.nuget.org/packages/FuzzySearch.Net


# Usage

Searching for strings in strings
``` csharp
  // Search with default options, substitutions, insertions, deletions and default maximum distance (3)
  var results = FuzzySearch.Find("sometext", "here is someteext for you");   
  
   // Search with specified maximum distance
  var results = FuzzySearch.Find("sometext", "here is someteext for you", 1);  
    
   // Search using only substitutions and default maximum distance (3)
  var results = FuzzySearch.Find("sometext", "here is someteext for you", SearchOptions.SubstitutionsOnly);  
  
  // Find returns a list of MatchResults with information about matches
  public class MatchResult
  {
      public int StartIndex { get; set; }
      public int EndIndex { get; set; }
      public int Distance { get; set; }
      public string Match { get; set; } = "";
      public int Deletions { get; set; }
      public int Substitutions { get; set; }
      public int Insertions { get; set; }
  }
```
