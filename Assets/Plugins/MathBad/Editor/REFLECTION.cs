#region
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
#endregion

namespace MathBad
{
public static class REFLECTION
{
  public const BindingFlags MAX_BINDING_FLAGS = (BindingFlags)62;

  public static object GetFieldValue(this object o, string fieldName)
  {
    Type startType = o as Type ?? o.GetType();
    Type curType = startType;

    while(curType != null)
    {
      if(curType.GetField(fieldName, MAX_BINDING_FLAGS) is FieldInfo fieldInfo)
        return fieldInfo.GetValue(o is Type ? null : o);
      else
        curType = curType.BaseType;
    }

    throw new Exception("Field '" + fieldName + "' not found in '" + startType.Name + "' type and its parent types");
  }

  public static object GetPropertyValue(this object o, string propertyName, int typeHeight = 0)
  {
    Type startType = o is Type t ? t : o.GetType();
    Type curType = startType;

    while(curType != null)
    {
      if(curType.GetProperty(propertyName, MAX_BINDING_FLAGS) is PropertyInfo propertyInfo)
        return propertyInfo.GetValue(o is Type ? null : o);
      else curType = curType.BaseType;
    }

    throw new Exception("Property '" + propertyName + "' not found in '" + startType.Name + "' type and its parent types");
  }

  public static void SetFieldValue(this object o, string fieldName, object value, int typeHeight = 0)
  {
    Type startType = o as Type ?? o.GetType();
    Type curType = startType;

    while(curType != null)
      if(curType.GetField(fieldName, MAX_BINDING_FLAGS) is FieldInfo fieldInfo)
      {
        fieldInfo.SetValue(o is Type ? null : o, value);
        return;
      }
      else
        curType = curType.BaseType;

    throw new Exception("Field '" + fieldName + "' not found in '" + startType.Name + "' type and its parent types");
  }

  public static void SetPropertyValue(this object o, string propertyName, object value, int typeHeight = 0)
  {
    Type startType = o as Type ?? o.GetType();
    Type curType = startType;

    while(curType != null)
      if(curType.GetProperty(propertyName, MAX_BINDING_FLAGS) is PropertyInfo propertyInfo)
      {
        propertyInfo.SetValue(o is Type ? null : o, value);
        return;
      }
      else
        curType = curType.BaseType;

    throw new Exception("Property '" + propertyName + "' not found in '" + startType.Name + "' type and its parent types");
  }

  public static object InvokeMethod(this object o, string methodName, params object[] parameters)
  {
    Type type = o is Type t ? t : o.GetType();

    return type.InvokeMember(methodName, MAX_BINDING_FLAGS | BindingFlags.InvokeMethod, null, o, parameters);
  }

  public static T GetFieldValue<T>(this object o, string fieldName) => (T)o.GetFieldValue(fieldName);
  public static T GetPropertyValue<T>(this object o, string propertyName, int typeHeight = 0) => (T)o.GetPropertyValue(propertyName, typeHeight);
  public static T InvokeMethod<T>(this object o, string methodName, params object[] parameters) => (T)o.InvokeMethod(methodName, parameters);

  public static List<Type> GetSubclasses(this Type t) => t.Assembly.GetTypes().Where(type => type.IsSubclassOf(t)).ToList();
  public static object GetDefaultValue(this FieldInfo f, params object[] constructorVars) => f.GetValue(Activator.CreateInstance(((MemberInfo)f).ReflectedType, constructorVars));
  public static object GetDefaultValue(this FieldInfo f) => f.GetValue(Activator.CreateInstance(((MemberInfo)f).ReflectedType));

  public static IEnumerable<FieldInfo> GetFieldsWithoutBase(this Type t) => t.GetFields().Where(r => !t.BaseType.GetFields().Any(rr => rr.Name == r.Name));
  public static IEnumerable<PropertyInfo> GetPropertiesWithoutBase(this Type t) => t.GetProperties().Where(r => !t.BaseType.GetProperties().Any(rr => rr.Name == r.Name));

  public static List<MethodInfo> GetAllMethodsWithAttribute<TAttribute>(string[] includeAssemblyNames) where TAttribute : Attribute
  {
    List<MethodInfo> results = new List<MethodInfo>();
    foreach(Assembly assembly in DomainUtil.assemblies)
    foreach(Type type in assembly.GetTypes())
    foreach(MethodInfo methodInfo in type.GetMethods())
    {
      if(Attribute.GetCustomAttribute(methodInfo, typeof(TAttribute)) is TAttribute)
        results.Add(methodInfo);
    }

    return results;
  }

  public static List<Type> GetDerivedWithAttribute<T, TAttr>() where T : ScriptableObject
                                                               where TAttr : Attribute
  {
    Type[] types = GetAllDerivedTypes(typeof(T));
    List<Type> results = new List<Type>();

    for(int i = 0; i < types.Length; i++)
    {
      Type type = types[i];
      if(Attribute.GetCustomAttributes(type, typeof(TAttr)).Length > 0)
        results.Add(type);
    }

    return results;
  }

  public static void InvokeMethod(MethodInfo methodInfo)
  {
    if(methodInfo.IsStatic)
    {
      methodInfo.Invoke(null, null);
    }
    else
    {
      if(methodInfo.ReflectedType != null)
      {
        object classInstance = Activator.CreateInstance(methodInfo.ReflectedType);
        methodInfo.Invoke(classInstance, null);
      }
    }
  }

  public static List<MethodInfo> GetMethodsWithAttribute<T>(Type type) where T : Attribute
  {
    List<MethodInfo> results = new List<MethodInfo>();
    MethodInfo[] methods = type.GetMethods(MAX_BINDING_FLAGS);
    methods.Foreach(methodInfo =>
    {
      if(Attribute.GetCustomAttribute(methodInfo, typeof(T)) is T)
        results.Add(methodInfo);
    });

    return results;
  }

  public static List<MethodInfo> GetMethodsWithAttribute<T>(BindingFlags bindingFlags, params Type[] types) where T : Attribute
  {
    List<MethodInfo> results = new List<MethodInfo>();

    types.Foreach(type =>
    {
      MethodInfo[] methods = type.GetMethods(bindingFlags);
      methods.Foreach(methodInfo =>
      {
        if(Attribute.GetCustomAttribute(methodInfo, typeof(T)) != null)
          results.Add(methodInfo);
      });
    });

    return results;
  }

  public static Type[] GetAllDerivedTypes<T>() => GetAllDerivedTypes(typeof(T));
  public static Type[] GetAllDerivedTypes(Type type)
  {
    List<Type> results = new List<Type>();

    foreach(Assembly assembly in DomainUtil.assemblies)
    {
      Type[] types = assembly.GetTypes();
      foreach(Type t in types)
      {
        if(t.IsSubclassOf(type))
          results.Add(t);
      }
    }

    return results.ToArray();
  }

  public static Type[] GetAllDerivedTypes(params Type[] types)
  {
    if(types.IsNullOrEmpty())
      return null;

    List<Type> results = new List<Type>();
    foreach(Type type in types)
    {
      results.AddRange(GetAllDerivedTypes(type));
    }
    return results.ToArray();
  }
  public static Type[] GetTypesWithInterface<T>() => GetTypesWithInterface(typeof(T));
  public static Type[] GetTypesWithInterface(Type type)
  {
    List<Type> results = new List<Type>();
    foreach(Assembly assembly in DomainUtil.assemblies)
    {
      Type[] types = assembly.GetTypes();
      foreach(Type t in types)
      {
        if(type.IsAssignableFrom(t))
          results.Add(t);
      }
    }
    results.Remove(type);
    return results.ToArray();
  }
}

public static class DomainUtil
{
  static DomainUtil()
  {
    _assemblies = GetUserCreatedAssemblies(AppDomain.CurrentDomain);
  }
  static IEnumerable<Assembly> _assemblies;
  public static IEnumerable<Assembly> assemblies => _assemblies;
  public static string[] rootAssemblyNames => _rootAssemblyNames;
  public static HashSet<string> includedAssemblyNames => _includedAssemblyNames;
  public static HashSet<string> internalAssemblyNames => _internalAssemblyNames;

  static readonly string[] _rootAssemblyNames =
  {
    "Assembly-CSharp",
    "Assembly-CSharp-firstpass",
  };

  public static IEnumerable<Assembly> GetUserCreatedAssemblies(this AppDomain appDomain)
  {
    foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
    {
      if(assembly.IsDynamic)
      {
        continue;
      }

      string assemblyName = assembly.GetName().Name;
      if(_internalAssemblyNames.Contains(assemblyName))
      {
        continue;
      }

      yield return assembly;
    }
  }

  static readonly HashSet<string> _includedAssemblyNames
    = new HashSet<string>()
      {
        "Framework",
        "Framework_Editor"
      };

  static readonly HashSet<string> _internalAssemblyNames
    = new HashSet<string>()
      {
        "mscorlib",
        "System",
        "System.Core",
        "System.Security.Cryptography.Algorithms",
        "System.Net.Http",
        "System.Data",
        "System.Runtime.Serialization",
        "System.Xml.Linq",
        "System.Numerics",
        "System.Xml",
        "System.Configuration",
        "ExCSS.Unity",
        "Unity.Cecil",
        "Unity.CompilationPipeline.Common",
        "Unity.SerializationLogic",
        "Unity.TestTools.CodeCoverage.Editor",
        "Unity.ScriptableBuildPipeline.Editor",
        "Unity.Addressables.Editor",
        "Unity.ScriptableBuildPipeline",
        "Unity.CollabProxy.Editor",
        "Unity.Timeline.Editor",
        "Unity.PerformanceTesting.Tests.Runtime",
        "Unity.Settings.Editor",
        "Unity.PerformanceTesting",
        "Unity.PerformanceTesting.Editor",
        "Unity.Rider.Editor",
        "Unity.ResourceManager",
        "Unity.TestTools.CodeCoverage.Editor.OpenCover.Mono.Reflection",
        "Unity.PerformanceTesting.Tests.Editor",
        "Unity.TextMeshPro",
        "Unity.Timeline",
        "Unity.Addressables",
        "Unity.TestTools.CodeCoverage.Editor.OpenCover.Model",
        "Unity.VisualStudio.Editor",
        "Unity.TextMeshPro.Editor",
        "Unity.VSCode.Editor",
        "UnityEditor",
        "UnityEditor.UI",
        "UnityEditor.TestRunner",
        "UnityEditor.CacheServer",
        "UnityEditor.WindowsStandalone.Extensions",
        "UnityEditor.Graphs",
        "UnityEditor.UnityConnectModule",
        "UnityEditor.UIServiceModule",
        "UnityEditor.UIElementsSamplesModule",
        "UnityEditor.UIElementsModule",
        "UnityEditor.SceneTemplateModule",
        "UnityEditor.PackageManagerUIModule",
        "UnityEditor.GraphViewModule",
        "UnityEditor.CoreModule",
        "UnityEngine",
        "UnityEngine.UI",
        "UnityEngine.XRModule",
        "UnityEngine.WindModule",
        "UnityEngine.VirtualTexturingModule",
        "UnityEngine.TestRunner",
        "UnityEngine.VideoModule",
        "UnityEngine.VehiclesModule",
        "UnityEngine.VRModule",
        "UnityEngine.VFXModule",
        "UnityEngine.UnityWebRequestWWWModule",
        "UnityEngine.UnityWebRequestTextureModule",
        "UnityEngine.UnityWebRequestAudioModule",
        "UnityEngine.UnityWebRequestAssetBundleModule",
        "UnityEngine.UnityWebRequestModule",
        "UnityEngine.UnityTestProtocolModule",
        "UnityEngine.UnityCurlModule",
        "UnityEngine.UnityConnectModule",
        "UnityEngine.UnityAnalyticsModule",
        "UnityEngine.UmbraModule",
        "UnityEngine.UNETModule",
        "UnityEngine.UIElementsNativeModule",
        "UnityEngine.UIElementsModule",
        "UnityEngine.UIModule",
        "UnityEngine.TilemapModule",
        "UnityEngine.TextRenderingModule",
        "UnityEngine.TextCoreModule",
        "UnityEngine.TerrainPhysicsModule",
        "UnityEngine.TerrainModule",
        "UnityEngine.TLSModule",
        "UnityEngine.SubsystemsModule",
        "UnityEngine.SubstanceModule",
        "UnityEngine.StreamingModule",
        "UnityEngine.SpriteShapeModule",
        "UnityEngine.SpriteMaskModule",
        "UnityEngine.SharedInternalsModule",
        "UnityEngine.ScreenCaptureModule",
        "UnityEngine.RuntimeInitializeOnLoadManagerInitializerModule",
        "UnityEngine.ProfilerModule",
        "UnityEngine.Physics2DModule",
        "UnityEngine.PhysicsModule",
        "UnityEngine.PerformanceReportingModule",
        "UnityEngine.ParticleSystemModule",
        "UnityEngine.LocalizationModule",
        "UnityEngine.JSONSerializeModule",
        "UnityEngine.InputLegacyModule",
        "UnityEngine.InputModule",
        "UnityEngine.ImageConversionModule",
        "UnityEngine.IMGUIModule",
        "UnityEngine.HotReloadModule",
        "UnityEngine.GridModule",
        "UnityEngine.GameCenterModule",
        "UnityEngine.GIModule",
        "UnityEngine.DirectorModule",
        "UnityEngine.DSPGraphModule",
        "UnityEngine.CrashReportingModule",
        "UnityEngine.CoreModule",
        "UnityEngine.ClusterRendererModule",
        "UnityEngine.ClusterInputModule",
        "UnityEngine.ClothModule",
        "UnityEngine.AudioModule",
        "UnityEngine.AssetBundleModule",
        "UnityEngine.AnimationModule",
        "UnityEngine.AndroidJNIModule",
        "UnityEngine.AccessibilityModule",
        "UnityEngine.ARModule",
        "UnityEngine.AIModule",
        "SyntaxTree.VisualStudio.Unity.Bridge",
        "nunit.framework",
        "Newtonsoft.Json",
        "ReportGeneratorMerged",
        "Unrelated",
        "netstandard",
        "SyntaxTree.VisualStudio.Unity.Messaging"
      };
}
}
