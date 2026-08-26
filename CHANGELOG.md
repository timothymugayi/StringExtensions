# Changelog

## 2.0.0

Breaking changes from the .NET Framework 4.5.1 / Visual Studio 2013 package:

- **Target framework.** The library is now **.NET Standard 2.0** (SDK-style project). It no longer targets .NET Framework 4.5.1 only.
- **Encrypt / Decrypt.** Windows RSA key-container crypto (`CspParameters` / `RSACryptoServiceProvider`) is replaced with **AES-256-CBC**, **HMAC-SHA256** integrity, and **PBKDF2-HMAC-SHA256** (100,000 iterations) for key derivation. Ciphertext is a hyphen-separated hex payload: salt + IV + ciphertext + tag. Payloads produced by 1.x cannot be decrypted. Wrong passphrase or a tampered payload throws `CryptographicException`.
- **CreateParameters removed.** The SQL-concatenation helper is gone. Use parameterized queries.
- **IsValidIPv4.** Uses `IPAddress.TryParse` and requires a canonical dotted-quad (`AddressFamily.InterNetwork`). Scheme-prefixed strings, IPv6, and non-canonical forms such as `127.00.0.1` are rejected.
- **JsonToObject.** Throws `InvalidOperationException` when JSON deserializes to null instead of returning `default(T)`.
- **ReplaceLineFeeds.** Removes CR/LF sequences only. It no longer strips `.` characters (the previous regex matched a literal period).
- **QueryStringToDictionary.** Reads the query after the first `?`, strips a `#` fragment, URL-decodes keys and values (`Uri.UnescapeDataString`; `+` is a space), and last-wins on duplicate keys.

Other notable changes:

- Hash helpers (`CreateHashSha256`, `CreateHashSha512`) and `GetLength` are extension methods (`this`).
- Several P0 contract fixes: `IsLength` honors the maximum bound; `RemoveSuffix` returns the original string when the suffix is missing; `CountOccurrences` treats the needle as a literal; `Capitalize` / `IsEmailAddress` are null-safe.
