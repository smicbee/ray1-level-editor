# Third-party notices

This repository vendors the following libraries under `ext/`. They are copyright of their respective authors and licensed under the MIT License (full text in each directory's `LICENSE` file).

| Directory | Project | Copyright |
|---|---|---|
| `ext/BinarySerializer` | [RayCarrot/BinarySerializer](https://github.com/RayCarrot/BinarySerializer) | (c) 2021 RayCarrot |
| `ext/BinarySerializer.Ray1` | [RayCarrot/BinarySerializer.Ray1](https://github.com/RayCarrot/BinarySerializer.Ray1) | (c) 2021 RayCarrot |
| `ext/BinarySerializer.PlayStation` | [RayCarrot/BinarySerializer.PlayStation](https://github.com/RayCarrot/BinarySerializer.PlayStation) | (c) 2024 RayCarrot |

## Local modification

`ext/BinarySerializer.Ray1/src/BinarySerializer.Ray1/DataTypes/Map/Block.cs` carries one small local patch: padding bytes inside blocks are preserved instead of being zeroed, so that reading and re-writing a `.LEV` file is byte-identical. All other vendored files are unmodified upstream code.

## Knowledge sources (no code copied)

- [RayCarrot/Ray1Map](https://github.com/RayCarrot/Ray1Map) and [RayCarrot/Ray1Editor](https://github.com/RayCarrot/Ray1Editor) - format knowledge
- [fuerchter/rayman-ps1-decomp](https://github.com/fuerchter/rayman-ps1-decomp) - engine behavior reference (PS1 decompilation)
