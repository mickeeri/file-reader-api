
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
            // Using Regex to only match whole words. 
            string  pattern = String.Format(@"\b{0}\b", element.Word);

            // Surround the common word with foo and bar.             
            processedContent = Regex.Replace(processedContent, pattern, String.Format("foo{0}bar", element.Word));

            // Add the word to list of most common word. 
            mostCommonWords.Add(element.Word);
        }

        return new Result { FileName = fileName, ProcessedContent = processedContent, MostCommonWords = mostCommonWords };
    }
  }
}
