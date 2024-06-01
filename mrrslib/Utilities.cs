using System;
using System.IO;

public class Utilities {
    public static string LoadTextFile(string path) {
        string content = File.ReadAllText(path);
        return content;
    }
}