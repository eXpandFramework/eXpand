---
name: expand-build
description: Use when working on or invoking /devexpress in the eXpand repo. Thin loader that imports the Reactive.XAF engine with expandProfile — no pane, watcher, or azdo copy.
---

# expand-build

Project-local extension at `D:/expand/.pi/extensions/expand-build/`.
Pi auto-discovers `cwd/.pi/extensions`, so this only loads in the eXpand
tree. Skill lives at `D:/expand/.pi/Skills/expand-build/SKILL.md`.

`activate` loads the Reactive.XAF engine with jiti (the `.ts` sources)
and registers `/devexpress` with `{ profile: expandProfile }`. The
engine, pane, watcher, and azdo scripts stay in Reactive.XAF.

## Expand flow (via the shared engine)

DX pins → RX depPins (`Xpand.Extensions*` / `Xpand.XAF.*` from the matching
feed) → `bx lab` / `bx Release` in a pane → commit (required) → `git push`
to `lab` or `eXpand` → `px` / `px -Release` → watch 32/39 → 38 → 37.

- Lab GitHub `eXpand.lab`: assert published (do not PATCH).
- Release GitHub `eXpand`: publish the draft.
- Version file is written by the local `bx` build. We never edit
  `XpandAssemblyInfo.cs`. `build.ps1` is the version we bump.

See `reactive-xaf-build/profile.md` for the profile fields.
