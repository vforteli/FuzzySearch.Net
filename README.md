# FuzzySearchNet

[![Build](https://github.com/vforteli/FuzzySearch.Net/actions/workflows/build.yml/badge.svg)](https://github.com/vforteli/FuzzySearch.Net/actions/workflows/build.yml)

## Fuzzy search strings using levenshtein distance.  
This package can be used to search strings for sub sequences with a specified max levenshtein distance. Multiple matches with their indexes and distances will be returned if found.

Inspired by fuzzysearch for python (https://github.com/taleinat/fuzzysearch) and to some extent follows the same conventions.

# Installation
Build from source or download NuGet package: https://www.nuget.org/packages/FuzzySearch.Net

Target frameworks .Net 6 and .Net Standard 2.1

# Usage

Searching for strings in strings
``` csharp
  // Search with default options, substitutions, insertions, deletions and default maximum distance (3)
  var results = FuzzySearch.Find("sometext", "here is someteext for you");   
  
  // Search with specified maximum distance
  var results = FuzzySearch.Find("sometext", "here is someteext for you", 1);  
    
  // Search using only substitutions and default maximum distance (3)
  var results = FuzzySearch.Find("sometext", "here is someteext for you", SearchOptions.SubstitutionsOnly);  
  
  // Search using with more specific options, for example allowing more substitutions than insertions and deletions
  var results = FuzzySearch.Find(word, text, new FuzzySearchOptions(3, 1, 1));
  
  // Check for any matches using Linq. Using Any or First is more efficient than count since enumeration will stop after first match.
  // This will not necessarily yield the best match though.
  if(FuzzySearch.Find(word, text, 2).Any()) {
    // do stuff
  }

  // Use stream and asynchronously enumerate matches
  await foreach (var match in FuzzySearch.FindLevenshteinAsync("somepattern", textstream))
  {
    // do stuff
  }
  
    
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

# Performance considerations
Prefer limiting matching to substitutions only in case insertions and deletions are not needed. Substitution only matching is generally several orders of magnitude faster than Levenshtein distance with insertions and deletions.   

With small texts, the non async methods will put much less pressure on garbage collections. With larger texts, the streaming async methods can avoid reading the whole text into memory at the cost of more GC pressure.