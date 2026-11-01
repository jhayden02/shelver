#!/usr/bin/env -S uv run --script
################################################################################################
#
#   generate_brand_images.py
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
# dependencies = ["fonttools[woff]", "uharfbuzz", "resvg-py", "pillow"]
# ///

# This writes the README banner and the browser tab icon from the site font, so neither ever
# has to be redrawn by hand. Both are the navbar brand's white box with a black border: the
# banner holds "shelver" and the icon holds "s". Text is converted to vector paths because
# GitHub shows README images in a sandbox that cannot load web fonts. Run it with
# "uv run tools/generate_brand_images.py" after changing the font, which installs the Python
# version and packages listed above.

from io import BytesIO
from pathlib import Path
from xml.etree import ElementTree

import resvg_py
import uharfbuzz
from fontTools.misc.transform import Transform
from fontTools.pens.boundsPen import BoundsPen
from fontTools.pens.svgPathPen import SVGPathPen
from fontTools.pens.transformPen import TransformPen
from fontTools.ttLib import TTFont
from PIL import Image

repository_root: Path = Path(__file__).resolve().parent.parent
fonts_directory: Path = repository_root / "src" / "Shelver" / "wwwroot" / "fonts"
font_path: Path = fonts_directory / "roboto-regular.woff2"
banner_path: Path = repository_root / "res" / "img" / "banner.svg"
icon_path: Path = repository_root / "src" / "Shelver" / "wwwroot" / "favicon.ico"

banner_text: str = "shelver"
font_size_px: float = 64.0

# These are the navbar brand's proportions from site.css and Bootstrap, measured in ems of the
# brand's font size, so the banner looks like a scaled-up copy of the brand.
line_height_em: float = 1.5
horizontal_padding_em: float = 0.4
border_width_em: float = 0.05

icon_letter: str = "s"

# Browsers pick the closest size from the icon file. Each size is drawn on its own rather than
# shrunk from the largest, so its border stays a whole number of pixels and stays sharp.
icon_sizes_px: list[int] = [16, 32, 48, 256]

# The letter's ink fills this share of the space inside the border, so it stays readable at
# 16 pixels.
icon_letter_fill: float = 0.7

text_color: str = "#000000"
background_color: str = "#ffffff"
border_color: str = "#000000"


def format_number(value: float) -> str:
    rounded_text: str = f"{value:.2f}".rstrip("0").rstrip(".")
    return "0" if rounded_text == "-0" else rounded_text


def create_svg_root(width_px: float, height_px: float, label: str) -> ElementTree.Element:
    width_text: str = format_number(width_px)
    height_text: str = format_number(height_px)
    svg_root: ElementTree.Element = ElementTree.Element("svg")
    svg_root.set("xmlns", "http://www.w3.org/2000/svg")
    svg_root.set("width", width_text)
    svg_root.set("height", height_text)
    svg_root.set("viewBox", f"0 0 {width_text} {height_text}")
    svg_root.set("role", "img")
    svg_root.set("aria-label", label)
    return svg_root


def add_bordered_box(svg_root: ElementTree.Element, width_px: float, height_px: float, border_px: float) -> None:
    # The border is stroked along a rectangle inset by half its width so the full stroke stays
    # inside the image.
    border_inset_px: float = border_px / 2
    border_rect: ElementTree.Element = ElementTree.SubElement(svg_root, "rect")
    border_rect.set("x", format_number(border_inset_px))
    border_rect.set("y", format_number(border_inset_px))
    border_rect.set("width", format_number(width_px - border_px))
    border_rect.set("height", format_number(height_px - border_px))
    border_rect.set("fill", background_color)
    border_rect.set("stroke", border_color)
    border_rect.set("stroke-width", format_number(border_px))


def add_text_path(svg_root: ElementTree.Element, path_data: str) -> None:
    text_path: ElementTree.Element = ElementTree.SubElement(svg_root, "path")
    text_path.set("fill", text_color)
    text_path.set("d", path_data)


def to_svg_text(svg_root: ElementTree.Element) -> str:
    ElementTree.indent(svg_root)
    return ElementTree.tostring(svg_root, encoding="unicode") + "\n"


def draw_glyph(font: TTFont, glyph_name: str, glyph_transform: Transform) -> str:
    svg_pen: SVGPathPen = SVGPathPen(font.getGlyphSet(), ntos=format_number)
    font.getGlyphSet()[glyph_name].draw(TransformPen(svg_pen, glyph_transform))
    return svg_pen.getCommands()


def build_banner_svg(font: TTFont) -> str:
    units_per_em: int = font["head"].unitsPerEm
    font_ascent: int = font["hhea"].ascent
    font_descent: int = -font["hhea"].descent
    px_per_unit: float = font_size_px / units_per_em

    padding_px: float = horizontal_padding_em * font_size_px
    border_px: float = border_width_em * font_size_px
    banner_height_px: float = line_height_em * font_size_px

    # This centers the font's ascent and descent inside the line height the way CSS
    # distributes half of the leading above and half below the text.
    content_height_px: float = (font_ascent + font_descent) * px_per_unit
    half_leading_px: float = (banner_height_px - content_height_px) / 2
    baseline_y_px: float = half_leading_px + font_ascent * px_per_unit

    # HarfBuzz lays out the text the way browsers do, including the font's kerning, so the
    # glyphs are spaced exactly as in the navbar. It reads the font as uncompressed bytes.
    uncompressed_font: BytesIO = BytesIO()
    font.flavor = None
    font.save(uncompressed_font)
    shaping_buffer: uharfbuzz.Buffer = uharfbuzz.Buffer()
    shaping_buffer.add_str(banner_text)
    shaping_buffer.guess_segment_properties()
    uharfbuzz.shape(uharfbuzz.Font(uharfbuzz.Face(uncompressed_font.getvalue())), shaping_buffer)

    glyph_paths: list[str] = []
    pen_x_px: float = padding_px
    for glyph_info, glyph_position in zip(shaping_buffer.glyph_infos, shaping_buffer.glyph_positions):
        glyph_transform: Transform = (
            Transform()
            .translate(pen_x_px + glyph_position.x_offset * px_per_unit, baseline_y_px)
            .scale(px_per_unit, -px_per_unit)
        )
        glyph_path: str = draw_glyph(font, font.getGlyphName(glyph_info.codepoint), glyph_transform)
        if glyph_path:
            glyph_paths.append(glyph_path)
        pen_x_px += glyph_position.x_advance * px_per_unit

    banner_width_px: float = pen_x_px + padding_px
    svg_root: ElementTree.Element = create_svg_root(banner_width_px, banner_height_px, banner_text)
    add_bordered_box(svg_root, banner_width_px, banner_height_px, border_px)
    add_text_path(svg_root, " ".join(glyph_paths))
    return to_svg_text(svg_root)


def build_icon_svg(font: TTFont, size_px: int) -> str:
    # The border is one pixel at 16 pixels and grows in whole pixels with the size.
    border_px: int = max(1, round(size_px / 16))
    inner_size_px: float = size_px - 2 * border_px

    # The letter is scaled by its ink, not its em box, so it is centered by what is drawn.
    glyph_name: str = font.getBestCmap()[ord(icon_letter)]
    bounds_pen: BoundsPen = BoundsPen(font.getGlyphSet())
    font.getGlyphSet()[glyph_name].draw(bounds_pen)
    ink_left, ink_bottom, ink_right, ink_top = bounds_pen.bounds
    ink_width: float = ink_right - ink_left
    ink_height: float = ink_top - ink_bottom
    px_per_unit: float = inner_size_px * icon_letter_fill / max(ink_width, ink_height)

    # Font units have y increasing upward and SVG has it increasing downward, so the glyph is
    # flipped, then moved so its ink box sits in the middle of the icon.
    center_px: float = size_px / 2
    glyph_transform: Transform = (
        Transform()
        .translate(center_px, center_px)
        .scale(px_per_unit, -px_per_unit)
        .translate(-(ink_left + ink_width / 2), -(ink_bottom + ink_height / 2))
    )

    svg_root: ElementTree.Element = create_svg_root(size_px, size_px, icon_letter)
    add_bordered_box(svg_root, size_px, size_px, border_px)
    add_text_path(svg_root, draw_glyph(font, glyph_name, glyph_transform))
    return to_svg_text(svg_root)


def write_icon(font: TTFont) -> None:
    icon_images: list[Image.Image] = []
    for size_px in icon_sizes_px:
        png_bytes: bytes = resvg_py.svg_to_bytes(svg_string=build_icon_svg(font, size_px), width=size_px, height=size_px)
        icon_images.append(Image.open(BytesIO(png_bytes)).convert("RGBA"))

    # Pillow takes the images for the other sizes from append_images instead of resizing the
    # first one.
    icon_images[-1].save(icon_path, format="ICO", sizes=[(size_px, size_px) for size_px in icon_sizes_px], append_images=icon_images[:-1])


def main() -> None:
    font: TTFont = TTFont(font_path)
    banner_path.write_text(build_banner_svg(font), encoding="utf-8")
    print(f"Wrote {banner_path.relative_to(repository_root)}.")
    write_icon(font)
    print(f"Wrote {icon_path.relative_to(repository_root)}.")


if __name__ == "__main__":
    main()
