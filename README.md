
# Persona 5 Strikers PC Save Utility

A simple save decryption, encryption and conversion utility for P5S PC.

## Decrypting or Encrypting PC Saves

- Locate `SAVEDATA.BIN` under `%APPDATA%/SEGA/Steam/P5S/<account_id>/`

- Get the decimal **SteamID64** associated with the account under which the save was created

  The last segment of **SteamID3** will also work (`[U:1:<last_segment>]`)

  The name of the directory containing `SAVEDATA.BIN` (`<account_id>`) will also work

- To decrypt or encrypt the save:

  `p5spc.saveutil.exe crypt --input </path/to/SAVEDATA.BIN> --steam <SteamID64>`

- Example (using `powershell`):

  ```powershell
  # decrypt SAVEDATA.BIN created with account ID 123454321
  p5spc.saveutil.exe crypt `
    --input "$env:APPDATA/SEGA/Steam/P5S/123454321/SAVEDATA.BIN" `
    --steam 123454321

  # encrypt the output with account ID 9876543210
  p5spc.saveutil.exe crypt `
    --input "$env:APPDATA/SEGA/Steam/P5S/123454321/SAVEDATA.BIN_crypt" `
    --steam 9876543210 `
    --output "$env:APPDATA/SEGA/Steam/P5S/9876543210/SAVEDATA.BIN"
  ```

_Preview - encrypted save above, decrypted below_:

![crypt](img/crypt.png)

## Converting Saves

### Supported Save Formats

- `Switch_JP` / `Switch_EN`
- `PC` - Saves must be [decrypted](#decrypting-or-encrypting-pc-saves) before converting
- `PS4_JP` / `PS4_EN` - Saves must be decrypted before converting (e.g. using homebrew)

### Usage

- Get a save to convert:
  - Decrypt a PC save (`SAVEDATA.BIN`) or
  - Dump a save from Switch (`savedata`) or
  - Dump a **decrypted** save from PS4 (`APP.BIN`)

- The source format will be auto-detected

- To convert:

  `p5spc.saveutil.exe convert --input <path/to/savedata> --target <target_format>`

  With `<target_format>` being any of the formats described [above](#supported-save-formats)

- Saves converted to PC format must be [encrypted](#decrypting-or-encrypting-pc-saves) before they can be used:

  `p5spc.saveutil.exe crypt --input <path/to/savedata_conv> --steam <SteamID64>`

- Example (using `powershell`):

  ```powershell
  # switch (en/jp) save to PC format
  p5spc.saveutil.exe convert --input "/path/to/savedata" --target PC

  # switch (en/jp) save to PS4 jp format
  p5spc.saveutil.exe convert --input "/path/to/savedata" --target PS4_JP

  # decrypted PC save to switch en format
  p5spc.saveutil.exe convert --input "/path/to/SAVEDATA.BIN_crypt" --target Switch_EN

  # decrypted PS4 (en/jp) save to PC format
  p5spc.saveutil.exe convert --input "/path/to/APP.BIN" --target PC
  ```

_Preview - Switch JP save converted to PC save_:

![convert](img/convert.png)
