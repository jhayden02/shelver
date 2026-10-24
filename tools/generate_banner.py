#!/usr/bin/env -S uv run --script
################################################################################################
#
#   generate_banner.py
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
# dependencies = ["fonttools"]
# ///

# This writes res/img/banner.svg from the site font so the README banner never has to be
# redrawn by hand. The text is converted to vector paths because GitHub shows README images
# in a sandbox that cannot load web fonts. Run it with "uv run tools/generate_banner.py"
# after changing the font, which installs the Python version and packages listed above.

from pathlib import Path
from xml.etree import ElementTree

from fontTools.misc.transform import Transform
from fontTools.pens.svgPathPen import SVGPathPen
from fontTools.pens.transformPen import TransformPen
from fontTools.ttLib import TTFont

repository_root: Path = Path(__file__).resolve().parent.parent
fonts_directory: Path = repository_root / "src" / "Shelver" / "wwwroot" / "fonts"
font_path: Path = fonts_directory / "inconsolata-regular.woff"
banner_path: Path = repository_root / "res" / "img" / "banner.svg"

banner_text: str = "shelver"
font_size_px: float = 64.0

# These are the navbar brand's proportions from site.css and Bootstrap, measured in ems of the
# brand's font size, so the banner looks like a scaled-up copy of the brand.
line_height_em: float = 1.5
horizontal_padding_em: float = 0.4
border_width_em: float = 0.05

text_color: str = "#000000"
background_color: str = "#ffffff"
border_color: str = "#000000"


def format_number(value: float) -> str:
    rounded_text: str = f"{value:.2f}".rstrip("0").rstrip(".")
    return "0" if rounded_text == "-0" else rounded_text


def build_banner_svg() -> str:
    font: TTFont = TTFont(font_path)
    units_per_em: int = font["head"].unitsPerEm
    font_ascent: int = font["hhea"].ascent
    font_descent: int = -font["hhea"].descent
    character_map: dict[int, str] = font.getBestCmap()
    glyph_set = font.getGlyphSet()
    px_per_unit: float = font_size_px / units_per_em

    padding_px: float = horizontal_padding_em * font_size_px
    border_px: float = border_width_em * font_size_px
    banner_height_px: float = line_height_em * font_size_px

    # This centers the font's ascent and descent inside the line height the way CSS
    # distributes half of the leading above and half below the text.
    content_height_px: float = (font_ascent + font_descent) * px_per_unit
    half_leading_px: float = (banner_height_px - content_height_px) / 2
    baseline_y_px: float = half_leading_px + font_ascent * px_per_unit

    # Inconsolata is monospaced and has no kerning, so each glyph starts where the
    # previous glyph's advance width ends.
    glyph_paths: list[str] = []
    pen_x_px: float = padding_px
    for character in banner_text:
        glyph_name: str = character_map[ord(character)]
        svg_pen: SVGPathPen = SVGPathPen(glyph_set, ntos=format_number)
        glyph_transform: Transform = (
            Transform()
            .translate(pen_x_px, baseline_y_px)
            .scale(px_per_unit, -px_per_unit)
        )
        glyph_set[glyph_name].draw(TransformPen(svg_pen, glyph_transform))
        glyph_path: str = svg_pen.getCommands()
        if glyph_path:
            glyph_paths.append(glyph_path)
        pen_x_px += font["hmtx"][glyph_name][0] * px_per_unit

    banner_width_px: float = pen_x_px + padding_px
    width_text: str = format_number(banner_width_px)
    height_text: str = format_number(banner_height_px)

    svg_root: ElementTree.Element = ElementTree.Element("svg")
    svg_root.set("xmlns", "http://www.w3.org/2000/svg")
    svg_root.set("width", width_text)
    svg_root.set("height", height_text)
    svg_root.set("viewBox", f"0 0 {width_text} {height_text}")
    svg_root.set("role", "img")
    svg_root.set("aria-label", banner_text)

    # The border is stroked along a rectangle inset by half its width so the full stroke stays
    # inside the image.
    border_inset_px: float = border_px / 2
    border_rect: ElementTree.Element = ElementTree.SubElement(svg_root, "rect")
    border_rect.set("x", format_number(border_inset_px))
    border_rect.set("y", format_number(border_inset_px))
    border_rect.set("width", format_number(banner_width_px - border_px))
    border_rect.set("height", format_number(banner_height_px - border_px))
    border_rect.set("fill", background_color)
    border_rect.set("stroke", border_color)
    border_rect.set("stroke-width", format_number(border_px))

    text_path: ElementTree.Element = ElementTree.SubElement(svg_root, "path")
    text_path.set("fill", text_color)
    text_path.set("d", " ".join(glyph_paths))

    ElementTree.indent(svg_root)
    return ElementTree.tostring(svg_root, encoding="unicode") + "\n"


def main() -> None:
    banner_path.write_text(build_banner_svg(), encoding="utf-8")
    print(f"Wrote {banner_path.relative_to(repository_root)}.")


if __name__ == "__main__":
    main()
