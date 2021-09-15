using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Email_Extraction
{
    class Program
    {
        static void Main(string[] args)
        {
            const string filePath = @"sample.txt";

            if (TryReadFile(filePath, out string fileText))
            {
                Part1(fileText);
                Part2(fileText);
                Part3(fileText);
            }
        }

        private static bool TryReadFile(string filePath, out string fileText)
        {
            try
            {
                fileText = File.ReadAllText(filePath);
                return true;
            }
            catch (IOException e)
            {
                Console.WriteLine($"Oh no! An exception was thrown while trying to read {filePath}.\nException message: {e.Message}");
                fileText = null;
                return false;
            }
        }

        //This finds the number of substrings matching @softwire.com so @softwire.comet will also be matched here.
        private static void Part1(string fileText)
        {
            var softwireDomain = "@softwire.com";

            var matchCount = 0;

            for (var i = 0; i <= fileText.Length - softwireDomain.Length; i++)
            {
                //Domains are case insensitive so StringComparison.OrdinalIgnoreCase is used.
                if (fileText.Substring(i, softwireDomain.Length).Equals(softwireDomain, StringComparison.OrdinalIgnoreCase))
                {
                    matchCount++;
                }
            }
            Console.WriteLine($"Part 1\nNumber of substrings matching {softwireDomain}:\t{matchCount}\n");
        }


        private static void Part2(string fileText)
        {
            const string pattern = @"\b\S+@softwire\.com\b";
            var rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            var validEmails = ExtractValidEmailMatchesFromFile(rgx, fileText);
            Console.WriteLine($"Part 2\nNumber of Softwire emails found using regular expressions:\t{validEmails.Count()}\n");
        }

        private static void Part3(string fileText)
        {
            var domainCounts = new Dictionary<string, int>();
            // This pattern will incorrectly match email address with a whitespace or an @ symbol in the local part.
            const string pattern = @"\b\S+@(\S*)\b";
            var rgx = new Regex(pattern);
            var validMatches = ExtractValidEmailMatchesFromFile(rgx, fileText);

            foreach (var email in validMatches)
            {
                var domain = email.Groups[1].Value;
                domainCounts.TryGetValue(domain, out var currentCount);
                domainCounts[domain] = currentCount + 1;
            }

            var orderedDomains = domainCounts.OrderByDescending(d => d.Value);

            Console.WriteLine("Part 3");
            Console.WriteLine("{0,-20}{1,-20}", "Domain", "Count");
            foreach (var domainCount in orderedDomains)
            {
                Console.WriteLine("{0,-20}{1,-20}", domainCount.Key, domainCount.Value);
            }
        }

        private static IEnumerable<Match> ExtractValidEmailMatchesFromFile(Regex regex, string fileText)
        {
            return regex.Matches(fileText)
                .Cast<Match>()
                .Where(m => IsValidEmail(m.Value));
        }

        private static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
    }
}