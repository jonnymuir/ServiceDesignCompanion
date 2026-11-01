# Service Design Companion: Project Guide

## What this is

A free mobile app and website for service designers: the UK Service Standard in plain words, a
phase finder, and service design method cards. Built as an ordinary consumer of the published
`UmbracoPrism` and `Wayfinder.Umbraco` packages from nuget.org. Read [docs/plan.md](docs/plan.md)
before changing scope.

Never copy Prism or Wayfinder code into this repo. If something is missing from a package, fix it
in [`jonnymuir/Umbraco.Prism`](https://github.com/jonnymuir/Umbraco.Prism) or
[`jonnymuir/Wayfinder.Umbraco`](https://github.com/jonnymuir/Wayfinder.Umbraco) and take the new
version.

## Projects

| Project | Purpose |
|---|---|
| `src/ServiceDesignCompanion.Site` | The Umbraco 17 site the app is generated from and served by |

## Build

```bash
dotnet build ServiceDesignCompanion.slnx
dotnet run --project src/ServiceDesignCompanion.Site
```

Package versions are managed centrally in `Directory.Packages.props`. Restore is from nuget.org
only (`NuGet.config`).

## Rules that matter here

- **No user data on the server.** Do not add accounts, analytics, logging of answers or anything
  that stores personal data without an explicit decision recorded in `docs/plan.md`.
- **Content is seeded from code.** The database is disposable. Anything the site needs must be
  recreated from this repo on boot.
- **No official status.** Never use the crown, GOV.UK logotype, GDS Transport or GOV.UK brand
  blue, and never word anything as official, approved or predictive of an assessment outcome.
  Credit Open Government Licence content on every page that uses it.

## Conventions

- Every change reaches `main` through a pull request.
- [Conventional Commits](https://www.conventionalcommits.org/): `feat:`, `fix:`, `docs:`,
  `chore:` and so on, with a `BREAKING CHANGE:` body line when relevant.
- No em dashes anywhere, in prose, docs, code comments or commit messages. Use a comma or a full
  stop.
- Docs describe current state, not the history of how it was reached.
- No speculative abstractions and no comments unless the reason is genuinely non-obvious.
- Mobile work lands on iOS and Android together.
