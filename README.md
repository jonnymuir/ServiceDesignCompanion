# Service Design Companion

A free pocket companion for service designers and delivery teams: the UK Service Standard in plain
words, a phase finder that suggests what your team should focus on next, and a set of service
design method cards.

It is also a real-world example of an app built on
[Umbraco Prism](https://github.com/jonnymuir/Umbraco.Prism) and
[Wayfinder.Umbraco](https://github.com/jonnymuir/Wayfinder.Umbraco), installed from nuget.org
like any other consumer would.

## Status

Early setup. See [docs/plan.md](docs/plan.md) for what version 1 contains and the decisions behind
it.

## Independence

This is an independent app. It is not made, endorsed or approved by the Government Digital
Service or any part of government. Only an assessment panel decides whether a service meets the
Service Standard.

Contains public sector information licensed under the
[Open Government Licence v3.0](https://www.nationalarchives.gov.uk/doc/open-government-licence/version/3/).

## Build and run

```bash
dotnet build ServiceDesignCompanion.slnx
mkdir -p src/ServiceDesignCompanion.Site/umbraco/Data
dotnet run --project src/ServiceDesignCompanion.Site
```

## Licence

Code is [MIT](LICENSE). Service Standard content is reused under the Open Government Licence v3.0.
