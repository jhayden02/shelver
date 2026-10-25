# Floor Plans

The setup section for floor plans and the shelves on them. Administrators only (planned; the
section is currently open to everyone because accounts do not exist yet). Designed for a
desktop.

## List

**Status: built.**

- `/FloorPlans` lists every floor plan by name, with its size in feet and its state, and a
  link to edit it.
- When no floor plans exist, the page shows only a centered "Create floor plan" button in the
  space a floor plan would fill.
- A "Create floor plan" button also sits above the list when floor plans exist.

## Create

**Status: built.**

- A short form asks for a name and the building's width and height in decimal feet.
- Name is required and at most 100 characters. Width and height are required and between 3
  and 3,000 ft.
- Invalid input redisplays the form with errors, checked in the browser and again on the
  server.
- A new floor plan starts in the Setup state and opens in the editor.

## Editor

**Status: planned.** `/FloorPlans/Edit/{id}` currently shows a placeholder.

The editor works on one floor plan and has two modes, switched with a toggle. The mode decides
what can be clicked; the other layer stays visible but faded.

- **Structure mode:** draw and edit walls and place doors. Shelves are shown faded as a
  reference.
- **Shelves mode:** add, move, and delete shelves, rename them, and change their number of
  levels. Walls and doors are shown faded.

Walls are segments with draggable endpoint handles and snapping. "Add wall" drops in a new
segment to drag into place. Shelves are dragged and resized on the plan. Both use interact.js
on the server-rendered SVG, and each change is saved through a controller action.

## Setup and Done

**Status: planned.**

- A floor plan starts in **Setup**. The editor opens in Structure mode.
- Marking it **Done**:
  - Locks Structure mode. Editing walls or doors again requires unlocking it, with a
    confirmation.
  - Makes the editor open in Shelves mode from then on.
  - Makes the floor plan available on [Home](home.md).

## Shelf rules

**Status: planned.**

- A shelf's level count is set when it is created and can be changed later.
- Lowering the level count moves products on removed levels to the shelf's unplaced area
  rather than dropping them.
- A shelf can only be deleted when it is empty. Emptiness is checked against the store at the
  moment of deletion, not against shelver's own records, so a product added in the store
  since shelver last looked still blocks it. To remove a shelf whose products have no new
  home, first move them to nowhere in the [shelf view](shelf-view.md).
- Renaming a shelf updates every product in the store; see the rename order in
  [data.md](../data.md#sync-rules).
- Shelves whose names exist in the store but are not placed on any floor plan are listed in
  Shelves mode so they can be placed.

## Open questions

- What deleting a floor plan does when its shelves still hold products: blocked like shelf
  deletion, or allowed after a confirmation.
- Whether a Done floor plan can return to Setup, or only have its structure unlocked.
- Whether shelves can be rotated, or only placed square to the plan.
