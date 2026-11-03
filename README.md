# .NAH

**.NAH/dotnah** is the .NET runtime and command-line interface (CLI) for [No#](https://github.com/Arian-ctrl/nosharp), a programming language currently under development.

It handles compiling and executing No# programs using WebAssembly and [Wasmtime](https://wasmtime.dev/).

## How it works

dotnah works together with the No# compiler:

1. Reads a `.nos` source file.
2. Passes the source code to the No# compiler.
3. Receives the generated WebAssembly module.
4. Loads and executes the module through Wasmtime.

The .NET runtime also provides host functions that No# programs can import and use.

## Usage

Run a No# program from the command line:

```powershell
dotnah run path/to/file.nos
```

**Be sure to put `dotnah.exe` and `nosharp_parser` in the same directory**

## Technologies

- C#
- .NET
- Wasmtime
- WebAssembly

## Project status

No# and dotnah are still under development. Features and internal implementations may change as the language evolves.

## Related project

- [No# Programming Language](https://github.com/Arian-ctrl/nosharp)
