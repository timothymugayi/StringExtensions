# StringExtensions

c# StringExtensions Library provides comprehensive string extension methods that go behold just the common string validation methods extending the .Net System.string class. The idea to create such a library was motivated by the lack of such a StringUtil library such as org.apache.commons.lang3.StringUtils in the the .Net realm. The aim of this library is to serve as a goto library for those wishing to have such a library readily available to incorporate in to new or existing projects. 


## Installation

```PM>
Install-Package StringExtensionsLibrary
```

## Usage

Once you have installed the String extension library within your project. String extensions functions will be available on all string types 


    if("64.233.161.1470".IsValidIPv4()){
    	\\do something
    }
    

## String Extension functions



| Function Name           | Description                                                                                                                                                                                                         |
|-------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| IsDateTime              | Checks if date with dateFormat is parse-able to System.DateTime format returns boolean                                                                                                                              |
| ToInt32                 | Converts the string representation of a number to its 32-bit signed integer equivalent                                                                                                                              |
| ToInt64                 | Converts the string representation of a number to its 64-bit signed integer equivalent                                                                                                                              |
| ToInt16                 | Converts the string representation of a number to its 16-bit signed integer equivalent                                                                                                                              |
| ToDecimal               | Converts the string representation of a number to its System.Decimal equivalent                                                                                                                                     |
| ToBoolean               | Converts string to its boolean equivalent                                                                                                                                                                           |
| ToBytes                 | Convert a string to its equivalent byte array                                                                                                                                                                       |
| SplitTo                 | Returns an enumerable collection of the specified type containing the substrings in this instance that are delimited by elements of a specified Char array                                                          |
| ToEnum                  | Converts string to its Enum type,,Checks if string is a member of type T enum before converting. if fails returns default enum                                                                                      |
| Format                  | Replaces one or more format items in a specified string with the string representation of a specified object                                                                                                        |
| GetEmptyStringIfNull    | Gets empty String if passed value is of type Null                                                                                                                                                                   |
| GetDefaultIfEmpty       | Returns a default String value if given value is null or empty                                                                                                                                                      |
| IsInteger               | IsInteger Function checks if a string is a valid int32 value                                                                                                                                                        |
| IsNumeric               | Checks if a string is a valid floating value                                                                                                                                                                        |
| IsAlpha                 | Checks if String contains only Unicode letters                                                                                                                                                                      |
| IsAlphaNumeric          | Checks if the String contains only Unicode letters & digits.                                                                                                                                                        |
| IsValidIPv4             | Checks if a string is valid IPv4                                                                                                                                                                                    |
| IsEmailAddress          | checks if string is a valid email address                                                                                                                                                                           |
| Truncate                | Truncate String and appends trailing ...                                                                                                                                                                            |
| Capitalize              | Reads in a sequence of words from standard input and capitalize each,one (make first letter uppercase; make rest lowercase                                                                                          |
| FristCharacter          | Gets the first character in string                                                                                                                                                                                  |
| LastCharacter           | Gets last character in string                                                                                                                                                                                       |
| Replace                 | Replace specified characters with an empty string                                                                                                                                                                   |
| RemoveChars             | Remove Characters from string                                                                                                                                                                                       |
| Reverse                 | Reverse string                                                                                                                                                                                                      |
| ParseStringToCsv        | Escapes string by appending quotes for csv output                                                                                                                                                                   |
| Encrypt                 | Encrypt a string using the supplied key. Encoding is done using RSA encryption                                                                                                                                      |
| Decrypt                 | Decrypt a string using the supplied key. Decoding is done using RSA encryption                                                                                                                                      |
| CountOccurrences        | Count number of occurrences in string based on the string to match                                                                                                                                                  |
| JsonToDictionary        | Converts a Json string to dictionary object. function is only applicable for single hierarchy objects i.e no parent child relationships, for parent child relationships JsonToExpanderObject                        |
| JsonToExpanderObject    | Converts a Json string to ExpandoObject method applicable for multi hierarchy objects i.e,having zero or many parent child relationships                                                                            |
| JsonToObject            | Converts a Json string to object of type T. function applicable for multi hierarchy objects i.e,having zero or many parent child relationships, Ignore loop references and do not serialize if cycles are detected. |
| RemovePrefix            | Removes the first part of the string, if no match found return original string                                                                                                                                      |
| RemoveSuffix            | Removes the end part of the string, if no match found return original string                                                                                                                                        |
| EndsWithIgnoreCase      | Check a String ends with another string ignoring the case.                                                                                                                                                          |
| StartsWithIgnoreCase    | Check a String starts with another string ignoring the case.                                                                                                                                                        |
| DoesNotStartWith        | Check if a string does not start with prefix                                                                                                                                                                        |
| DoesNotEndWith          | Check if a string does not end with prefix                                                                                                                                                                          |
| AppendSuffixIfMissing   | Appends the suffix to the end of the string if the string does not already end in the suffix                                                                                                                        |
| AppendPrefixIfMissing   | Appends the prefix to the start of the string if the string does not already start with prefix                                                                                                                      |
| CreateHashSha512        | Convert string to Hash using Sha512                                                                                                                                                                                 |
| CreateHashSha256        | Convert string to Hash using Sha256                                                                                                                                                                                 |
| QueryStringToDictionary | Convert url query string to IDictionary value key pair                                                                                                                                                              |
| ReverseSlash            | Reverse back or forward slashes                                                                                                                                                                                     |
| ReplaceLineFeeds        | Replace Line Feeds                                                                                                                                                                                                  |
| GetByteSize             | Calculates the amount of bytes occupied by the input string based on the specified encoding argument                                                                                                                |
| Left                    | Extracts the left part of the input string limited by the length argument                                                                                                                                           |
| Right                   | Extracts the right part of the input string limited by the length argument                                                                                                                                          |
| ToTextElements          | Converts a string to an Enumerable collection type of string elements                                                                                                                                               |
| IsNull                  | Checks if a string is null                                                                                                                                                                                          |
| IsMinLength             | Checks if string length is a certain minimum number of characters, does not ignore leading and trailing,white-space.,null strings will always evaluate to false.                                                    |
| IsMaxLength             | Checks if string length consists of the specified allowable maximum char length                                                                                                                                     |
| IsLength                | Checks if string length satisfies minimum and maximum allowable char length. does not ignore leading and,trailing white-space                                                                                       |
| GetLength               | Gets the number of characters in string checks if string is null                                                                                                                                                    |
| CreateParameters        | Create basic dynamic SQL where parameters from a JSON key value pair string                                                                                                                                         |
