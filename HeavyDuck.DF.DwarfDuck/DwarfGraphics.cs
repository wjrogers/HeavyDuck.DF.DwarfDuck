using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using log4net;

namespace HeavyDuck.DF.DwarfDuck
{
    internal static class DwarfGraphics
    {
        private static readonly ILog m_logger = LogManager.GetLogger(typeof(DwarfGraphics));
        private static readonly Dictionary<string, Image> m_images
            = new Dictionary<string, Image>();
        private static readonly Dictionary<string, Image> m_images_disabled
            = new Dictionary<string, Image>();

        private static readonly Regex m_regex_dim_tile = new Regex(@"\[TILE_DIM:(\d+):(\d+)\]", RegexOptions.IgnoreCase);
        private static readonly Regex m_regex_dim_page = new Regex(@"\[PAGE_DIM:(\d+):(\d+)\]", RegexOptions.IgnoreCase);
        private static readonly Regex m_regex_entry = new Regex(@"\[(\w+):DWARVES:(\d+):(\d+):", RegexOptions.IgnoreCase);

        static DwarfGraphics()
        {
            Size dim_tile = Size.Empty;
            Size dim_page = Size.Empty;
            Rectangle dest = Rectangle.Empty;
            Rectangle src;
            var dwarves = Properties.Resources.dwarves;

            try
            {
                // read the graphic definition file
                using (var reader = new StringReader(Properties.Resources.graphics_dwarves))
                {
                    Match match;
                    string line;

                    while (null != (line = reader.ReadLine()))
                    {
                        if (dim_tile.IsEmpty || dim_page.IsEmpty)
                        {
                            if ((match = m_regex_dim_tile.Match(line)).Success)
                            {
                                dim_tile = new Size(Convert.ToInt32(match.Groups[1].Value), Convert.ToInt32(match.Groups[2].Value));
                                dest = new Rectangle(0, 0, dim_tile.Width, dim_tile.Height);
                            }
                            if ((match = m_regex_dim_page.Match(line)).Success)
                                dim_page = new Size(Convert.ToInt32(match.Groups[1].Value), Convert.ToInt32(match.Groups[2].Value));
                        }
                        else
                        {
                            if ((match = m_regex_entry.Match(line)).Success)
                            {
                                Image image;
                                Image image_disabled;
                                var profession = match.Groups[1].Value;
                                var col = Convert.ToInt32(match.Groups[2].Value);
                                var row = Convert.ToInt32(match.Groups[3].Value);

                                if (row >= dim_page.Height || col >= dim_page.Width)
                                {
                                    m_logger.WarnFormat("Dwarf tile ({0}, {1}) out of range.", col, row);
                                    continue;
                                }

                                // create the images
                                src = new Rectangle(col * dim_tile.Width, row * dim_tile.Height, dim_tile.Width, dim_tile.Height);
                                image = new Bitmap(dim_tile.Width, dim_tile.Height);
                                image_disabled = new Bitmap(dim_tile.Width, dim_tile.Height);

                                // draw the tile from the page into the images
                                using (var g = Graphics.FromImage(image))
                                    g.DrawImage(dwarves, dest, src, GraphicsUnit.Pixel);
                                using (var g = Graphics.FromImage(image_disabled))
                                    ControlPaint.DrawImageDisabled(g, image, 0, 0, Color.Transparent);

                                // store them for later lookup
                                m_images[profession] = image;
                                m_images_disabled[profession] = image_disabled;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                m_logger.Error("Dwarf graphics initialization failed!", ex);
            }
        }

        /// <summary>
        /// Get the default dwarf image.
        /// </summary>
        public static Image GetDefaultImage()
        {
            return GetImage("STANDARD");
        }

        /// <summary>
        /// Get the default disabled dwarf image.
        /// </summary>
        public static Image GetDefaultImageDisabled()
        {
            return GetImageDisabled("STANDARD");
        }

        /// <summary>
        /// Get the dwarf image for the specified profession.
        /// </summary>
        public static Image GetImage(string profession)
        {
            Image image;

            if (m_images.TryGetValue(profession, out image))
                return image;
            else
                return null;
        }

        /// <summary>
        /// Get the disabled dwarf image for the specified profession.
        /// </summary>
        public static Image GetImageDisabled(string profession)
        {
            Image image;

            if (m_images_disabled.TryGetValue(profession, out image))
                return image;
            else
                return null;
        }
    }
}
