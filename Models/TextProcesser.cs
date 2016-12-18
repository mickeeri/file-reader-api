
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileReaderAPI.Models 
{
  public static class TextProcesser
  {
    public static Result ReplaceMostCommonWords(string fileName, string source) 
    {
        char[] delimiters = { ' ', '.', ',', ';', '\'', '-', ':', '!', '?', '(', ')', '<', '>', '=', '*', '/', '[', ']', '{', '}', '\\', '"', '\r', '\n' };                            

        // Split text and create new object ordered by highest word count. 
        var results = source.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Where(x => x.Length > 3)
                                    .GroupBy(x => x)
                                    .Select(x => new { Count = x.Count(), Word = x.Key })
                                    .OrderByDescending(x => x.Count);

        // Extract the word count of the first object. 
        int highestWordCount = results.First().Count;
        
        // Get the elements with that word count. 
        var elementsWithHighestWordCount = results.Where(x => x.Count == highestWordCount);

        string processedContent = source;
        List<string> mostCommonWords = new List<string>();

        foreach (var element in elementsWithHighestWordCount)
        {
            // Surround the common word with foo and bar. 
            processedContent = processedContent.Replace(element.Word, "foo" + element.Word + "bar");

            // Add the word to list of most common word. 
            mostCommonWords.Add(element.Word);
        }

        return new Result { FileName = fileName, ProcessedContent = processedContent, MostCommonWords = mostCommonWords };
    }
  }
}
