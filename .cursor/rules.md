# Project Rules

Always read and use `spec.md` as the source of truth before making implementation decisions.

## Project Context

- This project is a C# console application.
- It implements an online shopping backend system.
- The architecture uses lightweight Domain-Driven Design.
- Submission 1 is the priority.
- Submission 2 enhancements must not interfere with Submission 1 completion.

## Core Rules

- Always refer back to `spec.md` before implementing any feature.
- Do not invent requirements that are not supported by `spec.md`.
- Keep the architecture simple, practical, and beginner-friendly.
- Use a single console project with folders, not multiple projects, unless explicitly requested.
- Prefer readability and maintainability over clever abstractions.
- Keep `Program.cs` thin.
- Keep console input/output out of domain entities.
- Use in-memory collections as the primary runtime data store.
- Support JSON persistence in Infrastructure.
- Do not overengineer repositories, factories, dependency injection, or patterns unless they clearly help.

## Output Constraints

- Never assign a variable with var.
- Do not generate a whole feature at once.
- Do not generate large multi-file rewrites unless explicitly requested.
- Do not change unrelated files.
- Do not restructure the project unless explicitly asked.
- Do not add optional enhancements unless explicitly requested.

## Incremental Development Rules

- Work only on the current issue or the exact scope requested.
- Before coding, briefly state:
  1. what will be changed
  2. which files will be touched
  3. the smallest safe implementation chunk
- Generate only one small implementation chunk at a time.
- Each chunk should be small enough to fit into one meaningful commit.
- Prefer updating existing files over generating entirely new structures.

## Code Rules

- Use clear class and method names.
- Keep methods short and focused.
- Validate user input at the point of feature implementation.
- Use LINQ where it naturally fits and where the specification expects it.
- Keep domain models focused on business meaning.
- Keep application services focused on use-case orchestration.
- Keep persistence logic inside Infrastructure.
- Avoid circular references that will complicate JSON serialization.

## Git-Friendly Rules

- Aim for commit-sized chunks.
- A single response should ideally support one commit.
- If a task is larger than one commit, split it into sub-steps before generating code.
- When relevant, suggest a commit message for the current chunk.

## If Unclear

- Default to the smallest implementation that satisfies `spec.md`.
- Do not guess beyond the spec unless clearly marked as a reasonable assumption.
