# Buildings

Setup for buildings, their floors, rooms, and shelves. Administrators only (planned; open to
everyone until accounts exist). Designed for a desktop.

The built pages still use the earlier single-floor model under the name Floor Plans
(`/FloorPlans`). Everything below describes the building model.

## List

**Status: built as Floor Plans.**

- Lists every building with its code, number of floors, and state, and a link to edit it.
- With no buildings, the page shows only a centered "Create building" button in the space a
  floor would fill.

## Create

**Status: planned.** The built Floor Plans form asks for a name, width, and height.

- **Name:** required, at most 100 characters.
- **Code:** required, one letter A–Z, unique across buildings. Starts every shelf name in the
  building.
- **Number of floors:** fixed; a building never gains or loses floors.
- **Floor 1 width and height** in decimal feet, 3–3,000 ft. Other floors start with the same
  size and no offset.

A new building starts in Setup and opens in the editor.

## Floors

**Status: planned.**

- Numbered 1, 2, 3 upward.
- Each has its own width, height, and offset within the building's shared coordinates, so a
  point on one floor is directly above the same point on the floor below.

## Editor

**Status: planned.**

Edits one floor at a time, with a floor picker. Two modes; the other mode's layer stays visible
but faded and cannot be clicked.

- **Structure:** walls, doors, and stairs.
  - Walls are segments with draggable endpoints and snapping. "Add wall" drops in a segment.
  - Stairs are rectangles with a marked top and bottom side, linking a floor to the one above.
    They appear on both floors.
- **Shelves:** rooms and shelves.

All dragging and resizing uses interact.js on the server-rendered SVG; each change is saved
through a controller action.

## Setup and Done

**Status: planned.**

- A building starts in **Setup**; the editor opens in Structure mode.
- Marking it **Done** locks Structure mode (unlocking needs a confirmation), opens the editor
  in Shelves mode from then on, and makes the building available on [Home](home.md).

## Rooms

**Status: planned.**

- A room is a rectangle dragged out in Shelves mode that snaps to walls and doors.
- Each room has a letter, A–Z, unique within its building.
- Each room has one entrance door, defaulting to the first door on its edge. An Administrator
  can choose another.

## Shelf names

**Status: planned.**

- Shelves are named `BUILDING-ROOM` followed by a number, for example `W-A1`.
- A new shelf in a room takes the room's next unused number.
- Names never change unless an Administrator asks, in [reorganize mode](#reorganize-mode).

### Numbering order

Used when a room is numbered or renumbered. Directions are as seen from the room's entrance,
facing in:

1. Left to right across the room.
2. Shelves whose left-to-right ranges overlap are numbered from the far side toward the
   entrance.

The goal is predictability: from any shelf, higher numbers are to its right.

## Reorganize mode

**Status: planned.**

1. Move, add, or remove shelves in a room or floor. Nothing is written to the store yet.
2. Finishing shows a plan: every rename the new layout needs under the numbering order, how
   many variants each touches, and a printable list of physical labels to change.
3. Confirming applies the renames to the store as one batch.

## Shelf rules

**Status: planned.**

- A shelf's level count is set when it is created and can be changed later. Lowering it moves
  variants on removed levels to the unplaced area.
- A shelf can only be deleted when empty, checked against the store at the moment of deletion.
  To empty a shelf with no new home for its variants, move them to nowhere in the
  [shelf view](shelf-view.md).
- Names found in the store that match no shelf are listed in Shelves mode to be placed.

## Open questions

- Deleting a building whose shelves still hold variants: blocked, or allowed after a
  confirmation.
- Whether a Done building can return to Setup, or only have its structure unlocked.
- Whether shelves and rooms can be rotated, or only placed square to the floor.
- Whether shelves can be placed outside any room, and how they are named.
- What happens to existing store names that do not follow the `BUILDING-ROOM` format when a
  store is first connected.
