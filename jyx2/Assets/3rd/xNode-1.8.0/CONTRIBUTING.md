## Contributing to xNode
💙Thank you for taking the time to contribute💙

If you haven't already, join our [Discord channel](https://discord.gg/qgPrHv4)!

## Pull Requests
Try to keep your pull requests relevant, neat, and manageable. If you are adding multiple features, split them into separate PRs.
These are the main points to follow:

1) Use formatting which is consistent with the rest of xNode base (see below)
2) Keep _one feature_ per PR (see below)
3) xNode aims to be compatible with C# 4.x, do not use new language features
4) Avoid including irellevant whitespace or formatting changes
5) Comment your code
6) Spell check your code / comments
7) Use concrete types, not *var*
8) Use english language

## New features
xNode aims to be simple and extendible, not trying to fix all of Unity's shortcomings.

Approved changes might be rejected if bundled with rejected changes, so keep PRs as separate as possible.

If your feature aims to cover something not related to editing nodes, it generally won't be accepted. If in doubt, ask on the Discord channel.

## Coding conventions
Using consistent formatting is key to having a clean git history. Skim through the code and you'll get the hang of it quickly.
* Methods, Types and properties PascalCase
* Variables camelCase
* Public methods XML commented. Params described if not obvious
* Explicit usage of brackets when doing multiple math operations on the same line

## Formatting
I use VSCode with the C# FixFormat extension and the following setting overrides:
```json
"csharpfixformat.style.spaces.beforeParenthesis": false,
"csharpfixformat.style.indent.regionIgnored": true
```
* Open braces on same line as condition
* 4 spaces for indentation.
