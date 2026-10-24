#!/usr/bin/env -S uv run --script
################################################################################################
#
#   reset_database.py
#
#   Copyright (c) 2026 Josh Hayden (@jhayden02)
#
#   shelver is free software: you can redistribute it and/or modify
#   it under the terms of the GNU General Public License v3.0 as published
#   by the Free Software Foundation.
#
#   shelver is distributed in the hope that it will be useful,
#   but WITHOUT ANY WARRANTY; without even the implied warranty of
#   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#   GNU General Public License for more details.
#
#   You should have received a copy of the GNU General Public License
#   along with this program.  If not, see <https://www.gnu.org/licenses/>.
#
################################################################################################

# /// script
# requires-python = ">=3.9"
# dependencies = []
# ///

# This deletes the local SQLite database so every floor plan is removed. The app creates a
# fresh, empty database the next time it starts. Stop the app before running it with
# "uv run tools/reset_database.py", and pass "--yes" to skip the confirmation prompt.

import argparse
import json
import sys
from pathlib import Path

repository_root: Path = Path(__file__).resolve().parent.parent
project_directory: Path = repository_root / "src" / "Shelver"
settings_path: Path = project_directory / "appsettings.json"

# SQLite keeps write-ahead log and shared memory files beside the database while it is used.
database_file_suffixes: list[str] = ["", "-wal", "-shm"]


def find_database_path() -> Path:
    # The path is read from the same connection string the app uses, and a relative path is
    # resolved against the project folder exactly as Program.cs does.
    settings: dict = json.loads(settings_path.read_text(encoding="utf-8-sig"))
    connection_string: str = settings["ConnectionStrings"]["Shelver"]
    for connection_setting in connection_string.split(";"):
        setting_name, _, setting_value = connection_setting.partition("=")
        if setting_name.strip().lower() == "data source":
            return project_directory / setting_value.strip()
    raise ValueError(f"No Data Source found in the Shelver connection string in {settings_path}.")


def main() -> int:
    argument_parser: argparse.ArgumentParser = argparse.ArgumentParser(description="Delete the local shelver database.")
    argument_parser.add_argument("--yes", action="store_true", help="delete without asking for confirmation")
    arguments: argparse.Namespace = argument_parser.parse_args()

    database_path: Path = find_database_path()
    existing_files: list[Path] = [Path(f"{database_path}{suffix}") for suffix in database_file_suffixes if Path(f"{database_path}{suffix}").exists()]
    if not existing_files:
        print(f"No database found at {database_path.relative_to(repository_root)}. Nothing to reset.")
        return 0

    if not arguments.yes:
        answer: str = input(f"Delete {database_path.relative_to(repository_root)} and every floor plan in it? [y/N] ")
        if answer.strip().lower() not in ("y", "yes"):
            print("Cancelled.")
            return 1

    for existing_file in existing_files:
        existing_file.unlink()
        print(f"Deleted {existing_file.relative_to(repository_root)}.")
    print("The app will create an empty database the next time it starts.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
