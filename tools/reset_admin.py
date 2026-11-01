#!/usr/bin/env -S uv run --script
################################################################################################
#
#   reset_admin.py
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

# This creates an Administrator, or resets an existing Administrator's PIN, by running the app's
# reset-admin command. The app does the work, so Identity validates and hashes the PIN exactly
# as it does for sign-in. Run it with "uv run tools/reset_admin.py <username>".

import argparse
import getpass
import shutil
import subprocess
import sys
from pathlib import Path

repository_root: Path = Path(__file__).resolve().parent.parent
project_path: Path = repository_root / "src" / "Shelver" / "Shelver.csproj"

pin_length: int = 6


def read_pin() -> str:
    # The PIN is typed twice without being shown, so a typo cannot lock the Administrator out.
    pin: str = getpass.getpass(f"New {pin_length}-digit PIN: ")
    if not (len(pin) == pin_length and pin.isascii() and pin.isdigit()):
        raise ValueError(f"The PIN must be exactly {pin_length} digits.")
    if getpass.getpass("Repeat the PIN: ") != pin:
        raise ValueError("The PINs do not match.")
    return pin


def main() -> int:
    argument_parser: argparse.ArgumentParser = argparse.ArgumentParser(description="Create a shelver Administrator, or reset one's PIN.")
    argument_parser.add_argument("username", help="the Administrator's username")
    arguments: argparse.Namespace = argument_parser.parse_args()

    if shutil.which("dotnet") is None:
        print("dotnet is not installed or not on PATH.", file=sys.stderr)
        return 1

    try:
        pin: str = read_pin()
    except ValueError as pin_error:
        print(pin_error, file=sys.stderr)
        return 1

    # The PIN goes through standard input rather than the arguments, because arguments are
    # visible to other users in the process list.
    print("Building and running shelver's reset-admin command.", flush=True)
    completed_process: subprocess.CompletedProcess[str] = subprocess.run(["dotnet", "run", "--project", str(project_path), "--", "reset-admin", arguments.username], input=f"{pin}\n", text=True)
    return completed_process.returncode


if __name__ == "__main__":
    sys.exit(main())
