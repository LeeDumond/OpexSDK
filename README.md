# OpexSDK
A simple .NET Standard 2.0 library that facilitates the reading of data contained in batch information files (\*.oxi) created by OPEX scanning devices, along with optional validation against an XSD schema definition file supplied by the user.

**DISCLAIMER: This software is not authored, supported, maintained, or endorsed by OPEX Corporation or any of its employees. OPEX Corporation assumes no liability from the use of this software.**

## How it works
The major player here is the `BatchReader` object, contained in the `OpexSDK` class library project. The `OpexSDK.ConsoleDemo` project is included as a demonstration of usage.

To use, import the `OpexSDK` library, then instantiate a `BatchReader` object using that class constructor. The constructor accepts up to three arguments:
- `batchFilePath` (required): The file path to the batch information file that you want to extract the data from. This file must end in an .oxi extension.
- `schemaFilePath` (optional): The path to an XSD schema definition file to validate against. By default this argument is `null`, which means no validation is performed.
- `throwOnValidationError` (optional): If set to `true`, an exception will be thrown if any validation errors are encountered. If set to `false`, validation errors will be added to the `ValidationErrors` collection, but no exception will be thrown. The default is `false`. Note that if no schema is supplied (`schemaFilePath` is `null`), this parameter has no effect.
```C#
var reader = new BatchReader("C:\images\sample.oxi", "C:\schemas\oxi1_60.xsd", false);
```
Once you have a `BatchReader`, call either `ReadBatch()` or `ReadBatchAsync()`, which will return a `Batch` object containing all the data read from the batch information file supplied to the reader.
```C#
Batch batch = reader.ReadBatch();
```
or
```C#
Batch batch = await reader.ReadBatchAsync();
```
From there, you may use the properties and collections contained in the `Batch` object to access the data, or access any validation errors that occured using the reader's `ValidationErrors` collection.
```C#
var errors = reader.ValidationErrors;
```
## Navigating a Batch
The structure of a `Batch` mirrors how a typical batch information file is structured. Each element in the file maps to a class in the `OpexSDK` library; e.g. the `<Batch>` element maps to a `Batch` object; each `<Transaction>` matches to a `Transaction` object, which is represented by the collection property `Batch.Transactions`; each `<Group>` maps to a `Group` object, which is represented by the collection property `Transition.Groups`, and so on. The attributes of each element is represented as a property on its corresponding object; e.g. the `"FormatVersion"` attribute of `<Batch>` maps to the `Batch.FormatVersion` property, and so on.

All of the objects are designed to be immutable; meaning their content cannot be programatically changed. Properties have no accessible setters, and all collections are read-only. If your requirements are such that you need to modify properties or collections post-read, you can derive your own subclasses from those in the library which implement this functionality.

By default, the read algorithm is forgiving in that unexpected or missing attibutes or elements, or attributes with empty values, will not stop the reader, even if they are marked "required" by the schema (assuming `throwOnValidationError` is set to `false`, which is the default behavior): 
- Any unexpected attributes or elements will be ignored. 
- The properties corresponding to missing attributes will be set to `null`. 
- Properties corresponding to attributes with empty values will be set to `null` (or `string.Empty` in the case of string properties).

If you provided a schema definition file to validate against, you should check the `ValidationErrors` collection and decide whether any anomalies that are logged there are important to you.

## Contributions

Issues and contributions are most welcome. *Please target all pull requests to the* `development` *branch only*.
