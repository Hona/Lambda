# Contributing

Lambda is an open-source project, and I welcome any and all
contributions made. However, please conform to the
following guidelines to make sure your pull request is accepted:

## Development Cycle

Please discuss new features before implementation to make sure it is an approved feature, or work on one of the issues.

### Pull Requests

Please describe what you are changing and why (i.e. fixing a bug, implementing a new feature).

## Coding Style

I attempt to conform to the .NET Foundation's [Coding Style](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md)
where possible.

If you aren't sure of the style please refer to any of the other files already in the project

As a general rule, follow the coding style already set in the file you
are editing, or look at a similar file if you are adding a new one.

**If your pull request doesn't conform to the preexisting coding style then it will be declined.**

### Documentation Style for Members

When creating a new public member, the member must be annotated with sufficient documentation. This should include the
following, but not limited to:

* `<summary>` summarizing the purpose of the method.
* `<param>` or `<typeparam>` explaining the parameter.
* `<return>` explaining the type of the returned member and what it is.
* `<exception>` if the method directly throws an exception.

The length of the documentation should also follow the ruler as suggested by our
[Visual Studio Code workspace](Discord.Net.code-workspace).

#### Recommended Reads

* [Official Microsoft Documentation](https://docs.microsoft.com)
* [Sandcastle User Manual](https://ewsoftware.github.io/XMLCommentsGuide/html/4268757F-CE8D-4E6D-8502-4F7F2E22DDA3.htm)