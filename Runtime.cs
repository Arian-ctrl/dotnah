using System.Runtime.InteropServices;
using Wasmtime;

namespace dotnah {
    public class Runtime {
        private readonly Engine engine;
        private readonly Store store;
        private readonly Linker linker;

        public Runtime() {
            engine = new();
            store = new(engine);
            linker = new(engine);

            linker.DefineFunction("std", "print_i32", (int value) => Console.WriteLine(value));
            linker.DefineFunction("std", "print_str", (Caller caller, int ptr, int len) => {
                var memory = caller.GetMemory("memory");
                var bytes = memory.GetSpan(ptr, len)[ptr..(ptr + len)];

                Console.WriteLine(System.Text.Encoding.UTF8.GetString(bytes));
            });
        }

        public Module LoadModule(string path) {
            return Module.FromFile(engine, path);
        }

        public Instance Instantiate(Module module) {
            return linker.Instantiate(store, module);
        }

        public void Execute(string path) {
            Console.WriteLine($"Executing: {path}");
            var module = LoadModule(path);
            var instance = Instantiate(module);
            var main = instance.GetAction("main");

            if (main != null) {
                main();
            } else {
                throw new Exception("No exported 'main' function found.");
            }
        }
    }
}