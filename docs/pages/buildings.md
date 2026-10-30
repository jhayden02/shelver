# Buildings

Setup for buildings, their floors, rooms, and shelves. Administrators only (planned; open to
everyone until accounts exist). Designed for a desktop.

The built pages still use the earlier single-floor model under the name Floor Plans
(`/FloorPlans`). Everything below describes the building model.

## List

**Status: built as Floor Plans.**

- Lists every building with its number of floors, and any flagged problems, and a link
  to edit it.
- With no buildings, the page shows only a centered "Create building" button in the space a
  floor would fill.
- Spare shelves are listed below the buildings by the first 8 characters of their ID, and can
  be retired from here.

## Create

**Status: planned.** The built Floor Plans form asks for a name, width, and height.

- **Name:** required, at most 100 characters.
- **Number of floors:** fixed after creation.
- **Floor 1 width and height** in decimal feet, 3–3,000 ft. Other floors start with the same
  size and no offset.

A new building opens in guided setup. Everything here except the number of floors can be
changed later.

## Floors

**Status: planned.**

- Numbered 1, 2, 3 upward.
- Each has its own width, height, and offset within the building's shared coordinates, so a
  point on one floor is directly above the same point on the floor below.
- The number of floors never changes. A building created with the wrong count is deleted and
  created again.

## Editor

**Status: planned.**

Edits one floor at a time, with a floor picker, in three steps. Each step depends on the ones
before it. Layers from other steps stay visible but faded and cannot be clicked.

1. **Size and floors:** each floor's size and offset.
2. **Structure:** walls, doors, and stairs.
   - Walls are segments with draggable endpoints and snapping. "Add wall" drops in a segment.
   - Stairs are rectangles with a marked top and bottom side, linking a floor to the one above.
     They appear on both floors.
3. **Rooms and shelves.**

All dragging and resizing uses interact.js on the server-rendered SVG; each change is saved
through a controller action.

## Setup and changes

**Status: planned.**

- **Guided setup:** a new building goes through the three steps in order. A step opens once the
  previous step's checks pass.
- The building appears on [Home](home.md) once it has a room.
- Afterwards, an Administrator can return to any step at any time. There is no lock.
- A change re-checks every later step and flags what no longer fits, without moving or
  deleting anything. Flagged items are listed in the editor as a to-do list. Store data is
  untouched, so Home keeps working.
- Renaming a room renames its shelves, and goes through the same plan and confirmation as
  [changing shelves](#changing-shelves).

### Checks

- Every wall, door, stair, room, and shelf lies within its floor.
- Every floor above floor 1 is reached by at least one stair.
- No room crosses a wall.
- Every room's entrance is a door on its boundary.

## Rooms

**Status: planned.**

- A room is an area selected in the Rooms and shelves step, bounded by walls and doors.
- Each room has a name, unique across all buildings.
- Each room has one entrance door, defaulting to the first door on its boundary. An
  Administrator can choose another.

## Shelf identity

**Status: planned.**

- Every shelf has a permanent ID (a GUID) that never changes, and a name that can.
- A shelf exists apart from any building. Placing it in a room names it. A shelf in no room is
  **spare**: it has no name and cannot hold variants.
- **ID tag:** applied once to the shelf and never moved. Shows a QR code of the ID and the ID's
  first 8 characters. Scanning it opens the shelf's [view](shelf-view.md), which says when the
  shelf is spare or retired.
- **Name label:** shows the shelf's current name. When names shift, labels move between
  shelves.

## Shelf names

**Status: planned.**

- Shelves are named by their room followed by a number, for example `A1`.
- Numbers are applied by the numbering order, never chosen. Every shelf added to, moved within,
  or removed from a room renumbers the room; removing a shelf shifts higher numbers down.

### Numbering order

Directions are as seen from the room's entrance, facing in:

1. Left to right across the room, by each shelf's footprint as rotated.
2. Shelves whose left-to-right ranges overlap are numbered from the far side toward the
   entrance.

The goal is predictability: from any shelf, higher numbers are to its right.

## Changing shelves

**Status: planned.**

1. Shelf changes in the Rooms and shelves step are held until saved.
2. Saving shows a plan: every rename and how many variants each touches.
3. Confirming applies the renames to the store as one batch.
4. A move diagram of the room shows each affected shelf as ~~`A4`~~ `A5`, marking which
   labels move, which are new, and which are left over. Each shelf has a checkbox for when its
   label is moved.
5. The room is marked **labels pending** in the editor and on Home until every box is checked.

## Moving shelves

**Status: planned.**

- Within a floor, a shelf can be dragged into another room.
- **Move to room** moves a shelf to a room on any floor of any building.
- A move keeps the shelf's ID, tag, history, and variants. It renames the shelf and renumbers
  both rooms through [changing shelves](#changing-shelves).

## Shelf rules

**Status: planned.**

- A shelf has a width and depth in decimal feet, and a front along one of its width sides.
- A shelf rotates in 90-degree steps. Its front is clearly marked wherever the shelf is drawn.
- A shelf's level count is set when it is created and can be changed later. Lowering it moves
  variants on removed levels to the unplaced area.
- Creating a shelf in a room creates and places it in one step. Spare shelves are listed in
  the Rooms and shelves step to be placed.
- A shelf can only be removed from its room when empty, checked against the store at that
  moment. It becomes spare. To empty a shelf with no new home for its variants, move them to
  nowhere in the [shelf view](shelf-view.md).
- Retiring a spare shelf ends its ID. Its history is kept.
- Shelf entries found in the store that match no shelf are listed in the Rooms and shelves
  step to be placed.

## Deleting a building

**Status: planned.**

- A building can only be deleted when all its shelves are empty, checked against the store.
  Its shelves become spare, keeping their IDs, tags, and history.
- **Move everything to nowhere** empties every shelf in the building at once. It shows how many
  variants and shelves are affected.
- Both actions require typing the building's name to confirm.
