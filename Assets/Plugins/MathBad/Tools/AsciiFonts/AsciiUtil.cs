#region
using System;
using System.Collections.Generic;
using UnityEngine;
#endregion

//    _            _  _  _   _  _    _  _      //
//   /_\   ___ __ (_)(_)| | | || |_ (_)| | ___ //
//  / _ \ (_-</ _|| || || |_| ||  _|| || |(_-< //
// /_/ \_\/__/\__||_||_| \___/  \__||_||_|/__/ //
//                                             //
namespace MathBad
{
public static class AsciiUtil
{
  public static string[] GetCharacter(AsciiFontType type, char c)
  {
    bool success = AsciiFonts.FontLookup[type].TryGetValue(c, out string[] ascii);
    if(success)
      return ascii;
    else
      return AsciiFonts.FontLookup[type][' '];
  }

  public static string[] GetCharacter(Dictionary<char, string[]> font, char c)
  {
    bool success = font.TryGetValue(c, out string[] ascii);
    if(success)
      return ascii;
    else
      return font[' '];
  }

  /// <summary>
  /// Converts selected text in the editor, to AsciiText
  /// </summary>
  /// <param name="input">The string to be converted</param>
  /// <returns></returns>
  public static string ToAsciiCommentHeader(string input)
  {
    string[] output = ToAsciiLines(AsciiFonts.Small, input);
    int height = output.Length;
    int width = output[0].Length;
    //----------------------------------------------------------------------------------------------------//

    string result = "";
    for(int i = 0; i <= height; i++)
    {
      if(i < height)
      {
        string m = output[i];
        result += "// " + m + new string(' ', Math.Max(0, 99 - m.Length)) + "\n";
      }
      else
      {
        result += "//" + new string('-', 100) + "\n";
      }
    }

    return result;
  }

  /// <summary>
  /// Converts selected text in the editor, to AsciiText
  /// </summary>
  /// <param name="input">The string to be converted</param>
  /// <returns></returns>
  public static string ToAsciiComment(string input)
  {
    string[] output = ToAsciiLines(AsciiFonts.Small, input);
    int height = output.Length;
    string result = "";

    for(int i = 0; i < height; i++) result += "// " + output[i] + (i == height - 1 ? "" : "\n");

    return result;
  }

  public static string ToAscii(AsciiFontType font, string input)
  {
    string result = "";
    string[] lines = ToAsciiLines(AsciiFonts.FontLookup[font], input);
    foreach(string line in lines) { result += line + "\n"; }
    return result;
  }

  public static string[] ToAsciiLines(AsciiFontType font, string input) => ToAsciiLines(AsciiFonts.FontLookup[font], input);

  public static string[] ToAsciiLines(Dictionary<char, string[]> font, string input)
  {
    if(string.IsNullOrEmpty(input))
    {
      Debug.LogError("Input is null or empty, returning empty string array.");
      return Array.Empty<string>();
    }

    string[] space = GetCharacter(font, ' ');
    int height = space.Length;

    string[] lines = input.Split('\n');
    string[] result = new string[lines.Length * height];

    for(int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
    {
      char[] chars = lines[lineIndex].ToCharArray();

      for(int i = 0; i < height; i++)
        result[lineIndex * height + i] = string.Empty;

      foreach(char ch in chars)
      {
        string[] ascii = GetCharacter(font, ch);
        if(ascii != null && ascii.Length > 0)
          for(int i = 0; i < ascii.Length; i++)
            result[lineIndex * height + i] += ascii[i];
        else
          for(int i = 0; i < height; i++)
            result[lineIndex * height + i] += " ";
      }
    }

    return result;
  }
}
}
