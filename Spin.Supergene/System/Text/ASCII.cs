using System;

namespace System.Text;

/// <summary>
/// Byte type with enumeration constants for ASCII control codes.
/// </summary>
public enum ASCII : byte
{
  /// <summary>
  /// Null
  /// </summary>
  NULL = 0x00,

  /// <summary>
  /// Start of Heading
  /// </summary>
  SOH = 0x01,

  /// <summary>
  /// Start of Text
  /// </summary>
  STX = 0x02,

  /// <summary>
  /// End of Text
  /// </summary>
  ETX = 0x03,

  /// <summary>
  /// End of Transmission
  /// </summary>
  EOT = 0x04,

  /// <summary>
  /// Enquiry
  /// </summary>
  ENQ = 0x05,

  /// <summary>
  /// Acknowledge
  /// </summary>
  ACK = 0x06,

  /// <summary>
  /// Bell
  /// </summary>
  BELL = 0x07,

  /// <summary>
  /// Backspace
  /// </summary>
  BS = 0x08,

  /// <summary>
  /// Horizontal Tab
  /// </summary>
  TAB = 0x09,

  /// <summary>
  /// Line Feed / New Line
  /// </summary>
  LF = 0x0A,

  /// <summary>
  /// Vertical Tab
  /// </summary>
  VT = 0x0B,

  /// <summary>
  /// Form Feed / New Page
  /// </summary>
  FF = 0x0C,

  /// <summary>
  /// Carriage Return
  /// </summary>
  CR = 0x0D,

  /// <summary>
  /// Shift out
  /// </summary>
  SO = 0x0E,

  /// <summary>
  /// Shift in
  /// </summary>
  SI = 0x0F,

  /// <summary>
  /// Data Link Escape
  /// </summary>
  DLE = 0x10,

  /// <summary>
  /// Device Control 1
  /// </summary>
  DC1 = 0x11,

  /// <summary>
  /// Device Control 2
  /// </summary>
  DC2 = 0x12,

  /// <summary>
  /// Device Control 3
  /// </summary>
  DC3 = 0x13,

  /// <summary>
  /// Device Control 4
  /// </summary>
  DC4 = 0x14,

  /// <summary>
  /// Negative Acknowledge
  /// </summary>
  NAK = 0x15,

  /// <summary>
  /// Synchronous Idle
  /// </summary>
  SYN = 0x16,

  /// <summary>
  /// End of transmission
  /// </summary>
  ETB = 0x17,

  /// <summary>
  /// Cancel
  /// </summary>
  CAN = 0x18,

  /// <summary>
  /// End of Medium
  /// </summary>
  EM = 0x19,

  /// <summary>
  /// Substitute
  /// </summary>
  SUB = 0x1A,

  /// <summary>
  /// Esscape
  /// </summary>
  ESC = 0x1B,

  /// <summary>
  /// File Seperator
  /// </summary>
  FS = 0x1C,

  /// <summary>
  /// Group Seperator
  /// </summary>
  GS = 0x1D,

  /// <summary>
  /// Record Seperator
  /// </summary>
  RS = 0x1E,

  /// <summary>
  /// Unit Seperator
  /// </summary>
  US = 0x1F,

  /// <summary>
  /// Space
  /// </summary>
  SP = 0x20,

  /// <summary>
  /// Delete
  /// </summary>
  DEL = 0x7F
}
