# AI Workflow

Always read `spec.md` before implementing anything.

This workflow exists to prevent over-generation and keep work aligned with small commits.

## Main Principle

Cursor must behave like a careful pair programmer working issue-by-issue, not like a one-shot code generator.

## Required Process For Every Task

### Step 1: Scope the task

Before generating code, state:

- the exact issue or feature slice being worked on
- what part of `spec.md` it satisfies
- which files need to change
- the smallest safe implementation chunk

### Step 2: Keep the output small

Generate only one chunk at a time.
A chunk should usually be one of:

- folder or scaffolding setup
- one entity
- one enum
- one service skeleton
- one menu method
- one use-case flow
- one persistence method
- one validation improvement

Do not generate:

- full features end-to-end
- giant multi-file rewrites
- code for future issues
- bonus enhancements unless explicitly requested

### Step 3: Respect commit boundaries

Each generated chunk should ideally map to one meaningful commit.

After generating a chunk, also provide:

- a short explanation of what changed
- a suggested commit message

### Step 4: Stop after the chunk

Do not continue automatically into the next chunk.
Wait for the next instruction.

## Preferred Output Format

For code tasks, respond in this order:

1. Scope summary
2. Files to change
3. Small implementation chunk
4. Suggested commit message

## Review-Oriented Behavior

Prefer to:

- explain structure before implementation
- keep methods and classes small
- make code easy to review
- minimize unrelated changes

## Debugging Behavior

When fixing bugs:

- identify the likely cause first
- explain the smallest fix
- avoid rewriting unrelated logic
- preserve current architecture

## Refactoring Behavior

Only refactor when:

- the user asks for it
- the current code is blocking progress
- the refactor is small and local

## Feature Completion Behavior

If asked to implement a feature, break it into phases first.
Example:

- domain model
- service logic
- menu wiring
- validation and exception handling polish

Then generate only the first phase unless explicitly asked for more.
