
# Persona 5 Strikers PC Save Utility

A simple save decryption, encryption and conversion utility for P5S PC.

## Decrypting/Encrypting PC Saves

- Locate `SAVEDATA.BIN` under `%AppData%\SEGA\Steam\P5S`.

- Get the **SteamID64** associated with the account under which the save was created.

  The last segment of **SteamID3** will also work (`[U:1:<last_segment>]`).

- To decrypt or encrypt the save:

  `p5spc.saveutil.exe crypt --input </path/to/SAVEDATA.BIN> --steam <SteamID64>`

_Example - encrypted save above, decrypted below_:

![crypt](img/crypt.png)

## Converting Saves

Only conversion between Switch JP saves and PC saves is supported for now.

- Decrypt a PC save (`SAVEDATA.BIN`), or dump a save from Switch (`savedata`).

- The source and destination formats will be auto-detected.

- To convert:

  `p5spc.saveutil.exe convert --input <path/to/savedata>`

_Example - Switch JP save converted to PC save_:

![convert](img/convert.png)
