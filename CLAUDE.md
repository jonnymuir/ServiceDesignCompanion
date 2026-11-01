# Service Design Companion: Project Guide

## What this is

A free mobile app and website for service designers: the UK Service Standard in plain words, a
phase finder, and service design method cards. Built as an ordinary consumer of the published
`UmbracoPrism` and `Wayfinder.Umbraco` packages from nuget.org. Read [docs/plan.md](docs/plan.md)
before changing scope.

Engineering standards (security, simplicity, testing, change flow, docs, writing style) are in
[jonnymuir/engineering-standards](https://github.com/jonnymuir/engineering-standards/blob/main/STANDARDS.md).
Follow them; this file adds only what is specific to this repo.

Never copy Prism or Wayfinder code into this repo. If something is missing from a package, fix it
in [`jonnymuir/Umbraco.Prism`](https://github.com/jonnymuir/Umbraco.Prism) or
[`jonnymuir/Wayfinder.Umbraco`](https://github.com/jonnymuir/Wayfinder.Umbraco) and take the new
version.

## Projects

| Project | Purpose |
|---|---|
| `src/ServiceDesignCompanion.Site` | The Umbraco 17 site the app is generated from and served by |

## Build and test

```bash
dotnet build ServiceDesignCompanion.slnx
dotnet test ServiceDesignCompanion.slnx
node scripts/check-policies.mjs
node scripts/check-doc-links.mjs
```

Run the site on a fresh database (the database is disposable):

```bash
mkdir -p src/ServiceDesignCompanion.Site/umbraco/Data
dotnet run --project src/ServiceDesignCompanion.Site
```

Package versions are managed centrally in `Directory.Packages.props`. Restore is from nuget.org
only (`NuGet.config`).

## Releasing

No releases yet. Store builds and their versioning are set up when the mobile app pipeline is.

## Rules specific to this repo

- **No user data on the server.** Do not add accounts, analytics, logging of answers or anything
  that stores personal data without an explicit decision recorded in `docs/plan.md`.
- **Content is seeded from code.** The database is disposable. Anything the site needs must be
  recreated from this repo on boot.
- **No official status.** Never use the crown, GOV.UK logotype, GDS Transport or GOV.UK brand
  blue, and never word anything as official, approved or predictive of an assessment outcome.
  Credit Open Government Licence content on every page that uses it.
- Mobile work lands on iOS and Android together.
