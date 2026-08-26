/**
 * expand-build — project-local /devexpress loader for eXpand.
 * Boot only registers the command. The RX engine loads on first use.
 */

import type { attachEngine } from "./engine.js";

export default function (pi: any): void {
  let handler: ((args: unknown, ctx: unknown) => Promise<unknown>) | undefined;
  pi.registerCommand("devexpress", {
    description:
      "DevExpress menu: Build → RX-XAF | eXpand → Lab | Release; args: status | cancel | watch | build lab|release | publish lab|release",
    handler: async (args: unknown, ctx: unknown) => {
      if (!handler) {
        const spec = "./engine.js";
        const mod = await import(spec) as { attachEngine: typeof attachEngine };
        handler = mod.attachEngine(pi);
      }
      return handler(args, ctx);
    },
  });
}
