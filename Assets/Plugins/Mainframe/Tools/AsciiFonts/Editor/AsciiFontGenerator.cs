//    _            _  _  ___             _     ___                             _                      //
//   /_\   ___ __ (_)(_)| __| ___  _ _  | |_  / __| ___  _ _   ___  _ _  __ _ | |_  ___  _ _          //
//  / _ \ (_-</ _|| || || _| / _ \| ' \ |  _|| (_ |/ -_)| ' \ / -_)| '_|/ _` ||  _|/ _ \| '_|         //
// /_/ \_\/__/\__||_||_||_|  \___/|_||_| \__| \___|\___||_||_|\___||_|  \__,_| \__|\___/|_|           //
//                                                                                                    //
//----------------------------------------------------------------------------------------------------//

#region
using System.IO;
using System.Text;
using Mainframe;
using UnityEditor;
using UnityEngine;
#endregion

namespace Mainframe_Editor
{
public class AsciiFontGenerator
{
  const string ENUM_TYPE = "AsciiFontType";
  const string FILE_NAME = "GeneratedAsciiFonts.cs";

  [MenuItem("Mainframe/Generate/AsciiFonts")]
  public static void Generate()
  {
    string filePath = Path.Join("Assets/Plugins/Framework/Tools/AsciiFonts", FILE_NAME);

    // Generate
    using(StreamWriter writer = new StreamWriter(filePath, false, Encoding.Default))
    {
      writer.WriteLine("// This script is auto-generated by the Framework.");
      writer.WriteLine("// Changes will be lost on re-generation.");
      writer.WriteLine();
      writer.WriteLine("using Framework;");
      writer.WriteLine("using UnityEngine;");
      writer.WriteLine("using System.Collections.Generic;");
      writer.WriteLine();

      writer.Write(Header);

      writer.WriteLine();

      //--- Get Font Names ---//
      TextAsset[] fonts = Resources.LoadAll<TextAsset>("Framework/AsciiFonts/Fonts");

      string[] fontNames = new string[fonts.Length];
      for(int i = 0; i < fonts.Length; i++)
        fontNames[i] = fonts[i].name;

      //--- Keys ---//
      writer.WriteLine("// Characters:");
      writer.WriteLine("// 0 1 2 3 4 5 6 7 8 9");
      writer.WriteLine("// A B C D E F G H I J K L M N O P Q R S T U V W X Y Z");
      writer.WriteLine("// a b c d e f g h i j k l m n o p q r s t u v w x y z");
      writer.WriteLine("// ! \" # $ % & ' ( ) * + , - . / : ; < = > ? @ [ \\ ] ^ _ { | } ~");

      //--- Namespace ---//
      writer.WriteLine("namespace Framework");
      writer.WriteLine("{");
      //--- Enum (Font Names) ---//
      GenerateEnums(writer, "AsciiFontType", fontNames);
      writer.WriteLine();

      //--- static class ---//
      writer.WriteLine(Indents + $"public static class AsciiFonts");
      writer.WriteLine(Indents + "{");

      writer.WriteLine();

      GenerateLookup(writer, fontNames);
      writer.WriteLine();

      //--- Fonts ---//
      GenerateFonts(writer, fonts);
      writer.WriteLine();
      RemoveIndent();
      writer.WriteLine("}");
      writer.WriteLine();
      RemoveIndent();
      writer.WriteLine("}");
    }

    AssetDatabase.Refresh();

    Debug.Log("Generated Ascii-Fonts Complete.");
  }

  static void GenerateLookup(StreamWriter writer, string[] names)
  {
    writer.WriteLine(Indents + "static AsciiFonts()");
    writer.WriteLine(Indents + "{");

    AddIndent();
    writer.WriteLine(Indents + $"FontLookup = new Dictionary<{ENUM_TYPE}, Dictionary<char, string[]>>()");
    writer.WriteLine(Indents + "{");
    AddIndent();
    writer.WriteLine(Indents + $"{{ {ENUM_TYPE}.None, null }},");
    for(int i = 0; i < names.Length; i++)
    {
      string publicField = names[i].RemoveWhiteSpace();
      string enumType = names[i].MakeEnumCompatible();
      writer.WriteLine(Indents + $"{{ {ENUM_TYPE}.{enumType}, {publicField}  }}, \n");
    }
    RemoveIndent();
    writer.WriteLine(Indents + "};");
    RemoveIndent();

    writer.WriteLine(Indents + "}");
    writer.WriteLine(Indents + "//--- LookUp ---//");

    writer.WriteLine(Indents + $"public static readonly Dictionary<{ENUM_TYPE}, Dictionary<char, string[]>> FontLookup;");
  }

  public static void GenerateEnums(StreamWriter writer, string enumType, string[] enumEntries)
  {
    writer.WriteLine(Indents + "//--- " + enumType + " ---//");
    writer.WriteLine(Indents + "public enum " + enumType);
    writer.WriteLine(Indents + "{");
    writer.WriteLine(Indents + "\t" + "None = 0,");

    for(int i = 0; i < enumEntries.Length; i++)
    {
      string entry = enumEntries[i].MakeEnumCompatible();
      writer.WriteLine("\t" + entry + ",");
    }

    writer.WriteLine(Indents + "}");
  }

  static void GenerateFonts(StreamWriter writer, TextAsset[] fonts)
  {
    writer.WriteLine(Indents + "//--- Fonts ---//");

    for(int fileIndex = 0; fileIndex < fonts.Length; fileIndex++)
    {
      string name = fonts[fileIndex].name.RemoveWhiteSpace();
      writer.WriteLine(Indents + "//--- " + name.ToUpper() + " ---//");
      string raw = fonts[fileIndex].text;
      string[] asciiCharacters = raw.Split("#CHAR#");

      int height = int.Parse(asciiCharacters[0]);

      AddIndent();
      writer.WriteLine(Indents + $"public static readonly Dictionary<char, string[]> {name} =");
      AddIndent();
      writer.WriteLine(Indents + $"new Dictionary<char, string[]>()");
      writer.WriteLine(Indents + "{");

      // Add Characters
      for(int i = 1; i < asciiCharacters.Length; i++)
      {
        string[] linesOfCharacter = asciiCharacters[i].Split("\n");
        if(linesOfCharacter.IsNullOrEmpty())
          continue;

        char key = linesOfCharacter[0][0];
        string keyString = $"{key}";
        if(key.ToString() == "\\"
        || key.ToString() == "\""
        || key.ToString() == "\'")
          keyString = $"\\{keyString}";

        AddIndent();
        writer.WriteLine(Indents + "{");
        AddIndent();
        writer.WriteLine(Indents + $"'{keyString}',");
        writer.WriteLine(Indents + "new []");
        writer.WriteLine(Indents + "{");
        AddIndent();

        for(int li = 1; li < linesOfCharacter.Length; li++)
        {
          string line = linesOfCharacter[li].Trim();
          if(line == string.Empty)
            continue;
          string result = line.RemoveFromStart("\"").RemoveFromEnd("\"");
          writer.WriteLine(Indents + $"@\"{result}\",");
        }

        RemoveIndent();
        writer.WriteLine(Indents + "}");
        RemoveIndent();
        writer.WriteLine(Indents + "},");
        RemoveIndent();
      }
      writer.WriteLine(Indents + "};");
      RemoveIndent();
    }
    writer.WriteLine();
  }

  static string Indents => _indent.ToString();
  static StringBuilder _indent = new StringBuilder("");
  static string AddIndent(int amount = 1)
  {
    for(int i = 0; i < amount; i++)
      _indent.Append("    ");
    return _indent.ToString();
  }
  static string RemoveIndent(int amount = 1)
  {
    int tabsToRemove = Mathf.Min(amount * 4, _indent.Length);
    if(tabsToRemove > 0) _indent.Length -= tabsToRemove;
    return _indent.ToString();
  }

  public static string Header => @"//    _            _  _  ___             _        //" + "\n" +
                                 @"//   /_\   ___ __ (_)(_)| __| ___  _ _  | |_  ___ //" + "\n" +
                                 @"//  / _ \ (_-</ _|| || || _| / _ \| ' \ |  _|(_-< //" + "\n" +
                                 @"// /_/ \_\/__/\__||_||_||_|  \___/|_||_| \__|/__/ //" + "\n" +
                                 @"//                                                //" + "\n";
}
}
