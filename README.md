# Plated

A .NET MAUI app (Android + iOS) for looking up license plates and reading/leaving comments about them.

- **Sign-in**: Google Sign-In via Firebase Auth
- **Data**: Firestore (plates, comments, reports), Firebase Storage (photos)
- **OCR**: on-device ML Kit text recognition (via `Plugin.Maui.OCR`) to auto-fill a plate number from a photo
- **Moderation**: sign-in required to post; comments can be reported/flagged and are auto-hidden after 3 reports

## Project layout

- `Plated.Core` — platform-agnostic library: models, services (auth/Firestore/storage/OCR/photo capture), view models, converters, navigation route constants. Targets plain `net10.0` so it builds without any mobile workload.
- `Plated` — the MAUI app head (`net10.0-android`, `net10.0-ios`): XAML views, Shell navigation, platform bootstrap (`MainActivity`, `AppDelegate`), resources/branding.

## One-time setup (required before the app will actually sign in / read or write data)

### 1. Create a Firebase project

1. Go to the [Firebase console](https://console.firebase.google.com) and create a project.
2. Add an **Android app** with application ID `com.plated.app` (or update `ApplicationId` in `Plated/Plated.csproj` to match whatever you use). Download the generated **`google-services.json`** and place it at `Plated/Platforms/Android/google-services.json`.
3. Add an **iOS app** with bundle ID `com.plated.app`. Download **`GoogleService-Info.plist`** and place it at `Plated/Platforms/iOS/GoogleService-Info.plist`.
4. In **Build > Authentication > Sign-in method**, enable **Google**.
5. In **Build > Firestore Database**, create a database (start in production mode), then publish the rules in [`firestore.rules`](firestore.rules) (repo root) via the Firebase console's Rules tab or the Firebase CLI (`firebase deploy --only firestore:rules`).
6. In **Build > Storage**, create a default bucket, then publish [`storage.rules`](storage.rules) the same way.

Both `.rules` files are provided as a starting point: authenticated users can read; writes are restricted to the user's own uid on their fields.

### 2. Wire up Google Sign-In client IDs

**Android** — after enabling Google sign-in in step 1.4, Firebase auto-creates a **Web client ID** OAuth client (Google Cloud Console → APIs & Services → Credentials → "Web client (auto created by Google Service)"). Copy it into:

```
Plated/AppConfig.cs → GoogleWebClientId
```

**iOS** — open the `GoogleService-Info.plist` you downloaded and copy its `REVERSED_CLIENT_ID` value into:

```
Plated/Platforms/iOS/Info.plist → CFBundleURLTypes → CFBundleURLSchemes
```

(replacing the `REPLACE_WITH_REVERSED_CLIENT_ID` placeholder). No manual client ID is needed elsewhere on iOS — it's read from the plist automatically at runtime.

### 3. Build

Open `Plated.slnx` in Visual Studio 2026 and run the `Plated` project on an Android emulator/device, or on iOS via a paired Mac. `net10.0-ios` can only be built with Xcode/a Mac present (that's an Apple platform requirement, not specific to this project).

## Notes / follow-ups

- OCR runs fully on-device (Google ML Kit on Android, Vision framework on iOS) — no network call, no extra cost, but accuracy varies with photo angle/lighting; the recognized text is always editable before posting.
- The report/flag threshold (3 reports → auto-hide) lives in `FirestorePlateService.HideAfterReportCount`.
- There's currently no admin/moderation UI to review or un-hide reported comments — that'd be a good next feature.
- Package versions for `Plugin.Firebase.*` currently target `net9.0`-flavored TFMs (the latest available at the time of writing); NuGet resolves these into the `net10.0-android`/`net10.0-ios` app just fine, but watch for newer releases that add native `net10.0` support.
