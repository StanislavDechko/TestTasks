Prompt:

You are working in a C# project using .NET 7. Your task is to generate a comprehensive markdown documentation file for the `NotificationService` class, following the structure and style of the provided documentation example (`DocumentConversionService API`).

The documentation should be clear, well-structured, and suitable for both developers and API consumers. Use markdown best practices, including YAML front matter, section headings, tables, and code blocks. Highlight all method names, class names, and code elements using markdown backticks.

**Requirements:**
- Start with YAML front matter including `title`, `version`, and `date`.
- Include the following sections, in order:
  1. **Overview**: Briefly describe the purpose and capabilities of `NotificationService` (sending Email, SMS, and Push notifications). Mention the return type `NotificationResult` and what it represents.
  2. **API Reference**: Present a table listing all public methods (`SendEmailAsync`, `SendSmsAsync`, `SendPushAsync`), their descriptions, and return types.
  3. **Examples**:
     - Provide C# usage examples for each method, using proper code blocks.
     - Include a cURL example for at least one method (e.g., sending SMS), with a sample JSON request and expected JSON response in separate code blocks.
  4. **Error Codes**: Present a table of possible error codes and their meanings.
  5. **Notes**: List any important notes about the documentation structure, code block languages, and table usage.
- Use the existing markdown documentation example as a template for formatting and completeness. Match its level of detail and clarity.
- Ensure all code blocks are properly labeled (e.g., `csharp`, `bash`, `json`).
- Use tables for API reference and error codes.
- The documentation should be self-contained and not require external references.
- Reference the `NotificationResult` model in the documentation, including its properties.

**Reference Example:**
See the `DocumentConversionService API` markdown documentation for the expected structure, formatting, and level of detail.

**Deliverable:**
A complete markdown documentation file for `NotificationService`, following all the above requirements and using the provided example as a template.