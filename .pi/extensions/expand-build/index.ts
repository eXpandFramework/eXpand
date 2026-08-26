/**
 * expand-build — project-local /devexpress loader for eXpand.
 * Loads the Reactive.XAF engine with jiti so the .ts sources resolve.
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

export default function (pi: any): void {
  const { expandProfile, registerBuildCommand } = loadEngine();
  registerBuildCommand(pi, { profile: expandProfile });
}
