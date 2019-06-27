# OpexSDK
A simple .NET Standard 2.0 library that facilitates the reading of data contained in batch information files conforming to the OPEX XML Interface format (\*.oxi) created by OPEX scanning devices, along with optional validation against an XSD schema definition file supplied by the user at runtime.

**DISCLAIMER: This software is not authored, supported, maintained, or endorsed by OPEX Corporation or any of its employees. OPEX Corporation assumes no liability from the use of this software.**

## How it works
The major player here is the `BatchReader` object, contained in the `OpexSDK` class library project. The `OpexSDK.ConsoleDemo` project is included as a demonstration of usage.

To use, import the `OpexSDK` library, then instantiate a `BatchReader` object. 
```C#
var reader = new BatchReader();
```
Once you have a `BatchReader`, call either `ReadBatch()` or `ReadBatchAsync()`, which will return a `Batch` object containing all the data read from the batch information file indicated by the supplied file path.
These methods accept up to three arguments:
- `batchFilePath` (required): The file path to the batch information file that you want to extract the data from. This file must end in an `.oxi` extension.
- `schemaFilePath` (optional): The path to an XSD schema definition file to validate against. By default this argument is `null`, which means no validation is performed.
- `throwOnValidationError` (optional): If set to `true`, an exception will be thrown if any validation errors are encountered. If set to `false`, validation errors will be added to the `BatchReader.ValidationErrors` collection, but no exception will be thrown. The default is `false`. Note that if no schema is supplied (`schemaFilePath` is `null`), this parameter has no effect.
```C#
Batch batch = reader.ReadBatch("C:\images\sample.oxi", "C:\schemas\oxi1_60.xsd", false);
```
or
```C#
Batch batch = await reader.ReadBatchAsync("C:\images\sample.oxi", "C:\schemas\oxi1_60.xsd", false);
```
From there, you may use the properties and collections contained in the `Batch` object to access the data.
## Navigating a Batch
The structure of a `Batch` mirrors how a typical batch information file is structured. Each element in the file maps to a class in the `OpexSDK` library; e.g. the `<Batch>` element maps to a `Batch` object; each `<Transaction>` matches to a `Transaction` object, which is represented by the collection property `Batch.Transactions`; each `<Group>` maps to a `Group` object, which is represented by the collection property `Transition.Groups`; and so on. The attributes of each element is represented as a property on its corresponding object; e.g. the `"FormatVersion"` attribute of `<Batch>` maps to the `Batch.FormatVersion` property, and so on.

All of the objects are designed to be immutable; meaning their content cannot be programatically changed. Properties have no accessible setters, and all collections are read-only. If your requirements are such that you need to modify properties or collections post-read, you can derive your own subclasses which implement this functionality.

## Validation
The read algorithm is forgiving in that, by default, anomalies such as unexpected or missing attibutes or elements, or attributes with empty values, will not stop the reader: 
- The order of attibutes is ignored.
- General formatting such as whitespace, indentation, and the like is ignored.
- Any comments or processing instructions are ignored.
- Any unexpected attributes or elements are ignored.
- Any missing attributes or elements are ignored.  
- The properties corresponding to missing attributes will be set to `null`. 
- Properties corresponding to attributes with empty values will be set to `null` (or `string.Empty` in the case of string properties).

If you provide a schema definition file to validate against, an exception will be raised for each schema validation error that is encountered. 
- If `throwOnValidationError` is `false`, these exceptions will be stored in the `BatchReader.ValidationErrors` collection, but they will *not* be thrown, which allows the read process to continue uninterrupted. In this case, once a batch is read you should inspect `ValidationErrors` to see if any of the exceptions logged there are important to you.
- If `throwOnValidationError` is `true`, the exception is thrown (which of course causes the read process to fail).

The validation itself is a two-step process. First, the schema itself is checked for validity. If the schema itself is not valid, the errors are treated as outlined above, and no further validation takes place. Once the schema passes validation, the contents of the batch information file are validated against it, again with any errors found handled as previously described.

## How Attributes are Mapped to Properties
In most cases, if an attribute is defined as a string in the schema, it is mapped to a property of type `string`. The same holds for integers (which are mapped to `int`), floating point values (`float`), date/time strings (`DateTime`) and so on. 

However, in other cases, properties *with a limited range of valid values* are mapped to enumerations, which makes working with the data a bit easier.

For example, because the `"TransportId"` attribute can be any string, the correspondng `Batch.TransportId` property is of type `string`. But the `"OperatingMode"` attribute (although defined as a string) is restricted to two only possible values: `MANUAL_SCAN` or `MODIFIED`. Therefore, it is mapped to a nullable `OperatingMode` enumeration whose members are `OperatingMode.ManualScan` and `OperatingMode.Modified`. In this case, if for any reason the attribute value in the file is empty or falls outside the range of expected values, `OperatingMode` would be set to `null`. This is only one example; you will find several places where we use enumerations to represent a limited set of possible values.

## Contributions

Issues and contributions are most welcome. *Please target all pull requests to the* `development` *branch only*.
