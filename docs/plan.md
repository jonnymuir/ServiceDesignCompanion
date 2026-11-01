# Plan

## What the app is

A free mobile app (iOS and Android) and companion website with three parts:

1. **The Service Standard in plain words.** The 14 points, each with a short explainer written for
   this app, questions to think about, and links to the live guidance on GOV.UK.
2. **Phase finder.** A short anonymous Wayfinder journey. A team answers a few questions about
   their project and gets a suggestion of their likely phase (discovery, alpha, beta or live),
   what is usually expected next, and which points to focus on. The answer is shown on screen
   and never stored.
3. **Method cards.** Around 20 to 30 service design methods, each with what it is for, when to use
   it, the phase it suits and the related Standard point. Filterable by phase and by point.
   Favourites are kept on the device only.

## Name and brand

| | |
|---|---|
| App Store name | Pocket Service Design |
| App Store subtitle | Your service design companion |
| Home-screen label | Pocket SD |
| Brand colour | Signpost teal `#0B6E69` (dark mode `#4FC2B8`) |
| Text and background | Ink `#1B2626` on `#F4F8F7` |
| Focus | Warm yellow `#FFC94A` with a dark underline |
| Typeface | Atkinson Hyperlegible Next, served from the site itself |

Brand colours are set once as Prism `--prism-*` tokens; Prism's bridge carries them onto GOV.UK
Frontend's `--govuk-*` properties. Every pairing meets WCAG AA, body text AAA. Phase tags use the
same light tints with dark text in every theme, and always name the phase in text.

## Principles

- **No user data on the server.** No accounts, no analytics, no tracking in version 1. Everything
  the server holds is published content, so a lost database loses nothing. Fonts and scripts are
  served from the site itself, so no third party learns who visits.
- **All content is seeded from code.** Method cards, Standard explainers and the phase finder
  blueprint live in this repo and are seeded on every boot.
- **No false promises.** The app helps teams prepare and reflect. It never claims to predict an
  assessment outcome or to be official.
- **Link out rather than copy.** Explainers are short and in our own words, with links to the
  current GOV.UK guidance, so the app stays correct when that guidance changes.

## Content and branding rules

- Credit the source on every Service Standard page: "Contains public sector information licensed
  under the Open Government Licence v3.0", linking to the licence and to the Service Standard.
- Never suggest official status or endorsement by GDS or government.
- No crown, Royal Arms, GOV.UK logotype, GDS Transport typeface or GOV.UK brand blue. GOV.UK
  Frontend components are used with the generic header and our own brand, following
  [Using GOV.UK Frontend without GOV.UK branding](https://frontend.design-system.service.gov.uk/using-govuk-frontend-without-govuk-branding/).
- Wording: "prepare", "rehearse", "reflect". Never "pass", "approved" or "score".
- The store name avoids "GOV.UK" and "GDS".

## Hosting

| | |
|---|---|
| Public hostname | `servicedesign.prismreference.com` |
| Routing | A published application route on the existing Cloudflare Tunnel, to `http://localhost:5003` |
| Service | `servicedesign` systemd unit, `www-data`, with its own `EnvironmentFile` |
| Database | SQLite, disposable, reseeded from code on every boot |

Production details (host, secrets, service files) belong in the private infra notes, not here.

## Notifications

Not in version 1. Prism's push registration needs a signed-in member, which would bring back
accounts and stored personal data. Two routes are open for later:

- **Optional sign-in.** The app stays fully usable without an account; signing in adds topic
  notifications (Prism genres, sent automatically when matching content is published) and synced
  favourites. Needs a database that survives deploys, in-app account deletion and a demo account
  for app review.
- **Anonymous topic push.** Devices subscribe to topics directly through Firebase topic messaging,
  so the server holds no tokens or accounts. Needs a Prism change on iOS and Android.

## Prerequisites in Umbraco Prism

- **Publish the mobile bundle generator.** `UmbracoPrism.MobileBundleCli` is not packable, so this
  repo cannot generate its native app in CI. Publishing it as a `dotnet tool` alongside the other
  Prism packages fixes that.

## App store checklist

- Apple Developer Program and Google Play developer accounts in your own name.
- Google Play closed test with at least 12 testers for 14 days before production, for new
  personal accounts.
- Privacy policy URL and support URL on the companion site.
- Privacy labels: no data collected.
- Native value beyond a website: offline reading, the interactive phase finder, favourites.
