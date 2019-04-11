# Style Guide

## C# and Unity
Our C# style guide is derived from the [Microsoft C# coding conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions).

### Basic Text Formatting
* Tab with 4 spaces, never tab characters
* Leave one empty line at the bottom of every file
* Never have multiple blank lines except to separate section headers (as described below)
* Do not need to limit lines to 80 characters
* Use Allman style indentation, meaning that open curly braces go on their own line
* Always wrap code blocks in curly braces, even when the block is only one line long

### Capitalization
Use PascalCase for: 
* All methods
* All properties
* All public fields (regardless of const or static)
* All classes
* All enums
* All enum options

Use camelCase for:
* All function parameters
* All local variables
* All private fields (regardless of const or static)

See [here](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/capitalization-conventions) for details.


### Naming
Prefer clear variable names.  Prioritize clarity over length.  In other words, its okay to have somewhat long variable names if it means they are clearer.

Special Cases
* Methods called when a UI button is pressed should be called “Handle<ButtonName>”
* Methods called when a UI element such as a dropdown or slider changes should be called “Change<UIElementName>”

### Organization
Each class should have its own file. 

Seperate files into the following order:
1. Unity-Assigned Fields
2. Enums
3. Properties, ordered based on
   * Public, protected, private
   * Const, static, regular
4. Fields, ordered based on
   * Public, protected, private
   * Const, static readonly, static, regular
5. Constructors
6. Public Methods, with static first
7. Unity Methods in order Awake, Start, Update, then any others
8. Protected Methods, with static first
9. Private Methods, with static first

If any of these sections contain several entries, separate it with a block comment preceded by 3 newlines.  For example: 

```
...
public GameObject Sky;



////////////////////////////////////////////////////////////////
// Enums
////////////////////////////////////////////////////////////////

/// <summary>
...
```

It is also okay to combine sections under a single header if there are not many entries per section (ex: combine all methods into "Methods").  

### Docstrings and Commenting
Add C# style docstrings to the following
* All methods
* All fields
* All properties
* All enums
* All classes

Docstring notes
* All docstrings begin with a capital and do not end in a period
* Method docstrings begin with a singular verb, such as “creates”, “calculates”, etc.

Commenting notes
* Aside from docstrings, only add comments where necessary and helpful
* Comments that start on their own line begin with a capital
* Comments that are preceded by code on the same line start with a lowercase letter
* Comments never end in a period

### Miscellaneous
* Alphabetize usings and trim unnecessary usings
* Encapsulate behavior into modular methods; avoid massive Update or Start functions
* Use inheritance, abstract classes, interfaces, and other good OOPs idioms when applicable
* Avoid using Unity-assigned fields whenever possible
* Always explicitly add “private”, “public”, or “protected” keywords
* Always use the “this” keyword when possible

## HTML, JavaScript, CSS
Use Google's style guides for [HTML/CSS](https://google.github.io/styleguide/htmlcssguide.html) and [JavaScript](https://google.github.io/styleguide/jsguide.html). 

## Python
Use the [Jupyter Notebook](https://jupyter.readthedocs.io/en/latest/development_guide/coding_style.html) and [Python](https://www.python.org/dev/peps/pep-0008/) style guides.

In addition, here are the more relevant specifications for this project.

### Indentation and Spacing
* Four spaces for indentation
* Functions are separated by two lines

### Naming
* All lower case naming
* Underscores should separate words in variable / module names

### Comments
* Text blocks with sub-headings should be included explaining overarching goals of the notebook
* All functions should contain docstrings
* Blocks of more than 4-5 lines of code should have an inline comment
