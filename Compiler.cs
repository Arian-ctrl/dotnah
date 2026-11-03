using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text.Json;

namespace dotnah {
    public class Compiler {
        [StructLayout(LayoutKind.Sequential)]
        private struct WasmCompileResult {
            public byte Success;
            public IntPtr WasmPtr;
            public nuint WasmLen;
            public IntPtr ErrorsJson;
        }

        [DllImport("nosharp_parser.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern WasmCompileResult nosharp_compile_to_wasm(IntPtr source, IntPtr globals);

        [DllImport("nosharp_parser.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void nosharp_free_wasm(IntPtr ptr, nuint len);

        [DllImport("nosharp_parser.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void nosharp_free_string(IntPtr ptr);

        public byte[] Compile(string source, Dictionary<string, object> globals) {
            IntPtr sourcePtr = Marshal.StringToCoTaskMemUTF8(source);
            IntPtr globalsPtr = Marshal.StringToCoTaskMemUTF8(
                JsonSerializer.Serialize(globals)
            );

            try {
                var result = nosharp_compile_to_wasm(sourcePtr, globalsPtr);
                if (result.Success == 0) {
                    string errors = Marshal.PtrToStringUTF8(result.ErrorsJson) ?? "Unknown compiler error";

                    nosharp_free_string(result.ErrorsJson);
                    throw new InvalidOperationException(errors);
                }

                try {
                    byte[] wasm = new byte[checked((int)result.WasmLen)];
                    Marshal.Copy(result.WasmPtr, wasm, 0, wasm.Length);

                    return wasm;
                } finally {
                    nosharp_free_wasm(result.WasmPtr, result.WasmLen);
                }
            } finally {
                Marshal.FreeCoTaskMem(sourcePtr);
                Marshal.FreeCoTaskMem(globalsPtr);
            }
        }
    }
}