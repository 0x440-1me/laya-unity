#!/usr/bin/env python3
import json
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
REQUIRED = [
    "package.json", "README.md", "CHANGELOG.md", "LICENSE.md",
    "Third Party Notices.md", "CONTRIBUTING.md", "SECURITY.md",
    "CODE_OF_CONDUCT.md", "Runtime/Laya.Unity.asmdef",
    "Tests/Editor/Laya.Unity.Editor.Tests.asmdef",
    ".ci/Laya.Unity.Compile/Laya.Unity.Compile.csproj",
    ".ci/Laya.Unity.DotNet.Tests/Laya.Unity.DotNet.Tests.csproj",
]
MODEL_SUFFIXES = {".onnx", ".safetensors", ".pt", ".pth", ".gguf"}
SECRET_PATTERNS = [
    re.compile(r"-----BEGIN (?:RSA |EC |OPENSSH )?PRIVATE KEY-----"),
    re.compile(r"gh[pousr]_[A-Za-z0-9]{20,}"),
]


def fail(message: str) -> None:
    print(f"ERROR: {message}", file=sys.stderr)
    raise SystemExit(1)


for relative in REQUIRED:
    if not (ROOT / relative).is_file():
        fail(f"missing required file: {relative}")

manifest = json.loads((ROOT / "package.json").read_text(encoding="utf-8"))
expected = {
    "name": "com.0x4401me.laya.unity",
    "version": "0.1.0",
    "displayName": "Laya Unity",
    "unity": "2022.3",
}
for key, value in expected.items():
    if manifest.get(key) != value:
        fail(f"package.json {key!r} must be {value!r}")

dependency = manifest.get("dependencies", {}).get("com.unity.nuget.newtonsoft-json")
if dependency != "3.2.2":
    fail("Newtonsoft Unity package must be pinned to verified version 3.2.2")

for json_file in ROOT.rglob("*.json"):
    if any(part in {"bin", "obj"} for part in json_file.parts):
        continue
    try:
        json.loads(json_file.read_text(encoding="utf-8-sig"))
    except Exception as exc:
        fail(f"invalid JSON in {json_file.relative_to(ROOT)}: {exc}")

for path in ROOT.rglob("*"):
    if not path.is_file() or ".git" in path.parts or any(part in {"bin", "obj"} for part in path.parts):
        continue
    relative = path.relative_to(ROOT)
    if path.suffix.lower() in MODEL_SUFFIXES or path.name.lower() == "pytorch_model.bin":
        fail(f"model artifact must not be committed: {relative}")
    if path.stat().st_size > 5 * 1024 * 1024:
        fail(f"unexpected file over 5 MiB: {relative}")
    if path.suffix.lower() in {".md", ".json", ".cs", ".yml", ".yaml", ".py"}:
        text = path.read_text(encoding="utf-8-sig", errors="replace")
        for pattern in SECRET_PATTERNS:
            if pattern.search(text):
                fail(f"possible secret in {relative}")

print("Package validation passed.")
