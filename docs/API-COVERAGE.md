# API coverage matrix

Inventory of public methods on `StringExtensionLibrary.StringExtensions` and the test class that covers them.

| Method | Concern | Tests |
|--------|---------|--------|
| IsDateTime | validate | ValidationTests |
| ToInt32, ToInt16, ToInt64, ToDecimal, ToBoolean | convert | ConversionTests |
| SplitTo | convert | ConversionTests |
| ToEnum | convert | ConversionTests |
| Format | convert | ConversionTests |
| ToBytes | convert | ConversionTests |
| GetEmptyStringIfNull, GetNullIfEmptyString, GetDefaultIfEmpty | mutate | MutationTests |
| IsInteger, IsNumeric, IsAlpha, IsAlphaNumeric | validate | ValidationTests |
| Capitalize, FirstCharacter, LastCharacter | mutate | MutationTests |
| EndsWithIgnoreCase, StartsWithIgnoreCase | validate | ValidationTests |
| Replace, RemoveChars, Truncate, Reverse, ParseStringToCsv | mutate | MutationTests |
| IsEmailAddress, IsValidIPv4 | validate | ValidationTests |
| Encrypt, Decrypt | crypto | CryptoTests |
| CountOccurrences | mutate | MutationTests |
| JsonToDictionary, JsonToExpanderObject, JsonToObject | json | JsonAndQueryTests |
| RemovePrefix, RemoveSuffix, AppendSuffixIfMissing, AppendPrefixIfMissing | mutate | MutationTests |
| CreateHashSha256, CreateHashSha512 | mutate | MutationTests |
| QueryStringToDictionary | json/query | JsonAndQueryTests |
| ReverseSlash, ReplaceLineFeeds, Left, Right, GetByteSize, ToTextElements | mutate | MutationTests |
| DoesNotStartWith, DoesNotEndWith, IsNull, IsNullOrEmpty | validate | ValidationTests |
| IsMinLength, IsMaxLength, IsLength, GetLength | validate | ValidationTests |

Pass rule: each method has happy path, null/empty, and documented error behavior in the listed test class.
