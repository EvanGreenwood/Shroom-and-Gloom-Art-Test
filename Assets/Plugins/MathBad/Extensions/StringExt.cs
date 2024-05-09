#region
using System.Collections.Generic;
using System.Text.RegularExpressions;
#endregion
namespace MathBad
{
public static class CharExt
{
  public static bool IsUpper(this char c) => char.IsUpper(c);
  public static bool IsLower(this char c) => char.IsLower(c);
  public static bool IsLetter(this char c) => char.IsLetter(c);
  public static bool IsNumber(this char c) => char.IsNumber(c);
}

public static class StringExt
{
  public static bool IsNull(this string str) => str == null;
  public static bool IsEmpty(this string str) => str == string.Empty;
  public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
  public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);

  // Util
  //----------------------------------------------------------------------------------------------------
  public static string RemoveWhiteSpace(this string str)
  {
    if(str.IsNullOrEmpty())
      return str;
    return Regex.Replace(str, @"\s+", "");
  }

  public static string FirstToLower(this string str)
  {
    if(str.IsNullOrEmpty())
      return str;
    return char.ToLower(str[0]) + str[1..];
  }

  public static string FirstToUpper(this string str)
  {
    if(str.IsNullOrEmpty())
      return str;
    return char.ToUpper(str[0]) + str[1..];
  }

  public static string AddSpaceBeforeUpper(this string str)
  {
    if(str.IsNullOrEmpty())
      return str;

    string result = string.Empty;
    result += str[0];

    for(int i = 1; i < str.Length; i++)
    {
      if(char.IsUpper(str[i]) && str[i - 1] != ' ' && !char.IsUpper(str[i - 1]))
        result += ' ';
      result += str[i];
    }

    return result;
  }

  public static string RemoveFromStart(this string str, string remove)
  {
    if(str.StartsWith(remove))
      return str[remove.Length..];
    return str;
  }

  public static string RemoveFromEnd(this string str, string remove)
  {
    if(str.EndsWith(remove)) return str[..^remove.Length];
    return str;
  }

  // Path
  //----------------------------------------------------------------------------------------------------
  public static string CleanPath(this string path) => path.Replace('\\', '/').Trim('/');

  // Script Generation
  //----------------------------------------------------------------------------------------------------
  public static readonly HashSet<string> CSharpKeyWords =
    new HashSet<string>
    {
      "abstract", "as", "base", "bool", "break", "byte", "case",
      "catch", "char", "checked",
      "class", "const", "continue", "decimal", "default", "delegate",
      "do", "double",
      "else",
      "enum", "event", "explicit", "extern", "false", "finally",
      "fixed", "float", "for",
      "foreach", "goto", "if", "implicit", "in", "int", "interface",
      "internal", "is",
      "lock",
      "long", "namespace", "new", "null", "object", "operator", "out",
      "override", "params",
      "private", "protected", "public", "readonly", "ref", "return",
      "sbyte", "sealed",
      "short", "sizeof", "stackalloc", "static", "string", "struct",
      "switch", "this",
      "throw",
      "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe",
      "ushort", "using",
      "virtual", "void", "volatile", "while"
    };

  public static string MakeCSharpSafe(this string str)
  {
    string result = str;
    if(CSharpKeyWords.Contains(result))
      result = char.ToUpper(result[0]) + result[1..];
    return result;
  }

  public static string MakeFieldCompatible(this string str)
  {
    if(str.Length <= 0)
    {
      return "FIELD_ERROR";
    }

    string result = "";

    // First char must be a letter or an underscore
    if(char.IsLetter(str[0]) || str[0] == '_')
    {
      result += str[0];
    }

    // Strip out anything that's not a digit or underscore
    for(int i = 1; i < str.Length; ++i)
    {
      if(char.IsLetterOrDigit(str[i]) || str[i] == '_')
        result += str[i];
      else if(str[i] == '.')
      {
        result += char.ToUpper(str[i + 1]);
        i++;
      }
    }

    if(result.Length <= 0)
    {
      return "invalidFieldName";
    }

    result = result.MakeCSharpSafe();

    return result;
  }

  public static string ToMemberName(this string input) => $"_{input.MakeEnumCompatible().FirstToLower()}";
  public static string ToPropertyName(this string input) => input.MakeEnumCompatible().FirstToLower();
  public static string MakeEnumCompatible(this string str)
  {
    if(str.Length <= 0)
      return "InvalidEnumName";

    string result = "";
    str = str.RemoveWhiteSpace();
    if(char.IsLetter(str[0]))
      result += str[0];

    for(int i = 1; i < str.Length; ++i)
      if(char.IsLetterOrDigit(str[i]))
        result += str[i];

    if(result.Length <= 0)
      return "InvalidEnumName";

    result = result.MakeCSharpSafe();

    return result.FirstToUpper();
  }
}

//  ___  _      _     _____            _    ___       _                   _                           
// | _ \(_) __ | |_  |_   _| ___ __ __| |_ | __|__ __| |_  ___  _ _   ___(_) ___  _ _   ___           
// |   /| |/ _|| ' \   | |  / -_)\ \ /|  _|| _| \ \ /|  _|/ -_)| ' \ (_-<| |/ _ \| ' \ (_-<           
// |_|_\|_|\__||_||_|  |_|  \___|/_\_\ \__||___|/_\_\ \__|\___||_||_|/__/|_|\___/|_||_|/__/           
//                                                                                                    
//----------------------------------------------------------------------------------------------------

public static class RichTextExtensions
{
  public const string BOLD_START = "<b>";
  public const string BOLD_END = "</b>";

  public const string ITALIC_START = "<i>";
  public const string ITALIC_END = "</i>";

  public static string Bold(this string str) => "<b>" + str + "</b>";
  public static string Italic(this string str) => "<i>" + str + "</i>";
  public static string Size(this string str, int size) => string.Format("<size={0}>{1}</size>", size, str);
  public static string Color(this string str, string clr) => string.Format("<color={0}>{1}</color>", clr, str);
  public static string RandomFormat(this string str)
  {
    string result = str;
    int s = RNG.Int(0, 4);
    if(s == 0) result = str.Bold();
    else if(s == 1) result = str.Italic();
    else if(s == 2) result = str.Bold().Italic();
    return result;
  }

  public static string Condition(this string str)
  {
    if(str.ToLower().Contains("true")) return str.Lime();
    else if(str.ToLower().Contains("false")) return str.Red();
    return str.Grey();
  }

  public static string Cyan(this string str) => str.Color("#00ffffff");
  public static string Black(this string str) => str.Color("#000000ff");
  public static string Blue(this string str) => str.Color("#0000ffff");
  public static string Brown(this string str) => str.Color("#a52a2aff");
  public static string DarkBlue(this string str) => str.Color("#0000a0ff");
  public static string Green(this string str) => str.Color("#00ff00ff");
  public static string Grey(this string str) => str.Color("#808080ff");
  public static string LightBlue(this string str) => str.Color("#add8e6ff");
  public static string Lime(this string str) => str.Color("#008000ff");
  public static string Magenta(this string str) => str.Color("#ff00ffff");
  public static string Maroon(this string str) => str.Color("#800000ff");
  public static string Navy(this string str) => str.Color("#000080ff");
  public static string Olive(this string str) => str.Color("#808000ff");
  public static string Orange(this string str) => str.Color("#ffa500ff");
  public static string Purple(this string str) => str.Color("#800080ff");
  public static string Red(this string str) => str.Color("#ff0000ff");
  public static string Silver(this string str) => str.Color("#c0c0c0ff");
  public static string Teal(this string str) => str.Color("#008080ff");
  public static string White(this string str) => str.Color("#ffffffff");
  public static string Yellow(this string str) => str.Color("#ffff00ff");
}
}
