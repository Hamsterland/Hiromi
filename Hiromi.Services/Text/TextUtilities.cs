using System;
using System.Text;

namespace Hiromi.Services.Text
{
    /// <summary>
    /// Utilities for text manipulation.
    /// </summary>
    public static class TextUtilities
    {
        /// <summary>
        /// "Uwufies" text by replacing all occurrences of "l" and "r" with "w" and
        /// all occurrences of "mo" and "no" with "myo" and "nyo".  
        /// </summary>
        /// <param name="text">The text to uwufy.</param>
        /// <returns>
        /// An uwufied version of <see cref="text"/>.
        /// </returns>
        public static string Uwufy(string text)
        {
            _ = text ?? throw new ArgumentNullException(nameof(text));

            var builder = new StringBuilder();

            for (var i = 0; i < text.Length; i++)
            {
                string TryInsertY() =>
                    i + 1 < text.Length && text[i + 1] == 'o'
                        ? $"{text[i]}y"
                        : $"{text[i]}";

                var append = text[i] switch
                {
                    'l' => "w",
                    'r' => "w",
                    'm' => TryInsertY(),
                    'n' => TryInsertY(),
                    _ => $"{text[i]}"
                };

                builder.Append(append);
            }

            return builder.ToString();
        }
    }
}