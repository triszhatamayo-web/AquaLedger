using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AQUALEDGERS
{
    // Draws your real logo. Finds images in Properties > Resources by loose name,
    // or as files next to the .exe.
    internal static class Brand
    {
        private static Image icon, full, fullLight;
        private static bool loaded;

        private static void Load()
        {
            if (loaded) return;
            loaded = true;

            var found = new Dictionary<string, Image>();
            try
            {
                foreach (Type t in typeof(Brand).Assembly.GetTypes())
                {
                    if (!t.FullName.EndsWith(".Properties.Resources")) continue;
                    foreach (PropertyInfo p in t.GetProperties(
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if (p.PropertyType != typeof(Bitmap) && p.PropertyType != typeof(Image)) continue;
                        Image img = null;
                        try { img = p.GetValue(null, null) as Image; } catch { }
                        if (img != null) found[p.Name] = img;
                    }
                }
            }
            catch { }

            foreach (var kv in found)
            {
                string n = kv.Key.ToLowerInvariant();
                if (n.Contains("icon")) icon = kv.Value;
                else if (n.Contains("light")) fullLight = kv.Value;
                else if (n.Contains("full") || n == "logo") full = kv.Value;
            }

            // fallback: files next to the .exe
            if (icon == null) icon = FromFile("logo_icon.png");
            if (full == null) full = FromFile("logo_full.png");
            if (fullLight == null) fullLight = FromFile("logo_full_light.png");
            icon = Trim(icon); full = Trim(full); fullLight = Trim(fullLight);

            if (icon == null || full == null)
            {
                MessageBox.Show(
                    "Logo images not found.\n\nImage resources in this project:\n" +
                    (found.Count == 0 ? "(none)" : string.Join("\n", found.Keys)) +
                    "\n\nName them: logo_icon, logo_full, logo_full_light",
                    "Brand logo");
            }
        }

        private static Image FromFile(string name)
        {
            try
            {
                string[] dirs = { "", "Images", "Resources" };
                foreach (string d in dirs)
                {
                    string path = Path.Combine(Application.StartupPath, d, name);
                    if (File.Exists(path))
                        using (Image tmp = Image.FromFile(path)) return new Bitmap(tmp);
                }
            }
            catch { }
            return null;
        }

        // emblem only, scaled to fit the rectangle (keeps proportions)
        private static Rectangle emblemRect;
        private static bool emblemDone;

        // emblem only, scaled to fit the rectangle (keeps proportions)
        public static void DrawIcon(Graphics g, Rectangle r)
        {
            Load();
            Image img = icon ?? full;
            if (img == null) { Gfx.DrawLogo(g, r); return; }

            if (img.Width > img.Height * 1.4f)      // wide = full logo, so cut out the emblem
            {
                if (!emblemDone) { emblemRect = FindEmblem(img); emblemDone = true; }
                Rectangle src = emblemRect;
                float s = Math.Min((float)r.Width / src.Width, (float)r.Height / src.Height);
                int w = (int)(src.Width * s), h = (int)(src.Height * s);
                GraphicsState st = g.Save();
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(img, new Rectangle(r.X + (r.Width - w) / 2, r.Y + (r.Height - h) / 2, w, h),
                    src, GraphicsUnit.Pixel);
                g.Restore(st);
            }
            else Draw(g, img, r);
        }

        // finds where the emblem ends: the first empty vertical gap before the wordmark
        private static Rectangle FindEmblem(Image img)
        {
            using (Bitmap bmp = new Bitmap(img))
            {
                int minGap = Math.Max(6, bmp.Width / 120);
                bool inside = false; int run = 0, end = bmp.Width, start = 0;
                for (int x = 0; x < bmp.Width; x++)
                {
                    bool any = false;
                    for (int y = 0; y < bmp.Height; y += 3)
                        if (bmp.GetPixel(x, y).A > 25) { any = true; break; }

                    if (any) { if (!inside) { inside = true; start = x; } run = 0; }
                    else if (inside && ++run >= minGap) { end = x - run + 1; break; }
                }
                return new Rectangle(start, 0, Math.Max(1, end - start), bmp.Height);
            }
        }

        // full logo; light = true for dark backgrounds (sidebar)
        public static Rectangle DrawFull(Graphics g, Rectangle r, bool light)
        {
            Load();
            Image img = light ? (fullLight ?? full) : full;
            if (img == null) { Gfx.DrawLogo(g, new Rectangle(r.X, r.Y, r.Height, r.Height)); return r; }
            return Draw(g, img, r);
        }

        private static Rectangle Draw(Graphics g, Image img, Rectangle r)
        {
            float s = Math.Min((float)r.Width / img.Width, (float)r.Height / img.Height);
            int w = (int)(img.Width * s), h = (int)(img.Height * s);
            Rectangle dest = new Rectangle(r.X + (r.Width - w) / 2, r.Y + (r.Height - h) / 2, w, h);
            GraphicsState st = g.Save();
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawImage(img, dest);
            g.Restore(st);
            return dest;
        }
        // crops transparent margins so layout uses the real artwork size
        private static Image Trim(Image img)
        {
            if (img == null) return null;
            try
            {
                using (Bitmap src = new Bitmap(img))
                {
                    BitmapData d = src.LockBits(new Rectangle(0, 0, src.Width, src.Height),
                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    int minX = src.Width, minY = src.Height, maxX = -1, maxY = -1;
                    byte[] row = new byte[d.Stride];
                    for (int y = 0; y < src.Height; y++)
                    {
                        Marshal.Copy(IntPtr.Add(d.Scan0, y * d.Stride), row, 0, d.Stride);
                        for (int x = 0; x < src.Width; x++)
                            if (row[x * 4 + 3] > 25)
                            {
                                if (x < minX) minX = x; if (x > maxX) maxX = x;
                                if (y < minY) minY = y; if (y > maxY) maxY = y;
                            }
                    }
                    src.UnlockBits(d);
                    if (maxX < 0) return img;

                    Rectangle box = new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
                    Bitmap dst = new Bitmap(box.Width, box.Height);
                    using (Graphics g = Graphics.FromImage(dst))
                        g.DrawImage(src, new Rectangle(0, 0, box.Width, box.Height), box, GraphicsUnit.Pixel);
                    return dst;
                }
            }
            catch { return img; }
        }
    }

}