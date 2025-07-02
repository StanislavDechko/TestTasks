---
title: "DocumentConversionService API"
version: "1.0.0"
date: "2025-07-02"
---

# Overview
A service for converting documents between formats (PDF, DOCX, HTML) and extracting plain text.

# API Reference
| Method                                           | Description                                        | Return Type                   |
|--------------------------------------------------|----------------------------------------------------|-------------------------------|
| `ConvertToPdfAsync(inputBytes, options)`         | Converts a document (DOCX/HTML) to PDF.            | `Task<ConversionResult>`      |
| `ConvertToDocxAsync(inputBytes, options)`        | Converts a document (PDF/HTML) to DOCX.            | `Task<ConversionResult>`      |
| `ConvertToHtmlAsync(inputBytes, options)`        | Converts a document (PDF/DOCX) to HTML.            | `Task<ConversionResult>`      |
| `ExtractTextAsync(inputBytes, options)`          | Extracts plain text from a document.               | `Task<ConversionResult>`      |

# Examples
**C# usage:**
```csharp
// Convert DOCX to PDF
var pdfResult = await documentService.ConvertToPdfAsync(
    File.ReadAllBytes("report.docx"),
    new ConversionOptions { PageSize = "A4", EmbedFonts = true }
);
// Check success
if (pdfResult.IsSuccess)
{
    File.WriteAllBytes("report.pdf", pdfResult.OutputBytes);
}

// Extract text from PDF
var textResult = await documentService.ExtractTextAsync(
    File.ReadAllBytes("slides.pdf"),
    new ConversionOptions { PreserveLayout = false }
);
// textResult.OutputText contains the extracted string

curl -X POST https://api.myapp.com/convert/pdf \
  -H "Content-Type: application/octet-stream" \
  --data-binary @presentation.html \
  --header "X-Options: {\"PageSize\":\"Letter\"}"
# Expected JSON:
# {
#   "isSuccess": true,
#   "errorCode": null,
#   "errorMessage": null,
#   "outputBytes": "<base64-encoded PDF>"
# }

# Error codes
// ConversionError	General conversion failure
// UnsupportedFormat	Input format not supported
// ValidationError	Input content failed validation (e.g., corrupt)

# YAML‑front‑matter: metadata  
# Sections in order: Overview, API Reference, Examples, Error Codes, Models  
# Tables: API Reference and Error Codes  
# Code blocks: ```csharp``` and ```bash```  