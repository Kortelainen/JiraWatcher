# Releasing a New Version

Publishing a release is fully automated via GitHub Actions. All you need to do is push a version tag.

## Steps

### 1. Finish and merge your changes
Make sure all changes intended for the release are committed and pushed to `main`.

### 2. Choose a version number
Follow [Semantic Versioning](https://semver.org/):
- `MAJOR.MINOR.PATCH` — e.g. `2.1.0`, `2.2.0`, `3.0.0`
- Bump **MINOR** for new features, **PATCH** for bug fixes, **MAJOR** for breaking changes.

### 3. Create and push a git tag

```powershell
git tag v2.1.0
git push origin v2.1.0
```

That's it. The GitHub Actions workflow triggers automatically.

### What happens automatically
- The workflow builds a self-contained single-file `win-x86` release exe.
- `FileVersion` in the exe is set to the tag version (e.g. `2.1.0.0`).
- The exe is zipped as `JiraWatcher-2.1.0-win-x64.zip`.
- A GitHub Release is created with the zip attached and auto-generated release notes.

You can monitor the build at: **GitHub → Actions tab**

## If something goes wrong

To delete a tag and re-run (e.g. you tagged too early):

```powershell
# Delete locally
git tag -d v2.1.0

# Delete on remote
git push origin --delete v2.1.0
```

Then fix the issue, commit, and re-tag.

## Notes
- The `FileVersion` field in `JiraWatcher.csproj` does **not** need to be updated manually — the workflow injects it from the tag.
- You can update `JiraWatcher.csproj` manually if you want the version visible during local debug builds, but it is not required for releases.
