using System;
using System.IO;
using System.Text.RegularExpressions;

public class Configs {
    public string DatabasePath { get; set; }

    public static Configs LoadConfigs(string path) {
        string[] lines = File.ReadAllLines(path);
        Configs configs = new Configs();

        foreach (var line in lines) {
            var tokens = Regex.Split(line, @"\s*=\s*");
            if (tokens[0] == "DatabasePath") {
                configs.DatabasePath = tokens[1];
            }
        }

        return configs;
    }
}