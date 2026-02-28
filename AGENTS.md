# AGENTS.md

## Purpose
This project uses one shared behavior contract for AI coding assistants.
Use this file as the source of truth for how to work in this repository.

## Language
- Default communication language: Dutch.
- Keep answers concise and practical.
- Explain choices when they affect risk, performance, or architecture.

## Working Style
- Make reasonable assumptions and continue unless a decision is risky.
- Prefer doing the work over only proposing steps.
- Before major edits, briefly state what you are about to change.
- After edits, summarize what changed and why.

## Code Quality
- Favor small, focused changes.
- Preserve existing architecture and naming conventions unless asked to refactor.
- Add comments only where intent is not obvious.
- Avoid unrelated cleanup in the same change.

## Validation
- Run the smallest relevant checks first (targeted tests/build/lint).
- If checks cannot run, state that clearly and why.
- Report failures with likely cause and next fix.

## Git Hygiene
- Do not revert user changes you did not make.
- Do not use destructive git/file commands unless explicitly requested.
- Keep commit messages clear and scoped when asked to commit.

## Unity + MCP
- Unity work should prefer MCP tools when available.
- Set the Unity project root before Unity MCP operations.
- If Unity Editor is not running, say so explicitly and provide the next step.

## Response Format
- Start with outcome first.
- Then list key changes/files.
- End with optional next steps only if they are useful.

## Cross-Agent Consistency
- `CLAUDE.md` should mirror these rules.
- If one file changes, update the other in the same edit.
