# Contributing

## Null policy

Keep these contracts consistent when adding methods:

- **Predicates** (`Is*`, `DoesNot*`): accept null and never throw. `IsNull` is true for null; other `Is*` methods return false; `DoesNotStartWith` / `DoesNotEndWith` return true when either side is null.
- **Ignore-case start/end**: throw `ArgumentNullException` when `val` or the prefix/suffix is null.
- **Transforms** (`Reverse`, `Left`, `ToBytes`, `SplitTo`, `Replace`, …): throw `ArgumentNullException` (or `ArgumentException` when empty is invalid, e.g. encrypt/hash).
- **Null-coalescing helpers** (`GetEmptyStringIfNull`, `GetDefaultIfEmpty`, `Truncate`): defined results for null (empty string or the provided default).
- Use `nameof` in exception arguments. Use invariant culture for case conversion unless the caller passes a culture.

## Tests

Every public method needs a happy path, a null/empty case, and the documented error case. Split tests by concern (`ConversionTests`, `ValidationTests`, `MutationTests`, `JsonAndQueryTests`, `CryptoTests`).

## SQL

Do not add helpers that concatenate untrusted strings into SQL.

## Crypto

`Encrypt` / `Decrypt` use AES-256-CBC with HMAC-SHA256 over the payload and PBKDF2-HMAC-SHA256 (100,000 iterations) for key derivation on all target frameworks. Ciphertext from the old Windows RSA key-container implementation will not decrypt. This is passphrase-based authenticated encryption for application data, not a key-management system.

## Email and IPv4

`IsEmailAddress` is a practical heuristic (plus-tags allowed, input is trimmed). It is not RFC 5322-complete. `IsValidIPv4` requires a canonical dotted-quad.
