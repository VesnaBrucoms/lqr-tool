# Structure of the LQR File Format

LQR files are broken down into at least five sections:
* header
* column definition
* table rows
* unknown
* unknown
* text

## Header

The header stores the checksum, offsets, and column definitions.

| Type  | Bytes | Name         | Description                                      |
| ----- | ----- | ------------ | ------------------------------------------------ |
| int32 | 4	    | unknown      | Always `106`                                     |
| int32	| 4	    | unknown      | Possibly checksum                                |
| long	| 8	    | columnOffset | Offset to `rowsOffset` and the column definition |
| long	| 8	    | offset       | Number of text entries in the file               |
| long	| 8	    | offset       | Number of text entries in the file               |
| long	| 8	    | textOffset   | Offset to the start of the text section          |
| long	| 8	    | rowsOffset   | Offset to the start of the table rows section    |

## Column Definition

This section defines the number of columns and the data type for each column.

| Type  | Bytes | Name        | Description                     |
| ----- | ----- | ----------- | ------------------------------- |
| int32	| 4	    | columns     | Number of columns in this table |

Following the `columns` field is an array of int32. The `columns` field indicates the size of the array.
This can be 0, in which case, the section ends. For each column:

| Type  | Bytes | Name        | Description                          |
| ----- | ----- | ----------- | ------------------------------------ |
| int32	| 4	    | dataType    | Will be values 0-4                   |

The `dataType` is the ID of the type of data in that respective column. They are:
* 0 - int32
* 1 - float
* 2 - string
* 3 - int32
* 4 - string

## Table Rows

This section, as the name suggests, is where the data for each table row is kept.

The first two fields are always the same:

| Type  | Bytes | Name        | Description                               |
| ----- | ----- | ----------- | ----------------------------------------- |
| int32	| 4	    | enabledRows | The number of table rows that are enabled |
| int32	| 4	    | totalRows   | The total number of rows in this table    |

`tableRows` indicates the number of rows to read. Each row starts with the same two fields:

| Type  | Bytes | Name      | Description                                                                         |
| ----- | ----- | --------- | ----------------------------------------------------------------------------------- |
| int32	| 4	    | id        | Primary key                                                                         |
| byte	| 1	    | isEnabled | Indicates whether the row is enabled (in use). Only the values `0` and `1` are used |

If `isEnabled` is `0`, then there are no further bytes to read for this row. So move onto the next row.

Otherwise, what to read next is determined by `header.columns` and the `header.dataTypes` array.

For `dataType == 0`:

| Type  | Bytes | Name        |
| ----- | ----- | ----------- |
| int32	| 4	    | columnValue |

For `dataType == 1`:

| Type  | Bytes | Name        |
| ----- | ----- | ----------- |
| float	| 4	    | columnValue |

For `dataType == 2`:

| Type                          | Bytes | Name           |
| ----------------------------- | ----- | -------------- |
| byte	                        | 1     | unknown        |
| int32	                        | 4	    | characterCount |
| string (UTF-16 little endian) | x	    | text           |

For `dataType == 3`:

| Type  | Bytes | Name        |
| ----- | ----- | ----------- |
| int32	| 4	    | columnValue |

For `dataType == 4`:

| Type                          | Bytes | Name           |
| ----------------------------- | ----- | -------------- |
| byte	                        | 1	    | unknown        |
| int32	                        | 4	    | characterCount |
| string (UTF-16 little endian) | x     | text           |

## Unknown

Under investigation...

## Unknown

Under investigation...

## Text

Here is where text related to the table is stored. Such as column headings, but also UI text and what appears to be
internal string IDs.

This section is simply an array of UTF-16 strings:

| Type                          | Bytes | Name           | Description                                 |
| ----------------------------- | ----- | -------------- | ------------------------------------------- |
| int32	                        | 4	    | length         | The size of the array                       |
| int32	                        | 4	    | bytes          | Total number of bytes taken up by the array |
| string (UTF-16 little endian) | x     | text           | The array of strings                        |