namespace dotnah {
    internal class CLI {
        static void Main(string[] args) {
            if (args.Length >= 2 && args[0] == "run") {
                string path = args[1];
                if (!File.Exists(path)) {
                    Console.WriteLine($"Source file '{path}' does not exist.");
                    return;
                }

                string wasmPath;
                if (args.Length >= 3) {
                    string customPath = args[2];
                    string? directory = Path.GetDirectoryName(Path.GetFullPath(customPath));

                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
                        Console.WriteLine($"Error: Target directory '{directory}' does not exist.");
                        return;
                    }

                    wasmPath = customPath;
                } else {
                    wasmPath = Path.Combine(
                        Path.GetDirectoryName(Path.GetFullPath(path))!,
                        "output_" + Path.GetFileNameWithoutExtension(path) + ".wasm"
                    );
                }

                string sourceCode = File.ReadAllText(path);
                Compiler compiler = new();

                var globals = new Dictionary<string, object> {
                    ["print_i32"] = new {
                        parameters = new[] { "int" },
                        returns = "void"
                    },

                    ["print_str"] = new {
                        parameters = new[] { "int", "int" },
                        returns = "void"
                    }
                };

                byte[] wasm = compiler.Compile(sourceCode, globals);

                File.WriteAllBytes(wasmPath, wasm);
                Console.WriteLine($"Compiled: {wasmPath}");

                Runtime runtime = new();
                runtime.Execute(wasmPath);
            }
        }
    }
}