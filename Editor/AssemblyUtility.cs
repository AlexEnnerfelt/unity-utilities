using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.IO;
using System.Linq;

[InitializeOnLoad]
public static class AssemblyUtility
{
    public static Assembly[] assemblies { get; private set; }
    public static Dictionary<string, Type> typesTable { get; private set; }

    static AssemblyUtility() {
        try {
            // This is the path to the Assets folder in your Unity project
            string assetsFolderPath = "Assets";
            string exclude = "Plugins";

            string[] scriptFiles = GetAllScriptFilesInDirectory(assetsFolderPath, exclude);
            assemblies = GetAssembliesFromScripts(scriptFiles);
            typesTable = GetTypetableFromAssemblies(assemblies);
        }
        catch (Exception) {

        }
    }

    public static string[] GetAllScriptFilesInDirectory(string directoryPath, string excludeDirectory = null) {
        // Use the Directory.GetFiles method to search the Assets folder for files with the .cs extension
        var directories = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories).Where(filePath => !filePath.StartsWith(Path.Combine(directoryPath, excludeDirectory)));
        return directories.ToArray();
    }
    public static Assembly[] GetAssembliesFromScripts(string[] scripts) {
        List<Assembly> assemblies = new List<Assembly>();

        // Print the names of all the script files that were found
        foreach (string scriptFile in scripts) {
            var obj = AssetDatabase.LoadAssetAtPath(scriptFile, typeof(MonoScript));
            MonoScript script = (MonoScript)obj;
            Assembly asm = Assembly.GetAssembly(script.GetClass());
            if (!assemblies.Contains(asm)) {
                assemblies.Add(asm);
            }
        }
        return assemblies.ToArray();
    }
    public static Dictionary<string, Type> GetTypetableFromAssemblies(Assembly[] assemblies) {
        var typesTable = new Dictionary<string, Type>();
        foreach (Assembly asm in assemblies) {
            foreach (var type in asm.GetTypes()) {
                if (type.IsClass) {
                    typesTable.Add(type.FullName, type);
                }
            }
        }
        return typesTable;
    }
}