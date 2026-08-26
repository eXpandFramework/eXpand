/**
 * expand-build/engine — jiti-loads the Reactive.XAF /devexpress engine.
 * Imported only from the command handler, never at boot.
 */

import { createRequire } from "node:module";
import * as fs from "node:fs";
import * as path from "node:path";

const RX = "C:/Work/Reactive.XAF/.pi/extensions/reactive-xaf-build";

function requireFromPi(): NodeRequire {
  const piPkg = path.join(
    path.dirname(process.execPath),
    "node_modules",
    "@earendil-works",
    "pi-coding-agent",
    "package.json",
  );
  if (!fs.existsSync(piPkg)) {
    throw new Error("expand-build: pi-coding-agent not next to node.exe at " + piPkg);
  }
  return createRequire(piPkg);
}

function loadEngine(): {
  expandProfile: unknown;
  registerBuildCommand: (pi: unknown, seams?: unknown) => void;
} {
  const fileOf = (name: string) => {
    const file = path.join(RX, name + ".ts");
    if (!fs.existsSync(file)) throw new Error("expand-build: missing " + file);
    return file;
  };
  const { createJiti } = requireFromPi()("jiti") as {
    createJiti: (id: string, opts?: object) => (id: string) => any;
  };
  const jiti = createJiti(__filename, { moduleCache: false });
  return {
    expandProfile: jiti(fileOf("profile")).expandProfile,
    registerBuildCommand: jiti(fileOf("build")).registerBuildCommand,
  };
}

export function attachEngine(pi: any): (args: unknown, ctx: unknown) => Promise<unknown> {
  let captured: { handler?: (args: unknown, ctx: unknown) => Promise<unknown> } | undefined;
  const orig = pi.registerCommand.bind(pi);
  pi.registerCommand = (name: string, def: unknown) => {
    if (name === "devexpress") {
      captured = def as { handler?: (args: unknown, ctx: unknown) => Promise<unknown> };
    }
    return orig(name, def);
  };
  const engine = loadEngine();
  engine.registerBuildCommand(pi, { profile: engine.expandProfile });
  pi.registerCommand = orig;
  if (!captured?.handler) throw new Error("expand-build: registerBuildCommand did not register");
  return captured.handler;
}
