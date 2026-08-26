---
name: expand-build/engine
description: "Use when the /devexpress command first fires and the loader jiti-loads the Reactive.XAF engine (profile + build) from C:/Work/Reactive.XAF."
---

# engine.ts — lazy engine attach

`attachEngine(pi)` runs once, on the first `/devexpress` invocation
(index.ts imports it only inside the command handler, never at boot).

## What it does

1. **Captures the engine's handler.** Wraps `pi.registerCommand` so the
   "devexpress" definition the RX engine registers is captured, then
   restores the original `registerCommand` right after.
2. **jiti-loads the RX engine.** Resolves jiti via `createRequire` on
   `pi-coding-agent`'s package.json located next to `node.exe`, then
   loads `profile.ts` and `build.ts` from
   `C:/Work/Reactive.XAF/.pi/extensions/reactive-xaf-build` with
   `moduleCache: false`, so sources re-load fresh every boot.
3. **Registers with the profile.** Calls
   `registerBuildCommand(pi, { profile: expandProfile })` — the
   profile comes from the RX repo, the same one the SKILL.md flow
   describes.
4. **Returns the captured handler.** index.ts's own command handler
   delegates every subsequent `/devexpress` call to it.

## Failure modes (all throw)

- `pi-coding-agent` package.json not found next to `node.exe` → the
  jiti require fails loudly.
- `profile.ts` or `build.ts` missing in the RX extension dir → error
  names the missing file.
- The engine registers no "devexpress" handler → "registerBuildCommand
  did not register".

The loader never silently falls back; a broken RX checkout surfaces as
a command error on first use, not a boot failure.
