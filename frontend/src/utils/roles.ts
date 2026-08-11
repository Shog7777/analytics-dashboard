import type { Role } from "../types";

const RANK: Record<Role, number> = { Viewer: 0, Editor: 1, Admin: 2 };

/** True when `role` grants at least the privileges of `minimum` (Viewer < Editor < Admin). */
export function hasAtLeastRole(role: Role | undefined, minimum: Role): boolean {
  if (!role) return false;
  return RANK[role] >= RANK[minimum];
}
