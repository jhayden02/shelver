# Contributing

## Rules

- Never commit directly to `main`. Every change goes through a short-lived branch and a pull
  request.
- One branch per change.
- Commits stay small and signed. Merges keep every commit as written (no squash merges).
- Unsettled decisions are GitHub issues.

## Order of operations

### 1. Open an issue

For a question or planned change, open an issue labeled `question` or `enhancement` and one
`area:` label. Link the doc section it concerns.

### 2. Branch from an up-to-date `main`

```bash
git switch main
git pull
git switch -c docs/example
```

Name the branch `<type>/<change>`, where the type is `docs`, `feature`, or `fix`. Name it
after the change, not a file. An issue number may be included, as in `docs/1-example`.

### 3. Commit

Each commit is one change. Messages are full sentences in the past tense, ending with a
period, such as `Added the shelf view layout.`

When a commit answers an issue, add `Closes #<number>.` to its message. Merges are done
locally, so the commit message is what closes the issue.

### 4. Push and open a pull request

```bash
git push -u origin docs/example
gh pr create --fill
```

In the description, `Closes #<number>` closes an issue on merge. `Refs #<number>` only links
it.

Changes to Markdown files only do not run CI.

### 5. Merge locally

```bash
git switch main
git pull
git merge --ff-only docs/example
git push
```

GitHub marks the pull request as merged when its commits reach `main`. Do not use GitHub's
merge buttons: they rewrite commits, which removes their signatures.

If `--ff-only` refuses because `main` has moved, rebase the branch and try again:

```bash
git switch docs/example
git rebase main
git push --force-with-lease
```

### 6. Delete the branch

```bash
git branch -d docs/example
git push origin --delete docs/example
```
